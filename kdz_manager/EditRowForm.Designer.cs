namespace kdz_manager
{
    partial class EditRowForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button_SubmitData = new System.Windows.Forms.Button();
            this.editorTextBox_Template = new kdz_manager.EditorTextBox();
            this.SuspendLayout();
            // 
            // button_SubmitData
            // 
            this.button_SubmitData.Location = new System.Drawing.Point(50, 12);
            this.button_SubmitData.Name = "button_SubmitData";
            this.button_SubmitData.Size = new System.Drawing.Size(95, 30);
            this.button_SubmitData.TabIndex = 0;
            this.button_SubmitData.Text = "Submit data";
            this.button_SubmitData.UseVisualStyleBackColor = true;
            this.button_SubmitData.Click += new System.EventHandler(this.button_SubmitData_Click);
            // 
            // editorTextBox_Template
            // 
            this.editorTextBox_Template.Label = "______";
            this.editorTextBox_Template.Location = new System.Drawing.Point(12, 48);
            this.editorTextBox_Template.Name = "editorTextBox_Template";
            this.editorTextBox_Template.Size = new System.Drawing.Size(460, 52);
            this.editorTextBox_Template.TabIndex = 1;
            this.editorTextBox_Template.Visible = false;
            // 
            // EditRowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 427);
            this.Controls.Add(this.editorTextBox_Template);
            this.Controls.Add(this.button_SubmitData);
            this.Name = "EditRowForm";
            this.Text = "EditRowForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_SubmitData;
        private EditorTextBox editorTextBox_Template;
    }
}