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
    /// Класс фактически разобрать строки в CSV -файл.
    /// </summary>
    public class CSVReader : IEnumerable<string[]>, IDisposable
    {
        // delimeter
        private char _separator = ',';
        // text qualifier
        private char _string_escape = '"';

        private StreamReader _instream;

        /// <summary>
        /// Построить новый читатель CSV от источника потока
        /// </summary>
        /// <param name="source"></param>
        /// <param name="separator"></param>
        /// <param name="text_escape"></param>
        public CSVReader(StreamReader source, char separator, char text_escape)
        {
            _instream = source;
            _separator = separator;
            _string_escape = text_escape;
        }

        /// <summary>
        /// перебора всех строк в этой CSV файла
        /// </summary>
        /// <returns>An array of all data columns in the line</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetLines().GetEnumerator();
        }

        /// <summary>
        /// перебора всех строк этого файла CSV
        /// </summary>
        /// <returns>An array of all data columns in the line</returns>
        IEnumerator<string[]> System.Collections.Generic.IEnumerable<string[]>.GetEnumerator()
        {
            return GetLines().GetEnumerator();
        }

        /// <summary>
        /// перебора всех строк этого файла CSV
        /// </summary>
        /// <returns>An array of all data columns in the line</returns>
        public IEnumerable<string[]> GetLines()
        {
            while (true)
            {

                // Attempt to parse the line successfully
                string[] line = NextLine();

                // If we were unable to parse the line successfully, that's all the file has
                if (line == null) break;

                // We got something - give the caller an object
                yield return line;
            }
        }

        /// <summary>
        /// Получить следующую строку из файла.
        /// </summary>
        /// <returns>One line from the file.</returns>
        public string[] NextLine()
        {
            return ParseMultiLine(_instream);
        }

        /// <summary>
        /// закрывать ресурсы - в частности, читатель поток
        /// </summary>
        public void Dispose()
        {
            _instream.Close();
            _instream.Dispose();
        }

        /// <summary>
        /// десериализации CSV файл в списке набранных объектов
        /// </summary>
        /// <typeparam name="T">The type of objects to deserialize from this CSV.</typeparam>
        /// <returns>An array of objects that were retrieved from the CSV file.</returns>
        public List<T> Deserialize<T>() where T : class, new()
        {
            List<T> result = new List<T>();
            Type return_type = typeof(T);

            // Read in the first line - we have to have headers!
            string[] first_line = ParseMultiLine(_instream);
            if (first_line == null) return result;
            int num_columns = first_line.Length;

            // Determine how to handle each column in the file - check properties, fields, and methods
            Type[] column_types = new Type[num_columns];
            TypeConverter[] column_convert = new TypeConverter[num_columns];
            PropertyInfo[] prop_handlers = new PropertyInfo[num_columns];
            FieldInfo[] field_handlers = new FieldInfo[num_columns];
            for (int i = 0; i < num_columns; i++)
            {
                prop_handlers[i] = return_type.GetProperty(first_line[i]);

                // If we failed to get a property handler (perhaps try a field handler?)
                if (prop_handlers[i] == null)
                {
                    throw new Exception(String.Format("The column header '{0}' was not found in the class '{1}'.", first_line[i], return_type.FullName));
                }
                else
                {
                    column_types[i] = prop_handlers[i].PropertyType;
                }

                // Retrieve a converter (a class that can convert between this type and other types)
                if (column_types[i] != null)
                {
                    column_convert[i] = TypeDescriptor.GetConverter(column_types[i]);
                    if (column_convert[i] == null)
                    {
                        throw new Exception(String.Format("The column {0} (type {1}) does not have a type converter.", first_line[i], column_types[i]));
                    }
                }
            }

            // Alright, let's retrieve CSV lines and parse each one!
            int row_num = 1;
            foreach (string[] line in GetLines())
            {
                // Does this line match the length of the first line?  Does the caller want us to complain?
                if (line.Count() != num_columns)
                {
                    throw new Exception(String.Format("Line #{0} contains {1} columns; expected {2}", row_num, line.Length, num_columns));
                }

                // Construct a new object and execute each column on it
                T obj = new T();
                for (int i = 0; i < Math.Min(line.Length, num_columns); i++)
                {
                    // Attempt to convert this to the specified type
                    object value = null;
                    if (column_convert[i] != null && column_convert[i].IsValid(line[i]))
                    {
                        value = column_convert[i].ConvertFromString(line[i]);
                    }
                    else
                    {
                        throw new Exception(String.Format("The value '{0}' cannot be converted to the type {1}.", line[i], column_types[i]));
                    }

                    // Can we set this value to the object as a property?
                    if (prop_handlers[i] != null)
                    {
                        prop_handlers[i].SetValue(obj, value);
                    }
                }

                // Keep track of where we are in the file
                result.Add(obj);
                row_num++;
            }

            // Here's your array!
            return result;
        }

        /// <summary>
        /// Разбор линию, значения которого могут включать новые символы строки или CR / LF
        /// </summary>
        /// <param name="sr"></param>
        /// <returns></returns>
        public string[] ParseMultiLine(StreamReader sr)
        {
            StringBuilder sb = new StringBuilder();
            string[] array = null;
            while (!sr.EndOfStream)
            {

                // Read in a line
                sb.Append(sr.ReadLine());

                // Does it parse?
                string s = sb.ToString();
                if (TryParseLine(s, out array))
                {
                    return array;
                }

                // We didn't succeed on the first try - our line must have an embedded newline in it
                sb.Append("\n");
            }

            // Fails to parse - return the best array we were able to get
            return array;
        }

        /// <summary>
        /// Разбор строки и вернуть массив, если это удастся, или в лучшем случае как мы можем получить
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public string[] ParseLine(string s)
        {
            string[] array = null;
            TryParseLine(s, out array);
            return array;
        }

        /// <summary>
        /// прочитал в строке текста, а также использовать функцию Add (), чтобы добавить эти элементы к существующей структуре CSV
        /// </summary>
        /// <param name="s"></param>
        public bool TryParseLine(string s, out string[] array)
        {
            bool success = true;
            bool trailing_separator = (s.Trim().Last() == _separator);
            List<string> list = new List<string>();
            StringBuilder work = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];

                // If we are starting a new field, is this field text qualified?
                if ((c == _string_escape) && (work.Length == 0))
                {
                    int p2;
                    while (true)
                    {
                        p2 = s.IndexOf(_string_escape, i + 1);

                        // for some reason, this text qualifier is broken
                        if (p2 < 0)
                        {
                            work.Append(s.Substring(i + 1));
                            i = s.Length;
                            success = false;
                            break;
                        }

                        // Append this qualified string
                        work.Append(s.Substring(i + 1, p2 - i - 1));
                        i = p2;

                        // If this is a double quote, keep going!
                        if (((p2 + 1) < s.Length) && (s[p2 + 1] == _string_escape))
                        {
                            work.Append(_string_escape);
                            i++;

                            // otherwise, this is a single qualifier, we're done
                        }
                        else
                        {
                            break;
                        }
                    }

                }
                else if (c == _separator)   // or Does this start a new field?
                {
                    list.Add(work.ToString());
                    work.Length = 0;

                    // Test for special case: when the user has written a casual comma, space, and text qualifier, skip the space
                    // Checks if the second parameter of the if statement will pass through successfully
                    // e.g. "bob", "mary", "bill"
                    if (i + 2 <= s.Length - 1)
                    {
                        if (s[i + 1].Equals(' ') && s[i + 2].Equals(_string_escape))
                        {
                            i++;
                        }
                    }
                }
                else
                {
                    work.Append(c);
                }
            }
            if (trailing_separator == false)
            {
                list.Add(work.ToString());
            }

            // Return the array we parsed
            array = list.ToArray();
            return success;
        }
    }


    class OpenData
    {

        /// <summary>
        /// Простой тип.
        /// </summary>
        public List<RegistryOfficeDataRow> Inner;
        /// <summary>
        /// Расширенный тип, который объединяет простые типы.
        /// </summary>
        public List<AdminAreaDataRow> Outer;
        /// <summary>
        /// Результат разбора файла CSV
        /// </summary>
        public List<MapDataRow> Raw;

        /// <summary>
        /// подключить несколько внутреннее до нескольких внешнее.
        /// </summary>
        private Dictionary<string, List<RegistryOfficeDataRow>> Outer2Inner;
        private Dictionary<string, int> Outer2InnerQty;

        // as you wish name for the column to sort by regions in an area
        public string Outer2InnerQtyDynamicColumn = "RegionsInAdmArea";
        // will group regions by this column
        public string OuterKeyColumn = "AdmAreaCode";
        public string FilterAdmArea = "AdmArea";
        public string FilterAdmAreaCode = "AdmAreaCode";

        public OpenData() { }

        /// <summary>
        /// Открыть файл, читать и проверять данные, сделать таблицу данных и запустить контроля инициализацый.
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public void ParseAsMapDataCSV(string filepath)
        {
            using (var input_stream = new StreamReader(filepath))
            {
                using (CSVReader cr = new CSVReader(input_stream, separator: ';', text_escape: '"'))
                {
                    // read into list then convert to data table
                    Raw = cr.Deserialize<MapDataRow>();
                }
            }
        }

        /// <summary>
        /// перерасчет динамический столбец строки.
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="oldrow"></param>
        /// <param name="newrow"></param>
        public void UpdateDynamicColumnOnRowEdit(DataTable dt, DataRow oldrow, DataRow newrow)
        {
            // oh my god....
            try
            {
                // deleting a row
                if (oldrow != null)
                {
                    Outer2InnerQty[(string)oldrow[OuterKeyColumn]] -= 1;
                }
                // adding a row
                if (newrow != null)
                {
                    Outer2InnerQty[(string)newrow[OuterKeyColumn]] += 1;
                }
                foreach (var row in dt.AsEnumerable())
                {
                    row[Outer2InnerQtyDynamicColumn] = Outer2InnerQty[(string)row[OuterKeyColumn]];
                }
            }
            catch (Exception) { }
        }

        private IEnumerable<string> GetUniqAdmCodes()
        {
            // this will be AdmCode
            return Raw.GroupBy(o => o.AdmAreaCode).Select(g => g.Key);
        }

        private IEnumerable<IGrouping<string,MapDataRow>> GetRegistryOfficeByArea()
        {
            // like a dictionary
            // area1 : [regoffice8, regoffice5, regoffice3]; area2 : [regoffice6, regoffice13];
            return Raw.GroupBy(o => o.AdmAreaCode);
        }

        /// <summary>
        /// Функция для заполнения внутренними и внешними списками на основе данных Raw списке.
        /// </summary>
        public void ImportProcessing()
        {
            Inner = new List<RegistryOfficeDataRow>();
            Outer = new List<AdminAreaDataRow>();
            Outer2Inner = new Dictionary<string,List<RegistryOfficeDataRow>>();
            foreach (var c in GetUniqAdmCodes())
            {
                Outer2Inner[c] = new List<RegistryOfficeDataRow>();
            }
            foreach (IGrouping<string,MapDataRow> grp in GetRegistryOfficeByArea())
            {
                foreach (MapDataRow raw in  grp)
                {
                    var reg_office = new RegistryOfficeDataRow(raw);
                    Outer2Inner[grp.Key].Add(reg_office);
                    // we must build the inner list here, then "reg_office" refers to the same object
                    Inner.Add(reg_office);
                }
            }
            // all of the dictionary tricks were necessary so that we can now this:
            Outer = GetRegistryOfficeByArea().Select(grp => grp.First())
                .Select(raw => new AdminAreaDataRow(raw)).ToList();
            Outer2InnerQty = new Dictionary<string,int>();
            foreach (AdminAreaDataRow area in Outer)
            {
                area.Offices = Outer2Inner[area.AdmAreaCode];
                Outer2InnerQty[area.AdmAreaCode] = area.GetQtyOfDistinctBookPrices();
            }
        }

        /// <summary>
        /// прочитал свойств Т и
        /// собрать DataTable, которая будет представлять его.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public DataTable ToDataTable<T>(IList<T> data)
        {
            // make columns from base type
            var table = EmptyTableFromType<T>();
            // add dynamic column (bad hack)
            var dcolumn = table.Columns.Add(Outer2InnerQtyDynamicColumn, typeof(int));
            // fill table with rows
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }
                // bad hack fill dynamic column
                row[Outer2InnerQtyDynamicColumn] = Outer2InnerQty[(string)row[OuterKeyColumn]];
                table.Rows.Add(row);
            }
            return table;
        }

        /// <summary>
        /// сделать пустую таблицу данных с макета, чтобы содержать типа T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public DataTable EmptyTableFromType<T>()
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            var table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
            {
               // handle nullable types
               table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            return table;
        }

        /// <summary>
        /// открывает диалоговое окно для получения путь файла для открытия из тэ пользователя.
        /// </summary>
        /// <returns></returns>
        public static string OpenFileDialogGetPath()
        {
            OpenFileDialog file_dialog = new OpenFileDialog
            {
                InitialDirectory = Properties.Settings.Default.RecentDirectory,
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                FilterIndex = 0,
                RestoreDirectory = true,
                Title = "Select a csv file...",
            };
            if (file_dialog.ShowDialog() == DialogResult.OK) {
                return file_dialog.FileName;
            }
            return null;
        }

    }
}
