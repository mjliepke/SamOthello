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

        public static Dictionary<BoardStates, System.Drawing.Color> BoardStateColors = new Dictionary<BoardStates, System.Drawing.Color>()
        {
            {BoardStates.black, System.Drawing.Color.Black },
            {BoardStates.white, System.Drawing.Color.White },
            {BoardStates.empty, System.Drawing.Color.DarkOliveGreen }
        };

        public int BoardSize {get; private set;}
        public int MaxMoves { get; private set; }
        public BoardStates WhosTurn { get; private set; }
        BoardStates[,] Board;

        public OthelloBoard(int boardsize = 8)
        {
            SetupBoard(boardsize);
            WhosTurn = BoardStates.black;
            MaxMoves = (boardsize * boardsize) - 4;//4 preoccupied spaces
        }

        public BoardStates[,] GetBoard()
        {
            return (BoardStates[,])Board.Clone();
        }

        public BoardStates OpposingPlayer(BoardStates Player)
        {
            return (Player.Equals(BoardStates.white) ? BoardStates.black : BoardStates.white);
        }

        public bool GameOver()
        {
            bool over = false;
            for(int i=0; i< BoardSize; i++)
            {
                for(int j=0; j<BoardSize; j++)
                {
                    over |= ValidMove(BoardStates.black, new int[] { i, j });
                    over |= ValidMove(BoardStates.white, new int[] { i, j });
                    if (over) break;
                }
            }
            return over;
        }

        public int PlayesMade()
        {
            int playCount = -4; // Board Starts with 4 peices in play
            foreach(BoardStates piece in Board){
                playCount += Convert.ToInt32(!piece.Equals(BoardStates.empty));
            }
            return playCount;
        }

        public void MakeMove(BoardStates player, int[] location){
            bool valid = true;

            valid &= OnBoard(location);
            valid &= Board[location[0], location[1]].Equals(BoardStates.empty);
            valid &= WhosTurn.Equals(player);
            int[,] takenPieces = TakesPieces(player, location);
            valid &= takenPieces.Length > 0;

            if (valid)
            { 
                Board[location[0], location[1]] = player;
                for (int i=0; i<takenPieces.GetLength(0); i++)
                {
                    Board[takenPieces[i, 0], takenPieces[i, 1]] = player;
                }
                WhosTurn = OpposingPlayer(player);
            }
        }

        public bool ValidMove(BoardStates player, int[] location)
        {
            bool valid = true;

            valid &= OnBoard(location);
            valid &= Board[location[0], location[1]].Equals(BoardStates.empty);
            valid &= WhosTurn.Equals(player);
            int[,] takenPieces = TakesPieces(player, location);
            valid &= takenPieces.Length > 0;

            return valid;
        }

        //**********************Private Methods************************

        private int[,] TakesPieces(BoardStates player, int[] location)
        {
            int[,] taken = new int[(BoardSize-2)*3,2];//array of all pieces to be flipped
            int takenCount = 0;

            int minX = location[0] > 0 ? location[0] -1 : 0;
            int minY = location[1] > 0 ? location[1] -1 : 0;
            int maxX = location[0] + 2 < BoardSize ? location[0] + 1 : BoardSize - 1;
            int maxY = location[1] + 2 < BoardSize ? location[1] + 1 : BoardSize - 1;

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    int[,] subtaken = new int[(BoardSize - 2), 2];
                    int subtakenCount = 0;

                    if ((x != location[0] || y != location[1]) && Board[x, y].Equals(OpposingPlayer(player)))
                    {
                        int[] direction = new int[] { x - location[0], y - location[1] };
                       // if (direction[0] == 0 && direction[1] == 0) break; //Can't test current location.. infinite loop

                        int[] searchedLocation = new int[] { x , y };

                        while (OnBoard(searchedLocation) && Board[searchedLocation[0], searchedLocation[1]].Equals(OpposingPlayer(player)))
                        {
                            subtaken[subtakenCount, 0] = searchedLocation[0];
                            subtaken[subtakenCount, 1] = searchedLocation[1];
                            subtakenCount++;
                            searchedLocation[0] += direction[0];
                            searchedLocation[1] += direction[1];
                
                        }

                        if(!OnBoard(searchedLocation) || Board[searchedLocation[0], searchedLocation[1]].Equals(BoardStates.empty))
                        {
                            subtakenCount = 0;
                        }
                    }

                    for(int i=0;i<subtakenCount; i++)
                    {
                        taken[i + takenCount, 0] = subtaken[i, 0];
                        taken[i + takenCount, 1] = subtaken[i, 1];
                    }
                    takenCount += subtakenCount;
                }
            }

            int[,] trimmedTaken = new int[takenCount, 2];
            for(int i=0;i<takenCount; i++)
            {
                trimmedTaken[i, 0] = taken[i, 0];
                trimmedTaken[i, 1] = taken[i, 1];
            }
            return trimmedTaken;
        }

        private void SetupBoard(int boardsize)
        {
            BoardSize = boardsize;
            Board = new BoardStates[boardsize, boardsize];
            for(int i=0; i<BoardSize; i++)
            {
                for(int j=0; j<BoardSize; j++)
                {
                    Board[i, j] = BoardStates.empty;
                }
            }
            Board[BoardSize / 2 - 1, BoardSize / 2 - 1] = BoardStates.white;
            Board[BoardSize / 2 - 1, BoardSize / 2] = BoardStates.black;
            Board[BoardSize / 2, BoardSize / 2 - 1] = BoardStates.black;
            Board[BoardSize / 2, BoardSize / 2] = BoardStates.white;
        }

        private bool OnBoard(int[] location)
        {
            return (location[0] >= 0 && location[0] < BoardSize && location[1] >= 0 && location[1] < BoardSize);
        }
    }
}
