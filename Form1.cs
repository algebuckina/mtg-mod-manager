﻿using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace mtg_manager
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static List<string> swgemu_live = new List<string>(); //array for swgemu_live.cfg
        public static List<string> mod_deploy = new List<string>(); //array for swgemu_live.cfg
        public static List<string> swgemu = new List<string>(); //array for swgemu_live.cfg
        public static List<string> cfgcontent = new List<string>(); //array for program cfg cile
        public static string cfgname = "modmanager.cfg";
        public static string[] tre_files;

        private void Form1_Load(object sender, EventArgs e)
        {
            string cfgname = "modmanager.cfg";
            string swgemuPath;

            if (!File.Exists(cfgname)) //creates 1st time run cfg file
            {
                using (StreamWriter sw = File.CreateText(cfgname))
                {
                    sw.WriteLine("File created by SWGEmu Mod Manager, do not edit this file, only delete it and rerun the program");
                    sw.WriteLine("");
                    sw.WriteLine("SWGEmu.exe folder path");
                    sw.WriteLine("mod folder path");
                    sw.WriteLine("");
                    sw.WriteLine("Folder path");
                    sw.WriteLine("By Algebuckina Design, https://algebuckina-design.au");
                }
            }

            foreach (string line in File.ReadLines(cfgname)) //reads cfg file to array whenever the program loads
            {
                cfgcontent.Add(line);
            }

            if (File.Exists("SWGEmu.exe")) //checks if program is in the SWGEmu folder and writes the changes to the CFG file
            {
                cfgcontent[2] = "SWGEmu.exe";
                if (!Directory.Exists("mods"))
                {
                    Directory.CreateDirectory("mods");
                }
                cfgcontent[3] = "mods";
                cfgcontent[4] = "";
                File.WriteAllLines(cfgname, cfgcontent);
            }

            else
            {
                if (cfgcontent[2] == "SWGEmu.exe folder path")//open the "open file" dialogue to find the swgemu.exe path
                {
                    MessageBox.Show("Please find your SWGEmu.exe", "1st time setup",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.FileName = "SWGEmu.exe";
                    ofd.Filter = "exe files (*.exe*)|*.exe*";
                    ofd.FilterIndex = 1;
                    ofd.Multiselect = false;

                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        swgemuPath = ofd.FileName;
                        Console.WriteLine(swgemuPath);
                        cfgcontent[2] = swgemuPath;
                    }
                }

                cfgcontent[3] = cfgcontent[2].Replace("SWGEmu.exe", "mods");
                if (!Directory.Exists(cfgcontent[3]))
                {
                    Directory.CreateDirectory(cfgcontent[3]);
                }
                cfgcontent[4] = cfgcontent[2].Replace("SWGEmu.exe", "");
                File.WriteAllLines(cfgname, cfgcontent);
            }

            foreach (string line1 in File.ReadLines(cfgcontent[4] + "swgemu_live.cfg")) //reads the swgemu_live.cfg file
            {
                swgemu_live.Add(line1);
                Console.WriteLine(line1);
            }

            foreach (string line in File.ReadLines(cfgcontent[4] + "swgemu.cfg")) //reads the swgemu.cfg file
            {
                swgemu.Add(line);
                Console.WriteLine(line);
            }

            bool exists = false;
            if (exists = swgemu.Any(s => s.Contains("mod"))) { }
            else
            {
                swgemu.Add(".include \"mods.cfg\"");
                File.WriteAllLines(cfgcontent[4] + "swgemu.cfg", swgemu);
            }

            try
            {
                using (StreamWriter writer = new StreamWriter(cfgcontent[4] + "mods.cfg"))//ignore this. 
                    writer.Write("");
            }
            catch { }


            tre_files = Directory.GetFiles(cfgcontent[3], "*.tre"); //identify mods (if any) in the mod folder
            int y = 0;
            while (y < tre_files.Length)
            {
                tre_files[y] = Path.GetFileNameWithoutExtension(tre_files[y]);
                y++;
            }
            checkedListBox1.Items.AddRange(tre_files);
        }

        private void button4_Click(object sender, EventArgs e)//runs swgemu. idk why this works like this, but it works so oh well
        {
            Console.WriteLine(cfgcontent[2]);
            Process.Start("cmd.exe", ("/C cd \"" + cfgcontent[4] + "\" & start SWGEmu.exe"));
        }

        private void button1_Click(object sender, EventArgs e)//writes swgemu_live array to file
        {

            File.WriteAllLines(cfgcontent[4] + "mods.cfg", mod_deploy);
            Console.WriteLine("array successfully written to file");
            MessageBox.Show("Mods have successfully been deployed!", "Mod Deploy",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button2_Click(object sender, EventArgs e)// copys mod from chosen location to the /mods folder
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "TRE files (*.tre*)|*.tre*";
            ofd.FilterIndex = 1;
            ofd.Multiselect = false;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string modLocation = ofd.FileName;
                string modName = ofd.SafeFileName;
                Console.WriteLine(modLocation);
                Console.WriteLine(cfgcontent[3] + "/" + modName);
                File.Copy(modLocation, cfgcontent[3] + "/" + modName, true);
            }
            tre_files = Directory.GetFiles(cfgcontent[3], "*.tre"); //identify mods (if any) in the mod folder
            checkedListBox1.Items.Clear();
            int y = 0;
            while (y < tre_files.Length)
            {
                tre_files[y] = Path.GetFileNameWithoutExtension(tre_files[y]);
                y++;
            }
            checkedListBox1.Items.AddRange(tre_files);
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            Console.WriteLine("Enable button");
        }

        private void label6_Click(object sender, EventArgs e)
        {
            Process.Start("https://algebuckina-design.au");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Disable button");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Disable All button");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Feature is a work in progress", "Tre pack",
                   MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
