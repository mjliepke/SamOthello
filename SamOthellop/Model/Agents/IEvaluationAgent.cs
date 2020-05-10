using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamOthellop.Model.Agents
{
    /// <summary>
    /// IothelloAgent that ranks board states, allowing a min-max approach
    /// </summary>
    public abstract class IEvaluationAgent : IOthelloAgent
    {
        public abstract double EvaluateBoard(OthelloGame game, BoardStates player);
    }
}
