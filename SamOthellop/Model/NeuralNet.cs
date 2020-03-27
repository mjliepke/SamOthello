using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamOthellop.Model
{
    class BoardNeuralNet
    {
        int[] size =[64, 32, 16, 8, 4, 1];
        double[,,] weights;
        double[,] outs;

        double[,] delta;

    }
}
