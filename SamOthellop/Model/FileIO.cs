using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using IntelHexFormatReader;
using IntelHexFormatReader.Model;
using System.Collections.Generic;

namespace SamOthellop.Model
{
    static class FileIO
    {
        const int FILE_HEADER_LENGTH = 16; //bytes
        const int GAME_HEADER_LENGTH = 8;
        const int NUMBER_OF_PLAYS = 60;
        const int GAME_BLOCK_LENGTH = GAME_HEADER_LENGTH + NUMBER_OF_PLAYS;

        public static List<ThorGame> ReadThorFile(string path = @"C:\Users\mjlie\source\repos\SamOthellop\SamOthellop\Database\WTH_2001.wtb")
        {
            /*
             * Thor file capabilities from ledpup's Othello work
             * https://github.com/ledpup/Othello
             * 
             * Returns a list of game 
             */
            List<ThorGame> gameDatabase = new List<ThorGame>();
            //FileInfo fi = new FileInfo(path);

            if (File.Exists(path))
            {
                byte[] thorArray = File.ReadAllBytes(path);

                if (thorArray[12] != 8)
                    throw new Exception("Thor processor only supports 8x8 boards");

                for (var i = FILE_HEADER_LENGTH; i < thorArray.Length; i += GAME_BLOCK_LENGTH)
                {
                    ThorGame game = new ThorGame
                    {
                        TournamentId = BitConverter.ToInt16(new[] { thorArray[i], thorArray[i + 1] }, 0),
                        BlackId = BitConverter.ToInt16(new[] { thorArray[i + 2], thorArray[i + 3] }, 0),
                        WhiteId = BitConverter.ToInt16(new[] { thorArray[i + 4], thorArray[i + 5] }, 0),
                        BlackScore = thorArray[i + 6],
                        TheoreticalScore = thorArray[i + 7],
                    };

                    for (var playIndex = 0; playIndex < NUMBER_OF_PLAYS; playIndex++)
                    {
                        Byte play = thorArray[i + GAME_HEADER_LENGTH + playIndex];

                        if (play > 0)
                        {
                            game.Plays.Add(ToAlgebraicNotation(play));
                        }
                    }
                    gameDatabase.Add(game);

                }

                //foreach(var game in gameDatabase)
                //{
                //    System.Diagnostics.Debug.WriteLine(game.SerialisedPlays);
                //}
            }
            return gameDatabase;
        }
        public static string ToAlgebraicNotation(this byte play)
        {
            var chars = play.ToString().ToCharArray();
            return ((char)(chars[0] + 48)).ToString() + chars[1];
        }

    }
}
