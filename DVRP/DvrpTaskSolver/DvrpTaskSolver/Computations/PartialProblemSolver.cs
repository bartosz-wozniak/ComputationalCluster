using System;
using System.Collections.Generic;
using System.Linq;
using DvrpTaskSolverCommon.DvrpObjects;
using DvrpTaskSolverCommon.ProblemData;

namespace DvrpTaskSolver.Computations
{
    public class PartialProblemSolver
    {
        private double BestSolution { get; set; }
        private HashSet<Client> Clients { get; set; }
        private HashSet<Depot> Depots { get; set; }
        private double VehicleCapacity { get; set; }
        private double VehicleSpeed { get; set; }
        private List<IDvrpObject> CurrentPath { get; set; }

        public PartialSolution SolvePartialProblem(PartialProblem partialProblem, Problem problem)
        {
            if (partialProblem.Clients.Count != 0)
            {
                BestSolution = double.MaxValue;
                Clients = partialProblem.Clients;
                Depots = problem.Depots;
                VehicleCapacity = problem.VehiclesCapacity;
                VehicleSpeed = problem.VehicleSpeed;
                CurrentPath = new List<IDvrpObject>();
                var startDepot = Depots.OrderBy(item => item.StartTime).ToList()[0];
                CurrentPath.Add(startDepot);
                SolveRec(startDepot.StartTime, VehicleCapacity);
            }
            else
            {
                BestSolution = 0;
            }
            return new PartialSolution
            {
                Id = partialProblem.Id,
                Clients = partialProblem.Clients,
                Result = BestSolution
            };
        }

        private void SolveRec(double currentTime, double currentCapacity)
        {
            var currentCost = ComputeCost(CurrentPath);
            if (currentCost > BestSolution)
            {
                return;
            }
            if (Clients.Count == 0)
            {
                var possibleDepots = Depots.Where(item => item.EndTime >= currentTime + GetTime(ComputeDistance(item, CurrentPath.Last())));
                if (!possibleDepots.Any())
                {
                    return;
                }
                var orderedPossibleDepots = possibleDepots.OrderBy(item => ComputeDistance(item, CurrentPath.Last())).ToList();
                CurrentPath.Add(orderedPossibleDepots[0]);
                currentCost = ComputeCost(CurrentPath);
                if (currentCost < BestSolution)
                {
                    BestSolution = currentCost;
                }
                RemoveLast(orderedPossibleDepots[0]);
                return;
            }
            var depotsToVisit = Depots.ToList();
            foreach (var depot in Depots)
            {
                if (depot.EndTime < currentTime + GetTime(ComputeDistance(depot, CurrentPath.Last())))
                {
                    depotsToVisit.Remove(depot);
                }
            }
            if (depotsToVisit.Any() && currentCapacity < VehicleCapacity)
            {
                foreach (var depot in depotsToVisit)
                {
                    var time = GetTime(ComputeDistance(depot, CurrentPath.Last()));
                    CurrentPath.Add(depot);
                    SolveRec(currentTime + time > depot.StartTime ?
                        currentTime + time :
                        depot.StartTime, VehicleCapacity);
                    RemoveLast(depot);
                }
            }
            var possibleClients = Clients.ToList();
            foreach (var client in Clients)
            {
                if (currentCapacity < client.RequestSize)
                {
                    possibleClients.Remove(client);
                }
            }
            if (!possibleClients.Any()) return;
            foreach (var client in possibleClients)
            {
                var time = GetTime(ComputeDistance(client, CurrentPath.Last()));
                CurrentPath.Add(client);
                Clients.Remove(client);
                SolveRec(currentTime + time > client.StartTime ?
                    currentTime + time + client.UnloadTime :
                    client.StartTime + client.UnloadTime, currentCapacity - client.RequestSize);
                RemoveLast(client);
                Clients.Add(client);
            }
        }

        private double ComputeCost(IList<IDvrpObject> path)
        {
            var cost = 0.0;
            for (var i = 0; i < path.Count - 1; ++i)
            {
                cost += ComputeDistance(path[i], path[i + 1]);
            }
            return cost;
        }

        private double ComputeDistance(IDvrpObject object1, IDvrpObject object2)
        {
            return Math.Sqrt((object1.X - object2.X) * (object1.X - object2.X) +
                        (object1.Y - object2.Y) * (object1.Y - object2.Y));
        }

        private double GetTime(double distance)
        {
            return distance / VehicleSpeed;
        }

        private void RemoveLast(IDvrpObject obj)
        {
            int ind = CurrentPath.FindLastIndex(i => i.Id == obj.Id);
            if (ind != CurrentPath.Count -1)
                throw new Exception("Removing not last index");
            CurrentPath.RemoveAt(ind);
        }
    }
}
