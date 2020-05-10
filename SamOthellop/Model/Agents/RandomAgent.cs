using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamOthellop.Model.Agents
{
    public class RandomAgent : IOthelloAgent
    {
        public override byte[] MakeMove(OthelloGame game, BoardStates player)
        {
            List<byte[]> possibleMoves = game.GetPossiblePlayList(player);

            if (possibleMoves.Count == 0) return new byte[] { byte.MaxValue, byte.MaxValue };

            Random rndGenerator = new Random();
            int rnd = (int)Math.Floor(rndGenerator.NextDouble() * possibleMoves.Count);

            return possibleMoves[rnd];
        }
    }
}
