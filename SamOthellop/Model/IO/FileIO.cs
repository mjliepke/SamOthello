using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SamOthellop.Model
{
    static class FileIO
    {
        const int FILE_HEADER_LENGTH = 16; //bytes
        const int GAME_HEADER_LENGTH = 8;
        const int NUMBER_OF_PLAYS = 60;
        const int GAME_BLOCK_LENGTH = GAME_HEADER_LENGTH + NUMBER_OF_PLAYS;

        public static List<ThorGame> ReadThorFile(string path = @"C:\Users\mjlie\source\repos\SamOthellop\SamOthellop\Database\WTH_2001.wtb")
        {
            /*
             * Thor file capabilities from ledpup's Othello work
             * https://github.com/ledpup/Othello
             * 
             * Returns a list of game 
             */
            List<ThorGame> gameDatabase = new List<ThorGame>();
            //FileInfo fi = new FileInfo(path);

            if (File.Exists(path))
            {
                byte[] thorArray = File.ReadAllBytes(path);

                if (thorArray[12] != 8)
                    throw new Exception("Thor processor only supports 8x8 boards");

                for (var i = FILE_HEADER_LENGTH; i < thorArray.Length; i += GAME_BLOCK_LENGTH)
                {
                    ThorGame game = new ThorGame
                    {
                        TournamentId = BitConverter.ToInt16(new[] { thorArray[i], thorArray[i + 1] }, 0),
                        BlackId = BitConverter.ToInt16(new[] { thorArray[i + 2], thorArray[i + 3] }, 0),
                        WhiteId = BitConverter.ToInt16(new[] { thorArray[i + 4], thorArray[i + 5] }, 0),
                        BlackScore = thorArray[i + 6],
                        TheoreticalScore = thorArray[i + 7],
                    };

                    for (var playIndex = 0; playIndex < NUMBER_OF_PLAYS; playIndex++)
                    {
                        Byte play = thorArray[i + GAME_HEADER_LENGTH + playIndex];

                        if (play > 0)
                        {
                            game.Plays.Add(ToAlgebraicNotation(play));
                        }
                    }
                    gameDatabase.Add(game);

                }

                //foreach(var game in gameDatabase)
                //{
                //    System.Diagnostics.Debug.WriteLine(game.SerialisedPlays);
                //}
            }
            return gameDatabase;
        }
        public static string ToAlgebraicNotation(this byte play)
        {
            var chars = play.ToString().ToCharArray();
            return ((char)(chars[0] + 48)).ToString() + chars[1];
        }

        public static List<OthelloGame> ReadAllGames(string path)
        {//Task per game in a given File, tackles one file at a time
            List<OthelloGame> gameRepo = new List<OthelloGame>();
            List<string> files = GetFiles(path);

            object gameTransferLock = new object();

            foreach (var file in files) //For each file, read games 
            {
                object fileGameLock = new object();
                List<OthelloGame> fileGameRepo = new List<OthelloGame>();
                List<Task> fileTransferTask = new List<Task>();

                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                List<ThorGame> games = FileIO.ReadThorFile(file);

                foreach (ThorGame tgame in games) //for each game, transfer to OthelloGame
                {
                    OthelloGame oGame;

                    fileTransferTask.Add(Task.Run(() =>
                    {
                        try
                        {
                            oGame = new OthelloGame(tgame);
                            lock (fileGameLock)
                            {
                                fileGameRepo = fileGameRepo.Concat(OthelloGame.GetAllGameRotations(oGame)).ToList();
                            }
                        }
                        catch (Exception)
                        {
                            System.Diagnostics.Debug.WriteLine("failed a THOR->OthelloGame Transformation");
                        }
                    }));

                }
                Task.WaitAll(fileTransferTask.ToArray());
                foreach(Task t in fileTransferTask)
                {
                    t.Dispose();
                }
                    gameRepo = gameRepo.Concat(fileGameRepo).ToList();

                if (8 * games.Count != fileGameRepo.Count)
                {
                    Console.WriteLine("Games have been lost, " + fileGameRepo.Count + " / " + (8 * games.Count) + " games  transferred : " + ((double)fileGameRepo.Count / (8 * games.Count)) + " %");
                }


                stopwatch.Stop();
                Console.WriteLine("Elapsed time for transferring " + file + " info= {0}", stopwatch.Elapsed);
            }
            return gameRepo;
        }

        public static List<OthelloGame> ReadAllGames2(string path)
        {//Thread per File, tackles all at once, outdated
            List<OthelloGame> gameRepo = new List<OthelloGame>();
            List<Thread> fileIOThreadList = new List<Thread>();


            List<string> files = GetFiles(path);

            foreach (var file in files)
            {
                fileIOThreadList.Add(new Thread(() =>
                {
                    List<OthelloGame> fileGameRepo = new List<OthelloGame>();
                    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                    List<ThorGame> games = ReadThorFile(file);

                    foreach (ThorGame tgame in games)
                    {
                        OthelloGame oGame;
                        try
                        {
                            oGame = new OthelloGame(tgame);
                            fileGameRepo = fileGameRepo.Concat(OthelloGame.GetAllGameRotations(oGame)).ToList();
                        }
                        catch (Exception)
                        {
                            System.Diagnostics.Debug.WriteLine("failed a THOR->OthelloGame Transformation");
                        }
                    }

                    gameRepo = gameRepo.Concat(fileGameRepo).ToList();
                    stopwatch.Stop();
                    Console.WriteLine("Elapsed time for transferring " + file + " info= {0}", stopwatch.Elapsed);
                }));
            }
            foreach (Thread t in fileIOThreadList)
            {
                t.Start();
            }
            foreach (Thread t in fileIOThreadList)
            {
                t.Join();
            }
            return gameRepo;
        }

        public static List<OthelloGame> ReadAllGames3(string path)
        {//Parallel ForEach for files & games, all at once, outdated
            List<OthelloGame> gameRepo = new List<OthelloGame>();
            List<string> files = GetFiles(path);

            object gameTransferLock = new object();
            List<Task> gameTransferTask = new List<Task>();

            Parallel.ForEach(files, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, (file) => //For each file, read games 
            {
                object fileGameLock = new object();
                List<OthelloGame> fileGameRepo = new List<OthelloGame>();

                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                List<ThorGame> games = FileIO.ReadThorFile(file);

                //Parallel.ForEach(games, (tgame) => //for each game, transfer to OthelloGame
                //{
                foreach (ThorGame tgame in games)
                {
                    OthelloGame oGame;

                    try
                    {
                        oGame = new OthelloGame(tgame);
                        lock (fileGameLock)
                        {
                            fileGameRepo = fileGameRepo.Concat(OthelloGame.GetAllGameRotations(oGame)).ToList();
                        }
                    }
                    catch (Exception)
                    {
                        System.Diagnostics.Debug.WriteLine("failed a THOR->OthelloGame Transformation");
                    }
                }

                //});


                lock (gameTransferLock)
                {
                    gameRepo = gameRepo.Concat(fileGameRepo).ToList();
                }
                if (8 * games.Count != fileGameRepo.Count)
                {
                    Console.WriteLine("Games have been lost, " + fileGameRepo.Count + " / " + (8 * games.Count) + " games  transferred : " + ((double)fileGameRepo.Count / (8 * games.Count)) + " %");
                }


                stopwatch.Stop();
                Console.WriteLine("Elapsed time for transferring " + file + " info= {0}", stopwatch.Elapsed);
            });
            return gameRepo;
        }

        public static List<OthelloGame> ReadAllGames4(string path)
        {//Parallel ForEach for games in a file, tackles one file at a time, outdated
            List<OthelloGame> gameRepo = new List<OthelloGame>();
            List<string> files = GetFiles(path);

            object gameTransferLock = new object();
            List<Task> gameTransferTask = new List<Task>();

            foreach(string file in files)//For each file, read games 
            {
                object fileGameLock = new object();
                List<OthelloGame> fileGameRepo = new List<OthelloGame>();

                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                List<ThorGame> games = FileIO.ReadThorFile(file);

                Parallel.ForEach(games, (tgame) => //for each game, transfer to OthelloGame
                {
                    OthelloGame oGame;

                    try
                    {
                        oGame = new OthelloGame(tgame);
                        lock (fileGameLock)
                        {
                            fileGameRepo = fileGameRepo.Concat(OthelloGame.GetAllGameRotations(oGame)).ToList();
                        }
                    }
                    catch (Exception)
                    {
                        System.Diagnostics.Debug.WriteLine("failed a THOR->OthelloGame Transformation");
                    }


                });


                lock (gameTransferLock)
                {
                    gameRepo = gameRepo.Concat(fileGameRepo).ToList();
                }
                if (8 * games.Count != fileGameRepo.Count)
                {
                    Console.WriteLine("Games have been lost, " + fileGameRepo.Count + " / " + (8 * games.Count) + " games  transferred : " + ((double)fileGameRepo.Count / (8 * games.Count)) + " %");
                }


                stopwatch.Stop();
                Console.WriteLine("Elapsed time for transferring " + file + " info= {0}", stopwatch.Elapsed);
            }
            return gameRepo;
        }

        private static List<string> GetFiles(string path)
        {

            var files = Directory.GetFiles(path, "*.wtb")
                    .OrderBy(x => x.Length)
                    .Reverse()
                    .ToList();

            if (!files.Any())
            {
                throw new Exception("No Thor DB files can be found.");
            }
            return files;
        }
    }
}

