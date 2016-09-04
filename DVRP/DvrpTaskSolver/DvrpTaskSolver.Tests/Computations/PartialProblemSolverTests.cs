using System;
using System.Collections.Generic;
using DvrpTaskSolver.Computations;
using DvrpTaskSolverCommon.DvrpObjects;
using DvrpTaskSolverCommon.ProblemData;
using Shouldly;

namespace DvrpTaskSolver.Tests.Computations
{
    public class PartialProblemSolverTests
    {
        private readonly PartialProblemSolver problemSolver;

        public PartialProblemSolverTests()
        {
            problemSolver = new PartialProblemSolver();
        }

        public void SimpleProblem()
        {
            PartialProblem pp = new PartialProblem
            {
                Id = 1,
                SetId = 1,
                Clients = new HashSet<Client>
                {
                    new Client
                    {
                        RequestSize = 5,
                        StartTime = 14,
                        UnloadTime =1,
                        X = 0,
                        Y = 0
                    }
                }
            };
            Problem p = new Problem
            {
                VehiclesCapacity = 10,
                VehicleSpeed = 1,
                Depots = new HashSet<Depot>
                {
                    new Depot
                    {
                        StartTime = 12,
                        EndTime = 18,
                        X = 1,
                        Y = 1
                    }
                }
            };
            var ret = problemSolver.SolvePartialProblem(pp, p);
            ret.Result.ShouldBe(2 * Math.Sqrt(2));
        }

        public void SimpleProblemWithTwoClients()
        {
            PartialProblem pp = new PartialProblem
            {
                Id = 1,
                SetId = 1,
                Clients = new HashSet<Client>
                {
                    new Client
                    {
                        RequestSize = 5,
                        StartTime = 14,
                        UnloadTime = 1,
                        X = 0,
                        Y = 0
                    },
                    new Client
                    {
                        RequestSize = 5,
                        StartTime = 14,
                        UnloadTime = 1,
                        X = 0,
                        Y = 1
                    }
                }
            };
            Problem p = new Problem
            {
                VehiclesCapacity = 10,
                VehicleSpeed = 1,
                Depots = new HashSet<Depot>
                {
                    new Depot
                    {
                        StartTime = 12,
                        EndTime = 20,
                        X = 1,
                        Y = 1
                    }
                }
            };
            var ret = problemSolver.SolvePartialProblem(pp, p);
            ret.Result.ShouldBe(2 + Math.Sqrt(2));
        }

        public void SimpleProblemWithTwoClientsWithReturnToDepot()
        {
            PartialProblem pp = new PartialProblem
            {
                Id = 1,
                SetId = 1,
                Clients = new HashSet<Client>
                {
                    new Client
                    {
                        RequestSize = 6,
                        StartTime = 14,
                        UnloadTime = 1,
                        X = 0,
                        Y = 0
                    },
                    new Client
                    {
                        RequestSize = 5,
                        StartTime = 14,
                        UnloadTime = 1,
                        X = 0,
                        Y = 1
                    }
                }
            };
            Problem p = new Problem
            {
                VehiclesCapacity = 10,
                VehicleSpeed = 1,
                Depots = new HashSet<Depot>
                {
                    new Depot
                    {
                        StartTime = 12,
                        EndTime = 20,
                        X = 1,
                        Y = 1
                    }
                }
            };
            var ret = problemSolver.SolvePartialProblem(pp, p);
            ret.Result.ShouldBe(2 + 2 * Math.Sqrt(2));
        }
    }
}
