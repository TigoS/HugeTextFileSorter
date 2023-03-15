using System.Diagnostics;
using System.Text;
using static TestFileGenerator.FileSizeExtensions;

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

                tsslFileSize.Text = string.Empty;
                tsslLinesCount.Text = string.Empty;
                tsslExecutionTime.Text = string.Empty;

                long fileSize = ((double)nudFileSize.Value).GetFileSizeInBytes(MetricPrefixes.Mega);
                int maxNumber = (int)nudMaxNumber.Value;
                int maxWordsCount = (int)nudMaxWordsCount.Value;
                int maxWordLength = (int)nudMaxWordLength.Value;

                Log($"Generating test file. File size: {((double)fileSize).FormatFileSize()}; Max Number: {maxNumber}; Max Words Count: {maxWordsCount}; Max Word Length: {maxWordLength}.");

                var cts = new CancellationTokenSource();

                var fileStream = File.Create(saveFileDialog1.FileName);
                fileStream.Close();
                fileStream.Dispose();

                sw.Reset();
                sw.Start();

                Task.Run(() =>
                        GenerateTestFile(fileSize, maxNumber, maxWordsCount, maxWordLength),
                    cts.Token)
                    .ContinueWith(_ =>
                    {
                        Invoke(() =>
                        {
                            sw.Stop();

                            Log($"File successfully Generated and Saved in: {sw.Elapsed:c}");

                            tsslExecutionTime.Text = sw.Elapsed.ToString("c");
                            tsslLinesCount.Text = generatedLinesCount.ToString("##,###");

                            FileInfo fileInfo = new FileInfo(saveFileDialog1.FileName);
                            tsslFileSize.Text = ((double)fileInfo.Length).FormatFileSize();

                            gbGenerate.Enabled = true;
                        });
                    })
                    .ContinueWith(_ => Invoke(() => cts.Dispose()));
            }
        }

        private void nudFileSize_ValueChanged(object sender, EventArgs e)
        {
            lblFileSize.Text = ((double)nudFileSize.Value).FormatFileSize(benchmark: MetricPrefixes.Mega);
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