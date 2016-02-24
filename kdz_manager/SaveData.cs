using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Reflection;
using System.ComponentModel;
using System.Windows.Forms;

namespace kdz_manager
{
    /// <summary>
    /// Class to serialize CSV data from data table to file.
    /// </summary>
    public class CSVWriter : IDisposable
    {
        private char _separator;
        private char _string_escape;

        protected StreamWriter _outstream;

        /// <summary>
        /// Construct a new CSV writer to produce output on the enclosed StreamWriter
        /// </summary>
        public CSVWriter(StreamWriter source, char separator, char text_escape)
        {
            _outstream = source;
            _separator = separator;
            _string_escape = text_escape;
        }


        /// <summary>
        /// Write the data table to a stream in CSV format
        /// </summary>
        /// <param name="dt">The data table to write</param>
        /// <param name="write_header">Do not write the headers when appending</param>
        public void Write<T>(DataTable dt, bool write_header)
        {
            List<string> headers = new List<string>();
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            foreach (PropertyDescriptor prop in properties)
            {
                headers.Add(prop.Name);
            }
            // Write headers, if the caller requested we do so
            if (write_header)
            {
                WriteLine(headers);
            }
            // Now produce the rows
            foreach (DataRow dr in dt.Rows)
            {
                List<object> writeobjs = new List<object>();
                foreach (string colname in headers)
                {
                    writeobjs.Add(dr[colname]);
                }
                WriteLine(writeobjs);
            }
            // Flush the stream
            _outstream.Flush();
        }

        /// <summary>
        /// Close our resources - specifically, the stream reader
        /// </summary>
        public void Dispose()
        {
            _outstream.Flush();
            _outstream.Close();
            _outstream.Dispose();
        }

        /// <summary>
        /// Write one line to the file
        /// </summary>
        /// <param name="line">The array of values for this line</param>
        public void WriteLine(IEnumerable<object> line)
        {
            _outstream.WriteLine(MakeOutput(line));
        }

        /// <summary>
        /// Output a single field value as appropriate
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string MakeOutput(IEnumerable<object> line)
        {
            StringBuilder sb = new StringBuilder();
            foreach (object o in line)
            {
                // Null strings are just a delimiter
                if (o != null)
                {
                    string s = o.ToString();
                    if (s.Length > 0)
                    {
                        if (s.Contains(_separator) || s.Contains(_string_escape) || s.Contains(Environment.NewLine))
                        {
                            // Does this string contain any risky characters?  Risky is defined as delim, qual, or newline
                            sb.Append(_string_escape);

                            // Double up any _string_escapes that may occur
                            sb.Append(s.Replace(
                                _string_escape.ToString()
                                , _string_escape.ToString() + _string_escape.ToString()
                            ));
                            sb.Append(_string_escape);
                        }
                        else
                        {
                            sb.Append(s);
                        }
                    }
                }
                // Move to the next cell
                sb.Append(_separator);
            }
            // Subtract the trailing delimiter so we don't inadvertently add a column
            sb.Length -= 1;
            return sb.ToString();
        }
    }


    class SaveData
    {

        /// <summary>
        /// Write the data table to a stream in CSV format
        /// </summary>
        /// <param name="dt">The data table to write</param>
        /// <param name="filepath">The file path where the CSV text will be written</param>
        /// <param name="write_headers">Should we write the header line.</param>
        private static void DumpToDisk<T>(DataTable dt, string filepath, bool append, bool write_headers) 
        {
            using (var output_stream = new StreamWriter(filepath, append))
            {
                using (CSVWriter cw = new CSVWriter(output_stream, separator: ',', text_escape: '"'))
                {
                    cw.Write<T>(dt, write_header: write_headers);
                }
            }
        }

        /// <summary>
        /// Write the data table to a stream in CSV format
        /// </summary>
        /// <param name="dt">The data table to write</param>
        /// <param name="filepath">The file path where the CSV text will be written</param>
        public static void WriteFileCSV<T>(DataTable dt, string filepath) 
        {
            DumpToDisk<T>(dt, filepath, append:false, write_headers:true);
        }

        /// <summary>
        /// Write the data table to a stream in CSV format
        /// Append to existing file.
        /// </summary>
        /// <param name="dt">The data table to write</param>
        /// <param name="filepath">The file path where the CSV text will be written</param>
        public static void AppendFileCSV<T>(DataTable dt, string filepath) 
        {
            DumpToDisk<T>(dt, filepath, append:true, write_headers:false);
        }

        /// <summary>
        /// Opens the dialog to get the path at which to save the current data.
        /// </summary>
        /// <returns></returns>
        public static string SaveFileDialogGetPath()
        {
            SaveFileDialog file_dialog = new SaveFileDialog
            {
                InitialDirectory = Properties.Settings.Default.RecentDirectory,
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                FilterIndex = 0,
                RestoreDirectory = true,
                Title = "Select where to save your csv file...",
            };
            if (file_dialog.ShowDialog() == DialogResult.OK) {
                return file_dialog.FileName;
            }
            return null;
        }

        /// <summary>
        /// Opens the dialog to get the path at which to save the current data.
        /// Does not warn the user about overwriting a file.
        /// </summary>
        /// <returns></returns>
        public static string AppendFileDialogGetPath()
        {
            SaveFileDialog file_dialog = new SaveFileDialog
            {
                InitialDirectory = Properties.Settings.Default.RecentDirectory,
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                FilterIndex = 0,
                RestoreDirectory = true,
                Title = "Select where to save your csv file...",
                OverwritePrompt = false,
            };
            if (file_dialog.ShowDialog() == DialogResult.OK) {
                return file_dialog.FileName;
            }
            return null;
        }

    }
}
