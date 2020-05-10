using SamOthellop.Model.Agents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamOthellop.Model
{
    public class MinMaxAgent : IEvaluationAgent
    {
        private int SEARCH_DEPTH = 4;
        IEvaluationAgent evalAgent;

        public MinMaxAgent(IEvaluationAgent agent, int searchDepth = 4)
        {
            evalAgent = agent;
            SEARCH_DEPTH = searchDepth;
        }

        public MinMaxAgent(int searchDepth = 4)
        {
            SEARCH_DEPTH = searchDepth;
            evalAgent = new HeuristicAgent();
        }

        public override byte[] MakeMove(OthelloGame game, BoardStates player)
        {
            return PredictBestMove(SEARCH_DEPTH, game, player);
        }

        public override double EvaluateBoard(OthelloGame game, BoardStates player)
        {
            throw new NotImplementedException();
        }

        private byte[] PredictBestMove(int depth, OthelloGame game, BoardStates player)
        {
            byte[] bestMove = new byte[] { byte.MaxValue, byte.MaxValue };
            List<byte[]> moves = game.GetPossiblePlayList();

            double bestScore = int.MinValue + 1;

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
            if ((bestMove[0] == byte.MaxValue || bestMove[1] == byte.MaxValue) && moves.Count > 0)
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

                return evalAgent.EvaluateBoard(board, player);
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
    }
}
