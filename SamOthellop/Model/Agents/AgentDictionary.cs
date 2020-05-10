using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SamOthellop.Model.Agents
{
    public class AgentDict
    {
        public Dictionary<string, Type> AgentDictionary;
        public AgentDict()
        {
            var asm = Assembly.GetExecutingAssembly();
            var agents = AgentDictHelper.GetTypesWithInterface(asm);
            IEnumerable<string> agentStrings = agents.Select((agent) => ((Type)agent).Name);

            AgentDictionary = agentStrings.Zip(agents, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);
        } 
    }


    public static class AgentDictHelper
    {

        public static IEnumerable<Type> GetTypesWithInterface(Assembly asm)
        {
            var it = typeof(IOthelloAgent);
            return asm.GetLoadableTypes().Where(it.IsAssignableFrom)
                .Where(t=>!(t.Equals(typeof(IOthelloAgent))))
                .Where(t=>!(t.Equals(typeof(IEvaluationAgent)))).ToList();
        }

        private static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException("assembly");
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }

    }
}
