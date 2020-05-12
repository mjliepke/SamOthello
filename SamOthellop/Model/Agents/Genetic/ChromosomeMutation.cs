using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Mutations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamOthellop.Model.Genetic
{
    class ChromosomeMutation : IMutation
    {
        bool IChromosomeOperator.IsOrdered => throw new NotImplementedException();

        public void Mutate(IChromosome chromosome, float probability)
        {
            throw new NotImplementedException();
        }

        void IMutation.Mutate(IChromosome chromosome, float probability)
        {
            throw new NotImplementedException();
        }
    }
}
