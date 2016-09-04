using System;
using System.Linq;
using System.Reflection;
using DvrpTaskSolver.Computations;
using DvrpTaskSolverCommon.DataConversion;
using DvrpTaskSolverCommon.ProblemData;
using log4net;
using UCCTaskSolver;

namespace DvrpTaskSolver
{
    public class DvrpTaskSolver : TaskSolver
    {
        public override string Name { get; }

        private ISerializer serializer;
        private readonly Problem problem;
        private static ILog logger =LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public DvrpTaskSolver(byte[] problemData) : base(problemData)
        {
            Name = "DVRP";
            serializer = new Serializer();
            problem = serializer.DeserializeProblem(problemData);
            FixClientsStartTimes(problem);
        }

        public override byte[] Solve(byte[] partialData, TimeSpan timeout)
        {
            logger.Warn("Starting solving partial problem");
            PartialProblemSolver solver = new PartialProblemSolver();
            var partialProblemData = serializer.DeserializePartialProblem(partialData);
            var result = solver.SolvePartialProblem(partialProblemData, problem);
            logger.Warn($"Finished solving partial problem, result: {result.Result}");
            return serializer.SerializePartialSolution(result);
        }

        public override byte[][] DivideProblem(int threadCount)
        {
            logger.Warn("Starting dividing problem");
            ProblemDivider divider = new ProblemDivider();
            var result = divider.DivideProblem(problem);
            var ret = new byte[result.Count][];
            for (int i = 0; i < result.Count; i++)
            {
                ret[i] = serializer.SerializePartialProblem(result[i]);
            }
            logger.Warn("Finished dividing problem");
            return ret;
        }

        public override byte[] MergeSolution(byte[][] solutions)
        {
            logger.Warn("Starting merging problem");
            SolutionMerger merger = new SolutionMerger();
            var soluts = serializer.DeserializePartialSolutions(solutions);
            var result = merger.MergeSolution(soluts, problem);
            logger.Warn($"Solution: {result.Result}");

            logger.Warn("Finished merging problem");
            return serializer.SerializeSolution(result);
        }

        public static void FixClientsStartTimes(Problem p)
        {
            int dTime = p.Depots.First().EndTime / 2;
            foreach (var c in p.Clients)
            {
                if (c.StartTime > dTime)
                    c.StartTime = 0;
            }
        }
    }
}
