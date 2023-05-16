namespace PokemonProofOrganizer
{
    internal class Job
    {
        private string filePath;
        private Options options;
        private string tradeHistory;
        private int ternary;
        private string prefix;

        public Job(string filePath, Options options, string tradeHistory, int ternary, string prefix)
        {
            this.filePath = filePath;
            this.options = options;
            this.tradeHistory = tradeHistory;
            this.ternary = ternary;
            this.prefix = prefix;
        }

        public string FilePath
        {
            get => filePath;
            set => filePath = value;
        }

        public Options Options
        {
            get => options;
            set => options = value;
        }

        public string TradeHistory
        {
            get => tradeHistory;
            set => tradeHistory = value;
        }

        public int Ternary
        {
            get => ternary;
            set => ternary = value;
        }

        public string Prefix
        {
            get => prefix;
            set => prefix = value;
        }
    }

}
