using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;

namespace kdz_manager
{
    class RecentFilesFolders
    {
        public string CurrentlyOpenFilePath;

        public RecentFilesFolders()
        {
            if (Properties.Settings.Default.RecentFiles == null)
            {
                Properties.Settings.Default.RecentFiles = new StringCollection();
                Properties.Settings.Default.Save();
            }
            /// Set sensible value for directory in OpenFile dialog.
            if (Properties.Settings.Default.RecentDirectory == null)
            {
                Properties.Settings.Default.RecentDirectory
                    = Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
            }
        }

        /// <summary>
        /// The OpenRecent files have changed. Refresh the view in menu.
        /// </summary>
        public void ReplaceOpenRecentMenu(ToolStripMenuItem open_recent_menu, Action<string> onclick)
        {
            open_recent_menu.DropDownItems.Clear();
            if (Properties.Settings.Default.RecentFiles.Count == 0)
            {
                open_recent_menu.DropDownItems.Add("No recent files...");
                return;
            }
            foreach (var str in Properties.Settings.Default.RecentFiles)
            {
                var tool = open_recent_menu.DropDownItems.Add(str);
                // so that "str" var is not cached by LINQ use "tool.Text"
                tool.Click += delegate { onclick(tool.Text); };
            }
        }

        /// <summary>
        /// Add a new item to the Recent-Files menu and save it persistently
        /// </summary>
        /// <param name="file"></param>
        public void AddRecentFile(string file)
        {
            var recent = Properties.Settings.Default.RecentFiles;
            if (recent.Count > Properties.Settings.Default.QtyOfRecentFiles)
            {
                recent.RemoveAt(recent.Count - 1);
            }
            // Reinsert at 0 position (does not throw if not found)
            recent.Remove(file);
            recent.Insert(0, file);
            Properties.Settings.Default.Save();
        }
    }
}
