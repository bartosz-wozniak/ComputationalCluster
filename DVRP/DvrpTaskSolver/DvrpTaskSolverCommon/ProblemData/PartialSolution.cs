using System;
using System.Collections.Generic;
using DvrpTaskSolverCommon.DvrpObjects;

namespace DvrpTaskSolverCommon.ProblemData
{
    [Serializable]
    public class PartialSolution
    {
        public int Id { get; set; }
        public HashSet<Client> Clients { get; set; }
        public double Result { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, Cleints: {Clients.Count},Result: {(Result >= int.MaxValue ? "  MAX" : Result.ToString())}";
        }
    }
}
