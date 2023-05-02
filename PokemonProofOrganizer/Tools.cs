using System.IO;

namespace PokemonProofOrganizer
{
    internal class Tools
    {
        private static bool rename = false;
        private static bool createFolder = false;
        private static bool addTradeHistory = false;
        private static bool compress = false;

        public Tools(bool renameChecked, bool createFolderChecked, bool addTradeHistoryChecked, bool compressChecked)
        {
            rename = renameChecked;
            createFolder = createFolderChecked;
            addTradeHistory = addTradeHistoryChecked;
            compress = compressChecked;
        }

        public void runTools(string[] filePaths, int ternary)
        {
            int currentNumber = ternary;
            string fileName = "";

            foreach (string filePath in filePaths)
            {
                fileName = "" + (12 * 10000 + DecimalToTernary(ternary));

                if (rename)
                {
                    renameFiles(filePath, fileName + ".mp4");
                }
                

                ternary++;
            }
        }

        private void renameFiles(string filePath, string newFileName)
        {
            string directoryName = Path.GetDirectoryName(filePath);

            // Construct the new file path by combining the directory and the new file name
            string newFilePath = Path.Combine(directoryName, newFileName);

            // Rename the file
            File.Move(filePath, newFilePath);
        }

        private static int DecimalToTernary(int decimalNumber)
        {
            string ternaryNumber = "";

            while (decimalNumber > 0)
            {
                int remainder = decimalNumber % 3;
                ternaryNumber = remainder.ToString() + ternaryNumber;
                decimalNumber /= 3;
            }

            if (ternaryNumber == "")
            {
                ternaryNumber = "0";
            }

            return int.Parse(ternaryNumber); ;
        }

    }
}