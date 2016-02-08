using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using csv_parser;
using System.IO;

namespace kdz_manager
{
    public partial class Form1 : Form
    {
        string source = @"timestamp,TestString,SetComment,PropertyString,IntField,IntProperty
2012-05-01,test1,""Hi there, I said!"",Bob,57,0
2011-04-01,test2,""What's up, buttercup?"",Ralph,1,-999
1975-06-03,test3,""Bye and bye, dragonfly!"",Jimmy's The Bomb,12,13";

        public Form1()
        {
            InitializeComponent();
        }


        public void LoadData()
        {
            DataTable dt = CSV.LoadString(source, true, false);
        }


        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Stream input_stream = null;
            OpenFileDialog open_file_dia = new OpenFileDialog();

            // at least try to be portable
            string homePath = (Environment.OSVersion.Platform == PlatformID.Unix ||
                               Environment.OSVersion.Platform == PlatformID.MacOSX)
                ? Environment.GetEnvironmentVariable("HOME")
                : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");

            open_file_dia.InitialDirectory = homePath;
            open_file_dia.Filter = "CSV files (*.csv)|*.csv|txt files (*.txt)|*.txt|All files (*.*)|*.*";
            open_file_dia.FilterIndex = 0;
            open_file_dia.RestoreDirectory = true;

            if (open_file_dia.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((input_stream = open_file_dia.OpenFile()) != null)
                    {
                        using (input_stream)
                        {
                            // Insert code to read the stream here.
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }
    }
}
