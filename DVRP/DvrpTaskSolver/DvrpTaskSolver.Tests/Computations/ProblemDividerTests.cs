using System.Collections.Generic;
using System.Linq;
using DvrpTaskSolver.Computations;

using DvrpTaskSolverCommon.DvrpObjects;
using DvrpTaskSolverCommon.ProblemData;
using Shouldly;

namespace DvrpTaskSolver.Tests.Computations
{
    public class ProblemDividerTests
    {
        private readonly ProblemDivider problemDivider;

        public ProblemDividerTests()
        {
            problemDivider = new ProblemDivider();
        }

        public void empty_problem()
        {
            Problem p = new Problem() {Clients = new HashSet<Client>()};
            var ret = problemDivider.DivideProblem(p);
            ret.Count.ShouldBe(1);
        }

        public void three_client()
        {
            Problem p = new Problem();
            p.Clients = new HashSet<Client>()
            {
                new Client() {Id = 1},
                new Client() {Id = 2},
                new Client() {Id = 3}
            };
            p.VehiclesCount = 2;
            var ret = problemDivider.DivideProblem(p);
            ret.Count.ShouldBe(7);
        }

        public void many_clients_and_one_vehicle()
        {
            Problem p = new Problem();
            p.Clients = new HashSet<Client>()
            {
                new Client() {Id = 1},
                new Client() {Id = 2},
                new Client() {Id = 3},
                new Client() {Id = 4},
                new Client() {Id = 5}
            };
            p.VehiclesCount = 1;

            var ret = problemDivider.DivideProblem(p);
            ret.Count.ShouldBe(6);
        }

        public void one_client_and_many_vehicles()
        {
            Problem p = new Problem();
            p.Clients = new HashSet<Client>()
            {
                new Client() {Id = 1}
            };
            p.VehiclesCount = 6;

            var ret = problemDivider.DivideProblem(p);
            ret.Count.ShouldBe(2);
        }


    }
}
