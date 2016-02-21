using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Reflection;
using System.ComponentModel;


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
        /// Write one line to the file
        /// </summary>
        /// <param name="line">The array of values for this line</param>
        public void WriteLine(IEnumerable<object> line)
        {
            _outstream.WriteLine(MakeOutput(line));
        }

        /// <summary>
        /// Write the data table to a stream in CSV format
        /// </summary>
        /// <param name="dt">The data table to write</param>
        /// <param name="sw">The stream where the CSV text will be written</param>
        public void Write(DataTable dt)
        {
            // Write headers, if the caller requested we do so
            List<string> headers = new List<string>();
            foreach (DataColumn col in dt.Columns)
            {
                headers.Add(col.ColumnName);
            }
            WriteLine(headers);
            // Now produce the rows
            foreach (DataRow dr in dt.Rows)
            {
                WriteLine(dr.ItemArray);
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
        /// <param name="sw">The stream where the CSV text will be written</param>
        public static void WriteToStream(DataTable dt, StreamWriter sw)
        {
            using (CSVWriter cw = new CSVWriter(sw, separator: ',', text_escape: '"'))
            {
                cw.Write(dt);
            }
        }
    }
}
