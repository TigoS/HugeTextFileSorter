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
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            tsslLinesCount = new ToolStripStatusLabel();
            toolStripStatusLabel2 = new ToolStripStatusLabel();
            toolStripStatusLabel3 = new ToolStripStatusLabel();
            tsslFileSize = new ToolStripStatusLabel();
            toolStripStatusLabel4 = new ToolStripStatusLabel();
            toolStripStatusLabel5 = new ToolStripStatusLabel();
            tsslExecutionTime = new ToolStripStatusLabel();
            gbGenerate = new GroupBox();
            gbParameters = new GroupBox();
            tableLayoutPanel1 = new TableLayoutPanel();
            lblFileSize = new Label();
            label3 = new Label();
            label2 = new Label();
            nudMaxWordLength = new NumericUpDown();
            nudMaxWordsCount = new NumericUpDown();
            label6 = new Label();
            nudMaxNumber = new NumericUpDown();
            label4 = new Label();
            nudFileSize = new NumericUpDown();
            btnGenerate = new Button();
            txtLog = new TextBox();
            saveFileDialog1 = new SaveFileDialog();
            statusStrip1.SuspendLayout();
            gbGenerate.SuspendLayout();
            gbParameters.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudMaxWordLength).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudMaxWordsCount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudMaxNumber).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudFileSize).BeginInit();
            SuspendLayout();
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(20, 20);
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1, tsslLinesCount, toolStripStatusLabel2, toolStripStatusLabel3, tsslFileSize, toolStripStatusLabel4, toolStripStatusLabel5, tsslExecutionTime });
            statusStrip1.Location = new Point(0, 407);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(822, 26);
            statusStrip1.TabIndex = 1;
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(88, 20);
            toolStripStatusLabel1.Text = "Lines Count:";
            // 
            // tsslLinesCount
            // 
            tsslLinesCount.AutoSize = false;
            tsslLinesCount.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            tsslLinesCount.Name = "tsslLinesCount";
            tsslLinesCount.Size = new Size(100, 20);
            tsslLinesCount.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabel2
            // 
            toolStripStatusLabel2.AutoSize = false;
            toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            toolStripStatusLabel2.Size = new Size(10, 20);
            // 
            // toolStripStatusLabel3
            // 
            toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            toolStripStatusLabel3.Size = new Size(66, 20);
            toolStripStatusLabel3.Text = "File Size:";
            // 
            // tsslFileSize
            // 
            tsslFileSize.AutoSize = false;
            tsslFileSize.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            tsslFileSize.Name = "tsslFileSize";
            tsslFileSize.Size = new Size(220, 20);
            tsslFileSize.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabel4
            // 
            toolStripStatusLabel4.AutoSize = false;
            toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            toolStripStatusLabel4.Size = new Size(10, 20);
            // 
            // toolStripStatusLabel5
            // 
            toolStripStatusLabel5.Name = "toolStripStatusLabel5";
            toolStripStatusLabel5.Size = new Size(113, 20);
            toolStripStatusLabel5.Text = "Execution Time:";
            // 
            // tsslExecutionTime
            // 
            tsslExecutionTime.AutoSize = false;
            tsslExecutionTime.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            tsslExecutionTime.Name = "tsslExecutionTime";
            tsslExecutionTime.Size = new Size(150, 20);
            tsslExecutionTime.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // gbGenerate
            // 
            gbGenerate.Controls.Add(gbParameters);
            gbGenerate.Controls.Add(btnGenerate);
            gbGenerate.Dock = DockStyle.Top;
            gbGenerate.Location = new Point(0, 0);
            gbGenerate.Name = "gbGenerate";
            gbGenerate.Size = new Size(822, 236);
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
            gbParameters.Size = new Size(509, 210);
            gbParameters.TabIndex = 9;
            gbParameters.TabStop = false;
            gbParameters.Text = "Parameters";
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(lblFileSize, 2, 0);
            tableLayoutPanel1.Controls.Add(label3, 0, 5);
            tableLayoutPanel1.Controls.Add(label2, 0, 4);
            tableLayoutPanel1.Controls.Add(nudMaxWordLength, 1, 5);
            tableLayoutPanel1.Controls.Add(nudMaxWordsCount, 1, 4);
            tableLayoutPanel1.Controls.Add(label6, 0, 0);
            tableLayoutPanel1.Controls.Add(nudMaxNumber, 1, 3);
            tableLayoutPanel1.Controls.Add(label4, 0, 3);
            tableLayoutPanel1.Controls.Add(nudFileSize, 1, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(3, 23);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 6;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 18F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 18F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 18F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 18F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 18F));
            tableLayoutPanel1.Size = new Size(503, 184);
            tableLayoutPanel1.TabIndex = 5;
            // 
            // lblFileSize
            // 
            lblFileSize.AutoSize = true;
            lblFileSize.Dock = DockStyle.Left;
            lblFileSize.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            lblFileSize.Location = new Point(303, 0);
            lblFileSize.Name = "lblFileSize";
            lblFileSize.Size = new Size(64, 33);
            lblFileSize.TabIndex = 10;
            lblFileSize.Text = "100 MB";
            lblFileSize.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Dock = DockStyle.Right;
            label3.Location = new Point(48, 150);
            label3.Name = "label3";
            label3.Size = new Size(129, 34);
            label3.TabIndex = 9;
            label3.Text = "Max Word Length:";
            label3.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Dock = DockStyle.Right;
            label2.Location = new Point(48, 117);
            label2.Name = "label2";
            label2.Size = new Size(129, 33);
            label2.TabIndex = 3;
            label2.Text = "Max Words Count:";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // nudMaxWordLength
            // 
            nudMaxWordLength.Location = new Point(183, 153);
            nudMaxWordLength.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
            nudMaxWordLength.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudMaxWordLength.Name = "nudMaxWordLength";
            nudMaxWordLength.Size = new Size(59, 27);
            nudMaxWordLength.TabIndex = 10;
            nudMaxWordLength.Value = new decimal(new int[] { 12, 0, 0, 0 });
            // 
            // nudMaxWordsCount
            // 
            nudMaxWordsCount.Location = new Point(183, 120);
            nudMaxWordsCount.Maximum = new decimal(new int[] { 200, 0, 0, 0 });
            nudMaxWordsCount.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudMaxWordsCount.Name = "nudMaxWordsCount";
            nudMaxWordsCount.Size = new Size(59, 27);
            nudMaxWordsCount.TabIndex = 4;
            nudMaxWordsCount.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Dock = DockStyle.Right;
            label6.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label6.Location = new Point(69, 0);
            label6.Name = "label6";
            label6.Size = new Size(108, 33);
            label6.TabIndex = 12;
            label6.Text = "File Size (MB):";
            label6.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // nudMaxNumber
            // 
            nudMaxNumber.Location = new Point(183, 87);
            nudMaxNumber.Maximum = new decimal(new int[] { 1000000000, 0, 0, 0 });
            nudMaxNumber.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudMaxNumber.Name = "nudMaxNumber";
            nudMaxNumber.Size = new Size(114, 27);
            nudMaxNumber.TabIndex = 3;
            nudMaxNumber.Value = new decimal(new int[] { 1000000000, 0, 0, 0 });
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Dock = DockStyle.Right;
            label4.Location = new Point(32, 84);
            label4.Name = "label4";
            label4.Size = new Size(145, 33);
            label4.TabIndex = 11;
            label4.Text = "Max Positive Integer:";
            label4.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // nudFileSize
            // 
            nudFileSize.Location = new Point(183, 3);
            nudFileSize.Maximum = new decimal(new int[] { 102400, 0, 0, 0 });
            nudFileSize.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudFileSize.Name = "nudFileSize";
            nudFileSize.Size = new Size(114, 27);
            nudFileSize.TabIndex = 15;
            nudFileSize.Value = new decimal(new int[] { 100, 0, 0, 0 });
            nudFileSize.ValueChanged += nudFileSize_ValueChanged;
            // 
            // btnGenerate
            // 
            btnGenerate.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnGenerate.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            btnGenerate.Location = new Point(650, 198);
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
            txtLog.Location = new Point(0, 242);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = ScrollBars.Both;
            txtLog.Size = new Size(822, 162);
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
            Controls.Add(statusStrip1);
            Controls.Add(txtLog);
            MinimumSize = new Size(840, 480);
            Name = "frmMain";
            Text = "Test File Generator";
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            gbGenerate.ResumeLayout(false);
            gbParameters.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudMaxWordLength).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudMaxWordsCount).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudMaxNumber).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudFileSize).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripStatusLabel tsslLinesCount;
        private ToolStripStatusLabel toolStripStatusLabel2;
        private ToolStripStatusLabel toolStripStatusLabel3;
        private ToolStripStatusLabel tsslFileSize;
        private ToolStripStatusLabel toolStripStatusLabel4;
        private ToolStripStatusLabel toolStripStatusLabel5;
        private ToolStripStatusLabel tsslExecutionTime;
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
        private Label lblFileSize;
    }
}