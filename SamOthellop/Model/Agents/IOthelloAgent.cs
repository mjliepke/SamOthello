using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamOthellop.Model
{
    public abstract class IOthelloAgent
    {
        /// <summary>
        /// Returns Agent's move of choice
        /// </summary>
        /// <param name="game"></param>
        /// <param name="player"></param>
        /// <returns>Location of choice move</returns>
        public  abstract byte[] MakeMove(OthelloGame game, BoardStates player);
    }
}
