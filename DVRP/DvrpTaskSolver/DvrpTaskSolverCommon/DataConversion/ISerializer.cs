using System.Collections.Generic;
using DvrpTaskSolverCommon.ProblemData;

namespace DvrpTaskSolverCommon.DataConversion
{
    public interface ISerializer
    {
        Problem DeserializeProblem(byte[] data);

        byte[] SerializePartialProblem(PartialProblem partialProblem);

        List<PartialSolution> DeserializePartialSolutions(byte[][] partialSolutions);

        byte[] SerializeSolution(Solution solution);

        PartialProblem DeserializePartialProblem(byte[] partialProblem);

        byte[] SerializePartialSolution(PartialSolution partialSolution);

        byte[] SerializeProblem(Problem problem);

        Solution DeserializeSolution(byte[] solution);

    }
}
