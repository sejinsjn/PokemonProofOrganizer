using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PokemonProofOrganizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BlockingCollection<string> queue;
        List<string> filePaths = new List<string>();
        ManualResetEvent resetEvent = new ManualResetEvent(false);
        private ManualResetEvent threadStartedEvent = new ManualResetEvent(false);
        private static bool renameChecked = false;
        private static bool createFolderChecked = false;
        private static bool addTradeHistoryChecked = false;
        private static bool compressChecked = false;
        private static int ternary = 0;
        private static string tradeHistory = "";

        public MainWindow()
        {
            InitializeComponent();
            queue = new BlockingCollection<string>();
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
                foreach (string item in dlg.FileNames)
                {
                    queue.Add(item);
                }
                Path.Text = dlg.FileNames[0];
            }
        }

        private void RenameChecked(object sender, RoutedEventArgs e)
        {
            renameChecked = true;
        }

        private void RenameUnchecked(object sender, RoutedEventArgs e)
        {
            renameChecked = false;
        }

        private void CreateFolderChecked(object sender, RoutedEventArgs e)
        {
            createFolderChecked = true;
        }

        private void CreateFolderUnchecked(object sender, RoutedEventArgs e)
        {
            createFolderChecked = false;
        }

        private void AddTradeHistoryChecked(object sender, RoutedEventArgs e)
        {
            addTradeHistoryChecked = true;
        }

        private void AddTradeHistoryUnchecked(object sender, RoutedEventArgs e)
        {
            addTradeHistoryChecked = false;
        }

        private void CompressChecked(object sender, RoutedEventArgs e)
        {
            compressChecked = true;
        }

        private void CompressUnchecked(object sender, RoutedEventArgs e)
        {
            compressChecked = false;
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

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            if (queue != null && queue.Count > 0)
            {
                if (renameChecked || createFolderChecked || addTradeHistoryChecked || compressChecked)
                {
                    Tools tools = new Tools(queue, renameChecked, createFolderChecked, addTradeHistoryChecked, compressChecked);

                    Thread thread = new Thread(() => tools.runTools(filePaths, ternary, tradeHistory, resetEvent, threadStartedEvent));

                    thread.Start();

                    queue = new BlockingCollection<string>();
                    filePaths = new List<string>();
                }
                else
                {
                    MessageBox.Show("Select one at least one of the options!");
                }
            }
            else
            {
                MessageBox.Show("Select one or more files!");
            }
        }

        private void ToolsThreadFinished(object sender, EventArgs e)
        {
            // Show message box when thread finishes
            MessageBox.Show("Finished!");
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
    }
}
