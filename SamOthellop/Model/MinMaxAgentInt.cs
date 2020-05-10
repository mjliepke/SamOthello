using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamOthellop.Model
{
    class MinMaxAgentInt : IOthelloAgent
    {
        const int SEARCH_DEPTH = 4;


        //*****Weights for Board Evaluation
        int coinDiffWeight;
        int cornerDiffWeight;
        int nearCornerDiffWeight;
        int avalibleMoveDiffWeight;
        int nonTurnableCoinDiffWeight;
        int ControlledCornerDiffWeight;

        //*****Weight offset for Board Evaluation
        int coinDiffOffset;
        int cornerDiffOffset;
        int nearCornerDiffOffset;
        int avalibleMoveDiffOffset;
        int nonTurnableCoinDiffOffset;
        int ControlledCornerDiffOffset;

        //*****Time-dependent Order for Weights : value = weight * (time - offset)^power
        int coinDiffPower;
        int cornerDiffPower;
        int nearCornerDiffPower;
        int avalibleMoveDiffPower;
        int nonTurnableCoinDiffPower;
        int ControlledCornerDiffPower;

        public MinMaxAgentInt(int[] weights)
        {
            coinDiffWeight = weights[0];
            cornerDiffWeight = weights[1];
            nearCornerDiffWeight = weights[2];
            avalibleMoveDiffWeight = weights[3];
            nonTurnableCoinDiffWeight = weights[4];
            ControlledCornerDiffWeight = weights[5];

            coinDiffOffset = weights[6];
            cornerDiffOffset = weights[7];
            nearCornerDiffOffset = weights[8];
            avalibleMoveDiffOffset = weights[9];
            nonTurnableCoinDiffOffset = weights[10];
            ControlledCornerDiffOffset = weights[11];

            coinDiffPower = weights[12];
            cornerDiffPower = weights[13];
            nearCornerDiffPower = weights[14];
            avalibleMoveDiffPower = weights[15];
            nonTurnableCoinDiffPower = weights[16];
            ControlledCornerDiffPower = weights[17];
        }
        public MinMaxAgentInt()
        {
            //ok performing values
            coinDiffWeight = 3;
            cornerDiffWeight = 75;
            nearCornerDiffWeight = -5;
            avalibleMoveDiffWeight = 0;
            nonTurnableCoinDiffWeight = 10;
            ControlledCornerDiffWeight = 100;

            //30 is halfway through game, so default to function extrema/inflection
            coinDiffOffset = 30;
            cornerDiffOffset = 30;
            nearCornerDiffOffset = 30;
            avalibleMoveDiffOffset = 30;
            nonTurnableCoinDiffOffset = 30;
            ControlledCornerDiffOffset = 30;

            //linear if not initialized
            coinDiffPower = 1;
            cornerDiffPower = 1;
            nearCornerDiffPower = 1;
            avalibleMoveDiffPower = 1;
            nonTurnableCoinDiffPower = 1;
            ControlledCornerDiffPower = 1;
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
                        coinDiffWeight,
                        cornerDiffWeight,
                        nearCornerDiffWeight,
                        avalibleMoveDiffWeight,
                        nonTurnableCoinDiffWeight,
                        ControlledCornerDiffWeight,

                        coinDiffOffset,
                        cornerDiffOffset,
                        nearCornerDiffOffset,
                        avalibleMoveDiffOffset,
                         nonTurnableCoinDiffOffset,
                        ControlledCornerDiffOffset,

                        coinDiffPower,
                        cornerDiffPower,
                         nearCornerDiffPower,
                        avalibleMoveDiffPower,
                         nonTurnableCoinDiffPower,
                        ControlledCornerDiffPower
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
            const int endGame = 20; //<20 moves is endgame
            const int midGame = 40; // 20 moves in is midgame


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

            value += coinDiffWeight * Math.Pow((game.GetPieceCount(player) - game.GetPieceCount(~player) + empty - coinDiffOffset), coinDiffPower);
            value += cornerDiffWeight * Math.Pow((game.GetCornerCount(player) - game.GetCornerCount(~player) + empty - cornerDiffOffset), cornerDiffPower);
            value += nearCornerDiffWeight * Math.Pow((game.GetAdjCornerCount(player) - game.GetAdjCornerCount(~player) + empty - nearCornerDiffOffset), nearCornerDiffPower);
            value += avalibleMoveDiffWeight * Math.Pow((game.GetPossiblePlayList(player).Count() - game.GetPossiblePlayList(~player).Count() + empty - avalibleMoveDiffOffset), avalibleMoveDiffPower);
            value += nonTurnableCoinDiffWeight * Math.Pow((game.GetSafePeiceCountEstimation(player) - game.GetSafePeiceCountEstimation(~player) + empty - nonTurnableCoinDiffOffset), nonTurnableCoinDiffPower);
            value += ControlledCornerDiffWeight * Math.Pow((game.GetControlledCorners(player) - game.GetControlledCorners(~player) + empty - ControlledCornerDiffOffset), ControlledCornerDiffPower);
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
