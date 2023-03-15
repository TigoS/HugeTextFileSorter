namespace TestFileGenerator
{
    partial class frmMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            gbGenerate = new GroupBox();
            gbParameters = new GroupBox();
            tableLayoutPanel1 = new TableLayoutPanel();
            label6 = new Label();
            nudFileSize = new NumericUpDown();
            label4 = new Label();
            label2 = new Label();
            label3 = new Label();
            nudMaxNumber = new NumericUpDown();
            nudMaxWordsCount = new NumericUpDown();
            nudMaxWordLength = new NumericUpDown();
            btnGenerate = new Button();
            txtLog = new TextBox();
            saveFileDialog1 = new SaveFileDialog();
            gbGenerate.SuspendLayout();
            gbParameters.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudFileSize).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudMaxNumber).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudMaxWordsCount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudMaxWordLength).BeginInit();
            SuspendLayout();
            // 
            // gbGenerate
            // 
            gbGenerate.Controls.Add(gbParameters);
            gbGenerate.Controls.Add(btnGenerate);
            gbGenerate.Dock = DockStyle.Top;
            gbGenerate.Location = new Point(0, 0);
            gbGenerate.Name = "gbGenerate";
            gbGenerate.Size = new Size(822, 194);
            gbGenerate.TabIndex = 2;
            gbGenerate.TabStop = false;
            gbGenerate.Text = "Generate test file";
            // 
            // gbParameters
            // 
            gbParameters.Controls.Add(tableLayoutPanel1);
            gbParameters.Dock = DockStyle.Left;
            gbParameters.Location = new Point(3, 23);
            gbParameters.Name = "gbParameters";
            gbParameters.Size = new Size(509, 168);
            gbParameters.TabIndex = 9;
            gbParameters.TabStop = false;
            gbParameters.Text = "Parameters";
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(label6, 0, 0);
            tableLayoutPanel1.Controls.Add(nudFileSize, 1, 0);
            tableLayoutPanel1.Controls.Add(label4, 0, 1);
            tableLayoutPanel1.Controls.Add(label2, 0, 2);
            tableLayoutPanel1.Controls.Add(label3, 0, 3);
            tableLayoutPanel1.Controls.Add(nudMaxNumber, 1, 1);
            tableLayoutPanel1.Controls.Add(nudMaxWordsCount, 1, 2);
            tableLayoutPanel1.Controls.Add(nudMaxWordLength, 1, 3);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(3, 23);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.Size = new Size(503, 142);
            tableLayoutPanel1.TabIndex = 5;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Dock = DockStyle.Right;
            label6.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label6.Location = new Point(75, 0);
            label6.Name = "label6";
            label6.Size = new Size(102, 35);
            label6.TabIndex = 12;
            label6.Text = "File Size (MB):";
            label6.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // nudFileSize
            // 
            nudFileSize.Location = new Point(183, 3);
            nudFileSize.Maximum = new decimal(new int[] { 102400, 0, 0, 0 });
            nudFileSize.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudFileSize.Name = "nudFileSize";
            nudFileSize.Size = new Size(114, 27);
            nudFileSize.TabIndex = 15;
            nudFileSize.Value = new decimal(new int[] { 10000, 0, 0, 0 });
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Dock = DockStyle.Right;
            label4.Location = new Point(32, 35);
            label4.Name = "label4";
            label4.Size = new Size(145, 35);
            label4.TabIndex = 11;
            label4.Text = "Max Positive Integer:";
            label4.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Dock = DockStyle.Right;
            label2.Location = new Point(48, 70);
            label2.Name = "label2";
            label2.Size = new Size(129, 35);
            label2.TabIndex = 3;
            label2.Text = "Max Words Count:";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Dock = DockStyle.Right;
            label3.Location = new Point(48, 105);
            label3.Name = "label3";
            label3.Size = new Size(129, 37);
            label3.TabIndex = 9;
            label3.Text = "Max Word Length:";
            label3.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // nudMaxNumber
            // 
            nudMaxNumber.Location = new Point(183, 38);
            nudMaxNumber.Maximum = new decimal(new int[] { 1000000000, 0, 0, 0 });
            nudMaxNumber.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudMaxNumber.Name = "nudMaxNumber";
            nudMaxNumber.Size = new Size(114, 27);
            nudMaxNumber.TabIndex = 3;
            nudMaxNumber.Value = new decimal(new int[] { 1000000000, 0, 0, 0 });
            // 
            // nudMaxWordsCount
            // 
            nudMaxWordsCount.Location = new Point(183, 73);
            nudMaxWordsCount.Maximum = new decimal(new int[] { 200, 0, 0, 0 });
            nudMaxWordsCount.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudMaxWordsCount.Name = "nudMaxWordsCount";
            nudMaxWordsCount.Size = new Size(59, 27);
            nudMaxWordsCount.TabIndex = 4;
            nudMaxWordsCount.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // nudMaxWordLength
            // 
            nudMaxWordLength.Location = new Point(183, 108);
            nudMaxWordLength.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
            nudMaxWordLength.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudMaxWordLength.Name = "nudMaxWordLength";
            nudMaxWordLength.Size = new Size(59, 27);
            nudMaxWordLength.TabIndex = 10;
            nudMaxWordLength.Value = new decimal(new int[] { 12, 0, 0, 0 });
            // 
            // btnGenerate
            // 
            btnGenerate.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnGenerate.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            btnGenerate.Location = new Point(650, 153);
            btnGenerate.Name = "btnGenerate";
            btnGenerate.Size = new Size(160, 32);
            btnGenerate.TabIndex = 2;
            btnGenerate.Text = "Generate and Save";
            btnGenerate.UseVisualStyleBackColor = true;
            btnGenerate.Click += btnGenerateAndSave_Click;
            // 
            // txtLog
            // 
            txtLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtLog.Location = new Point(0, 200);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = ScrollBars.Both;
            txtLog.Size = new Size(822, 233);
            txtLog.TabIndex = 0;
            // 
            // saveFileDialog1
            // 
            saveFileDialog1.DefaultExt = "*.txt";
            saveFileDialog1.Filter = "Text files|*.txt|All files|*.*";
            // 
            // frmMain
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(822, 433);
            Controls.Add(gbGenerate);
            Controls.Add(txtLog);
            MinimumSize = new Size(840, 480);
            Name = "frmMain";
            Text = "Test File Generator";
            gbGenerate.ResumeLayout(false);
            gbParameters.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudFileSize).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudMaxNumber).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudMaxWordsCount).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudMaxWordLength).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private GroupBox gbGenerate;
        private Button btnGenerate;
        private TextBox txtLog;
        private SaveFileDialog saveFileDialog1;
        private TableLayoutPanel tableLayoutPanel1;
        private Label label4;
        private Label label2;
        private NumericUpDown nudMaxWordsCount;
        private Label label3;
        private NumericUpDown nudMaxWordLength;
        private NumericUpDown nudMaxNumber;
        private GroupBox gbParameters;
        private Label label6;
        private NumericUpDown nudFileSize;
    }
}