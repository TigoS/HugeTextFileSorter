using System.Diagnostics;
using System.Text;

namespace TestFileGenerator
{
    public partial class frmMain : Form
    {
        private readonly Stopwatch sw = new();

        private long generatedLinesCount;

        public frmMain()
        {
            InitializeComponent();
        }

        private void GenerateTestFile(
            long fileSize,
            int maxNumber = int.MaxValue,
            int maxWordsCount = 100,
            int maxWordLength = 12)
        {
            StringBuilder sb = new StringBuilder();
            FileInfo fileInfo = new FileInfo(saveFileDialog1.FileName);

            while (fileInfo.Length < fileSize - ushort.MaxValue)
            {
                generatedLinesCount++;

                if (sb.Length >= ushort.MaxValue)
                {
                    File.AppendAllText(saveFileDialog1.FileName, sb.ToString());

                    sb.Clear();

                    fileInfo = new FileInfo(saveFileDialog1.FileName);
                }

                sb.AppendLine(string.Format("{0}. {1}",
                    RandomLineGenerator.GetRandomNumber(maxNumber),
                    RandomLineGenerator.GetRandomString(maxWordsCount, maxWordLength)));
            }
        }

        private void btnGenerateAndSave_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                gbGenerate.Enabled = false;

                long fileSize = ((long)nudFileSize.Value);
                int maxNumber = (int)nudMaxNumber.Value;
                int maxWordsCount = (int)nudMaxWordsCount.Value;
                int maxWordLength = (int)nudMaxWordLength.Value;

                Log($"Generating test file. File size: {fileSize} Bytes; Max Number: {maxNumber}; Max Words Count: {maxWordsCount}; Max Word Length: {maxWordLength}.");

                var fileStream = File.Create(saveFileDialog1.FileName);
                fileStream.Close();
                fileStream.Dispose();

                sw.Reset();
                sw.Start();

                GenerateTestFile(fileSize, maxNumber, maxWordsCount, maxWordLength);

                sw.Stop();

                Log($"File successfully Generated and Saved in: {sw.Elapsed:c}");

                gbGenerate.Enabled = true;
            }
        }

        private void Log(string message, Exception? ex = null)
        {
            try
            {
                txtLog.AppendText($"{DateTime.Now:O}: {message}{Environment.NewLine}");

                if (ex is not null)
                {
                    txtLog.AppendText($"\tInternal error: {ex.Message}{Environment.NewLine}");
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}