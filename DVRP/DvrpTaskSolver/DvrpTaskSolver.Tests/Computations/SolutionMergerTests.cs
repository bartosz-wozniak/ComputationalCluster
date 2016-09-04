using System.Collections.Generic;
using System.Linq;
using DvrpTaskSolver.Computations;
using DvrpTaskSolverCommon.DvrpObjects;
using DvrpTaskSolverCommon.ProblemData;
using Shouldly;
using Shouldly.Configuration;

namespace DvrpTaskSolver.Tests.Computations
{
    public class SolutionMergerTests
    {
        private readonly SolutionMerger solutionMerger;

        public SolutionMergerTests()
        {
            solutionMerger = new SolutionMerger();
        }

        public void four_sets_merging_test()
        {
            List<Client> c = new List<Client>()
            {
                new Client() { Id = 1 },
                new Client() { Id = 2 },
                new Client() { Id = 3 }
            };

            List<PartialSolution> list = new List<PartialSolution>()
            {
                new PartialSolution() {Clients = new HashSet<Client>(), Result = double.Epsilon},
                new PartialSolution() {Clients = new HashSet<Client>() {c[0]} , Result = 16},
                new PartialSolution() {Clients = new HashSet<Client>() {c[1]} , Result = 55},
                new PartialSolution() {Clients = new HashSet<Client>() {c[2]} , Result = 2},
                new PartialSolution() {Clients = new HashSet<Client>() {c[0], c[1] } , Result = 5}, 
                new PartialSolution() {Clients = new HashSet<Client>() {c[0], c[2] } , Result = 11},
                new PartialSolution() {Clients = new HashSet<Client>() {c[1], c[2] } , Result = 5},
                new PartialSolution() {Clients = new HashSet<Client>() {c[0], c[1], c[2] } , Result = 100},
            };
            
            var res = solutionMerger.MergeSolution(list, new Problem()
            {
                Clients = new HashSet<Client>(c),
                VehiclesCount = 3
            });

            res.Result.ShouldBe(7);
        }
    }
}
