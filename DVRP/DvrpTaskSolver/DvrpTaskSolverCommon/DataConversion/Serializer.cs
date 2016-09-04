using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using DvrpTaskSolverCommon.ProblemData;

namespace DvrpTaskSolverCommon.DataConversion
{
    public class Serializer : ISerializer
    {
        private IFormatter formatter;

        public Serializer()
        {
            formatter = new BinaryFormatter();

        }

        public Problem DeserializeProblem(byte[] data)
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                return (Problem)formatter.Deserialize(stream);
            }
        }

        public byte[] SerializePartialProblem(PartialProblem partialProblem)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, partialProblem);
                return stream.ToArray();
            }
        }

        public List<PartialSolution> DeserializePartialSolutions(byte[][] partialSolutions)
        {
            List<PartialSolution> ret = new List<PartialSolution>();
            for (int i = 0; i < partialSolutions.GetLength(0); i++)
            {
                using (MemoryStream stream = new MemoryStream(partialSolutions[i]))
                {
                    ret.Add((PartialSolution)formatter.Deserialize(stream));
                }
            }
            return ret;
        }

        public byte[] SerializeSolution(Solution solution)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, solution);
                return stream.ToArray();
            }
        }

        public PartialProblem DeserializePartialProblem(byte[] partialProblem)
        {
            using (MemoryStream stream = new MemoryStream(partialProblem))
            {
                return (PartialProblem)formatter.Deserialize(stream);
            }
        }

        public byte[] SerializePartialSolution(PartialSolution partialSolution)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, partialSolution);
                return stream.ToArray();
            }
        }

        public byte[] SerializeProblem(Problem problem)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, problem);
                return stream.ToArray();
            }
        }

        public Solution DeserializeSolution(byte[] solution)
        {
            using (MemoryStream stream = new MemoryStream(solution))
            {
                return (Solution)formatter.Deserialize(stream);
            }
        }
    }
}
