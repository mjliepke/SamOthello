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

        private const string FOLDER_PATH = @"E:\Source\SamOthellop\SamOthellop\Model\Agents\Genetic\5-12b\";//path to save 
        private const int GENE_COUNT = 21;
        private const int MIN_GENE_VALUE = -100;
        private const int MAX_GENE_VALUE = 100;

        public static void RunRecursiveGeneticAlgorithm(int recursionDepth = 10)
        {
            //double[] priorGenes = LoadChromosome(@"E:\Source\SamOthellop\SamOthellop\Model\Agents\Genetic\bestchromosome5-10.dat");

            FloatingPointChromosome firstChm = new FloatingPointChromosome(
                                                                            new double[] { -100, -100, -100, -100, -100, -100, -100, -100, -100, -100, -100, -100, -100, -100, -100, -100, -100, -100, -100, -100, -100 },
                                                                            new double[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 },
                                                                            new int[] { 64, 64, 64, 64, 64, 64, 64, 64, 64, 64, 64, 64, 64, 64, 64, 64, 64, 64, 64, 64, 64 },
                                                                            new int[] { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 }
                                                                            );

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < recursionDepth; i++)
            {
                FloatingPointChromosome bestChm = firstChm;
                if (i == 0)
                {
                    List<IOthelloAgent> opposingAgents = new List<IOthelloAgent>(){new RandomAgent(), new GreedyAgent()} ;
                     bestChm = EvolveGeneticAlgorithm(firstChm, opposingAgents, "test" + i.ToString() + ".dat");
                }
                else
                {
                    List<string> previousAgentFiles = Directory.GetFiles(FOLDER_PATH, "*.dat").ToList();
                    List<IOthelloAgent> opposingAgents = previousAgentFiles.Select((file) => new HeuristicAgent(LoadChromosome(file))).Cast<IOthelloAgent>().ToList();
                    opposingAgents.Add(new GreedyAgent());
                    opposingAgents.Add(new RandomAgent());
                    bestChm = EvolveGeneticAlgorithm(bestChm, opposingAgents, "test" + i.ToString() + ".dat");
                    bestChm = new FloatingPointChromosome(new double[] { -100, -100, -100, -100, -100, -100, -100, -100, -100, -100, -100, -100, -100, -100, -100, -100, -100, -100, -100, -100, -100 },
                                                                            new double[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 },
                                                                            new int[] { 64, 64, 64, 64, 64, 64, 64, 64, 64, 64, 64, 64, 64, 64, 64, 64, 64, 64, 64, 64, 64 },
                                                                            new int[] { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 },
                                                                            NormalizeGenes(bestChm.ToFloatingPoints()));
                }
            }
            stopwatch.Stop();
            Console.WriteLine("Sucessfully recursively geneticially trained.  Training cost {0} time with {1} recursions", stopwatch.Elapsed, recursionDepth);

        }

        public static FloatingPointChromosome EvolveGeneticAlgorithm(FloatingPointChromosome chromosome, List<IOthelloAgent> agents, string chromosomeLabel = "")
        {
            IPopulation population = new TplPopulation(100,160, chromosome);
            IFitness fitness = new EvaluationFitness(agents);
            ISelection selection = new RouletteWheelSelection(); //Guess
            ICrossover crossover = new UniformCrossover(); //Guess
            IMutation mutation = new UniformMutation(); //Guess


            ITermination stagnation = new FitnessStagnationTermination(200);
            ITermination timeLimit = new TimeEvolvingTermination(new TimeSpan(3, 0, 0));
            ITermination threshold;
            if (agents.Count < 3)
            {
                 threshold = new FitnessThresholdTermination(.97);
            }
            else
            {
                 threshold = new FitnessThresholdTermination(.93);
            }
            
            OrTermination eitherTermination = new OrTermination(new ITermination[] { stagnation, threshold, timeLimit });

            ITaskExecutor taskExecutor = new ParallelTaskExecutor()
            {
                MaxThreads = Environment.ProcessorCount + 1,
                MinThreads = Environment.ProcessorCount -2
            };


            GeneticAlgorithm algorithm = new GeneticAlgorithm(population, fitness, selection, crossover, mutation)
            {
                TaskExecutor = new TplTaskExecutor(),
            };

            algorithm.TaskExecutor = taskExecutor;
            algorithm.Termination =  eitherTermination;

            algorithm.Start();

            SaveChromosome((FloatingPointChromosome)algorithm.BestChromosome, chromosomeLabel);

            Debug.WriteLine("finished Training, with {0} time spent on evolving", algorithm.TimeEvolving);
            Debug.WriteLine("fitness of this generation vs the last : {0}", algorithm.BestChromosome.Fitness);

            return (FloatingPointChromosome)algorithm.BestChromosome;
        }

       /// <summary>
       /// Normalizes Genes to avoid features having the max value allowable
       /// Allows Future generation to have more "breathing room" to grow.
       /// Requires linear heuristic function
       /// </summary>
       /// <param name="genes">un-normalized genes</param>
       /// <returns>normalized genes</returns>
        public static double[] NormalizeGenes(double[] genes)
        {
            double currentMaxGene = genes.ToList().Max();
            double scaleFactor = .3 * MAX_GENE_VALUE / currentMaxGene;
            return genes.ToList().Select(gene => gene * scaleFactor).ToArray();
        }

        private static void SaveChromosome(FloatingPointChromosome chromosome, string chromosomeLabel)
        {
            //string path = String.Format(@"{0}\bestchromosome.dat", Application.StartupPath);
            SaveChromosome(chromosome, chromosomeLabel, FOLDER_PATH);
        }

        private static void SaveChromosome(FloatingPointChromosome chromosome, string chromosomeLabel, string path)
        {
            string[] genes = chromosome.ToFloatingPoints().Select((gene) => gene.ToString()).ToArray();
            System.IO.File.WriteAllLines(path + @chromosomeLabel, genes);
        }

        private static double[] LoadChromosome(string path = @"E:\Source\SamOthellop\SamOthellop\Model\Agents\Genetic\bestchromosome.dat")
        {
            string[] genes = System.IO.File.ReadAllLines(path);
            return genes.Select((gene) => double.Parse(gene)).ToArray();
        }

        public static void PrintChromosomeFitness(string path, IOthelloAgent opponent)
        {
            FloatingPointChromosome chm = new FloatingPointChromosome(new double[] { -100, -100, -100, -100, -100, -100, -100, -100, -100, -100, -100, -100, -100, -100, -100, -100, -100, -100, -100, -100, -100 },
                                                                        new double[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 },
                                                                        new int[] { 64, 64, 64, 64, 64, 64, 64, 64, 64, 64, 64, 64, 64, 64, 64, 64, 64, 64, 64, 64, 64 },
                                                                        new int[] { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 },
                                                                        LoadChromosome(path));
            PrintChromosomeFitness(chm, opponent);

        }

        public static void PrintChromosomeFitness(FloatingPointChromosome chromosome, IOthelloAgent opponent)
        {
            EvaluationFitness fitnessTest = new EvaluationFitness(opponent);
            double fitness = fitnessTest.Evaluate(chromosome);

            Console.WriteLine("Fitness of {0} \n vs {1} : {2}", chromosome.GetGenes().ToString() , opponent.ToString(), fitness);
        }

    }
}
