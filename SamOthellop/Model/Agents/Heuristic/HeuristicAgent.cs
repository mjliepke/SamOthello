using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamOthellop.Model.Agents
{
    /// <summary>
    /// Evaluates board based on user-defined features
    /// </summary>
    class HeuristicAgent : IOthelloAgent
    {
        /// <summary>
        /// Formula takes the following form : value = (diffSlope * diff) + (offsetSlope * time)  + diffOffset
        /// </summary>

        //*****difference-dependent slope for Board Evaluation
        double coinDiffSlope;
        double cornerDiffSlope;
        double nearCornerDiffSlope;
        double avalibleMoveDiffSlope;
        double nonTurnableCoinDiffSlope;
        double controlledCornerDiffSlope;

        //*****offset for Board Evaluation
        double coinDiffOffset;
        double cornerDiffOffset;
        double nearCornerDiffOffset;
        double avalibleMoveDiffOffset;
        double nonTurnableCoinDiffOffset;
        double controlledCornerDiffOffset;

        ////*****Time-dependent Slope 
        double coinTimeSlope;
        double cornerTimeSlope;
        double nearCornerTimeSlope;
        double avalibleMoveTimeSlope;
        double nonTurnableTimeSlope;
        double controlledCornerTimeSlope;

        public HeuristicAgent()
        {
            try
            {
                string[] strWeights = System.IO.File.ReadAllLines("bestchromosome.dat");
                double[] weights = strWeights.Select((gene) => double.Parse(gene)).ToArray();
                coinDiffSlope = weights[0];
                cornerDiffSlope = weights[1];
                nearCornerDiffSlope = weights[2];
                avalibleMoveDiffSlope = weights[3];
                nonTurnableCoinDiffSlope = weights[4];
                controlledCornerDiffSlope = weights[5];

                coinDiffOffset = weights[6];
                cornerDiffOffset = weights[7];
                nearCornerDiffOffset = weights[8];
                avalibleMoveDiffOffset = weights[9];
                nonTurnableCoinDiffOffset = weights[10];
                controlledCornerDiffOffset = weights[11];

                coinTimeSlope = weights[11];
                cornerTimeSlope = weights[13];
                nearCornerTimeSlope = weights[14];
                avalibleMoveTimeSlope = weights[15];
                nonTurnableTimeSlope = weights[16];
                controlledCornerTimeSlope = weights[17];
            }
            catch (Exception)
            {
                //ok performing values
                coinDiffSlope = 3;
                cornerDiffSlope = 75;
                nearCornerDiffSlope = -5;
                avalibleMoveDiffSlope = 0;
                nonTurnableCoinDiffSlope = 10;
                controlledCornerDiffSlope = 100;
                
                coinDiffOffset = 30;
                cornerDiffOffset = 30;
                nearCornerDiffOffset = 30;
                avalibleMoveDiffOffset = 30;
                nonTurnableCoinDiffOffset = 30;
                controlledCornerDiffOffset = 30;
                
                coinTimeSlope = 0;
                cornerTimeSlope = 0;
                nearCornerTimeSlope = 0;
                avalibleMoveTimeSlope = 0;
                nonTurnableTimeSlope = 0;
                controlledCornerTimeSlope = 0;
            }
        }
        public HeuristicAgent(double[] weights)
        {
            coinDiffSlope = weights[0];
            cornerDiffSlope = weights[1];
            nearCornerDiffSlope = weights[2];
            avalibleMoveDiffSlope = weights[3];
            nonTurnableCoinDiffSlope = weights[4];
            controlledCornerDiffSlope = weights[5];

            coinDiffOffset = weights[6];
            cornerDiffOffset = weights[7];
            nearCornerDiffOffset = weights[8];
            avalibleMoveDiffOffset = weights[9];
            nonTurnableCoinDiffOffset = weights[10];
            controlledCornerDiffOffset = weights[11];

            coinTimeSlope = weights[11];
            cornerTimeSlope = weights[13];
            nearCornerTimeSlope = weights[14];
            avalibleMoveTimeSlope = weights[15];
            nonTurnableTimeSlope = weights[16];
            controlledCornerTimeSlope = weights[17];
        }

        public override byte[] MakeMove(OthelloGame game, BoardStates player)
        {
            byte[] bestMove = new byte[] { byte.MaxValue, byte.MaxValue };
            List<byte[]> moves = game.GetPossiblePlayList();

            double bestScore = int.MinValue + 1;
            if (game.GetPieceCount(BoardStates.empty) > 58)//first two moves, don't compute
            {
                return OpeningMove(player, game);
            }
            else if (moves.Count == 1) //don't compute if there is only 1 move
            {
                return moves[0];
            }

            foreach (byte[] move in moves)
            {
                OthelloGame testGame = game.DeepCopy();
                testGame.MakeMove(move);
                double thisScore = HeuristicEval(player, testGame);
                if (thisScore > bestScore)
                {
                    bestScore = thisScore;
                    bestMove = move;
                }
            }
            if ((bestMove[0] == byte.MaxValue || bestMove[1] == byte.MaxValue) && moves.Count > 0)
            {//All moves are valued at -inf, return one of em
                return moves[0];
            }
            return bestMove;
        }
        /// <summary>
        /// returns weights for Heuristic evaluation function
        /// </summary>
        /// <returns></returns>
        public double[] GetWeights()
        {
            return new double[] {
                        coinDiffSlope,
                        cornerDiffSlope,
                        nearCornerDiffSlope,
                        avalibleMoveDiffSlope,
                        nonTurnableCoinDiffSlope,
                        controlledCornerDiffSlope,

                        coinDiffOffset,
                        cornerDiffOffset,
                        nearCornerDiffOffset,
                        avalibleMoveDiffOffset,
                         nonTurnableCoinDiffOffset,
                        controlledCornerDiffOffset,

                        coinTimeSlope,
                        cornerTimeSlope,
                        nearCornerTimeSlope,
                        avalibleMoveTimeSlope,
                        nonTurnableTimeSlope,
                        controlledCornerTimeSlope
        };
        }

        private double HeuristicEval(BoardStates player, OthelloGame game)
        {
            ///Based of features of the board that humans have identified.  
            ///Hints of evaluation from any source I could find
            ///idealy these could me optimized using a genetic algorithm,
            ///but that is a different project


            double value = 0;
            int empty = game.GetPieceCount(BoardStates.empty);


            if (game.GameComplete)
            {
                return CompleteEval(player, game);
            }

            value += coinDiffSlope * (game.GetPieceCount(player) - game.GetPieceCount(~player)) + (empty * coinTimeSlope) + coinDiffOffset;
            value += cornerDiffSlope * (game.GetCornerCount(player) - game.GetCornerCount(~player)) + (empty * cornerTimeSlope) + cornerDiffOffset;
            value += nearCornerDiffSlope * (game.GetAdjCornerCount(player) - game.GetAdjCornerCount(~player)) + (empty * nearCornerTimeSlope) + nearCornerDiffOffset;
            value += avalibleMoveDiffSlope * (game.GetPossiblePlayList(player).Count() - game.GetPossiblePlayList(~player).Count()) + (empty * avalibleMoveTimeSlope) + avalibleMoveDiffOffset;
            value += nonTurnableCoinDiffSlope * (game.GetSafePeiceCountEstimation(player) - game.GetSafePeiceCountEstimation(~player)) + (empty * nonTurnableTimeSlope) + nonTurnableCoinDiffOffset;
            value += controlledCornerDiffSlope * (game.GetControlledCorners(player) - game.GetControlledCorners(~player)) + (empty * controlledCornerTimeSlope) + controlledCornerDiffOffset;
            return value;
        }

        private static int CompleteEval(BoardStates player, OthelloGame game)
        {
            ///Returns complete worth of board, -inf for loss, +inf for win
            if (game.FinalWinner == player)
            {
                return int.MaxValue;
            }
            else return int.MinValue;
        }

        private static byte[] OpeningMove(BoardStates player, OthelloGame game)
        {//avoid computation for first move - only one symmetric option
         //randomly select perpendicular or diagonal for second move - parallel 
         //has been shown to be much worse
         //SPECIFIC TO 8x8 BOARDS
            byte[][] firstMoves = new byte[4][] {
                                        new byte[]{ 2, 3 },
                                        new byte[]{ 3, 2 },
                                        new byte[]{ 4, 5 },
                                        new byte[]{ 5, 4 }};

            if (game.GetPieceCount(BoardStates.empty) == 60)
            {
                Random rndGen = new Random();
                int rand = (int)Math.Ceiling(rndGen.NextDouble() * 4);
                switch (rand)
                {
                    case 1:
                        return firstMoves[0];
                    case 2:
                        return firstMoves[1];
                    case 3:
                        return firstMoves[2];
                    case 4:
                        return firstMoves[3];
                    default:
                        throw new Exception("OpeningMove has faulted with random number generation");
                }
            }
            if (game.GetPieceCount(BoardStates.empty) == 59)
            {
                List<byte[]> moves = game.GetPossiblePlayList();
                Random rndGen = new Random();
                byte rand = (byte)Math.Ceiling(rndGen.NextDouble() * 2);
                switch (rand)
                {
                    case 1: //diagonal
                        return moves[0];
                    case 2: //perpendicular
                        return moves[0];
                    default:
                        throw new Exception("Opening move has faulted with random number generation");
                }
            }
            return new byte[] { byte.MaxValue, byte.MaxValue };
        }
    }
}
