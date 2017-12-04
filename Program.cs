using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;

namespace ShardReaders.TLOPO {

    class Program {
        // Actual Game Directory
        // static DirectoryInfo gameDirectory = new DirectoryInfo(@"D:\Games\The Legend of Pirates Online\TLOPO");
        // static DirectoryInfo logsDirectoryFull = new DirectoryInfo(gameDirectory.FullName + @"\logs");

        // static FileInfo districtFile = new FileInfo(gameDirectory.FullName + @"\district.txt");

        static DirectoryInfo currentDirectory = new DirectoryInfo(Environment.CurrentDirectory);
        static DirectoryInfo logsDirectory = new DirectoryInfo(currentDirectory.FullName + @"\logs");

        static FileInfo districtFile = new FileInfo(currentDirectory.FullName + @"\district.txt");

        const string shardIndicator = "New DO:";

        static Dictionary<int, string> districtDictionary = new Dictionary<int, string> {
            { 4010, "Abassa" },
            { 4020, "Andaba" },
            { 4030, "Aventurado" },
            { 4040, "Belleza" },
            { 4050, "Bequermo" },
            { 4060, "Exuma" },
            { 4070, "Jovencito" },
            { 4080, "Juntos" },
            { 4090, "Ladrones" },
            { 4100, "Levanta" },
            { 4110, "Marineros" },
            { 4120, "Nocivo" },
            { 4130, "Poderoso" },
            { 4140, "Sabada" },
            { 4150, "Temprano" },
            { 4160, "Valor" },
        };


        static void Main( string[] args ) {
            while( true ) {
                // Get the last log file by date modified.
                var log = logsDirectory.GetFiles().OrderByDescending(f => f.LastWriteTime).First();

                using( var stream = File.Open(log.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete) ) {
                    using( var reader = new StreamReader(stream) ) {
                        while( !reader.EndOfStream ) {

                            // Read last line of file.
                            string line = string.Empty;
                            string lastLine = string.Empty;
                            while( (line = reader.ReadLine()) != null ) {
                                if( line.Contains(shardIndicator) && !line.Contains("dclass:DistributedPopulationTracker") ) {
                                    lastLine = line;
                                }
                            }

                            string shardNumber = string.Empty;
                            // Obtain the shard number.
                            if( lastLine.Length != 0 ) {
                                shardNumber = lastLine.Substring(lastLine.IndexOf(":") + 1).Substring(0, lastLine.LastIndexOf(",") - 12);
                            } else if( lastLine.Length == 0 ) {
                                break;
                            }

                            // Shard converted to an integer.
                            int shardNumberInt = int.Parse(shardNumber);

                            // If the shard number has a corresponding district name.
                            if( districtDictionary.ContainsKey(shardNumberInt) ) {
                                // Get the district name using that shard number.
                                string districtName = districtDictionary[shardNumberInt];


                                // Check to see if the file isn't there.
                                if( File.Exists(districtFile.FullName) ) {
                                    // If it is, just write to the file.
                                    File.WriteAllText(districtFile.FullName, districtName);

                                    // If it isn't there.
                                } else if( File.Exists(districtFile.FullName) == false ) {
                                    // Create it.
                                    File.Create(districtFile.FullName);
                                    // Then write.
                                    File.WriteAllText(districtFile.FullName, districtName);

                                }

                                // Clear the line before writing it.
                                ClearCurrentConsoleLine();
                                // Write it to console.
                                Console.Write($"{shardNumberInt} = {districtName}\r");

                                // Runs every 'x' seconds to make sure it stays up to date and makes sure that your CPU doesn't melt.
                                Thread.Sleep(TimeSpan.FromSeconds(1));
                            }
                        }
                    }
                }
            }
        }

        public static void ClearCurrentConsoleLine() {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

    }
}
