using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamOthellop
{
    class OthelloGame
    {
        public enum BoardStates
        {
            white = 2,
            black = 1,
            empty = 0
        }

        public static Dictionary<BoardStates, System.Drawing.Color> BoardStateColors = new Dictionary<BoardStates, System.Drawing.Color>()
        {
            {BoardStates.black, System.Drawing.Color.Black },
            {BoardStates.white, System.Drawing.Color.White },
            {BoardStates.empty, System.Drawing.Color.DarkOliveGreen }
        };

        public int BoardSize { get; private set; }
        public int MaxMoves { get; private set; }
        public BoardStates WhosTurn { get; private set; }
        BoardStates[,] Board;

        public OthelloGame(int boardsize = 8)
        {
            SetupGame(boardsize);
            MaxMoves = (boardsize * boardsize) - 4;//4 preoccupied spaces
        }

        public OthelloGame(BoardStates [,] board, BoardStates whosTurn)
        {
            BoardSize = board.GetLength(0);
            Board = board;
            WhosTurn = whosTurn;

            MaxMoves = (BoardSize * BoardSize) - 4;//4 preoccupied spaces
        }

        public OthelloGame(int [,] board, int whosTurn)
        {
            BoardSize = board.GetLength(0);
            for(int i=0;i<BoardSize; i++)
            {
                for(int j=0; j<BoardSize; j++)
                {
                    Board[i, j] = board[i, j] == 0 ? BoardStates.empty : board[i, j] == 1 ? BoardStates.black : BoardStates.white;
                }
            }
        }

        public BoardStates OpposingPlayer(BoardStates Player)
        {
            return (Player.Equals(BoardStates.white) ? BoardStates.black : BoardStates.white);
        }

        public int MaxMovesLeft()
        ///Returns max moves left if all pieces are played
        {
            return (MaxMoves - MovesMade());
        }

        public bool GameOver()
        {
            bool moveExists = false;
            //for debugging:
            int i, j;
            for (i = 0; i < BoardSize; i++)
            {
                for (j = 0; j < BoardSize; j++)
                {
                    moveExists |= ValidMove(BoardStates.black, new int[] { i, j });
                    moveExists |= ValidMove(BoardStates.white, new int[] { i, j });
                    if (moveExists)
                    {
                        break;
                    }
                }
            }
            return !moveExists;
        }

        public void ResetBoard()
        {
            SetupGame(BoardSize);
        }

        public int MovesMade()
        {
            int playCount = -4; // Board Starts with 4 peices in play
            foreach (BoardStates piece in Board)
            {
                if (!piece.Equals(BoardStates.empty)) playCount++;
            }
            return playCount;
        }

        public void MakeMove(BoardStates player, int[] location)
        {
            bool valid = true;
            valid &= !GameOver();
            valid &= OnBoard(location);
            valid &= Board[location[0], location[1]].Equals(BoardStates.empty);
            valid &= WhosTurn.Equals(player);
            int[,] takenPieces = TakesPieces(player, location);
            valid &= takenPieces.Length > 0;

            if (valid)
            {
                Board[location[0], location[1]] = player;
                for (int i = 0; i < takenPieces.GetLength(0); i++)
                {
                    Board[takenPieces[i, 0], takenPieces[i, 1]] = player;
                }
                WhosTurn = OpposingPlayer(player);
            }
        }

        public void MakeRandomMove(BoardStates player)
        {
            int[,] possibleMoves = new int[MaxMoves - MovesMade(), 2];
            int possibleMoveCount = 0;
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    if (ValidMove(player, new int[] { i, j }))
                    {
                        possibleMoves[possibleMoveCount, 0] = i;
                        possibleMoves[possibleMoveCount, 1] = j;
                        possibleMoveCount++;
                    }
                }
            }
            if (possibleMoveCount == 0)
            {
                WhosTurn = OpposingPlayer(WhosTurn);
                return;
            }
            Random rndGenerator = new Random();
            int rnd = (int)Math.Floor(rndGenerator.NextDouble() * possibleMoveCount);

            MakeMove(player, new int[] { possibleMoves[rnd, 0], possibleMoves[rnd, 1] });
        }

        public void MakeRandomMove()
        {
            int moves = MovesMade();
            BoardStates player1 = WhosTurn;
            MakeRandomMove(WhosTurn);
            if (moves == MovesMade())//will happen when play does not have move & faults to next
            {
                MakeRandomMove(WhosTurn);
            }
        }

        public bool ValidMove(BoardStates player, int[] location, bool playerTurn = false)
        {
            bool valid = true;

            valid &= OnBoard(location);
            valid &= Board[location[0], location[1]].Equals(BoardStates.empty);
            if (playerTurn) valid &= WhosTurn.Equals(player);
            int[,] takenPieces = TakesPieces(player, location);
            valid &= takenPieces.Length > 0;

            return valid;
        }

        //**************************Get Methods****************************   

        public BoardStates[,] GetBoard() { return (BoardStates[,])Board.Clone(); }

        public bool[,] GetWhiteStateArray() { return GetStateArray(BoardStates.white); }

        public bool[,] GetBlackStateArray() { return GetStateArray(BoardStates.black); }

        public bool[,] GetPlayableStateArray() { return GetPlayableStateArray(WhosTurn); }

        public bool[,] GetPlayableStateArray(BoardStates player)
        {
            bool[,] bstates = new bool[BoardSize, BoardSize];
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    bstates[i,j] = ValidMove(player, new int[] { i, j }) ? true : false;
                }
            }
            return bstates;
        }

        public bool[] GetWhiteStateList() { return GetStateList(BoardStates.white); }

        public bool[] GetBlackStateList() { return GetStateList(BoardStates.black); }

        public bool[] GetBoardStateList()
        {
            bool[] bstate = new bool[2 * BoardSize * BoardSize];
            Array.Copy(GetBlackStateList(), bstate, BoardSize);
            Array.Copy(GetWhiteStateArray(), 0, bstate, BoardSize, BoardSize);
            return bstate;
        }

        public bool[] GetPlayableStateList() { return GetPlayableStateList(WhosTurn); }

        public int GetWhitePieceCount() { return GetPieceCount(BoardStates.white); }

        public int GetBlackPieceCount() { return GetPieceCount(BoardStates.black); }

        public int GetPieceCount(BoardStates bstate)
        {
            int count = 0;
            foreach (BoardStates piece in Board)
            {
                if (piece.Equals(bstate)) count++;
            }
            return count;
        }

        public bool[] GetPlayableStateList(BoardStates player)
        {
            bool[] bstates = new bool[BoardSize * BoardSize];
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    bstates[i + j] = ValidMove(player, new int[] { i, j }) ? true : false;
                }
            }
            return bstates;
        }

        //**************************Private Methods****************************

        private int[,] TakesPieces(BoardStates player, int[] location)
        {
            int[,] taken = new int[(BoardSize - 2) * 3, 2];//array of all pieces to be flipped
            int takenCount = 0;

            int minX = location[0] > 0 ? location[0] - 1 : 0;
            int minY = location[1] > 0 ? location[1] - 1 : 0;
            int maxX = location[0] + 2 < BoardSize ? location[0] + 1 : BoardSize - 1;
            int maxY = location[1] + 2 < BoardSize ? location[1] + 1 : BoardSize - 1;

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    int[,] subtaken = new int[BoardSize, 2];
                    int subtakenCount = 0;

                    if ((x != location[0] || y != location[1]) && Board[x, y].Equals(OpposingPlayer(player)))
                    {
                        int[] direction = new int[] { x - location[0], y - location[1] };
                        // if (direction[0] == 0 && direction[1] == 0) break; //Can't test current location.. infinite loop

                        int[] searchedLocation = new int[] { x, y };

                        while (OnBoard(searchedLocation) && Board[searchedLocation[0], searchedLocation[1]].Equals(OpposingPlayer(player)))
                        {
                            subtaken[subtakenCount, 0] = searchedLocation[0];
                            subtaken[subtakenCount, 1] = searchedLocation[1];
                            subtakenCount++;
                            searchedLocation[0] += direction[0];
                            searchedLocation[1] += direction[1];

                        }

                        if (!OnBoard(searchedLocation) || Board[searchedLocation[0], searchedLocation[1]].Equals(BoardStates.empty))
                        {
                            subtakenCount = 0;
                        }
                    }

                    for (int i = 0; i < subtakenCount; i++)
                    {
                        taken[i + takenCount, 0] = subtaken[i, 0];
                        taken[i + takenCount, 1] = subtaken[i, 1];
                    }
                    takenCount += subtakenCount;
                }
            }

            int[,] trimmedTaken = new int[takenCount, 2];
            for (int i = 0; i < takenCount; i++)
            {
                trimmedTaken[i, 0] = taken[i, 0];
                trimmedTaken[i, 1] = taken[i, 1];
            }
            return trimmedTaken;
        }

        private bool[,] GetStateArray(BoardStates bstate)
        ///
        ///Returns bool array of bstate peices the size of the board,
        ///With 1 meaning a bstate peice is there, 0 meaning it is not
        ///
        {
            bool[,] bstates = new bool[BoardSize, BoardSize];
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    bstates[i, j] = Board[i, j].Equals(bstate) ? true : false;
                }
            }
            return bstates;
        }

        private bool[] GetStateList(BoardStates bstate)
        ///
        ///
        ///
        {
            bool[] bstates = new bool[BoardSize * BoardSize];
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    bstates[i + j] = Board[i, j].Equals(bstate) ? true : false;
                }
            }
            return bstates;
        }

        private void SetupGame(int boardsize)
        {
            BoardSize = boardsize;
            Board = new BoardStates[boardsize, boardsize];
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    Board[i, j] = BoardStates.empty;
                }
            }
            Board[BoardSize / 2 - 1, BoardSize / 2 - 1] = BoardStates.white;
            Board[BoardSize / 2 - 1, BoardSize / 2] = BoardStates.black;
            Board[BoardSize / 2, BoardSize / 2 - 1] = BoardStates.black;
            Board[BoardSize / 2, BoardSize / 2] = BoardStates.white;

            WhosTurn = BoardStates.black;
        }

        private bool OnBoard(int[] location)
        {
            return (location[0] >= 0 && location[0] < BoardSize && location[1] >= 0 && location[1] < BoardSize);
        }
    }
}
