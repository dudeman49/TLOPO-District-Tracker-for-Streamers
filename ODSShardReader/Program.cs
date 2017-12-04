using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;

namespace ShardReaders.ODS {

    class Program {
        // Actual Game Directory
        // static DirectoryInfo gameDirectory = new DirectoryInfo(@"D:\Games\Toontown\Rewritten");
        // static DirectoryInfo logsDirectoryFull = new DirectoryInfo(gameDirectory.FullName + @"\logs");
        // 
        // static FileInfo districtFile = new FileInfo(gameDirectory.FullName + @"\district.txt");

        static DirectoryInfo currentDirectory = new DirectoryInfo(Environment.CurrentDirectory);
        static DirectoryInfo logsDirectory = new DirectoryInfo(currentDirectory.FullName + @"\logs");
        
        static FileInfo districtFile = new FileInfo(currentDirectory.FullName + @"\district.txt");

        const string shardIndicator = "Entering shard";

        static Dictionary<int, string> districtDictionary = new Dictionary<int, string> {
            { 5000, "Gulp Gulch" },
            { 5010, "Splashport" },
            { 5020, "Fizzlefield" },
            { 5030, "Whoosh Rapids" },
            { 5040, "Blam Canyon" },
            { 5050, "Hiccup Hills" },
            { 5060, "Splat Summit" },
            { 5070, "Thwackville" },
            { 5080, "Zoink Falls" },
            { 5090, "Kaboom Cliffs" },
            { 5100, "Bounceboro" },
            { 5110, "Boingbury" }
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
                                if( line.Contains(shardIndicator) ) {
                                    lastLine = line;
                                }
                            }

                            string shardNumber = string.Empty;
                            // Obtain the shard number.
                            if( lastLine.Length != 0 ) {
                                shardNumber = lastLine.Substring(lastLine.IndexOf(":") + ":OTPClientRepository: Entering shard ".Length).Remove(4, 5);
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
