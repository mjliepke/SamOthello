using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamOthellop.Model.Genetic
{
    public class EvaluationFitness : IFitness
    {
        int TEST_COUNT;
        public EvaluationFitness(int testCount = 50) : base()
        {
            TEST_COUNT = testCount;
        }

        public double Evaluate(IChromosome chromosome)
        {//Play n games vs a random (to be Neural Net), % win is Fitness


            FloatingPointChromosome myChromosome = (FloatingPointChromosome)chromosome;
            double[] genes = myChromosome.ToFloatingPoints();

            double fitness = 0;
            double wonCount = 0;
            object wonCountLock = new object();

            for (int index = 0; index < TEST_COUNT; index++)
            {
                //Parallel.For(0, TEST_COUNT, new ParallelOptions() { MaxDegreeOfParallelism = 2},
                //   (index) => {
                BoardStates player =  (index % 2 == 0) ? BoardStates.black : BoardStates.white;

                OthelloGame othelloGame = new OthelloGame();
                MinMaxAgent minMaxAgent = new MinMaxAgent(genes, 1);

                while (!othelloGame.GameComplete)
                {
                    if (othelloGame.WhosTurn == player)
                    {
                        othelloGame.MakeMove(minMaxAgent.MakeMove(othelloGame, player));
                    }
                    else
                    {
                        othelloGame.MakeRandomMove(~player);
                    }
                }
                if (othelloGame.GameComplete)//just gotta check
                {
                    if (othelloGame.FinalWinner == player)
                    {
                        lock (wonCountLock)
                        {
                            wonCount++;
                        }
                    }
                    else if (othelloGame.FinalWinner == BoardStates.empty)
                    {
                        lock (wonCountLock)
                        {
                            wonCount += .5;
                        }
                    }
                }
                else
                {
                    throw new Exception("EvaluationFitness didn't complete a game");
                }
                // });
            }
            fitness = (double)wonCount / TEST_COUNT;
            System.Diagnostics.Debug.WriteLine("Fitness: " + fitness);
            return fitness;
        }

    }
}
