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
    [Serializable]
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

        public const int BOARD_SIZE = 8;
        public int MaxMoves { get; private set; }
        public int FinalBlackTally { get; private set; }
        public int FinalWhiteTally { get; private set; }
        public BoardStates FinalWinner { get; private set; }
        public BoardStates WhosTurn { get; private set; }
        BoardStates[,] Board;
        BoardStates[][,] BoardHistory;

        public OthelloGame(int boardsize = 8)
        {
            SetupGame();
            MaxMoves = (boardsize * boardsize) - 4;//4 preoccupied spaces
        }

        public OthelloGame(ThorGame game)
        {
            SetupGame();
            MaxMoves = 60;//4 preoccupied spaces


            foreach (string play in game.Plays)
            {
                char colchar = play.ElementAt(0);
                char rowchar = play.ElementAt(1);
                int col = Int16.Parse((colchar - 'a' + 1).ToString()) - 1;//substract one to start @ index of 0
                int row = Int16.Parse(rowchar.ToString()) - 1;
                if (!MakeMove(new int[] { row, col }))
                {
                    MakeMove(new int[] { row, col });
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
            return ObjectCopier.Clone<OthelloGame>(this);
            //OthelloGame game = (OthelloGame)this.MemberwiseClone();
            //for (int i = 0; i < this.MovesMade(); i++)
            //{
            //    for (int j = 0; j < BOARD_SIZE; j++)
            //    {
            //        for (int k = 0; k < BOARD_SIZE; k++)
            //        {
            //            try
            //            {
            //                if (this.BoardHistory[i] != null)
            //                {
            //                    game.BoardHistory[i][j, k] = this.BoardHistory[i][j, k];
            //                }
            //            }
            //            catch (NullReferenceException) { }
            //        }
            //    }
            //}
            //for (int i = 0; i < BOARD_SIZE; i++)
            //{
            //    for (int j = 0; j < BOARD_SIZE; j++)
            //    {
            //        game.Board[i, j] = this.Board[i, j];
            //    }
            //}
            //return game;
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
            //for debugging:
            int i, j;
            for (i = 0; i < BOARD_SIZE; i++)
            {
                for (j = 0; j < BOARD_SIZE; j++)
                {
                    moveExists |= ValidMove(BoardStates.black, new int[] { i, j });
                    moveExists |= ValidMove(BoardStates.white, new int[] { i, j });
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

        public bool MakeMove(BoardStates player, int[] location)
        {
            bool moveMade = false;
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

                if (PlayerHasMove(OpposingPlayer(player)))
                {
                    WhosTurn = OpposingPlayer(player);//only switch whos turn it is if other player has moves avalible
                }

                BoardHistory[MovesMade()] = (BoardStates[,])Board.Clone();
                moveMade = true;
            }
            else
            {
                int debugLocation = 0;//to stop debugger on
            }
            return moveMade;
        }

        public bool MakeMove(int[] location)
        {
            BoardStates player = WhosTurn;
            return MakeMove(player, location);
        }

        public void MakeRandomMove(BoardStates player)
        {
            int[,] possibleMoves = new int[MaxMoves - MovesMade(), 2];
            int possibleMoveCount = 0;
            for (int i = 0; i < BOARD_SIZE; i++)
            {
                for (int j = 0; j < BOARD_SIZE; j++)
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
            if (!valid) return valid;
            valid &= Board[location[0], location[1]].Equals(BoardStates.empty);
            if (playerTurn) valid &= WhosTurn.Equals(player);
            int[,] takenPieces = TakesPieces(player, location);
            valid &= takenPieces.Length > 0;

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
            for (int i = 0; i < BOARD_SIZE; i++)
            {
                for (int j = 0; j < BOARD_SIZE; j++)
                {
                    bstates[i, j] = ValidMove(player, new int[] { i, j }) ? true : false;
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
            for (int i = 0; i < BOARD_SIZE; i++)
            {
                for (int j = 0; j < BOARD_SIZE; j++)
                {
                    bstates[i + j] = ValidMove(player, new int[] { i, j }) ? true : false;
                }
            }
            return bstates;
        }

        public List<int[]> GetPossiblePlayList()
        {
            List<int[]> possiblePlays = new List<int[]>();
            for(int i = 0; i < BOARD_SIZE; i++)
            {
                for(int j = 0; j < BOARD_SIZE; j++)
                {
                    if(ValidMove(WhosTurn, new int[] { i, j })){
                        possiblePlays.Add(new int[] { i, j });
                    }
                }
            }
            return possiblePlays;
        }

        public bool PlayerHasMove(BoardStates player)
        {
            bool hasMove = false;
            for (int i = 0; i < BOARD_SIZE; i++)
            {
                for (int j = 0; j < BOARD_SIZE; j++)
                {
                    if (ValidMove(player, new int[] { i, j }))
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

        private int[,] TakesPieces(BoardStates player, int[] location)
        {
            int[,] taken = new int[BOARD_SIZE * BOARD_SIZE, 2];//array of all pieces to be flipped
            int takenCount = 0;

            int minX = location[0] > 0 ? location[0] - 1 : 0;
            int minY = location[1] > 0 ? location[1] - 1 : 0;
            int maxX = location[0] + 2 < BOARD_SIZE ? location[0] + 1 : BOARD_SIZE - 1;
            int maxY = location[1] + 2 < BOARD_SIZE ? location[1] + 1 : BOARD_SIZE - 1;

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    int[,] subtaken = new int[BOARD_SIZE, 2];
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
            BoardHistory = new BoardStates[61][,];//60 moves + initial
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
            BoardHistory[0] = (BoardStates[,])Board.Clone();
        }

        private bool OnBoard(int[] location)
        {
            return (location[0] >= 0 && location[0] < BOARD_SIZE && location[1] >= 0 && location[1] < BOARD_SIZE);
        }

        private void InitializeEndOfGameAttributes()
        {
            int wtally = 0;
            int btally = 0;
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

