using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace PokemonProofOrganizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BlockingCollection<Job> queue;
        private List<string> filePaths;
        private ManualResetEvent resetEvent;
        private ManualResetEvent threadStartedEvent = new ManualResetEvent(false);
        private Options options;
        private int ternary;
        private string tradeHistory;

        public MainWindow()
        {
            InitializeComponent();
            queue = new BlockingCollection<Job>();
            filePaths = new List<string>();
            resetEvent = new ManualResetEvent(false);
            options = new Options(false, false, false, false);
            ternary = 0;
            tradeHistory = "";
        }

        private void BrowseFiles(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".mp4"; // Default file extension
            dlg.Filter = "Videos (*.mp4, *.mov, *.m4v)|*.mp4;*.mov;*.m4v"; // Filter files by extension
            dlg.Multiselect = true; //Select multiple files


            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                filePaths = dlg.FileNames.ToList();
                Path.Text = dlg.FileNames[0];
            }
        }

        private void RenameChecked(object sender, RoutedEventArgs e)
        {
            options.Rename = true;
        }

        private void RenameUnchecked(object sender, RoutedEventArgs e)
        {
            options.Rename = false;
        }

        private void CreateFolderChecked(object sender, RoutedEventArgs e)
        {
            options.CreateFolder = true;
        }

        private void CreateFolderUnchecked(object sender, RoutedEventArgs e)
        {
            options.CreateFolder = false;
        }

        private void AddTradeHistoryChecked(object sender, RoutedEventArgs e)
        {
            options.AddTradeHistory = true;
        }

        private void AddTradeHistoryUnchecked(object sender, RoutedEventArgs e)
        {
            options.AddTradeHistory = false;
        }

        private void CompressChecked(object sender, RoutedEventArgs e)
        {
            options.Compress = true;
        }

        private void CompressUnchecked(object sender, RoutedEventArgs e)
        {
            options.Compress = false;
        }

        private void PathAdded(object sender, TextChangedEventArgs e)
        {
            
        }
        
        private void TernaryNumberChanged(object sender, TextChangedEventArgs e)
        {
            int value;
            bool isNumeric = int.TryParse(TernaryNumber.Text, out value);
            if (isNumeric && value > -1)
            {
                ternary = value;
            }
        }
        private async void Start_Click(object sender, RoutedEventArgs e)
        {
            if (queue != null && queue.Count > 0)
            {
                if (options.Rename || options.CreateFolder || options.AddTradeHistory || options.Compress)
                {
                    Start.IsEnabled = false;
                    string prefix = Prefix.Text;
                    Tools tools = new Tools(queue, this); ;

                    await Task.Run(() => tools.runTools(resetEvent, threadStartedEvent));

                    filePaths = new List<string>();
                    progressBar.Value = 0;
                    progressLabel.Content = "0.00%";
                    Files.Content = $"File: 0/0";
                    Start.IsEnabled = true;
                }
                else
                {
                    MessageBox.Show("Select one at least one of the options!");
                }
            }
            else
            {
                MessageBox.Show("Add Jobs to the Queue!");
            }
        }

        private void EditTradeHistory(object sender, RoutedEventArgs e)
        {
            FlowDocument document = new FlowDocument();
            Paragraph paragraph = new Paragraph(new Run(tradeHistory));
            document.Blocks.Add(paragraph);

            TradeHistory th = new TradeHistory(document);
            th.Show();
            // Subscribe to the Closed event of the new window
            th.Closed += (sender, args) =>
            {
                // Retrieve data from the new window
                tradeHistory = th.Content;
            };
            
        }

        private void CancelAll_Click(object sender, RoutedEventArgs e)
        {
            resetEvent.Set();
        }

        private void AddToQueueClick(object sender, RoutedEventArgs e)
        {
            foreach (string path in filePaths)
            {
                Job job = new Job(path, options, tradeHistory, ternary, Prefix.Text);
                queue.Add(job);
                ternary++;
            }
        }

        private void ViewQueue(object sender, RoutedEventArgs e)
        {

        }
    }
}
