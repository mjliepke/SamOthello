using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SamOthellop.OthelloGame;

namespace SamOthellop.View
{
    public static class BoardColorDictionary
    {
         public static Dictionary<SamOthellop.BoardStates, System.Drawing.Color> BoardStateColors = new Dictionary<BoardStates, System.Drawing.Color>()
        {
            {BoardStates.black, System.Drawing.Color.Black },
            {BoardStates.white, System.Drawing.Color.White },
            {BoardStates.empty, System.Drawing.Color.DarkOliveGreen }
        };
    }
}
