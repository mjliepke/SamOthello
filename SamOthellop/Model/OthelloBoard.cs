using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamOthellop.Model
{
    [Flags]
    public enum BoardStates : Byte
    {
        white = 0x10,
        black = 0xef, //black = !white
        empty = 0x00,
    }
    class BoardBase
    {
        public const byte BOARD_SIZE = 8;
        public byte MaxMoves { get; protected set; }
        public byte FinalBlackTally { get; protected set; }
        public byte FinalWhiteTally { get; protected set; }
        public BoardStates FinalWinner { get; protected set; }
        public BoardStates WhosTurn { get; protected set; }
        public bool GameComplete { get; protected set; }
        public BoardStates[,] Board { get; protected set; }
        protected List<BoardStates[,]> BoardHistory;

        public BoardBase()
        {
            SetupBoard();

        }

        public void ResetBoard()
        {
            SetupBoard();
        }

        private bool GameOver()
        {
            bool moveExists = false;

            for (byte i = 0; i < BOARD_SIZE; i++)
            {
                for (byte j = 0; j < BOARD_SIZE; j++)
                {
                    if(ValidMove(BoardStates.black, new byte[] { i, j }) ||
                        ValidMove(BoardStates.white, new byte[] { i, j }))
                    {
                        moveExists = true;
                        break;
                    }
                }
            }
            if (!moveExists)
            {
                InitializeEndOfGameAttributes();
            }
            return !moveExists;
        }

        public bool MakeMove(BoardStates player, byte[] location)
        {
            bool moveMade = false;
            List<byte[]> takenPeices;
            bool valid = ValidMove(player, location, out takenPeices);
            if (valid)
            {
                Board[location[0], location[1]] = player;
                for (int i = 0; i < takenPeices.Count(); i++)
                {
                    Board[takenPeices[i][0], takenPeices[i][1]] = player;
                }
                BoardStates opponent = ~player;
                if (PlayerHasMove(opponent))
                {
                    WhosTurn = opponent;//only switch whos turn it is if other player has moves avalible
                }

                BoardHistory.Add(GetBoardCopy(Board));
                moveMade = true;
                
            }
            else
            {
                int debugLocation = 0;//to stop debugger on invalid move
            }
            GameOver();
            return moveMade;
        }

        public bool MakeMove(byte[] location)
        {
            BoardStates player = WhosTurn;
            return MakeMove(player, location);
        }

        public bool ValidMove(BoardStates player, byte[] location, out List<byte[]> takenPeices, bool playerTurn = false)
        {
            bool valid = true;
            valid &= !GameComplete;
            valid &= OnBoard(location);
            if (!valid)
            {
                takenPeices = null;
                return valid;
            }
            valid &= Board[location[0], location[1]] == BoardStates.empty;
            if (playerTurn) valid &= (WhosTurn == player);
            takenPeices = TakesPieces(player, location);
            valid &= takenPeices.Count() > 0;

            return valid;
        }

        public bool ValidMove(BoardStates player, byte[] location, bool playerTurn = false)
        {
            bool valid = true;
            valid &= !GameComplete;
            valid &= OnBoard(location);
            if (!valid)
            {
                return valid;
            }
            valid &= Board[location[0], location[1]] == BoardStates.empty;
            if (playerTurn) valid &= WhosTurn == player;
            List<byte[]> takenPeices = TakesPieces(player, location);
            valid &= takenPeices.Count() > 0;

            return valid;
        }


        //*************************Static Methods**************************

        public static BoardStates OpposingPlayer(BoardStates player)
        {
            if (player == BoardStates.empty) return BoardStates.empty;
            return ~player;
        }

        public static BoardStates GetCurrentLeader(OthelloGame game)
        {
            BoardStates leader = BoardStates.empty;
            leader = game.GetPieceCount(BoardStates.white) > game.GetPieceCount(BoardStates.black)
                ? BoardStates.white : BoardStates.black;
            return leader;
        }

        //**************************Get Methods****************************  
      
        public BoardStates[,] GetBoardAtMove(int move)
        {
            try
            {
                return BoardHistory[move];
            }
            catch (Exception)
            {
                System.Console.WriteLine("Can't retreive board history");
                return new BoardStates[8, 8];
            }
        }

        public BoardStates[,] GetBoard() { return (BoardStates[,])Board.Clone(); }

        public int GetMovesMade()
        {
            int playCount = -4; // Board Starts with 4 peices in play
            foreach (BoardStates piece in Board)
            {
                if (piece != BoardStates.empty) playCount++;
            }
            return playCount;
        }

        public int GetMaxMovesLeft()
        ///Returns max moves left if all pieces are played
        {
            return (MaxMoves - GetMovesMade());
        }

        public bool PlayerHasMove(BoardStates player)
        {
            bool hasMove = false;
            for (byte i = 0; i < BOARD_SIZE; i++)
            {
                for (byte j = 0; j < BOARD_SIZE; j++)
                {
                    if (ValidMove(player, new byte[] { i, j }))
                    {
                        hasMove = true;
                        return hasMove;
                    }
                }
            }
            return hasMove;
        }

        public int GetPieceCount(BoardStates bstate)
        {
            int count = 0;
            foreach (BoardStates piece in Board)
            {
                if (piece == bstate) count++;
            }
            return count;
        }

        //**************************Private Methods****************************

        protected bool[,] GetStateArray(BoardStates bstate)
        ///
        ///Returns bool array of bstate peices the size of the board,
        ///With 1 meaning a bstate peice is there, 0 meaning it is not
        ///
        {
            bool[,] bstates = new bool[BOARD_SIZE, BOARD_SIZE];
            for (int i = 0; i < BOARD_SIZE; i++)
            {
                for (int j = 0; j < BOARD_SIZE; j++)
                {
                    bstates[i, j] = (Board[i, j] == bstate) ? true : false;
                }
            }
            return bstates;
        }

        protected bool[] GetStateList(BoardStates bstate)
        ///
        ///
        ///
        {
            bool[] bstates = new bool[BOARD_SIZE * BOARD_SIZE];
            for (int i = 0; i < BOARD_SIZE; i++)
            {
                for (int j = 0; j < BOARD_SIZE; j++)
                {
                    bstates[i + j] = (Board[i, j] == bstate) ? true : false;
                }
            }
            return bstates;
        }

        protected bool OnBoard(byte[] location)
        {
            return (location[0] >= 0 && location[0] < BOARD_SIZE && location[1] >= 0 && location[1] < BOARD_SIZE);
        }

        protected bool OnBoard(sbyte[] location)
        {
            return (location[0] >= 0 && location[0] < BOARD_SIZE && location[1] >= 0 && location[1] < BOARD_SIZE);
        }

        protected static BoardStates[,] GetBoardCopy(BoardStates[,] board)
        {
            BoardStates[,] boardCopy = new BoardStates[BOARD_SIZE, BOARD_SIZE];
            for (int i = 0; i < BOARD_SIZE; i++)
            {
                for (int j = 0; j < BOARD_SIZE; j++)
                {
                    boardCopy[i, j] = board[i, j];
                }
            }
            return boardCopy;
        }

        protected static List<BoardStates[,]> GetBoardHistoryCopy(List<BoardStates[,]> boardHistory)
        {
            List<BoardStates[,]> historyCopy = new List<BoardStates[,]>();
            foreach (BoardStates[,] board in boardHistory)
            {
                historyCopy.Add(GetBoardCopy(board));
            }
            return historyCopy;
        }

        protected void InitializeEndOfGameAttributes()
        {
            byte wtally = 0;
            byte btally = 0;
            for (int i = 0; i < BOARD_SIZE; i++)
            {
                for (int j = 0; j < BOARD_SIZE; j++)
                {
                    if (Board[i, j] == BoardStates.white)
                    {
                        wtally++;
                    }
                    else if (Board[i, j] == BoardStates.black)
                    {
                        btally++;
                    }
                }
            }
            FinalWhiteTally = wtally;
            FinalBlackTally = btally;
            GameComplete = true;

            if (wtally > btally)
            {
                FinalWinner = BoardStates.white;
            }
            else if (btally > wtally)
            {
                FinalWinner = BoardStates.black;
            }
            else
            {
                FinalWinner = BoardStates.empty;//tie
            }
        }

        private List<byte[]> TakesPieces(BoardStates player, byte[] location)
        {

            List<byte[]> taken = new List<byte[]>();//array of all pieces to be flipped

            byte minX = location[0] > 0 ? (byte)(location[0] - 1) : (byte)0;
            byte minY = location[1] > 0 ? (byte)(location[1] - 1) : (byte)0;
            byte maxX = location[0] + 2 < BOARD_SIZE ? (byte)(location[0] + 1) : (byte)(BOARD_SIZE - 1);
            byte maxY = location[1] + 2 < BOARD_SIZE ? (byte)(location[1] + 1) : (byte)(BOARD_SIZE - 1);

            for (byte x = minX; x <= maxX; x++)
            {
                for (byte y = minY; y <= maxY; y++)
                {
                    // int subtakenCount = 0;
                    BoardStates opposingPlayer = ~player; //store to save some computing time

                    if (Board[x, y] == opposingPlayer && (x != location[0] || y != location[1]))
                    {
                        sbyte[] direction = new sbyte[] { (sbyte)(x - location[0]), (sbyte)(y - location[1]) };

                        byte[] searchedLocation = new byte[] { x, y };
                        List<byte[]> subTaken = new List<byte[]>();

                        while (OnBoard(searchedLocation) && Board[searchedLocation[0], searchedLocation[1]] == opposingPlayer)
                        {

                            subTaken.Add(searchedLocation);
                            searchedLocation = new byte[] { (byte)(searchedLocation[0] + direction[0]),
                                (byte)(searchedLocation[1] + direction[1]) };
                        }

                        if (OnBoard(searchedLocation) && Board[searchedLocation[0], searchedLocation[1]] == player)
                        {
                            taken = taken.Concat(subTaken).ToList();
                        }
                    }
                }
            }
            return taken;
        }

        private void SetupBoard()
        {
            BoardHistory = new List<BoardStates[,]>();//60 moves + initial
            Board = new BoardStates[BOARD_SIZE, BOARD_SIZE];
            for (int i = 0; i < BOARD_SIZE; i++)
            {
                for (int j = 0; j < BOARD_SIZE; j++)
                {
                    Board[i, j] = BoardStates.empty;
                }
            }
            Board[BOARD_SIZE / 2 - 1, BOARD_SIZE / 2 - 1] = BoardStates.white;
            Board[BOARD_SIZE / 2 - 1, BOARD_SIZE / 2] = BoardStates.black;
            Board[BOARD_SIZE / 2, BOARD_SIZE / 2 - 1] = BoardStates.black;
            Board[BOARD_SIZE / 2, BOARD_SIZE / 2] = BoardStates.white;

            WhosTurn = BoardStates.black;
            FinalWinner = BoardStates.empty;
            GameComplete = false;
            BoardHistory.Add(GetBoardCopy(this.Board));
        }
    }
}
