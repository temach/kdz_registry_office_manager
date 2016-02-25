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
using System.Reflection;

namespace kdz_manager
{
    public partial class EditorTextBox : UserControl
    {
        public static Color Success = Color.LightGreen;
        public static Color Failure = Color.LightPink;
        public static Color Default = Color.White;

        /// <summary>
        /// Если мы сможем успешно связываться с DataTable
        /// (имеется в виду
        /// </summary>
        public bool BindingOK { get; protected set; }

        /// <summary>
        /// Строка для описания окно ввода
        /// </summary>
        public string Label
        {
            get { return this.label_FieldName.Text; }
            set { this.label_FieldName.Text = value; }
        }

        /// <summary>
        /// С помощью этого конструктора, помните, чтобы связать
        /// этот контроль перед использованием (назовем Bind DataRowView )
        /// </summary>
        public EditorTextBox()
        {
            InitializeComponent();
        }

        /// <summary>
        /// связать этот контроль на объекте DataRowView
        /// </summary>
        /// <param name="rowview"></param>
        public void BindToDataRowView(DataRowView rowview, string column_name)
        {
            var binding = new Binding("Text", rowview, column_name);
            // so we get immediate feed back
            binding.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            // se we can highlight nicely
            binding.FormattingEnabled = true;
            binding.BindingComplete += Binding_BindingComplete;
            // so we push to control only once
            binding.Format += Binding_Format;
            this.textBox_FieldValue.DataBindings.Add(binding);
        }

        /// <summary>
        /// происходит при нажатии из DataTable с контролем.
        /// Мы только позволяют выдвинуть один раз.  Тогда пусть пользователь редактировать материал.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Binding_Format(object sender, ConvertEventArgs e)
        {
            ((Binding)sender).ControlUpdateMode = ControlUpdateMode.Never;
        }

        /// <summary>
        /// происходит при связывании закончилась, показать пользователю, если мы смогли записать значения или нет.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Binding_BindingComplete(object sender, BindingCompleteEventArgs e)
        {
            // when we are pushing from control to DataTable
            if (e.BindingCompleteContext == BindingCompleteContext.DataSourceUpdate)
            {
                if (e.BindingCompleteState == BindingCompleteState.Success)
                { 
                    this.BindingOK = true;
                    this.textBox_FieldValue.BackColor = Success;
                }
                else
                {
                    // ignore erorrs, they will be brought to user once again when he tries to submit.
                    e.Cancel = false;
                    this.BindingOK = false;
                    this.textBox_FieldValue.BackColor = Failure;
                }
            }
        }

        /// <summary>
        /// Сброс свойства элемента управления на значения по умолчанию.
        /// Это полезно, так как мы можем затем использовать элементы управления.
        /// </summary>
        public void UnbindFromData()
        {
            this.textBox_FieldValue.DataBindings.Clear();
            this.textBox_FieldValue.BackColor = Default;
        }

    }
}
