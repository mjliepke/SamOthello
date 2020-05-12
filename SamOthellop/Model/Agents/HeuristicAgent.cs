using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamOthellop.Model.Agents
{
    /// <summary>
    /// Evaluates board based on user-defined features
    /// </summary>
    class HeuristicAgent : IEvaluationAgent
    {
        /// <summary>
        /// Calculated Eval using the following funct : value = (diffSlope * diff)
        /// Where weights is divided in the following format:
        /// <para _weights>{diffSlope1EarlyGame, diffSlope1MidGame, diffSlope1EndGame, ...</para>
        /// </summary>

        private double[] _weights;
        public readonly int independentHeuristics = 7;


        public HeuristicAgent()
        {
            try
            {
                string[] strWeights = System.IO.File.ReadAllLines("bestchromosome.dat");
                _weights = strWeights.Select((gene) => double.Parse(gene)).ToArray();
  
            }
            catch (Exception)
            {
                //ok performing values
                _weights = new double[] { 3, 75, -5, 0, 10, 100,0, 3, 75, -5, 0, 10, 100,0, 3, 75, -5, 0, 10, 100,0 };
            }
        }
        public HeuristicAgent(double[] weights)
        {
            _weights = weights;
        }

        public override byte[] MakeMove(OthelloGame game, BoardStates player)
        {
            byte[] bestMove = new byte[] { byte.MaxValue, byte.MaxValue };
            List<byte[]> moves = game.GetPossiblePlayList();

            double bestScore = int.MinValue + 1;
            if (game.GetPieceCount(BoardStates.empty) > 58)//first two moves, don't compute
            {
                return OpeningMove(player, game);
            }
            else if (moves.Count == 1) //don't compute if there is only 1 move
            {
                return moves[0];
            }

            foreach (byte[] move in moves)
            {
                OthelloGame testGame = game.DeepCopy();
                testGame.MakeMove(move);
                double thisScore = EvaluateBoard(testGame, player);
                if (thisScore > bestScore)
                {
                    bestScore = thisScore;
                    bestMove = move;
                }
            }
            if ((bestMove[0] == byte.MaxValue || bestMove[1] == byte.MaxValue) && moves.Count > 0)
            {//All moves are valued at -inf, return one of em
                return moves[0];
            }
            return bestMove;
        }
        /// <summary>
        /// returns weights for Heuristic evaluation function
        /// </summary>
        /// <returns></returns>
      
        public double[] GetWeights()
        {
            return _weights;
        }

        public override double EvaluateBoard(OthelloGame game, BoardStates player)
        {
            ///Based of features of the board that humans have identified.  
            ///Hints of evaluation from any source I could find


            double value = 0;
            int movesToGo = game.GetPieceCount(BoardStates.empty);
            int endGame = 20;
            int midGame = 40;
            double[] applicableWeights = new double[independentHeuristics];

            if (game.GameComplete)
            {
                return CompleteEval(player, game);
            }if(movesToGo < endGame)
            {
                Array.Copy(_weights, independentHeuristics * 2, applicableWeights, 0, independentHeuristics);
            }else if(movesToGo < midGame)
            {
                Array.Copy(_weights, independentHeuristics, applicableWeights, 0, independentHeuristics);
            }
            else //Early Game
            {
                Array.Copy(_weights, 0, applicableWeights, 0, independentHeuristics);
            }
           
            value += applicableWeights[0] * (game.GetPieceCount(player) - game.GetPieceCount(~player)) ;
            value += applicableWeights[1] * (game.GetCornerCount(player) - game.GetCornerCount(~player));
            value += applicableWeights[2] * (game.GetAdjCornerCount(player) - game.GetAdjCornerCount(~player)) ;
            value += applicableWeights[3] * (game.GetPossiblePlayList(player).Count() - game.GetPossiblePlayList(~player).Count()) ;
            value += applicableWeights[4] * (game.GetSafePeiceCountEstimation(player) - game.GetSafePeiceCountEstimation(~player)) ;
            value += applicableWeights[5] * (game.GetControlledCorners(player) - game.GetControlledCorners(~player));
            value += applicableWeights[6] * (game.GetFronteirDisks(player) - game.GetFronteirDisks(~player));
            return value;
        }


        private static int CompleteEval(BoardStates player, OthelloGame game)
        {
            ///Returns complete worth of board, -inf for loss, +inf for win
            if (game.FinalWinner == player)
            {
                return int.MaxValue;
            }
            else return int.MinValue;
        }

        private static byte[] OpeningMove(BoardStates player, OthelloGame game)
        {//avoid computation for first move - only one symmetric option
         //randomly select perpendicular or diagonal for second move - parallel 
         //has been shown to be much worse
         //SPECIFIC TO 8x8 BOARDS
            byte[][] firstMoves = new byte[4][] {
                                        new byte[]{ 2, 3 },
                                        new byte[]{ 3, 2 },
                                        new byte[]{ 4, 5 },
                                        new byte[]{ 5, 4 }};

            if (game.GetPieceCount(BoardStates.empty) == 60)
            {
                Random rndGen = new Random();
                int rand = (int)Math.Ceiling(rndGen.NextDouble() * 4);
                switch (rand)
                {
                    case 1:
                        return firstMoves[0];
                    case 2:
                        return firstMoves[1];
                    case 3:
                        return firstMoves[2];
                    case 4:
                        return firstMoves[3];
                    default:
                        throw new Exception("OpeningMove has faulted with random number generation");
                }
            }
            if (game.GetPieceCount(BoardStates.empty) == 59)
            {
                List<byte[]> moves = game.GetPossiblePlayList();
                Random rndGen = new Random();
                byte rand = (byte)Math.Ceiling(rndGen.NextDouble() * 2);
                switch (rand)
                {
                    case 1: //diagonal
                        return moves[0];
                    case 2: //perpendicular
                        return moves[0];
                    default:
                        throw new Exception("Opening move has faulted with random number generation");
                }
            }
            return new byte[] { byte.MaxValue, byte.MaxValue };
        }


    }
}
