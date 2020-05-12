using GeneticSharp.Domain.Chromosomes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamOthellop.Model.Genetic
{
    class EvaluationChromosome : FloatingPointChromosome
    {
        public EvaluationChromosome() : base(-30, 30,10,2)
        {

        }
    }
}
