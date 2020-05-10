using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamOthellop.Model
{
    /// <summary>
    /// This entire class was retreived from ledpup's Othello work:
    /// https://github.com/ledpup/Othello
    /// </summary>
    public class ThorGame
    {
        public ThorGame()
        {
            Plays = new List<string>();
        }

        public int TournamentId;
        public int BlackId;
        public int WhiteId;
        public int BlackScore;
        public int TheoreticalScore;
        public List<string> Plays;
        public string SerialisedPlays
        {
            get { return string.Join(",", Plays.ToArray()); }
        }
    }
}
