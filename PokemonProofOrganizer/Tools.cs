using System.Diagnostics;
using System;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace PokemonProofOrganizer
{
    internal class Tools
    {
        private BlockingCollection<Job> queue;
        private static bool compressSuccess = false;
        private MainWindow mainWindow;

        public Tools(BlockingCollection<Job> queue, MainWindow mainWindow)
        {
            this.queue = queue;
            this.mainWindow = mainWindow;
        }

        public void runTools(ManualResetEvent resetEvent)
        {
            int fileCounter = 0, queueCount = queue.Count;
            string fileName = "";
            string newfilePath = "";
            string directoryPath = "";


            foreach (Job job in queue.GetConsumingEnumerable())
            {
                fileName = Path.GetFileName(job.FilePath);
                newfilePath = job.FilePath;

                if (job.Options.Compress)
                {
                    fileCounter++;

                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        mainWindow.Files.Content = $"File: {fileCounter}/{queueCount}";
                    });

                    if (job.Options.Rename)
                    {
                        fileName = $"{job.Prefix}{DecimalToTernary(job.Ternary).ToString().PadLeft(4, '0')}.mp4";
                        newfilePath = renameFiles(job.FilePath, fileName);
                    }

                    if (!job.Options.CreateFolder)
                    {
                        directoryPath = createFolder(Path.GetDirectoryName(job.FilePath), "output");
                    }
                    else
                    {
                        directoryPath = createFolder(Path.GetDirectoryName(job.FilePath), Path.GetFileNameWithoutExtension(fileName));
                    }

                    compressSuccess = true;

                    compressProof(newfilePath, directoryPath + @"\" + fileName, resetEvent);

                    if (!compressSuccess)
                    {
                        break;
                    }

                    if (job.Options.AddTradeHistory)
                    {
                        createTradeHistory(job.TradeHistory, directoryPath);
                    }
                }
                else
                {
                    if (job.Options.Rename)
                    {
                        fileName = $"{job.Prefix} {DecimalToTernary(job.Ternary).ToString().PadLeft(4, '0')}.mp4";
                        newfilePath = renameFiles(job.FilePath, fileName);
                    }

                    if (newfilePath != "")
                    {
                        if (job.Options.CreateFolder)
                        {
                            directoryPath = createFolderAndMove(newfilePath);
                        }

                        if (job.Options.AddTradeHistory)
                        {
                            Debug.WriteLine("hi");
                            createTradeHistory(job.TradeHistory, directoryPath);
                        }
                    }
                    else
                    {
                        MessageBox.Show("File already exists");
                    }
                }

                if(queue.Count == 0)
                {
                    MessageBox.Show("Queue finished!");
                    break;
                }
            }
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
            TimeSpan duration = TimeSpan.Parse("00:00:00.00"), time = TimeSpan.Parse("00:00:00.00");

            while (!process.StandardError.EndOfStream)
            {
                if (resetEvent.WaitOne(0))
                {
                    MessageBox.Show("Canceled!");
                    resetEvent.Reset();
                    compressSuccess = false;

                    ProcessStartInfo killProcess = new ProcessStartInfo();
                    killProcess.FileName = "cmd.exe";
                    killProcess.Arguments = "/C taskkill /f /t /im ffmpeg.exe /im choco.exe";
                    killProcess.WindowStyle = ProcessWindowStyle.Hidden;
                    killProcess.CreateNoWindow = true;

                    Process.Start(killProcess);
                }
                var line = process.StandardError.ReadLine();
                
                if (line.StartsWith("  Duration:"))
                {
                    Match matchD = Regex.Match(line, @"Duration: (\d{2}):(\d{2}):(\d{2})\.(\d{2})");

                    if (matchD.Success)
                    {
                        duration = new TimeSpan(0, Convert.ToInt32(matchD.Groups[1].Value),
                                                            Convert.ToInt32(matchD.Groups[2].Value),
                                                            Convert.ToInt32(matchD.Groups[3].Value),
                                                            Convert.ToInt32(matchD.Groups[4].Value) * 10);
                        Debug.WriteLine(duration.TotalSeconds);
                    }
                }
                if (line.StartsWith("frame"))
                {
                    Match match = Regex.Match(line, @"time=(\d{2}):(\d{2}):(\d{2})\.(\d{2})");

                    if (match.Success)
                    {
                        time = new TimeSpan(0, Convert.ToInt32(match.Groups[1].Value),
                                                            Convert.ToInt32(match.Groups[2].Value),
                                                            Convert.ToInt32(match.Groups[3].Value),
                                                            Convert.ToInt32(match.Groups[4].Value) * 10);
                    }
                }
                if (duration.TotalSeconds != 0 || time.TotalSeconds != 0)
                {
                    double ratio = time.TotalSeconds / duration.TotalSeconds;
                    double percentage = ratio * 100;


                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        mainWindow.progressBar.Value = Math.Round(percentage, 2); ;
                        mainWindow.progressLabel.Content = $"{percentage.ToString("F2")}%";
                    });
                    

                    Debug.WriteLine(percentage.ToString("F2") + "%");
                }
            }

            // Wait for the process to exit
            process.WaitForExit();
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