using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;


namespace kdz_manager
{
    public partial class EditRowForm : Form
    {
        
        // All the text inputs
        List<EditorTextBox> TextInputs = new List<EditorTextBox>();

        public EditRowForm(Type rowtype)
        {
            InitializeComponent();
            GenListTextLabels(rowtype);
        }

        /// <summary>
        /// Generate input fields based on class type. 
        /// So one input field corresponds to class property.
        /// </summary>
        /// <typeparam name="Type">The type which should be filled with information.</typeparam>
        public void GenListTextLabels(Type rowtype)
        {
            TypeConverter[] column_convert = rowtype.GetProperties()
                .Select(prop => TypeDescriptor.GetConverter(prop.PropertyType)).ToArray();
            PropertyInfo[] row_properties = rowtype.GetProperties();
            // now generate the actual controls
            var template_input = this.editorTextBox_Template;
            for (int i = 0; i < row_properties.Count(); i++)
            {
                var txtbox = new EditorTextBox();
                txtbox.Location = new Point(template_input.Location.X
                    , template_input.Location.Y + template_input.Height * i);
                txtbox.Size = template_input.Size;
                var prop_type_str = row_properties[i].PropertyType.ToString().Split('.').Last();
                txtbox.Label = row_properties[i].Name + ": " + prop_type_str;
                txtbox.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
                TextInputs.Add(txtbox);
            }
            // add created controls to this form.
            this.Controls.AddRange(TextInputs.ToArray());
        }

        /// <summary>
        /// Set text inputs to track the state of data grid.
        /// This is to allow the user to edit the records.
        /// Resets DataBindings and BackColor.
        /// Must reset the controls since we resue them.
        /// </summary>
        /// <param name="rowview"></param>
        public void ReBindControlsToDataRow(DataRowView rowview)
        {
            var columns = rowview.DataView.Table.Columns;
            for (int i = 0; i < columns.Count; i++)
            {
                TextInputs[i].UnbindFromData();
                TextInputs[i].BindToDataRowView(rowview, columns[i].ColumnName);
            }
        }

        /// <summary>
        /// User clicked the submit data button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_SubmitData_Click(object sender, EventArgs e)
        {
            bool allok = TextInputs.All(txtbox => txtbox.BindingOK);
            if (allok)
            {
                // this should automagically close the form (else call this.Close())
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Some input is not valid. Please fix the errors and try again.");
            }
        }

    }
}
