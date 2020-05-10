using GeneticSharp.Domain;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Infrastructure.Framework.Threading;
using SamOthellop.Model.Agents;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SamOthellop.Model.Genetic
{
    static class Evolution
    {
        /// <summary>
        /// Run GA against itself till evolution <paramref name="recursionDepth"/> times
        /// </summary>
        /// <param name="recursionDepth"></param>
        public static void RunRecursiveGeneticAlgorithm(int recursionDepth = 5)
        {
            double[] priorGenes = LoadChromosome();

            FloatingPointChromosome chromosome = new FloatingPointChromosome(
                                                                            new double[] { -100, -100, -100, -100, -100, -100, 0, 0, 0, 0, 0, 0, -10, -10, -10, -10, -10, -10 },
                                                                            new double[] { 100, 100, 100, 100, 100, 100, 255, 255, 255, 255, 255, 255, 10, 10, 10, 10, 10, 10 },
                                                                            new int[] { 64, 64, 64, 64, 64, 64, 8, 8, 8, 8, 8, 8, 64, 64, 64, 64, 64, 64 },
                                                                            new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                                                            priorGenes
                                                                            );

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < recursionDepth; i++)
            {
                if (i == 0)
                {
                    IOthelloAgent randAgent = new RandomAgent();
                    EvolveGeneticAlgorithm(chromosome, randAgent);
                }
                else
                {
                    IOthelloAgent heurAgent = new HeuristicAgent(LoadChromosome());
                    EvolveGeneticAlgorithm(chromosome, heurAgent);
                }
            }
            stopwatch.Stop();
            Console.WriteLine("Sucessfully recursively geneticially trained.  Training cost {0} time with {1} recursions", stopwatch.Elapsed, recursionDepth);

        }

        public static FloatingPointChromosome EvolveGeneticAlgorithm(FloatingPointChromosome chromosome, IOthelloAgent agent)
        {
            IPopulation population = new TplPopulation(30, 60, chromosome);
            IFitness fitness = new EvaluationFitness(agent);
            ISelection selection = new RouletteWheelSelection(); //Guess
            ICrossover crossover = new UniformCrossover(); //Guess
            IMutation mutation = new UniformMutation(); //Guess


            ITermination stagnation = new FitnessStagnationTermination(100);
            ITermination threshold = new FitnessThresholdTermination(.9);

            ITaskExecutor taskExecutor = new ParallelTaskExecutor()
            {
                MaxThreads = Environment.ProcessorCount,
                MinThreads = Environment.ProcessorCount / 2
            };


            GeneticAlgorithm algorithm = new GeneticAlgorithm(population, fitness, selection, crossover, mutation)
            {
                TaskExecutor = new TplTaskExecutor(),
                MutationProbability = .2f
            };
            algorithm.TaskExecutor = taskExecutor;
            algorithm.Termination = threshold;

            algorithm.Start();

            SaveChromosome((FloatingPointChromosome)algorithm.BestChromosome);
            
            Debug.WriteLine("finished Training, with {0} time spent on evolving", algorithm.TimeEvolving);

            return (FloatingPointChromosome)algorithm.BestChromosome;
        }

        private static void SaveChromosome(FloatingPointChromosome chromosome)
        {
            string path = @"E:\Source\SamOthellop\SamOthellop\Model\Agents\Genetic\bestchromosome.dat";
            //string path = String.Format(@"{0}\bestchromosome.dat", Application.StartupPath);
            SaveChromosome(chromosome, path);
        }

        private static void SaveChromosome(FloatingPointChromosome chromosome, string path)
        {
            string[] genes = chromosome.ToFloatingPoints().Select((gene) => gene.ToString()).ToArray();
            System.IO.File.WriteAllLines(path, genes);
        }

        private static double[] LoadChromosome(string path = @"E:\Source\SamOthellop\SamOthellop\Model\Agents\Genetic\bestchromosome.dat")
        {
            string[] genes = System.IO.File.ReadAllLines(path);
            return genes.Select((gene) => double.Parse(gene)).ToArray();
        }

    }
}
