using NumSharp.Extensions;
using SamOthellop.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SamOthellop
{

    public enum BoardStates : Byte
    {
        white,
        black,
        empty
    }
    //[Serializable]
    class OthelloGame
    {
        public const byte BOARD_SIZE = 8;
        public byte MaxMoves { get; private set; }
        public byte FinalBlackTally { get; private set; }
        public byte FinalWhiteTally { get; private set; }
        public BoardStates FinalWinner { get; private set; }
        public BoardStates WhosTurn { get; private set; }
        BoardStates[,] Board;
        List<BoardStates[,]> BoardHistory;

        public OthelloGame(byte boardsize = 8)
        {
            SetupGame();
            MaxMoves = Convert.ToByte((boardsize * boardsize) - 4);//4 preoccupied spaces
        }

        public OthelloGame(ThorGame game)
        {
            SetupGame();
            MaxMoves = 60;//4 preoccupied spaces


            foreach (string play in game.Plays)
            {
                char colchar = play.ElementAt(0);
                char rowchar = play.ElementAt(1);
                byte col = (byte)(Byte.Parse((colchar - 'a' + 1).ToString()) - 1);//substract one to start @ index of 0
                byte row = (byte)(Byte.Parse(rowchar.ToString()) - 1);
                if (!MakeMove(new byte[] { row, col }))
                {
                    MakeMove(new byte[] { row, col });
                    throw new Exception("An invalid move was made while trying to recreate ThorGame");
                }
            }
        }

        public OthelloGame ShallowCopy()
        {
            return (OthelloGame)this.MemberwiseClone();
        }

        public OthelloGame DeepCopy()
        {
            //return ObjectCopier.Clone<OthelloGame>(this);
            OthelloGame game = (OthelloGame)this.MemberwiseClone();
            game.Board = GetBoardCopy(this.Board);
            game.BoardHistory = GetBoardHistoryCopy(this.BoardHistory);
            return game;
        }

        public BoardStates OpposingPlayer(BoardStates Player)
        {
            if (Player == BoardStates.empty) { return BoardStates.empty; }
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

            for (byte i = 0; i < BOARD_SIZE; i++)
            {
                for (byte j = 0; j < BOARD_SIZE; j++)
                {
                    moveExists |= ValidMove(BoardStates.black, new byte[] { i, j });
                    moveExists |= ValidMove(BoardStates.white, new byte[] { i, j });
                    if (moveExists)
                    {
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

        public void ResetBoard()
        {
            SetupGame();
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

                if (PlayerHasMove(OpposingPlayer(player)))
                {
                    WhosTurn = OpposingPlayer(player);//only switch whos turn it is if other player has moves avalible
                }

                BoardHistory.Add(GetBoardCopy(Board));
                moveMade = true;
            }
            else
            {
                int debugLocation = 0;//to stop debugger on invalid move
            }
            return moveMade;
        }

        public bool MakeMove(byte[] location)
        {
            BoardStates player = WhosTurn;
            return MakeMove(player, location);
        }

        public void MakeRandomMove(BoardStates player)
        {
            byte[,] possibleMoves = new byte[MaxMoves - MovesMade(), 2];
            int possibleMoveCount = 0;
            for (byte i = 0; i < BOARD_SIZE; i++)
            {
                for (byte j = 0; j < BOARD_SIZE; j++)
                {
                    List<byte[]> takenPeices;
                    if (ValidMove(player, new byte[] { i, j }, out takenPeices))
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

            MakeMove(player, new byte[] { possibleMoves[rnd, 0], possibleMoves[rnd, 1] });
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

        public bool ValidMove(BoardStates player, byte[] location, out List<byte[]> takenPeices, bool playerTurn = false)
        {
            bool valid = true;
            //valid &= !GameOver();
            valid &= OnBoard(location);
            if (!valid)
            {
                takenPeices = null;
                return valid;
            }
            valid &= Board[location[0], location[1]].Equals(BoardStates.empty);
            if (playerTurn) valid &= WhosTurn.Equals(player);
            takenPeices = TakesPieces(player, location);
            valid &= takenPeices.Count() > 0;

            return valid;
        }

        public bool ValidMove(BoardStates player, byte[] location, bool playerTurn = false)
        {
            bool valid = true;
            //valid &= !GameOver();
            valid &= OnBoard(location);
            if (!valid)
            {
                return valid;
            }
            valid &= Board[location[0], location[1]].Equals(BoardStates.empty);
            if (playerTurn) valid &= WhosTurn.Equals(player);
            List<byte[]> takenPeices = TakesPieces(player, location);
            valid &= takenPeices.Count() > 0;

            return valid;
        }

        //*************************Static Methods**************************

        public static OthelloGame GetReflectedAcrossA8H1(OthelloGame game)
        {
            OthelloGame newGame = game.DeepCopy();
            for (int i = 0; i <= game.MovesMade(); i++)
            {
                for (int j = 0; j < BOARD_SIZE; j++)
                {
                    for (int k = 0; k < BOARD_SIZE; k++)
                    {
                        if (game.BoardHistory[i] != null)
                        {
                            newGame.BoardHistory[i][j, k] = game.BoardHistory[i][BOARD_SIZE - k - 1, BOARD_SIZE - j - 1];
                        }
                    }
                }
            }

            for (int i = 0; i < BOARD_SIZE; i++)
            {
                for (int j = 0; j < BOARD_SIZE; j++)
                {
                    newGame.Board[i, j] = game.Board[BOARD_SIZE - j - 1, BOARD_SIZE - i - 1];
                }
            }
            return newGame;
        }

        public static OthelloGame GetReflectedAcrossA1H8(OthelloGame game)
        {
            OthelloGame newGame = game.DeepCopy();
            for (int i = 0; i <= game.MovesMade(); i++)
            {
                for (int j = 0; j < BOARD_SIZE; j++)
                {
                    for (int k = 0; k < BOARD_SIZE; k++)
                    {
                        if (game.BoardHistory[i] != null)
                        {
                            newGame.BoardHistory[i][j, k] = game.BoardHistory[i][k, j];
                        }
                    }
                }
            }

            for (int i = 0; i < BOARD_SIZE; i++)
            {
                for (int j = 0; j < BOARD_SIZE; j++)
                {
                    newGame.Board[i, j] = game.Board[j, i];
                }
            }
            return newGame;
        }

        public static OthelloGame GetPiRotation(OthelloGame game)
        {
            OthelloGame newGame = GetHalfPiRotation(GetHalfPiRotation(game));
            return game;
        }

        public static OthelloGame GetHalfPiRotation(OthelloGame game)
        {
            OthelloGame newGame = game.DeepCopy();
            for (int i = 0; i <= game.MovesMade(); i++)
            {
                for (int j = 0; j < BOARD_SIZE; j++)
                {
                    for (int k = 0; k < BOARD_SIZE; k++)
                    {
                        if (game.BoardHistory[i] != null)
                        {
                            newGame.BoardHistory[i][j, k] = game.BoardHistory[i][BOARD_SIZE - k - 1, j];
                        }
                    }
                }
            }

            for (int i = 0; i < BOARD_SIZE; i++)
            {
                for (int j = 0; j < BOARD_SIZE; j++)
                {
                    newGame.Board[i, j] = game.Board[BOARD_SIZE - j - 1, i];
                }
            }
            return newGame;
        }

        public static OthelloGame GetInverseGame(OthelloGame game)
        {
            OthelloGame newGame = game.DeepCopy();
            for (int i = 0; i <= game.MovesMade(); i++)
            {
                for (int j = 0; j < BOARD_SIZE; j++)
                {
                    for (int k = 0; k < BOARD_SIZE; k++)
                    {
                        if (game.BoardHistory[i] != null)
                        {
                            newGame.BoardHistory[i][j, k] = game.OpposingPlayer(game.BoardHistory[i][j, k]);
                        }
                    }
                }
            }

            for (int i = 0; i < BOARD_SIZE; i++)
            {
                for (int j = 0; j < BOARD_SIZE; j++)
                {
                    newGame.Board[i, j] = game.OpposingPlayer(game.Board[i, j]);
                }
            }
            return newGame;
        }

        public static List<OthelloGame> GetAllGameRotations(OthelloGame game)
        {
            List<OthelloGame> gameList = new List<OthelloGame>();

            gameList.Add(game);
            gameList.Add(GetReflectedAcrossA1H8(game));
            gameList.Add(GetReflectedAcrossA8H1(game));
            gameList.Add(GetPiRotation(game));

            OthelloGame invGame = GetHalfPiRotation(GetInverseGame(game));
            gameList.Add(invGame);
            gameList.Add(GetReflectedAcrossA1H8(game));
            gameList.Add(GetReflectedAcrossA8H1(game));
            gameList.Add(GetPiRotation(game));

            return gameList;
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

        public bool[,] GetPlayableStateArray() { return GetPlayableStateArray(WhosTurn); }

        public bool[,] GetPlayableStateArray(BoardStates player)
        {
            bool[,] bstates = new bool[BOARD_SIZE, BOARD_SIZE];
            for (byte i = 0; i < BOARD_SIZE; i++)
            {
                for (byte j = 0; j < BOARD_SIZE; j++)
                {
                    bstates[i, j] = ValidMove(player, new byte[] { i, j }) ? true : false;
                }
            }
            return bstates;
        }

        public bool[] GetBoardStateList()
        {
            bool[] bstate = new bool[2 * BOARD_SIZE * BOARD_SIZE];
            Array.Copy(GetStateList(BoardStates.white), bstate, BOARD_SIZE);
            Array.Copy(GetStateList(BoardStates.black), 0, bstate, BOARD_SIZE, BOARD_SIZE);
            return bstate;
        }

        public bool[] GetPlayableStateList() { return GetPlayableStateList(WhosTurn); }

        public bool[] GetPlayableStateList(BoardStates player)
        {
            bool[] bstates = new bool[BOARD_SIZE * BOARD_SIZE];
            for (byte i = 0; i < BOARD_SIZE; i++)
            {
                for (byte j = 0; j < BOARD_SIZE; j++)
                {
                    bstates[i + j] = ValidMove(player, new byte[] { i, j }) ? true : false;
                }
            }
            return bstates;
        }

        public List<byte[]> GetPossiblePlayList()
        {
            List<byte[]> possiblePlays = new List<byte[]>();
            for (byte i = 0; i < BOARD_SIZE; i++)
            {
                for (byte j = 0; j < BOARD_SIZE; j++)
                {
                    if (ValidMove(WhosTurn, new byte[] { i, j }))
                    {
                        possiblePlays.Add(new byte[] { i, j });
                    }
                }
            }
            return possiblePlays;
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
                if (piece.Equals(bstate)) count++;
            }
            return count;
        }

        //**************************Private Methods****************************

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

                    if ((x != location[0] || y != location[1]) && Board[x, y].Equals(OpposingPlayer(player)))
                    {
                        sbyte[] direction = new sbyte[] { (sbyte)(x - location[0]), (sbyte)(y - location[1]) };

                        byte[] searchedLocation = new byte[] { x, y };
                        List<byte[]> subTaken = new List<byte[]>();

                        while (OnBoard(searchedLocation) && Board[searchedLocation[0], searchedLocation[1]].Equals(OpposingPlayer(player)))
                        {

                            subTaken.Add(searchedLocation);
                            searchedLocation = new byte[] { (byte)(searchedLocation[0] + direction[0]),
                                (byte)(searchedLocation[1] + direction[1]) };
                        }

                        if (OnBoard(searchedLocation) && Board[searchedLocation[0], searchedLocation[1]].Equals(player))
                        {
                           taken = taken.Concat(subTaken).ToList();
                        }
                    }
                }
            }
            return taken;
        }

        private bool[,] GetStateArray(BoardStates bstate)
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
            bool[] bstates = new bool[BOARD_SIZE * BOARD_SIZE];
            for (int i = 0; i < BOARD_SIZE; i++)
            {
                for (int j = 0; j < BOARD_SIZE; j++)
                {
                    bstates[i + j] = Board[i, j].Equals(bstate) ? true : false;
                }
            }
            return bstates;
        }

        private void SetupGame()
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
            BoardHistory.Add(GetBoardCopy(this.Board));
        }

        private bool OnBoard(byte[] location)
        {
            return (location[0] >= 0 && location[0] < BOARD_SIZE && location[1] >= 0 && location[1] < BOARD_SIZE);
        }

        private static BoardStates[,] GetBoardCopy(BoardStates[,] board)
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

        private static List<BoardStates[,]> GetBoardHistoryCopy(List<BoardStates[,]> boardHistory)
        {
            List<BoardStates[,]> historyCopy = new List<BoardStates[,]>();
            foreach (BoardStates[,] board in boardHistory)
            {
                historyCopy.Add(GetBoardCopy(board));
            }
            return historyCopy;
        }

        private void InitializeEndOfGameAttributes()
        {
            byte wtally = 0;
            byte btally = 0;
            for (int i = 0; i < BOARD_SIZE; i++)
            {
                for (int j = 0; j < BOARD_SIZE; j++)
                {
                    if (Board[i, j].Equals(BoardStates.white))
                    {
                        wtally++;
                    }
                    else if (Board[i, j].Equals(BoardStates.black))
                    {
                        btally++;
                    }
                }
            }
            FinalWhiteTally = wtally;
            FinalBlackTally = btally;

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

    }
}

