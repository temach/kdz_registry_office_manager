namespace kdz_manager
{
    partial class EditorTextBox
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditorTextBox));
            this.textBox_FieldValue = new System.Windows.Forms.TextBox();
            this.label_FieldName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBox_FieldValue
            // 
            resources.ApplyResources(this.textBox_FieldValue, "textBox_FieldValue");
            this.textBox_FieldValue.Name = "textBox_FieldValue";
            // 
            // label_FieldName
            // 
            resources.ApplyResources(this.label_FieldName, "label_FieldName");
            this.label_FieldName.Name = "label_FieldName";
            // 
            // EditorTextBox
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label_FieldName);
            this.Controls.Add(this.textBox_FieldValue);
            this.Name = "EditorTextBox";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_FieldValue;
        private System.Windows.Forms.Label label_FieldName;
    }
}
