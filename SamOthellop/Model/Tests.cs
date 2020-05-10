using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using SamOthellop.Model.Agents;

namespace SamOthellop.Model
{
    static class Tests
    {
        public static void TestMinMax(OthelloGame _myGame, int minimaxDepth = 3)
        {
            const int testCount = 100;

            object wonGamesLock = new object();
            int wonGames = 0;
            object tieGamesLock = new object();
            int tieGames = 0;

            var stopwatch = Stopwatch.StartNew();

            //Parallel.For(0, testCount, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },index => 
            //{
            for (int index = 0; index < testCount; index++)
            {//non-parallel for loop to debug
                BoardStates player =  (index % 2 == 0) ? BoardStates.black : BoardStates.white;

                OthelloGame testGame = new OthelloGame();
                MinMaxAgent othelloAgent = new MinMaxAgent(2);
                RandomAgent randAgent = new RandomAgent();
                while (!testGame.GameComplete)
                {
                    if (testGame.WhosTurn == player)
                    {

                        testGame.MakeMove(othelloAgent.MakeMove( testGame, player));
                    }
                    else
                    {
                        testGame.MakeMove(randAgent.MakeMove(testGame,~player));
                    }

                }
                if (testGame.GameComplete)//just gotta check
                {
                    if (testGame.FinalWinner == player)
                    {
                        lock (wonGamesLock) { wonGames++; }
                    }
                    else if (testGame.FinalWinner == BoardStates.empty)
                    {
                        lock (tieGamesLock) { tieGames++; }
                    }
                    Console.WriteLine("Finished Game " + index + ", " + testGame.FinalWinner.ToString()
                        + " won " + testGame.GetPieceCount(testGame.FinalWinner) + " to "
                        + testGame.GetPieceCount(OthelloGame.OpposingPlayer(testGame.FinalWinner))); ;
                }
                else
                {
                    throw new Exception("MiniMax Testing didn't complete a game");
                }
            }

            //});
            stopwatch.Stop();
            Console.WriteLine("Won " + wonGames + " / " + testCount + " games, " + ((double)wonGames / testCount) * 100 + " %");
            Console.WriteLine("Tied " + tieGames + " / " + testCount + " games, " + ((double)tieGames / testCount) * 100 + " %");
            Console.WriteLine("Lost " + (testCount - wonGames - tieGames) + " / " + testCount + " games, " + ((double)(testCount - wonGames - tieGames) / testCount) * 100 + " %");
            Console.WriteLine("Elapsed time for games : {0}", stopwatch.Elapsed);
        }

        public static void TestThorFileReading(string path)
        {
            //All methods are comparable, with method 3 & 1 being most competitive


            var totalstopwatch1 = Stopwatch.StartNew();
            List<OthelloGame> method1Games = FileIO.ReadAllGames(path);
            totalstopwatch1.Stop();
            Console.WriteLine("Elapsed time for Method1 : {0}", totalstopwatch1.Elapsed);
            var method1Count = method1Games.Count();

            var totalstopwatch2 = Stopwatch.StartNew();
            List<OthelloGame> method2Games = FileIO.ReadAllGames(path);
            totalstopwatch2.Stop();
            Console.WriteLine("Elapsed time for Method2 : {0}", totalstopwatch2.Elapsed);
            var method2Count = method2Games.Count();

            var totalstopwatch3 = Stopwatch.StartNew();
            List<OthelloGame> method3Games = FileIO.ReadAllGames(path);
            totalstopwatch3.Stop();
            Console.WriteLine("Elapsed time for Method3 : {0}", totalstopwatch3.Elapsed);
            var method3Count = method3Games.Count();

            var totalstopwatch4 = Stopwatch.StartNew();
            List<OthelloGame> method4Games = FileIO.ReadAllGames(path);
            totalstopwatch4.Stop();
            Console.WriteLine("Elapsed time for Method4 : {0}", totalstopwatch4.Elapsed);
            var method4Count = method4Games.Count();

            Console.WriteLine("Method1: " + method1Count + " games");
            Console.WriteLine("Method2: " + method2Count + " games");
            Console.WriteLine("Method3: " + method3Count + " games");
            Console.WriteLine("Method4: " + method4Count + " games");

            Console.WriteLine("Elapsed time for Method1 : {0}", totalstopwatch1.Elapsed);
            Console.WriteLine("Elapsed time for Method2 : {0}", totalstopwatch2.Elapsed);
            Console.WriteLine("Elapsed time for Method3 : {0}", totalstopwatch3.Elapsed);
            Console.WriteLine("Elapsed time for Method4 : {0}", totalstopwatch4.Elapsed);

            Console.WriteLine("Total should be about :" + 1010000);
        }


    }
}
