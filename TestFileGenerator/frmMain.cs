using System.Diagnostics;
using MultiSorterLib;
using static MultiSorterLib.FileSizeExtensions;

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
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
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

                sw.Reset();
                sw.Start();

                Task.Run(() =>
                            RandomFileGenerator.GenerateTestFile(
                                saveFileDialog.FileName,
                                fileSize,
                                maxNumber,
                                maxWordsCount,
                                maxWordLength),
                        cts.Token)
                    .ContinueWith(generatedLinesCountTask =>
                    {
                        Invoke(() =>
                        {
                            sw.Stop();

                            Log($"File successfully Generated and Saved in: {sw.Elapsed:c}");

                            tsslExecutionTime.Text = sw.Elapsed.ToString("c");

                            if (generatedLinesCountTask is { IsCanceled: false, IsFaulted: false })
                            {
                                var generatedLinesCount = generatedLinesCountTask.Result;
                                tsslLinesCount.Text = generatedLinesCount.ToString("##,###");

                                if (generatedLinesCount < 1)
                                {
                                    Log($"Failed to generate a new file: '{saveFileDialog.FileName}'");
                                }
                            }

                            FileInfo fileInfo = new FileInfo(saveFileDialog.FileName);
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
                    txtLog.AppendText($"\tUnderlying error: {ex.Message}{Environment.NewLine}");
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}