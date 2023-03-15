using System.Diagnostics;
using static TestFileGenerator.FileSizeExtensions;

namespace TestFileGenerator
{
    // TODO: On Max Number, Max Words Count & Max Word Length change calculate the estimated single line size in bytes
    public partial class frmMain : Form
    {
        private readonly Stopwatch sw = new();

        public frmMain()
        {
            InitializeComponent();
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
                            RandomFileGenerator.GenerateTestFile(saveFileDialog1.FileName, fileSize, maxNumber,
                                maxWordsCount, maxWordLength),
                        cts.Token)
                    .ContinueWith(generatedLinesCount =>
                    {
                        Invoke(() =>
                        {
                            sw.Stop();

                            Log($"File successfully Generated and Saved in: {sw.Elapsed:c}");

                            tsslExecutionTime.Text = sw.Elapsed.ToString("c");

                            if (generatedLinesCount is {IsCanceled: false, IsFaulted: false})
                            {
                                tsslLinesCount.Text = generatedLinesCount.Result.ToString("##,###");
                            }

                            FileInfo fileInfo = new FileInfo(saveFileDialog1.FileName);
                            tsslFileSize.Text = ((double) fileInfo.Length).FormatFileSize();

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