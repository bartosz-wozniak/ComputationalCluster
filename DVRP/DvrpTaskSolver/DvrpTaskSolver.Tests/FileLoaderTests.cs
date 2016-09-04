using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DvrpTaskSolverCommon;
using DvrpTaskSolverCommon.ProblemData;
using Shouldly;

namespace DvrpTaskSolver.Tests
{
    public class FileLoaderTests
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
            stream.Position = 0;
            double sol = loader.LoadSolution();
            sol.ShouldBe(787.45);
            p.Clients.Count.ShouldBe(10);
            p.Depots.Count.ShouldBe(1);
            p.Depots.First().X.ShouldBe(0);
            p.Depots.First().Y.ShouldBe(0);

            p.Clients.First(c => c.Id == 1).X.ShouldBe(-41);
            p.Clients.First(c => c.Id == 1).Y.ShouldBe(-74);
            p.Clients.First(c => c.Id == 1).UnloadTime.ShouldBe(20);
            p.Clients.First(c => c.Id == 1).StartTime.ShouldBe(333);
            p.Clients.First(c => c.Id == 1).RequestSize.ShouldBe(20);

            p.Clients.First(c => c.Id == 9).X.ShouldBe(-87);
            p.Clients.First(c => c.Id == 9).Y.ShouldBe(-47);
            p.Clients.First(c => c.Id == 9).UnloadTime.ShouldBe(20);
            p.Clients.First(c => c.Id == 9).StartTime.ShouldBe(507);
            p.Clients.First(c => c.Id == 9).RequestSize.ShouldBe(25);

            p.VehicleSpeed.ShouldBe(7);
            p.VehiclesCapacity.ShouldBe(100);
            p.VehiclesCount.ShouldBe(10);
        }

        private string str = @"VRPTEST io2_10a
COMMENT: 
COMMENT: Best known objective: 787.45
NAME: io2_10a
NUM_DEPOTS: 1
NUM_CAPACITIES: 1
NUM_VISITS: 10
NUM_LOCATIONS: 11
NUM_VEHICLES: 10
CAPACITIES: 100
DATA_SECTION
DEPOTS
  0
DEMAND_SECTION
  1 -20
  2 -25
  3 -26
  4 -45
  5 -4
  6 -39
  7 -29
  8 -32
  9 -25
  10 -12
LOCATION_COORD_SECTION
  0 0 0
  1 -41 -74
  2 74 50
  3 -48 29
  4 20 -80
  5 53 -24
  6 24 -48
  7 93 37
  8 -59 -58
  9 -87 -47
  10 79 -49
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
  9 9
  10 10
DURATION_SECTION
  1 20
  2 20
  3 20
  4 20
  5 20
  6 20
  7 20
  8 20
  9 20
  10 20
DEPOT_TIME_WINDOW_SECTION
  0 0 600
COMMENT: TIMESTEP: 7
TIME_AVAIL_SECTION
  1 333
  2 100
  3 541
  4 586
  5 359
  6 299
  7 385
  8 151
  9 507
  10 264
EOF";

    }
}
