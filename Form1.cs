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
        public static List<string> mod_deploy = new List<string>(); //array for mods.cfg
        public static List<string> mod_deployed = new List<string>(); //array to take mod_deploy and apply it to mods.cfg
        public static List<string> swgemu = new List<string>(); //array for swgemu_live.cfg
        public static List<string> cfgcontent = new List<string>(); //array for program cfg cile
        public static string cfgname = "modmanager.cfg";
        public static string[] tre_files;

        private void Form1_Load(object sender, EventArgs e) //runs on startup
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

        private void button1_Click(object sender, EventArgs e)//deploys mods
        {
            //this creates and overwrites the origional mods.
            int indexToModify = -1;
            bool foundLine = false;
            for (int i = 0; i < swgemu_live.Count; i++)//looks for the line with max search priority
            {
                if (swgemu_live[i].Contains("maxSearchPriority"))//finds a line with "maxSearchPriotiy
                {
                    Console.WriteLine("found it");
                    foundLine = true;
                    indexToModify = i;
                    break;
                }
            }

            // Change maxSearchPriority if it's found
            if (foundLine)
            {
                swgemu_live[indexToModify] = "maxSearchPriority=999";
                Console.WriteLine("changed it");
            }

            int itemCount = checkedListBox2.Items.Count;

            for (int i = 0; i < itemCount; i++)
            {
                mod_deploy.Add(checkedListBox2.Items[i].ToString());
            }

            try//creates mods.cfg
            {

                using (StreamWriter writer = new StreamWriter(cfgcontent[4] + "mods.cfg"))
                    writer.Write("");
            }
            catch { }

            int stp = 900;
            using (StreamWriter sw = File.CreateText(cfgcontent[4] + "mods.cfg"))//writes 
            {
                sw.WriteLine("[SharedFile]");
                sw.WriteLine("maxSearchPriority=999");
                for(int i = 0; i < mod_deploy.Count; i++)
                {
                    mod_deploy[i] = "searchTree_00_" + stp + "=mods/" + mod_deploy[i] + ".tre";
                    stp--;
                }
                
            }
            File.AppendAllLines(cfgcontent[4] + "mods.cfg", mod_deploy);


            //spot to turn mod_deploy into mod_deployed
            //Logic for this will be as follows:
            //mod_deploy will be made from checkedListBox2
            //mod_deployed will then be created, adding [SharedFile] as the 1st line, and maxSearchPriority=999 as the 2nd line
            //the 3rd line will look something like this mod_deployed[2]="searchTree__00__" + stp + "=mods/" + mod_deploy[0] + ".tre"
            //stp will be 900, and every line it will decrease by 1. A really dumb way of running


            File.WriteAllLines(cfgcontent[4] + "swgemu_live.cfg", swgemu_live);
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
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                if (checkedListBox1.GetItemChecked(i))
                {
                    // Add the item to CheckedListBox2
                    checkedListBox2.Items.Add(checkedListBox1.Items[i]);
                    // Remove the item from CheckedListBox1
                    checkedListBox1.Items.RemoveAt(i);
                    i--;
                }
            }
        }

        private void label6_Click(object sender, EventArgs e)
        {
            Process.Start("https://algebuckina-design.au");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            for (int i = checkedListBox2.CheckedItems.Count - 1; i >= 0; i--)
            {
                // Add the checked item to checkedListBox1
                checkedListBox1.Items.Add(checkedListBox2.CheckedItems[i]);
                // Remove the checked item from checkedListBox2
                checkedListBox2.Items.Remove(checkedListBox2.CheckedItems[i]);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Disable All button");
            {
                checkedListBox1.Items.AddRange(checkedListBox2.Items);
                checkedListBox2.Items.Clear();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Feature is a work in progress", "Tre pack",
                   MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button9_Click(object sender, EventArgs e)//move mod up in load order
        {
            int selectedIndex = checkedListBox2.SelectedIndex;
            if (selectedIndex > 0)
            {
                // Swap the selected item with the one above it
                object temp = checkedListBox2.Items[selectedIndex];
                checkedListBox2.Items[selectedIndex] = checkedListBox2.Items[selectedIndex - 1];
                checkedListBox2.Items[selectedIndex - 1] = temp;

                // Preserve the checked state of the item
                bool isChecked = checkedListBox2.GetItemChecked(selectedIndex);
                checkedListBox2.SetItemChecked(selectedIndex, checkedListBox2.GetItemChecked(selectedIndex - 1));
                checkedListBox2.SetItemChecked(selectedIndex - 1, isChecked);

                // Select the moved item
                checkedListBox2.SelectedIndex = selectedIndex - 1;
            }
        }

        private void button8_Click(object sender, EventArgs e)//move mod down in load order
        {
            int selectedIndex = checkedListBox2.SelectedIndex;
            if (selectedIndex < checkedListBox2.Items.Count - 1)
            {
                // Swap the selected item with the one below it
                object temp = checkedListBox2.Items[selectedIndex];
                checkedListBox2.Items[selectedIndex] = checkedListBox2.Items[selectedIndex + 1];
                checkedListBox2.Items[selectedIndex + 1] = temp;

                // Preserve the checked state of the item
                bool isChecked = checkedListBox2.GetItemChecked(selectedIndex);
                checkedListBox2.SetItemChecked(selectedIndex, checkedListBox2.GetItemChecked(selectedIndex + 1));
                checkedListBox2.SetItemChecked(selectedIndex + 1, isChecked);

                // Select the moved item
                checkedListBox2.SelectedIndex = selectedIndex + 1;
            }
        }
    }
}
