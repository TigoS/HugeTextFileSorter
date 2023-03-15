using System.Diagnostics;
using MultiSorterLib;
using TestFileGenerator.Properties;
using static MultiSorterLib.FileSizeExtensions;

namespace TestFileGenerator
{
    public partial class frmMain : Form
    {
        private readonly Stopwatch sw = new();

        private long generatedLinesCount;

        public frmMain()
        {
            InitializeComponent();

            nudLineParametersChanged(this, EventArgs.Empty);
        }

        private void btnGenerateAndSave_Click(object sender, EventArgs e)
        {
            var cts = new CancellationTokenSource();

            if (btnGenerateAndSave.Text.Equals(Resources.BTN_CANCEL))
            {
                if (MessageBox.Show(Resources.MSG_CANCEL, Resources.MSG_BOX_CAPTION, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Log("The file generation process was canceled by the User!");

                    cts.Cancel();

                    UpdateControls(false);
                }
            }
            else if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                UpdateControls(true);

                long fileSize = ((double)nudFileSize.Value).GetFileSizeInBytes(MetricPrefixes.Mega);
                int maxNumber = (int)nudMaxNumber.Value;
                int maxWordsCount = (int)nudMaxWordsCount.Value;
                int maxWordLength = (int)nudMaxWordLength.Value;
                short duplicateStringDensity = (short)(cbRandomDuplicates.Checked ? -1 : nudDuplicateStringDensity.Value);

                Log($"Generating test file. File Size: {((double)fileSize).FormatFileSize()}; Max Number: {maxNumber}; Max Words Count: {maxWordsCount}; Max Word Length: {maxWordLength}; Duplicate String Density: {(duplicateStringDensity == -1 ? "Random" : duplicateStringDensity + "%")}.");

                sw.Reset();
                sw.Start();

                try
                {
                    Task.Run(() =>
                                RandomFileGenerator.GenerateTestFile(
                                    saveFileDialog.FileName,
                                    fileSize,
                                    maxNumber,
                                    maxWordsCount,
                                    maxWordLength,
                                    duplicateStringDensity),
                            cts.Token)
                        .ContinueWith(generatedLinesCountTask =>
                        {
                            Invoke(() =>
                            {
                                sw.Stop();

                                Log($"File successfully Generated and Saved in: {sw.Elapsed:c}");

                                if (generatedLinesCountTask is { IsCanceled: false, IsFaulted: false })
                                {
                                    generatedLinesCount = generatedLinesCountTask.Result;

                                    if (generatedLinesCount < 1)
                                    {
                                        Log($"Failed to generate a new file: '{saveFileDialog.FileName}'");
                                    }
                                }

                                UpdateControls(false);

                                // Suppressed the possible null reference warning, as the directory is known to be valid here
                                Process.Start("explorer.exe", Path.GetDirectoryName(saveFileDialog.FileName)!);
                            });
                        }, cts.Token);
                }
                catch (AggregateException ae)
                {
                    foreach (Exception ex in ae.InnerExceptions)
                    {
                        Log(ex is TaskCanceledException exception
                            ? $"File generation is canceled by the User! Inner exception: {exception}"
                            : $"File generation failed! Error: {ex.GetType().Name} - {ex.Message}");
                    }
                }
                finally
                {
                    cts.Dispose();
                }
            }
        }

        private void nudFileSize_ValueChanged(object sender, EventArgs e)
        {
            lblFileSize.Text = ((double)nudFileSize.Value).FormatFileSize(metricBenchmark: MetricPrefixes.Mega);
        }

        private void cbRandomDuplicates_CheckedChanged(object sender, EventArgs e)
        {
            nudDuplicateStringDensity.Enabled = !cbRandomDuplicates.Checked;
        }

        private void nudLineParametersChanged(object sender, EventArgs e)
        {
            int maxNumber = (int)nudMaxNumber.Value;
            int maxWordsCount = (int)nudMaxWordsCount.Value;
            int maxWordLength = (int)nudMaxWordLength.Value;

            double minLineSize = RandomFileGenerator.GetEstimatedLineSizeInBytes(maxNumber, maxWordsCount, maxWordLength, EstimatedSizeType.Min);
            double maxLineSize = RandomFileGenerator.GetEstimatedLineSizeInBytes(maxNumber, maxWordsCount, maxWordLength, EstimatedSizeType.Max);
            double avgLineSize = RandomFileGenerator.GetEstimatedLineSizeInBytes(maxNumber, maxWordsCount, maxWordLength, EstimatedSizeType.Avg);

            lblLineSizeDetails.Text = $@"{avgLineSize.FormatFileSize()} / ({minLineSize.FormatFileSize()} - {maxLineSize.FormatFileSize()})";
        }

        private void UpdateControls(bool generationInProgress)
        {
            gbParameters.Enabled = !generationInProgress;
            this.Cursor = generationInProgress ? Cursors.WaitCursor : Cursors.Default;
            btnGenerateAndSave.Cursor = Cursors.Default;
            btnGenerateAndSave.Text = generationInProgress ? Resources.BTN_CANCEL : Resources.BTN_GENERATE_AND_SAVE;

            tsslFileSize.Text = generationInProgress ? string.Empty : ((double)new FileInfo(saveFileDialog.FileName).Length).FormatFileSize();
            tsslLinesCount.Text = generationInProgress ? string.Empty : generatedLinesCount.ToString("##,###");
            tsslExecutionTime.Text = generationInProgress ? string.Empty : sw.Elapsed.ToString("c");
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