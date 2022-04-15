using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization;
using Microsoft.Win32;


namespace Vega_Timings_tweak_autoloader
{

public partial class Form1 : Form
    {
        bool start = false;
        string filename = "WinAMDTweak.exe";
        int Column = 1;
        int Row = 0;
        int ElementForRow = 4;
        System.Diagnostics.Process pProcess;
        private ContextMenu contextMenu1;
        private MenuItem menuItem1;
        public TextBox AL;
        
        public  string[] timings = new string[] 
       { 
           "CL",
           "RAS",
           "RCDRD",
           "RCDWR",
           "RC",
           "RP",
           "RRDS",
           "RRDL",
           "RTP",
           "FAW",
           "CWL",
           "WTRS",
           "WTRL",
           "WR",
           "RREFD",
           "RDRDDD",
           "RDRDSD",
           "RDRDSC",
           "RDRDSCL",
           "WRWRDD",
           "WRWRSD",
           "WRWRSC",
           "WRWRSCL",
           "WRRD",
           "RDWR",
           "REF",
           "MRD",
           "MOD",
           "XS",
           "XSMRS",
           "PD",
           "CKSRE",
           "CKSRX",
           "RFCPB",
           "STAG",
           "XP",
           "CPDED",
           "CKE",
           "RDDATA",
           "WRLAT",
           "RDLAT",
           "WRDATA",
           "CKESTAG",
           "RFC",
           "AL"

       };
       public string Parameters() 
        {
            string s = "";
            foreach (Control cntrl in this.Controls.OfType<TextBox>()) 
            {
                cntrl.Text = cntrl.Text.Trim();
                    int number = 0;
                    if (int.TryParse(cntrl.Text, out number)) 
                    {
                    if (cntrl.Name != "AL")
                                s += "--" + cntrl.Name + " " + cntrl.Text + " ";
                    }
            }
            return s;
        }
        public void TweakTheVGA() 
        {
            pProcess = new System.Diagnostics.Process();
            pProcess.StartInfo.FileName = Folder.Text + "/" + filename;

            ////strCommandParameters are parameters to pass to program
            pProcess.StartInfo.Arguments = " --i 0,3,5 " + Parameters();

            pProcess.StartInfo.UseShellExecute = false;

            ////Set output of program to be written to process output stream
            pProcess.StartInfo.RedirectStandardOutput = true;

            pProcess.StartInfo.CreateNoWindow = true;

            ////Start the process
            if (Folder.Text != "")
            {
                pProcess.Start();
                label3.Text = "Memory Tweaked!";
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                notifyIcon1.Visible = true;
                this.Hide();
                e.Cancel = true;
            }
        }
        public void SystemEvents_PowerModeChanged(object sender, Microsoft.Win32.PowerModeChangedEventArgs e)
        {
            if (e.Mode == Microsoft.Win32.PowerModes.Suspend)
            {
                // Check what the status is and act accordingly
                label3.Text = "Memory not tweaked";
            }
           
            else if (e.Mode == Microsoft.Win32.PowerModes.Resume)
            {
                // Check what the status is and act accordingly
                TweakTheVGA();
            }
        }
        private void Form_Shown(object sender, EventArgs e)
        {
            //to minimize window
            this.WindowState = FormWindowState.Minimized;

            notifyIcon1.Visible = true;
            //to hide from taskbar
            this.Hide();
        }
        public Form1()
        {
            this.Shown += Form_Shown;
            this.Load += Form1_Load;
            this.FormClosing += Form1_FormClosing;
            SystemEvents.PowerModeChanged += new PowerModeChangedEventHandler(SystemEvents_PowerModeChanged);
            InitializeComponent();
            this.UseWaitCursor = false;
        }
        private void menuItem1_Click(object Sender, EventArgs e)
        {
            // Close the form, which closes the application.
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RegistryKey reg = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            object v = reg.GetValue("VEGAMTA");
            if (v != null) checkBox1.Checked = true;
            notifyIcon1.Visible = false;
            this.contextMenu1 = new ContextMenu();
            this.menuItem1 = new MenuItem();
            this.contextMenu1.MenuItems.AddRange(
            new MenuItem[] { this.menuItem1 });
 
            // Initialize menuItem1
            this.menuItem1.Index = 0;
            this.menuItem1.Text = "E&xit";
            this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
            notifyIcon1.ContextMenu = this.contextMenu1;
            for (int i = 0; i < timings.Length; i++)
            {
                if (Column == ElementForRow) { Row++; Column = 1; }
                int Xlabel = (Column - 1) * 150;
                int XTB = Xlabel + 70;
                //Create label
                Label label = new Label();
                label.Text = timings[i];
                //Position label on screen
                label.Size = new Size(70, 20);
                label.Left = Xlabel;
                label.Top = ((Row + 1) * 25) + 50;
                label.ForeColor = Color.White;
                //Create textbox
                TextBox textBox = new TextBox();

                //Position textbox on screen
                textBox.Size = new Size(50, 20);
                textBox.Left = XTB;
                textBox.Name = timings[i];
                textBox.Top = ((Row + 1) * 25) + 50;
                //Add controls to form
                if (timings[i] == "AL")
                {
                    textBox.Visible = false;
                    label.Visible = false;
                    textBox.Text = "0";
                    AL = textBox;
                    this.Controls.Add(label);
                    this.Controls.Add(AL);
                }
                else
                {
                    this.Controls.Add(label);
                    this.Controls.Add(textBox);
                }


                Column++;
            }

            ControlsSaveLoad.Load(this);
           
            string STRING = System.IO.Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]) + "\\path.txt";
            string readText = File.ReadAllText(STRING);
            Folder.Text = readText;
            if (AL.Text == "1") ALCB.Checked = true;
            if (Folder.Text != "" && Parameters() != "" && ALCB.Checked == true)
            {
                TweakTheVGA();

            }
            start = true;
        }

        [DataContract]
        public class ControlNameText
        {
            public ControlNameText(Control cntrl)
            {
                Name = cntrl.Name;
                Text = cntrl.Text;
            }

            [DataMember]
            public string Name { set; get; }

            [DataMember]
            public string Text { set; get; }
        }

        // we want to save a collection of ControlNameText:
        [DataContract]
        public static class ControlsSaveLoad
        {
            private static string baseFolderPath = System.IO.Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]) + "\\timings.xml";

            private static DataContractSerializer dcs = new DataContractSerializer(typeof(List<ControlNameText>));

            [DataMember] public static List<ControlNameText> Cntrls = new List<ControlNameText>();

            public static void Save(Form frm)
            {
                Cntrls = new List<ControlNameText>();
                foreach (Control cntrl in frm.Controls.OfType<TextBox>())
                {
                    if (cntrl.Name != "Folder")
                    Cntrls.Add(new ControlNameText(cntrl));
                }
                using (var writer =
                    new FileStream(baseFolderPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                   
                    dcs.WriteObject(writer, Cntrls);
                }
            }

            public static void Load(Form frm)
            {
                if (File.Exists(baseFolderPath))
                {
                    using (var reader = new FileStream(baseFolderPath, FileMode.Open, FileAccess.Read))
                    {
                        Cntrls = (List<ControlNameText>)dcs.ReadObject(reader);
                    }

                    foreach (ControlNameText cntrl in Cntrls)
                    {
                        if (cntrl.Name != "Folder")
                        frm.Controls[cntrl.Name].Text = cntrl.Text;
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (File.Exists(System.IO.Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]) + "\\timings.xml"))
                File.Delete(System.IO.Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]) + "\\timings.xml");

            ControlsSaveLoad.Save(this);
            if (Folder.Text != "" && Parameters() != "")
            {
                TweakTheVGA();
            }
        }
        private void label2_Click(object sender, EventArgs e)
        {
            DialogResult label2_Click = folderBrowserDialog1.ShowDialog();
            if (label2_Click == DialogResult.OK)
            {
                //
                // The user selected a folder and pressed the OK button.
                // A message pops up and identifies the number of files found within that folder.
                //
                string[] files = Directory.GetFiles(folderBrowserDialog1.SelectedPath);
                foreach (string f in files) 
                {
                    string s = Path.GetFileName(f);
                    if (s == filename) {
                        Folder.Text = folderBrowserDialog1.SelectedPath;
                        string STRING = System.IO.Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]) + "\\path.txt";
                        File.WriteAllText(STRING, folderBrowserDialog1.SelectedPath);
                    }
                }
             
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            RegistryKey reg = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            RegistryKey reg2 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (checkBox1.Checked && start) 
            {
                reg.SetValue("VEGAMTA", Application.ExecutablePath.ToString());
                reg2.SetValue("VEGAMTA", Application.ExecutablePath.ToString());
                MessageBox.Show("autoload ON", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (!checkBox1.Checked && start)
            {
                reg.DeleteValue("VEGAMTA",false);
                reg2.DeleteValue("VEGAMTA", false);
                MessageBox.Show("autoload OFF", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void ALCB_CheckedChanged(object sender, EventArgs e)
        {
            if (ALCB.Checked == true) 
                AL.Text = "1";
            else 
                AL.Text = "0";
        }
    }
}
