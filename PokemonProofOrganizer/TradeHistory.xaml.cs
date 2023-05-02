using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace PokemonProofOrganizer
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class TradeHistory : Window
    {
        public string Content { get; set; }
        private FlowDocument _document;

        public TradeHistory(FlowDocument document)
        {
            InitializeComponent();
            _document = document;

            // Set the Document property of the RichTextBox to the FlowDocument
            Text.Document = _document;
        }

        private void TextPreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                e.Handled = true;
                TextPointer caretPos = Text.CaretPosition;
                if (caretPos != null)
                {
                    caretPos.InsertTextInRun("\t");
                }
            }
        }


        private void Save(object sender, RoutedEventArgs e)
        {
            // Get the FlowDocument representing the contents of the RichTextBox
            FlowDocument document = Text.Document;

            // Create a TextRange that spans the entire document
            TextRange textRange = new TextRange(document.ContentStart, document.ContentEnd);

            // Get the text from the TextRange
            Content = textRange.Text;
            this.Close();
        }
    }
}
