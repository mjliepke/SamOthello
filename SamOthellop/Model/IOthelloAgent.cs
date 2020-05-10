using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamOthellop.Model
{
    abstract class IOthelloAgent
    {
        public  abstract byte[] MakeMove(OthelloGame game, BoardStates player);
    }
}
