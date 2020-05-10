using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamOthellop.Model.Agents
{
    class GreedyAgent : IEvaluationAgent
    {
        public override double EvaluateBoard(OthelloGame game, BoardStates player)
        {
            return game.GetPieceCount(player);
        }

        public override byte[] MakeMove(OthelloGame game, BoardStates player)
        {
            List<byte[]> moves = game.GetPossiblePlayList(player);

            double bestScore = double.MinValue;
            byte[] bestMove = new byte[] { byte.MaxValue, byte.MaxValue};

            foreach (byte[] move in moves)
            {
                OthelloGame testGame = game.DeepCopy();
                testGame.MakeMove(move);

                double thisScore = EvaluateBoard(testGame, player);

                if (thisScore > bestScore)
                {
                    bestScore = thisScore;
                    bestMove = move;
                }
            }
            return bestMove;
        }
    }
}
