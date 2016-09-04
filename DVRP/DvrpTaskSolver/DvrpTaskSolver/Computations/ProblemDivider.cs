using System.Collections.Generic;
using System.Linq;
using DvrpTaskSolverCommon.DvrpObjects;
using DvrpTaskSolverCommon.ProblemData;

namespace DvrpTaskSolver.Computations
{
    public class ProblemDivider
    {
        private HashSet<Client> clients;
        private int partialProblemId = 1;
        private List<HashSet<Client>> result;
        private int vehiclesCount;

        public List<PartialProblem> DivideProblem(Problem problem)
        {
            clients = problem.Clients;
            result = new List<HashSet<Client>>();
            vehiclesCount = problem.VehiclesCount;

            Rec(new HashSet<Client>());

            List<PartialProblem> ret = new List<PartialProblem>();
            foreach (var r in result)
            {
                ret.Add(new PartialProblem()
                {
                    Id = partialProblemId++,
                    Clients = r
                });
            }
            return ret;
        }


        private void Rec(HashSet<Client> set )
        {
            if (set.Count > vehiclesCount)
                return;
            if (!result.Contains(set,new ClientHashSetComparer()))
                result.Add(new HashSet<Client>(set));
            
            var otherClients = clients.Where(c => !set.Contains(c));
            foreach (var c in otherClients)
            {
                set.Add(c);
                Rec(set);
                set.Remove(c);
            }
        }

        class ClientHashSetComparer : IEqualityComparer<HashSet<Client>>
        {
            public bool Equals(HashSet<Client> x, HashSet<Client> y)
            {
                foreach (var i in x)
                {
                    if (!y.Contains(i)) return false;
                }
                foreach (var i in y)
                {
                    if (!x.Contains(i)) return false;
                }
                return x.Count == y.Count;
            }

            public int GetHashCode(HashSet<Client> obj)
            {
                return base.GetHashCode();
            }
        }
    }
}
