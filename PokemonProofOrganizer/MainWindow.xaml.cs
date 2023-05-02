using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private static bool renameChecked = false;
        private static bool createFolderChecked = false;
        private static bool addTradeHistoryChecked = false;
        private static bool compressChecked = false;
        private static string[] filePaths = null;
        private static int ternary = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BrowseFiles(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".mp4"; // Default file extension
            dlg.Filter = "Videos |*.mp4"; // Filter files by extension
            dlg.Multiselect = true; //Select multiple files

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                filePaths = dlg.FileNames;
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
            ternary = int.Parse(TernaryNumber.Text);
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            Tools tools = new Tools(renameChecked, createFolderChecked, addTradeHistoryChecked, compressChecked);
            tools.runTools(filePaths, ternary);
        }
    }
}
