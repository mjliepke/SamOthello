using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamOthellop.Model
{
    class MinMaxAgent : IOthelloAgent
    {
         private int SEARCH_DEPTH = 4;

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

        public MinMaxAgent(double[] weights, int searchDepth)
        {
            SEARCH_DEPTH = searchDepth;
            coinDiffSlope = weights[0];
            cornerDiffSlope = weights[1];
            nearCornerDiffSlope = weights[2];
            avalibleMoveDiffSlope = weights[3];
            nonTurnableCoinDiffSlope = weights[4];
            controlledCornerDiffSlope = weights[5];

            //coinDiffOffset = weights[6];
            //cornerDiffOffset = weights[7];
            //nearCornerDiffOffset = weights[8];
            //avalibleMoveDiffOffset = weights[9];
            //nonTurnableCoinDiffOffset = weights[10];
            //controlledCornerDiffOffset = weights[11];

            //coinTimeSlope = weights[11];
            //cornerTimeSlope = weights[13];
            //nearCornerTimeSlope = weights[14];
            //avalibleMoveTimeSlope = weights[15];
            //nonTurnableTimeSlope = weights[16];
            //controlledCornerTimeSlope = weights[17];
        }
      
        public MinMaxAgent(int searchDepth = 4)
        {
            SEARCH_DEPTH = searchDepth;
            //ok performing values
            coinDiffSlope = 3;
            cornerDiffSlope = 75;
            nearCornerDiffSlope = -5;
            avalibleMoveDiffSlope = 0;
            nonTurnableCoinDiffSlope = 10;
            controlledCornerDiffSlope = 100;

            //30 is halfway through game, so default to function extrema/inflection
            coinDiffOffset = 30;
            cornerDiffOffset = 30;
            nearCornerDiffOffset = 30;
            avalibleMoveDiffOffset = 30;
            nonTurnableCoinDiffOffset = 30;
            controlledCornerDiffOffset = 30;

            //linear if not initialized
            coinTimeSlope = 1;
            cornerTimeSlope = 1;
            nearCornerTimeSlope = 1;
            avalibleMoveTimeSlope = 1;
            nonTurnableTimeSlope = 1;
            controlledCornerTimeSlope = 1;
        }

        public override byte[] MakeMove(OthelloGame game, BoardStates player)
        {
            return PredictBestMove(SEARCH_DEPTH, game, player);
        }

        public byte[] MakeMove(int searchDepth, OthelloGame game, BoardStates player)
        {
            return PredictBestMove(searchDepth, game, player);
        }

        public double[] GetWeights()
        {
            return new double[] {
                        coinDiffSlope,
                        cornerDiffSlope,
                        nearCornerDiffSlope,
                        avalibleMoveDiffSlope,
                        nonTurnableCoinDiffSlope,
                        controlledCornerDiffSlope,

                        //coinDiffOffset,
                        //cornerDiffOffset,
                        //nearCornerDiffOffset,
                        //avalibleMoveDiffOffset,
                        // nonTurnableCoinDiffOffset,
                        //controlledCornerDiffOffset,

                        //coinDiffPower,
                        //cornerDiffPower,
                        // nearCornerDiffPower,
                        //avalibleMoveDiffPower,
                        // nonTurnableCoinDiffPower,
                        //ControlledCornerDiffPower
            };
        }

        private byte[] PredictBestMove(int depth, OthelloGame game, BoardStates player)
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
                double thisScore = MinimaxAlphaBeta(testGame, depth - 1, double.MinValue, double.MaxValue, player);
                if (thisScore > bestScore)
                {
                    bestScore = thisScore;
                    bestMove = move;
                }
            }
            if((bestMove[0] == byte.MaxValue || bestMove[1] == byte.MaxValue) && moves.Count > 0 )
            {//All moves are valued at -inf, return one of em
                return moves[0];
            }
            return bestMove;
        }

        private double MinimaxAlphaBeta(OthelloGame board, int depth, double a, double b, BoardStates player)// bool isMaxPlayer)
        {
            // The heart of our AI. Minimax algorithm with alpha-beta pruning to speed up computation.
            // Higher search depths = greater difficulty.
            //from oliverzh200/reversi https://github.com/oliverzh2000/reversi

            if (depth == 0 || board.GameComplete)
            {

                return HeuristicEval(player, board);
            }
            double bestScore = double.MinValue;
            List<byte[]> validMoves = board.GetPossiblePlayList();
            if (validMoves.Count > 0)
            {
                foreach (byte[] move in validMoves)
                {
                    OthelloGame childBoard = board.DeepCopy();
                    childBoard.MakeMove(move);
                    double nodeScore = MinimaxAlphaBeta(childBoard, depth - 1, a, b, player);

                    bestScore = Math.Max(bestScore, nodeScore);
                    a = Math.Max(bestScore, a);

                    if (b <= a) //Prune
                    {
                        break;
                    }
                }
            }
            else
            {
                return MinimaxAlphaBeta(board, depth, a, b, player);
            }
            return bestScore;
        }

        private double HeuristicEval(BoardStates player, OthelloGame game)
        {
            //Based of features of the board that humans have identified.  
            //Hints of evaluation from any source I could find
            //idealy these could me optimized using a genetic algorithm,
            //but that is a different project


            const int searchableDepthOverride = 2; //override min-max in favor of complete evaluation


            double value = 0;
            int empty = game.GetPieceCount(BoardStates.empty);


            if (game.GameComplete)
            {
                return CompleteEval(player, game);
            }
            else if (empty < searchableDepthOverride)
            {
                return MinimaxAlphaBeta(game, searchableDepthOverride, int.MinValue, int.MaxValue, player);
            }

            value += coinDiffSlope * (game.GetPieceCount(player) - game.GetPieceCount(~player)) + (empty * coinTimeSlope) + coinDiffOffset;
            value += cornerDiffSlope * (game.GetCornerCount(player) - game.GetCornerCount(~player)) + (empty * cornerTimeSlope) + cornerDiffOffset;
            value += nearCornerDiffSlope * (game.GetAdjCornerCount(player) - game.GetAdjCornerCount(~player)) + (empty * nearCornerTimeSlope) + nearCornerDiffOffset;
            value += avalibleMoveDiffSlope * (game.GetPossiblePlayList(player).Count() - game.GetPossiblePlayList(~player).Count())+ (empty * avalibleMoveTimeSlope) + avalibleMoveDiffOffset;
            value += nonTurnableCoinDiffSlope * (game.GetSafePeiceCountEstimation(player) - game.GetSafePeiceCountEstimation(~player))+ (empty * nonTurnableTimeSlope) + nonTurnableCoinDiffOffset;
            value += controlledCornerDiffSlope * (game.GetControlledCorners(player) - game.GetControlledCorners(~player)) + (empty * controlledCornerTimeSlope) + controlledCornerDiffOffset;
            return value;
        }

        private static int CompleteEval(BoardStates player, OthelloGame game)
        {
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
