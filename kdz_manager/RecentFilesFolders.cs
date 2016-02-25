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
        private string _currently_open_filepath;
        public string CurrentlyOpenFilePath
        {
            get { return _currently_open_filepath; }
            set {
                _currently_open_filepath = value;
                CurrentlyOpenFilePathChanged(_currently_open_filepath);
            }
        }

        /// <summary>
        /// событие огонь по текущим изменением пути к файлу.
        /// </summary>
        public event Action<string> CurrentlyOpenFilePathChanged;


        public RecentFilesFolders()
        {
            if (Properties.Settings.Default.RecentFiles == null)
            {
                Properties.Settings.Default.RecentFiles = new StringCollection();
                Properties.Settings.Default.Save();
            }
            /// указан разумным соотношением каталога в диалоговом окне Open File.
            if (Properties.Settings.Default.RecentDirectory == null)
            {
                Properties.Settings.Default.RecentDirectory
                    = Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
            }
        }

        /// <summary>
        /// открытом Последние файлы были изменены.  Обновите представление в меню.
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
        /// Добавить новый элемент в меню недавних файлов и сохранить его упорно
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
