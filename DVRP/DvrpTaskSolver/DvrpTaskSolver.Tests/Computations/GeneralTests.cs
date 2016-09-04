using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DvrpTaskSolver.Computations;
using DvrpTaskSolverCommon;
using DvrpTaskSolverCommon.ProblemData;
using Shouldly;

namespace DvrpTaskSolver.Tests.Computations
{
    public class GeneralTests
    {
        FileLoader loader;

        public void basic_test()
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(str);
            writer.Flush();
            stream.Position = 0;
            loader = new FileLoader(stream);

            Problem p = loader.LoadProblem();
            DvrpTaskSolver.FixClientsStartTimes(p);
            stream.Position = 0;
            double sol = loader.LoadSolution();

            ProblemDivider divider = new ProblemDivider();
            PartialProblemSolver solver = new PartialProblemSolver();
            SolutionMerger merger = new SolutionMerger();

            var partproblmes = divider.DivideProblem(p);

            List<PartialSolution> soluts = new List<PartialSolution>();
            foreach (var part in partproblmes/*.Where(ee => ee.SetId == 17)*/)
            {
                soluts.Add(solver.SolvePartialProblem(part,p));
            }

            var ssss = merger.MergeSolution(soluts, p);
            ssss.Result.ShouldBeInRange(sol-5, sol + 5);
        }

        private string str = @"VRPTEST io2_8a
COMMENT: 
COMMENT: Best known objective: 591.10
NAME: io2_8a
NUM_DEPOTS: 1
NUM_CAPACITIES: 1
NUM_VISITS: 8
NUM_LOCATIONS: 9
NUM_VEHICLES: 8
CAPACITIES: 100
DATA_SECTION
DEPOTS
  0
DEMAND_SECTION
  1 -18
  2 -29
  3 -34
  4 -14
  5 -18
  6 -18
  7 -7
  8 -13
LOCATION_COORD_SECTION
  0 0 0
  1 -5 53
  2 28 72
  3 85 -8
  4 -24 -16
  5 -30 37
  6 17 60
  7 -34 -77
  8 -47 91
DEPOT_LOCATION_SECTION
  0 0
VISIT_LOCATION_SECTION
  1 1
  2 2
  3 3
  4 4
  5 5
  6 6
  7 7
  8 8
DURATION_SECTION
  1 20
  2 20
  3 20
  4 20
  5 20
  6 20
  7 20
  8 20
DEPOT_TIME_WINDOW_SECTION
  0 0 560
COMMENT: TIMESTEP: 7
TIME_AVAIL_SECTION
  1 556
  2 513
  3 136
  4 155
  5 207
  6 274
  7 420
  8 103
EOF";
    }
}
