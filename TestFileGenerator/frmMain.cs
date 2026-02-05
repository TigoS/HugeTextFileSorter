using System.Diagnostics;
using MultiSorterLib;
using TestFileGenerator.Properties;
using static MultiSorterLib.FileSizeExtensions;

namespace TestFileGenerator
{
    /// <summary>
    /// Represents the main window of the application, providing the user interface for configuring parameters and
    /// generating test files with customizable content.
    /// </summary>
    /// <remarks>The frmMain form enables users to specify file generation parameters, initiate or cancel the
    /// file creation process, and view progress and log information. It manages user interactions, updates UI controls
    /// based on the application's state, and coordinates asynchronous file generation tasks. This form is intended to
    /// be used as the primary entry point for user operations within the application.</remarks>
    public partial class frmMain : Form
    {
        private readonly Stopwatch sw = new();

        /// <summary>
        /// Provides a mechanism for signaling cancellation to asynchronous operations.
        /// </summary>
        private CancellationTokenSource cts = new();

        private long generatedLinesCount;

        /// <summary>
        /// Initializes a new instance of the frmMain class.
        /// </summary>
        /// <remarks>This constructor sets up the main form and initializes its components. It also
        /// applies the initial state for line parameters by invoking the relevant event handler.</remarks>
        public frmMain()
        {
            InitializeComponent();

            nudLineParametersChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Handles the Click event of the Generate and Save button, initiating or canceling the test file generation
        /// process based on the current button state.
        /// </summary>
        /// <remarks>If the button is in 'Cancel' mode, this method prompts the user to confirm
        /// cancellation and, if confirmed, cancels the ongoing file generation. If the button is in 'Generate and Save'
        /// mode, it displays a Save File dialog, collects user-specified parameters, and starts the asynchronous file
        /// generation process. Upon completion or cancellation, it updates the UI and logs the outcome. The method is
        /// intended to be used as an event handler for a Windows Forms button.</remarks>
        /// <param name="sender">The source of the event, typically the Generate and Save button.</param>
        /// <param name="e">An EventArgs object that contains the event data.</param>
        private void btnGenerateAndSave_Click(object sender, EventArgs e)
        {
            if (btnGenerateAndSave.Text.Equals(Resources.BTN_CANCEL))
            {
                if (MessageBox.Show(Resources.MSG_CANCEL, Resources.MSG_BOX_CAPTION, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Log("File generation process cancellation requested by the User!");

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
                    ResetCancellationTokenSource();

                    Task.Run(() =>
                                RandomFileGenerator.GenerateTestFile(
                                    saveFileDialog.FileName,
                                    fileSize,
                                    maxNumber,
                                    maxWordsCount,
                                    maxWordLength,
                                    duplicateStringDensity,
                                    cts.Token))
                        .ContinueWith(generatedLinesCountTask =>
                        {
                            Invoke(() =>
                            {
                                sw.Stop();

                                Log(cts.IsCancellationRequested ? "File generation canceled by the User!" : $"File successfully Generated and Saved in: {sw.Elapsed:c}");

                                if (generatedLinesCountTask is {IsCanceled: false, IsFaulted: false})
                                {
                                    generatedLinesCount = generatedLinesCountTask.Result;

                                    if (generatedLinesCount < 1)
                                    {
                                        Log($"Failed to generate a new file: '{saveFileDialog.FileName}'");
                                    }
                                }

                                UpdateControls(false);

                                if (cbOpenDirectoryOnComplete.Checked && !cts.IsCancellationRequested)
                                {
                                    // Suppressed the possible null reference warning, as the directory is known to be valid here
                                    Process.Start("explorer.exe", Path.GetDirectoryName(saveFileDialog.FileName)!);
                                }
                            });
                        });
                }
                catch (AggregateException ae)
                {
                    foreach (Exception ex in ae.InnerExceptions)
                    {
                        bool isTaskCanceledException = ex is TaskCanceledException;
                        Log(isTaskCanceledException ? "File generation is canceled by the User!" : "File generation failed!",
                            isTaskCanceledException ? ex : ex as TaskCanceledException);
                    }
                }
                finally
                {
                    ResetCancellationTokenSource();
                }
            }
        }

        /// <summary>
        /// Handles the ValueChanged event of the file size numeric up-down control and updates the displayed file size
        /// in megabytes.
        /// </summary>
        /// <param name="sender">The source of the event, typically the numeric up-down control whose value has changed.</param>
        /// <param name="e">An EventArgs object that contains the event data.</param>
        private void nudFileSize_ValueChanged(object sender, EventArgs e)
        {
            lblFileSize.Text = ((double)nudFileSize.Value).FormatFileSize(metricBenchmark: MetricPrefixes.Mega);
        }

        /// <summary>
        /// Handles the CheckedChanged event of the Random Duplicates checkbox to enable or disable the duplicate string
        /// density control.
        /// </summary>
        /// <param name="sender">The source of the event, typically the Random Duplicates checkbox.</param>
        /// <param name="e">An EventArgs object that contains the event data.</param>
        private void cbRandomDuplicates_CheckedChanged(object sender, EventArgs e)
        {
            nudDuplicateStringDensity.Enabled = !cbRandomDuplicates.Checked;
        }

        /// <summary>
        /// Handles changes to the line parameter numeric controls and updates the line size details display
        /// accordingly.
        /// </summary>
        /// <param name="sender">The source of the event, typically a numeric up-down control whose value has changed.</param>
        /// <param name="e">An object that contains the event data.</param>
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

        /// <summary>
        /// Updates the state of UI controls to reflect whether a file generation operation is in progress.
        /// </summary>
        /// <remarks>This method enables or disables relevant controls and updates status indicators based
        /// on the current generation state. It should be called whenever the generation process starts or stops to
        /// ensure the UI accurately represents the application's status.</remarks>
        /// <param name="generationInProgress">true to indicate that file generation is currently in progress; otherwise, false.</param>
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

        /// <summary>
        /// Resets the internal CancellationTokenSource if a cancellation has been requested and the token source cannot
        /// be reset.
        /// </summary>
        /// <remarks>This method ensures that the CancellationTokenSource is in a non-canceled state for
        /// future operations. It should be called before starting new operations that require a fresh cancellation
        /// token.</remarks>
        private void ResetCancellationTokenSource()
        {
            if (cts.IsCancellationRequested && !cts.TryReset())
            {
                cts = new CancellationTokenSource();
            }
        }

        /// <summary>
        /// Appends a log entry with the specified message and optional exception details to the log display.
        /// </summary>
        /// <param name="message">The message to include in the log entry. Cannot be null.</param>
        /// <param name="ex">An optional exception whose message will be appended to the log entry. If null, no exception details are
        /// logged.</param>
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