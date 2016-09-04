using System;
using System.Collections.Generic;
using DvrpTaskSolverCommon.DvrpObjects;

namespace DvrpTaskSolverCommon.ProblemData
{
    [Serializable]
    public class PartialProblem
    {
        public int Id { get; set; }
        public int SetId { get; set; }
        public HashSet<Client> Clients { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, SetId: {SetId},Clients: {Clients.Count}";
        }
    }
}
