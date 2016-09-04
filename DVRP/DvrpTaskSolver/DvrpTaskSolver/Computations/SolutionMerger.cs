using System.Collections.Generic;
using System.Linq;
using DvrpTaskSolverCommon.DvrpObjects;
using DvrpTaskSolverCommon.ProblemData;

namespace DvrpTaskSolver.Computations
{
    public class SolutionMerger
    {
        private List<PartialSolution> partialSolutions;
        private Problem problem;
        private double bestResult = double.MaxValue;
        
        public Solution MergeSolution(List<PartialSolution> partialSolutions, Problem problem)
        {
            this.problem = problem;

            this.partialSolutions = partialSolutions;
            var empty = partialSolutions.First(p => p.Clients.Count == 0);
            partialSolutions.Remove(empty);

            Rec(new HashSet<PartialSolution>(), 0);

            return new Solution { Result = bestResult };
        }

        private void Rec(HashSet<PartialSolution> set, double cost)
        {
            if (cost >= bestResult)
                return;
            bool isValid = set.Count <= problem.VehiclesCount;
            // Loking for a client in many partialSolution (duplicate) 
            if (isValid)
            {
                foreach (var p in set)
                {
                    foreach (var client in p.Clients)
                    {
                        if (set.Where(pp => pp != p).Any(pp => pp.Clients.Contains(client)))
                        {
                            isValid = false;
                            break;
                        }
                    }
                    if (!isValid) break;
                }
            }
            if (!isValid) return;

            // Check if this is valid partition of all problem clients
            bool isPartition = (set.Sum(p => p.Clients.Count) == problem.Clients.Count);
            if (isPartition)
            {
                if (cost < bestResult)
                    bestResult = cost;
                return;
            }

            var otherPartialSolutions = partialSolutions.Where(p => !set.Contains(p));
            foreach (var p in otherPartialSolutions)
            {
                set.Add(p);
                Rec(set, cost + p.Result);
                set.Remove(p);
            }
        }
    }
}
