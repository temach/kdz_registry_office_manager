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
        /// If we can bind succesfully to the DataTable 
        /// (meaning: if the entered data is valid)
        /// </summary>
        public bool BindingOK { get; protected set; }

        /// <summary>
        /// String to describe the input box
        /// </summary>
        public string Label
        {
            get { return this.label_FieldName.Text; }
            set { this.label_FieldName.Text = value; }
        }

        /// <summary>
        /// With this constructor, remember to bind 
        /// this control before use (call BindToDataRowView)
        /// </summary>
        public EditorTextBox()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Bind this control to DataRowView object
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
        /// Occurs when pushing from DataTable to Control.
        /// We only allow you to push once. Then let the user edit stuff.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Binding_Format(object sender, ConvertEventArgs e)
        {
            ((Binding)sender).ControlUpdateMode = ControlUpdateMode.Never;
        }

        /// <summary>
        /// Occurs when binding is over, show the user if we were able to write the values or not.
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
        /// Resets control properties to default values.
        /// This is useful since we can then reuse the controls.
        /// </summary>
        public void UnbindFromData()
        {
            this.textBox_FieldValue.DataBindings.Clear();
            this.textBox_FieldValue.BackColor = Default;
        }

    }
}
