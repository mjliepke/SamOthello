using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamOthellop
{
    class OthelloBoard
    {
        public enum BoardStates
        {
            white = 0,
            black = 1,
            empty = 2
        }
        public Dictionary<BoardStates, System.Drawing.Color> BoardStateColors = new Dictionary<BoardStates, System.Drawing.Color>()
        {
            {BoardStates.black, System.Drawing.Color.Black },
            {BoardStates.white, System.Drawing.Color.White },
            {BoardStates.empty, System.Drawing.Color.DarkOliveGreen }
        };


        public int BoardSize
        {
            get; private set;
        }

        BoardStates[,] Board;

        public OthelloBoard(int boardsize = 8)
        {
            
            SetupBoard(boardsize);
        }

        public BoardStates[,] GetBoard()
        {
            return (BoardStates[,])Board.Clone();
        }

        public void MakeMove(BoardStates player, int[] location){
           if(ValidMove(player, location))
            {
                Board[location[0], location[1]] = player;
            }
        }

        public bool ValidMove(BoardStates player, int[] location)
        {
            bool valid = true;

            valid &= (location[0] > 0 || location[0] < BoardSize);
            valid &= (location[1] > 0 || location[1] < BoardSize);
            valid &= Board[location[0], location[1]].Equals(BoardStates.empty);

            return valid;
        }

        private void SetupBoard(int boardsize)
        {
            BoardSize = boardsize;
            Board = new BoardStates[boardsize, boardsize];
            Board[BoardSize / 2 - 1, BoardSize / 2 - 1] = BoardStates.white;
            Board[BoardSize / 2 - 1, BoardSize / 2] = BoardStates.black;
            Board[BoardSize / 2, BoardSize / 2 - 1] = BoardStates.black;
            Board[BoardSize / 2, BoardSize / 2] = BoardStates.white;
        }


    }
}
