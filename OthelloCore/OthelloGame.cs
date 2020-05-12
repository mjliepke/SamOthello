using System;
using System.Collections.Generic;
using System.Linq;


namespace SamOthellop.Model
{


    public class OthelloGame : BoardBase
    {

        public OthelloGame(byte boardsize = 8) : base()
        {
            MaxMoves = Convert.ToByte((boardsize * boardsize) - 4);//4 preoccupied spaces
        }

        public OthelloGame(ThorGame game) : base()
        {

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


        //*************************Static Methods**************************

        public static OthelloGame GetReflectedAcrossA8H1(OthelloGame game)
        {
            OthelloGame newGame = game.DeepCopy();
            for (int i = 0; i <= game.GetMovesMade(); i++)
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
            for (int i = 0; i <= game.GetMovesMade(); i++)
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
            for (int i = 0; i <= game.GetMovesMade(); i++)
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
            for (int i = 0; i <= game.GetMovesMade(); i++)
            {
                for (int j = 0; j < BOARD_SIZE; j++)
                {
                    for (int k = 0; k < BOARD_SIZE; k++)
                    {
                        if (game.BoardHistory[i] != null)
                        {
                            newGame.BoardHistory[i][j, k] = OpposingPlayer(game.BoardHistory[i][j, k]);
                        }
                    }
                }
            }

            for (int i = 0; i < BOARD_SIZE; i++)
            {
                for (int j = 0; j < BOARD_SIZE; j++)
                {
                    newGame.Board[i, j] = OpposingPlayer(game.Board[i, j]);
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

        public List<byte[]> GetPossiblePlayList(BoardStates player)
        {
            List<byte[]> possiblePlays = new List<byte[]>();
            for (byte i = 0; i < BOARD_SIZE; i++)
            {
                for (byte j = 0; j < BOARD_SIZE; j++)
                {
                    if (ValidMove(player, new byte[] { i, j }))
                    {
                        possiblePlays.Add(new byte[] { i, j });
                    }
                }
            }
            return possiblePlays;
        }

        public List<byte[]> GetPossiblePlayList()
        {
            return GetPossiblePlayList(WhosTurn);
        }

        //************************Potential Feature Functions******************
        public int GetCornerCount(BoardStates player)
        {
            int count = 0;
            count += Convert.ToInt32(Board[0, 0] == player);
            count += Convert.ToInt32(Board[0, BOARD_SIZE - 1] == player);
            count += Convert.ToInt32(Board[BOARD_SIZE - 1, 0] == player);
            count += Convert.ToInt32(Board[BOARD_SIZE - 1, BOARD_SIZE - 1] == player);
            return count;
        }

        public int GetAdjCornerCount(BoardStates player)
        {
            int count = 0;
            count += Convert.ToInt32(Board[0, 1] == player);
            count += Convert.ToInt32(Board[0, BOARD_SIZE - 2] == player);
            count += Convert.ToInt32(Board[1, 0] == player);
            count += Convert.ToInt32(Board[1, 1] == player);
            count += Convert.ToInt32(Board[1, BOARD_SIZE - 2] == player);
            count += Convert.ToInt32(Board[1, BOARD_SIZE - 1] == player);
            count += Convert.ToInt32(Board[BOARD_SIZE - 2, 0] == player);
            count += Convert.ToInt32(Board[BOARD_SIZE - 2, 1] == player);
            count += Convert.ToInt32(Board[BOARD_SIZE - 2, BOARD_SIZE - 2] == player);
            count += Convert.ToInt32(Board[BOARD_SIZE - 2, BOARD_SIZE - 1] == player);
            count += Convert.ToInt32(Board[BOARD_SIZE - 1, 1] == player);
            count += Convert.ToInt32(Board[BOARD_SIZE - 1, BOARD_SIZE - 2] == player);
            return count;
        }

        public int GetSafePeiceCountEstimation(BoardStates player)
        {
            //return number of player's peices that are impossible to be taken
            //calculated based on geometry of a safe peice having a connection to the edge 
            //in 4 unique directions with other like peices
            int safeCount = 0;
            sbyte[][] dirVec = new sbyte[4][] { new sbyte[] { -1, 1 },
                                            new sbyte[] { -1, -1 },
                                            new sbyte[] { 1, 0 },
                                            new sbyte[] { 0, 1 } };

            for (sbyte i = 0; i < BOARD_SIZE; i++)
            {
                for (sbyte j = 0; j < BOARD_SIZE; j++)
                {
                    if (Board[i, j] != player) continue;
                    int safeDirCount = 0;
                    foreach (sbyte[] dir in dirVec)
                    {
                        sbyte[] location = new sbyte[] { i, j };
                        do
                        {
                            location[0] += dir[0];
                            location[1] += dir[1];

                        } while (OnBoard(location) && Board[location[0], location[1]] == player);

                        if (!OnBoard(location))
                        {
                            safeDirCount++;
                        }
                        else
                        {
                            BoardStates surrounding = Board[location[0], location[1]];
                            bool goodPartialSurrounding = true;
                            do
                            {
                                goodPartialSurrounding &= (Board[location[0], location[1]] != BoardStates.empty);
                                location[0] += dir[0];
                                location[1] += dir[1];
                            } while (OnBoard(location) && goodPartialSurrounding);


                            location = new sbyte[] { i, j };
                            do //test neg dir
                            {
                                location[0] -= dir[0];
                                location[1] -= dir[1];
                            } while (OnBoard(location) && Board[location[0], location[1]] == player);

                            if (!OnBoard(location)
                                || (Board[location[0], location[1]] == surrounding
                                    && surrounding == ~player && goodPartialSurrounding))
                            {
                                safeDirCount++;
                            }
                        }

                    }
                    if (safeDirCount >= 4)
                    {
                        safeCount++;
                    }
                }
            }

            return safeCount;
        }

        public int GetControlledCorners(BoardStates player)
        {//If control corner & two adjacent "X" peices
            byte[][][] corners = new byte[4][][] {new byte[3][] { new byte[]{0,0 },
                                                                new byte[]{0,1 },
                                                                new byte[]{1,0 } },
                                                    new byte[3][] { new byte[]{BOARD_SIZE-1,BOARD_SIZE-1 },
                                                                new byte[]{ BOARD_SIZE - 1, BOARD_SIZE-2 },
                                                                new byte[]{ BOARD_SIZE - 2, BOARD_SIZE - 1 } },
                                                    new byte[3][] { new byte[]{0,BOARD_SIZE-1 },
                                                                new byte[]{0,BOARD_SIZE-2 },
                                                                new byte[]{1, BOARD_SIZE - 1 } },
                                                    new byte[3][] { new byte[]{ BOARD_SIZE - 1, 0 },
                                                                new byte[]{ BOARD_SIZE - 2, 0 },
                                                                new byte[]{ BOARD_SIZE - 1 , 1 } } };
            int cornerCount = 0;
            foreach (byte[][] corner in corners)
            {
                bool controlled = true;
                foreach (byte[] location in corner)
                {
                    controlled &= Board[location[0], location[1]] == player;
                }
                if (controlled) cornerCount++;
            }

            return cornerCount;
        }
    
        public int GetOutside16PeiceCount(BoardStates player)
        {//defined as in outer 2 rings
            BoardStates[,] outside16mask = new BoardStates[8, 8] {
                {player, player, player, player, player, player, player, player },
                {player, player, player, player, player, player, player, player },
                {player, player, 0, 0, 0, 0, player, player },
                {player, player, 0, 0, 0, 0, player, player },
                {player, player, 0, 0, 0, 0, player, player },
                {player, player, 0, 0, 0, 0, player, player },
                {player, player, player, player, player, player, player, player },
                {player, player, player, player, player, player, player, player }};
            int count = 0;

            for(byte i=0;i<BOARD_SIZE; i++)
            {
                for(byte j =0;j<BOARD_SIZE; j++)
                {
                    if(Board[i,j] == outside16mask[i, j])
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        public int GetFronteirDisks(BoardStates player)
        {
            //Defined as disks with >4 empty spots next to it
            int count = 0;
            for(byte i =0;i<BOARD_SIZE; i++)
            {
                for(byte j =0;j<BOARD_SIZE; j++)
                {
                    if(AdjEmptyCount(new byte[] { i, j }) > 4)
                    {
                        count++;
                    }
                }
            }
            return count;
        }
       
        private int AdjEmptyCount(byte[] location)
        {
            int count = 0;
            for(sbyte i=(sbyte)(location[0]-1); i<(sbyte)(location[0] + 1); i++)
            {
                for(sbyte j = (sbyte)(location[1] -1); j<(sbyte)(location[1] + 1); j++)
                {
                    if(j<0 || i < 0)
                    { 
                        continue;
                    }
                    else if(Board[i,j] == BoardStates.empty)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

    }
}

