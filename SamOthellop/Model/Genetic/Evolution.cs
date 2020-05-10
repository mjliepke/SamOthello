using GeneticSharp.Domain;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Infrastructure.Framework.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamOthellop.Model.Genetic
{
    static class Evolution
    {
        public static void RunGeneticAlgorithm()
        {

            FloatingPointChromosome chromosome = new FloatingPointChromosome(
                                                                            new double[] { -100, -100, -100, -100, -100, -100, 0, 0, 0, 0, 0, 0, -10, -10, -10, -10, -10, -10},
                                                                            new double[] { 100, 100, 100, 100, 100, 100, 255, 255, 255, 255, 255, 255, 10, 10, 10, 10, 10, 10 },
                                                                            new int[] { 64, 64, 64, 64, 64, 64 , 8, 8, 8, 8, 8, 8, 64, 64, 64, 64, 64, 64},
                                                                            new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
                                                                            );

  

            IPopulation population = new TplPopulation(25,50, chromosome);
            IFitness fitness = new EvaluationFitness();
            ISelection selection = new RouletteWheelSelection(); //Guess
            ICrossover crossover = new UniformCrossover(); //Guess
            IMutation mutation = new UniformMutation(); //Guess
      

            ITermination stagnation = new FitnessStagnationTermination(100);
            ITermination threshold = new FitnessThresholdTermination(.95);

            ITaskExecutor taskExecutor = new ParallelTaskExecutor()
            {
                MaxThreads = Environment.ProcessorCount,
                MinThreads = Environment.ProcessorCount / 2
            };


            GeneticAlgorithm algorithm = new GeneticAlgorithm(population, fitness, selection, crossover, mutation) 
                        { TaskExecutor = new TplTaskExecutor(), 
                            MutationProbability = .2f};
            algorithm.TaskExecutor = taskExecutor;
            algorithm.Termination = stagnation;

            algorithm.Start();

            SaveChromosome(algorithm.BestChromosome);

            Console.WriteLine("finished Training");

            Debug.WriteLine("best chromosome: "+algorithm.BestChromosome.ToString());

        }

        private static void SaveChromosome(IChromosome chromosome, string path = "bestchromosome.dat")
        {
            string[] genes = chromosome.GetGenes().Select((gene) => gene.ToString()).ToArray();
            System.IO.File.WriteAllLines(path, genes);
        }

        private static double[] LoadChromosome(string path = "bestchromosome.dat")
        {
            string[] genes = System.IO.File.ReadAllLines(path);
            return genes.Select((gene) => double.Parse(gene)).ToArray();
        }

    }
}
