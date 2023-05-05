using System.Diagnostics;
using System;
using System.IO;
using System.Collections.Generic;

namespace PokemonProofOrganizer
{
    internal class Tools
    {
        private static bool rename = false;
        private static bool create = false;
        private static bool addTradeHistory = false;
        private static bool compress = false;

        public Tools(bool renameChecked, bool createFolderChecked, bool addTradeHistoryChecked, bool compressChecked)
        {
            rename = renameChecked;
            create = createFolderChecked;
            addTradeHistory = addTradeHistoryChecked;
            compress = compressChecked;
        }

        public void runTools(List<string> filePaths, int ternary, string fileContent)
        {
            int currentNumber = ternary;
            string fileName = "";
            string newfilePath = "";
            string directoryPath = "";

            foreach (string filePath in filePaths)
            {
                fileName = "" + (12 * 10000 + DecimalToTernary(ternary)) + ".mp4";

                if (compress)
                {
                    if (rename)
                    {
                        newfilePath = renameFiles(filePath, fileName);
                    }

                    directoryPath = createFolder(newfilePath);
                    compressProof(newfilePath, directoryPath + @"\" + fileName, "Fast 720p30");

                    if (addTradeHistory)
                    {
                        Debug.WriteLine("hi");
                        createTradeHistory(fileContent, directoryPath);
                    }
                }
                else
                {
                    if (rename)
                    {
                        newfilePath = renameFiles(filePath, fileName);
                    }

                    if (create)
                    {
                        directoryPath = createFolderAndMove(newfilePath);
                    }

                    if (addTradeHistory)
                    {
                        Debug.WriteLine("hi");
                        createTradeHistory(fileContent, directoryPath);
                    }
                }

                ternary++;
            }
        }

        private void compressProof(string inputPath, string outputPath, string presetName)
        {
            // Set the path to the HandbrakeCLI executable
            //string handbrakePath = @"C:\Users\sedad\source\repos\PokemonProofOrganizer\PokemonProofOrganizer\HandbrakeCLI\HandBrakeCLI.exe";
            //ffmpeg:
            string arguments = $"ffmpeg -i \"{inputPath}\" -c:v libx265 -an -x265-params crf=25 \"{outputPath}\"";
            // Build the HandbrakeCLI arguments
            //string arguments = $"--preset-import-file \"{presetName}.json\" -Z \"{presetName}\" -i \"{inputPath}\" -o \"{outputPath}\"";
            //string arguments = $"-i \"{inputPath}\" -o \"{outputPath}\" --preset-import-file \"{presetName}.json\" --preset \"{presetName}\"";

            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.CreateNoWindow = true;
            startInfo.Arguments = $"/C ffmpeg -i \"{inputPath}\" -c:v libx265 -an -x265-params crf=25 \"{outputPath}\"";
            process.StartInfo = startInfo;
            process.Start();

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

        private string createFolder(string filePath)
        {
            string directoryPath = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath));
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            return directoryPath;
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