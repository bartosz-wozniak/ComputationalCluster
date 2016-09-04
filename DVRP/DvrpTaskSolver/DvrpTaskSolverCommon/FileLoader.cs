using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using DvrpTaskSolverCommon.DvrpObjects;
using DvrpTaskSolverCommon.ProblemData;

namespace DvrpTaskSolverCommon
{
    public class FileLoader
    {
        private enum LoadingState
        {
            NotStarted, Solution, VehiclesCount, VehicleCapacity, Depots, Demand, Locations, DepotLocations, ClientsLocations, ClientDuration, DepotTime, Speed, ClientStartTime, Finished
        }
        private Dictionary<LoadingState, string> headers = new Dictionary<LoadingState, string>()
        {
            { LoadingState.Solution, "COMMENT: Best known objective:"},
            { LoadingState.VehiclesCount, "NUM_VEHICLES"},
            { LoadingState.VehicleCapacity, "CAPACITIES"},
            { LoadingState.Depots, "DEPOTS"},
            { LoadingState.Demand, "DEMAND_SECTION"},
            { LoadingState.Locations, "LOCATION_COORD_SECTION"},
            { LoadingState.DepotLocations, "DEPOT_LOCATION_SECTION"},
            { LoadingState.ClientsLocations, "VISIT_LOCATION_SECTION"},
            { LoadingState.ClientDuration, "DURATION_SECTION"},
            { LoadingState.DepotTime, "DEPOT_TIME_WINDOW_SECTION"},
            { LoadingState.Speed, "COMMENT: TIMESTEP:"},
            { LoadingState.ClientStartTime, "TIME_AVAIL_SECTION"},
            { LoadingState.Finished, "EOF"}
        };

        private class Location
        {
            public int Id { get; set; }
            public double X { get; set; }
            public double Y { get; set; }

        }

        private char separator = ' ';

        private LoadingState state = LoadingState.NotStarted;
        private StreamReader file;
        private Problem problem;
        private List<Location> locations = new List<Location>();
        private double solution;

        public FileLoader() { }

        public FileLoader(Stream stream)
        {
            file = new StreamReader(stream);
        }

        public Problem LoadProblem(string path = null)
        {
            state = LoadingState.NotStarted;
            problem = new Problem();
            problem.Clients = new HashSet<DvrpTaskSolverCommon.DvrpObjects.Client>();
            problem.Depots = new HashSet<Depot>();

            if (path != null)
            {
                file = new StreamReader(path);
            }

            string line;
            while ((line = file.ReadLine()) != null)
            {
                ProcessLineWithProblem(line);
            }
            return problem;
        }

        public double LoadSolution(string path = null)
        {
            state = LoadingState.NotStarted;

            if (path != null)
            {
                file = new StreamReader(path);
            }
            string line;
            while ((line = file.ReadLine()) != null)
            {
                ProcessLineWithSolution(line);
            }

            return solution;
        }

        private void ProcessLineWithSolution(string line)
        {
            var parts = line.Split(separator);

            if (headers.ContainsKey(state + 1) && line.Contains(headers[state + 1]))
            {
                state++;
                switch (state)
                {
                    case LoadingState.Solution:
                        {
                            solution = double.Parse(parts.Last(), CultureInfo.InvariantCulture);
                            break;
                        }
                }
            }
        }

        private void ProcessLineWithProblem(string line)
        {
            var parts = line.Split(separator);

            if (headers.ContainsKey(state + 1) && line.Contains(headers[state + 1]))
            {
                state++;
                //Data that occurs in the same line that header  
                switch (state)
                {
                    case LoadingState.VehiclesCount:
                        {
                            problem.VehiclesCount = int.Parse(parts[1]);
                            break;
                        }
                    case LoadingState.VehicleCapacity:
                        {
                            problem.VehiclesCapacity = int.Parse(parts[1]);
                            break;
                        }
                    case LoadingState.Speed:
                        {
                            problem.VehicleSpeed = int.Parse(parts[2]);
                            break;
                        }
                }
            }
            else
            {
                switch (state)
                {
                    case LoadingState.Depots:
                        {
                            problem.Depots.Add(new Depot() { Id = int.Parse(parts[2]) });
                            break;
                        }
                    case LoadingState.Demand:
                        {
                            problem.Clients.Add(new DvrpTaskSolverCommon.DvrpObjects.Client()
                            {
                                Id = int.Parse(parts[2]),
                                RequestSize = Math.Abs(double.Parse(parts[3]))
                            });
                            break;
                        }
                    case LoadingState.Locations:
                        {
                            locations.Add(new Location()
                            {
                                Id = int.Parse(parts[2]),
                                X = double.Parse(parts[3]),
                                Y = double.Parse(parts[4])
                            });
                            break;
                        }
                    case LoadingState.DepotLocations:
                        {
                            int depotId = int.Parse(parts[2]);
                            int locationId = int.Parse(parts[3]);
                            var loc = locations.First(l => l.Id == locationId);
                            var depot = problem.Depots.First(c => c.Id == depotId);
                            depot.X = loc.X;
                            depot.Y = loc.Y;
                            break;
                        }
                    case LoadingState.ClientsLocations:
                        {
                            int clientId = int.Parse(parts[2]);
                            int locationId = int.Parse(parts[3]);
                            var loc = locations.First(l => l.Id == locationId);
                            var client = problem.Clients.First(c => c.Id == clientId);
                            client.X = loc.X;
                            client.Y = loc.Y;
                            break;
                        }
                    case LoadingState.ClientDuration:
                        {
                            int clientId = int.Parse(parts[2]);
                            var client = problem.Clients.First(c => c.Id == clientId);
                            client.UnloadTime = (int.Parse(parts[3]));
                            break;
                        }
                    case LoadingState.DepotTime:
                        {
                            int depotId = int.Parse(parts[2]);
                            var depot = problem.Depots.First(c => c.Id == depotId);
                            depot.StartTime = (int.Parse(parts[3]));
                            depot.EndTime = (int.Parse(parts[4]));
                            break;
                        }
                    case LoadingState.ClientStartTime:
                        {
                            int clientId = int.Parse(parts[2]);
                            var client = problem.Clients.First(c => c.Id == clientId);
                            client.StartTime = (int.Parse(parts[3]));
                            break;
                        }
                }
            }
        }

     


    }
}
