using System;
using System.Collections.Generic;
using DvrpTaskSolverCommon.DvrpObjects;

namespace DvrpTaskSolverCommon.ProblemData
{
    [Serializable]
    public class Problem
    {
        public int Id { get; set; }
        public int VehiclesCount { get; set; }
        public double VehicleSpeed { get; set; }
        public double VehiclesCapacity { get; set; }
        public HashSet<Client> Clients { get; set; }
        public HashSet<Depot> Depots { get; set; }
    }
}
