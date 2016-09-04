using UCCTaskSolver;

namespace DvrpTaskSolver
{
    public class DvrpTaskSolverCreator : TaskSolverCreator
    {
        public override TaskSolver CreateTaskSolverInstance(byte[] problemData)
        {
            return new DvrpTaskSolver(problemData);
        }
    }
}
