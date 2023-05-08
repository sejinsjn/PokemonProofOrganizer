﻿using System.Diagnostics;
using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Management;
using System.Security.Cryptography;
using System.CodeDom;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace PokemonProofOrganizer
{
    internal class Tools
    {
        private BlockingCollection<string> queue;

        private static bool rename = false;
        private static bool create = false;
        private static bool addTradeHistory = false;
        private static bool compress = false;
        private static bool compressSuccess = true;

        public Tools(BlockingCollection<string> queue, bool renameChecked, bool createFolderChecked, bool addTradeHistoryChecked, bool compressChecked)
        {
            this.queue = queue;
            rename = renameChecked;
            create = createFolderChecked;
            addTradeHistory = addTradeHistoryChecked;
            compress = compressChecked;
        }

        public void runTools(List<string> filePaths, int ternary, string fileContent, ManualResetEvent resetEvent, ManualResetEvent threadStartedEvent)
        {
            int currentNumber = ternary;
            string fileName = "";
            string newfilePath = "";
            string directoryPath = "";

            
            foreach (string filePath in queue.GetConsumingEnumerable())
            {
                fileName = Path.GetFileName(filePath);
                newfilePath = filePath;

                if (compress)
                {
                    if (rename)
                    {
                        fileName = "" + (12 * 10000 + DecimalToTernary(ternary)) + ".mp4";
                        newfilePath = renameFiles(filePath, fileName);
                    }

                    if (!create)
                    {
                        directoryPath = createFolder(Path.GetDirectoryName(filePath), "output");
                    }
                    else
                    {
                        directoryPath = createFolder(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(fileName));
                    }

                compressProof(newfilePath, directoryPath + @"\" + fileName, resetEvent);

                    if (!compressSuccess)
                    {
                        break;
                    }

                    if (addTradeHistory)
                    {
                        createTradeHistory(fileContent, directoryPath);
                    }
                }
                else
                {
                    if (rename)
                    {
                        fileName = "" + (12 * 10000 + DecimalToTernary(ternary)) + ".mp4";
                        newfilePath = renameFiles(filePath, fileName);
                    }

                    if (newfilePath != "")
                    {
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
                    else
                    {
                        MessageBox.Show("File already exists");
                    }
                }
                ternary++;
            }

            compressSuccess = true;
        }

        private void compressProof(string inputPath, string outputPath, ManualResetEvent resetEvent)
        {
            
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "ffmpeg.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.CreateNoWindow = true;
            startInfo.Arguments = $"-i \"{inputPath}\" -c:v libx265 -an -x265-params crf=25 \"{outputPath}\"";
            startInfo.RedirectStandardError = true;
            process.StartInfo = startInfo;
            process.Start();

            while (!process.StandardError.EndOfStream)
            {
                if (resetEvent.WaitOne(0))
                {
                    ProcessStartInfo killProcess = new ProcessStartInfo();
                    killProcess.FileName = "cmd.exe";
                    killProcess.Arguments = "/C taskkill /f /t /im ffmpeg.exe /im choco.exe";
                    killProcess.WindowStyle = ProcessWindowStyle.Hidden;
                    killProcess.CreateNoWindow = true;

                    Process.Start(killProcess);

                    resetEvent.Reset();
                    compressSuccess = false;
                }
                var line = process.StandardError.ReadLine();
                Debug.WriteLine(line);
            }

            // Wait for the process to exit
            process.WaitForExit();
        }

        private void ProcessExited(object sender, EventArgs e)
        {
            MessageBox.Show("Finished!");
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

        private string createFolder(string dirPath, string folderName)
        {
            string directoryPath = Path.Combine(dirPath, folderName);
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

            if (!File.Exists(newFilePath))
            {
                // Rename the file
                File.Move(filePath, newFilePath);
            }
            else
            {
                return "";
            }

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