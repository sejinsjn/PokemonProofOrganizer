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

        public void runTools(string[] filePaths, int ternary, string fileContent)
        {
            int currentNumber = ternary;
            string fileName = "";
            string newfilePath = "";
            string directoryPath = "";

            foreach (string filePath in filePaths)
            {
                fileName = "" + (12 * 10000 + DecimalToTernary(ternary));

                if (rename)
                {
                    newfilePath = renameFiles(filePath, fileName + ".mp4");
                }

                if (createFolder)
                {
                    directoryPath = createFolderAndMove(newfilePath);
                }

                if (addTradeHistory)
                {
                    createTradeHistory(fileContent, directoryPath);
                }

                ternary++;
            }
        }

        private void createTradeHistory(string fileContent, string targetDirectory)
        {
            string fileName = "Trade History.txt";

            using (StreamWriter writer = new StreamWriter(fileName))
            {
                writer.Write(fileContent);
            }

            // Move the file to the new directory
            string newFilePath = Path.Combine(targetDirectory, fileName);
            File.Move(fileName, newFilePath);
        }

        private string createFolderAndMove(string filePath)
        {
            string directoryPath = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath));
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                string newFilePath = Path.Combine(directoryPath, Path.GetFileName(filePath));
                File.Move(filePath, newFilePath);
            }

            return directoryPath;
        }

        private string renameFiles(string filePath, string newFileName)
        {
            string directoryName = Path.GetDirectoryName(filePath);

            // Construct the new file path by combining the directory and the new file name
            string newFilePath = Path.Combine(directoryName, newFileName);

            // Rename the file
            File.Move(filePath, newFilePath);

            return newFilePath;
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