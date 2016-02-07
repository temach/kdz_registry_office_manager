namespace kdz_manager
{
    partial class Form1
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.button_AddRecord = new System.Windows.Forms.Button();
            this.button_DeleteRecord = new System.Windows.Forms.Button();
            this.button_EditRecord = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.trackBar_QtyOfRecords = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.richTextBox_FilterInput = new System.Windows.Forms.RichTextBox();
            this.button_ApplyFilter = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_QtyOfRecords)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(172, 44);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(480, 499);
            this.dataGridView1.TabIndex = 0;
            // 
            // button_AddRecord
            // 
            this.button_AddRecord.Location = new System.Drawing.Point(18, 49);
            this.button_AddRecord.Name = "button_AddRecord";
            this.button_AddRecord.Size = new System.Drawing.Size(105, 24);
            this.button_AddRecord.TabIndex = 1;
            this.button_AddRecord.Text = "Add record";
            this.button_AddRecord.UseVisualStyleBackColor = true;
            // 
            // button_DeleteRecord
            // 
            this.button_DeleteRecord.Location = new System.Drawing.Point(18, 79);
            this.button_DeleteRecord.Name = "button_DeleteRecord";
            this.button_DeleteRecord.Size = new System.Drawing.Size(105, 23);
            this.button_DeleteRecord.TabIndex = 2;
            this.button_DeleteRecord.Text = "Delete current record";
            this.button_DeleteRecord.UseVisualStyleBackColor = true;
            // 
            // button_EditRecord
            // 
            this.button_EditRecord.Location = new System.Drawing.Point(18, 108);
            this.button_EditRecord.Name = "button_EditRecord";
            this.button_EditRecord.Size = new System.Drawing.Size(105, 23);
            this.button_EditRecord.TabIndex = 3;
            this.button_EditRecord.Text = "Edit current record";
            this.button_EditRecord.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button_ApplyFilter);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.trackBar_QtyOfRecords);
            this.panel1.Controls.Add(this.button_DeleteRecord);
            this.panel1.Controls.Add(this.button_EditRecord);
            this.panel1.Controls.Add(this.button_AddRecord);
            this.panel1.Location = new System.Drawing.Point(12, 44);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(154, 238);
            this.panel1.TabIndex = 4;
            // 
            // trackBar_QtyOfRecords
            // 
            this.trackBar_QtyOfRecords.Location = new System.Drawing.Point(19, 188);
            this.trackBar_QtyOfRecords.Name = "trackBar_QtyOfRecords";
            this.trackBar_QtyOfRecords.Size = new System.Drawing.Size(104, 45);
            this.trackBar_QtyOfRecords.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 172);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(141, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Number of records to display";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Filter";
            // 
            // richTextBox_FilterInput
            // 
            this.richTextBox_FilterInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox_FilterInput.Location = new System.Drawing.Point(53, 12);
            this.richTextBox_FilterInput.Name = "richTextBox_FilterInput";
            this.richTextBox_FilterInput.Size = new System.Drawing.Size(599, 26);
            this.richTextBox_FilterInput.TabIndex = 6;
            this.richTextBox_FilterInput.Text = "";
            // 
            // button_ApplyFilter
            // 
            this.button_ApplyFilter.Location = new System.Drawing.Point(19, 20);
            this.button_ApplyFilter.Name = "button_ApplyFilter";
            this.button_ApplyFilter.Size = new System.Drawing.Size(104, 23);
            this.button_ApplyFilter.TabIndex = 7;
            this.button_ApplyFilter.Text = "Apply Filter";
            this.button_ApplyFilter.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(664, 555);
            this.Controls.Add(this.richTextBox_FilterInput);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.dataGridView1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_QtyOfRecords)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button button_AddRecord;
        private System.Windows.Forms.Button button_DeleteRecord;
        private System.Windows.Forms.Button button_EditRecord;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar trackBar_QtyOfRecords;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox richTextBox_FilterInput;
        private System.Windows.Forms.Button button_ApplyFilter;
    }
}

