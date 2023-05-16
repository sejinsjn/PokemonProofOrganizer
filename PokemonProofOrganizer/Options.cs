using static System.Windows.Forms.Design.AxImporter;

namespace PokemonProofOrganizer
{
    internal class Options
    {
        private bool rename;
        private bool createFolder;
        private bool addTradeHistory;
        private bool compress;

        public Options(bool rename, bool createFolder, bool addTradeHistory, bool compress)
        {
            this.rename = rename;
            this.createFolder = createFolder;
            this.addTradeHistory = addTradeHistory;
            this.compress = compress;
        }

        public bool Rename
        {
            get => rename;
            set => rename = value;
        }

        public bool CreateFolder
        {
            get => createFolder;
            set => createFolder = value;
        }

        public bool AddTradeHistory
        {
            get => addTradeHistory;
            set => addTradeHistory = value;
        }

        public bool Compress
        {
            get => compress;
            set => compress = value;
        }
    }
}
