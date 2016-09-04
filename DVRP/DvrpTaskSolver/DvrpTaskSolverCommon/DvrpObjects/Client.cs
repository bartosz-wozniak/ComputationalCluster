using System;

namespace DvrpTaskSolverCommon.DvrpObjects
{
    [Serializable]
    public class Client : IDvrpObject
    {
        public int Id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public int StartTime { get; set; }
        public int UnloadTime { get; set; }
        public double RequestSize { get; set; }

        public override string ToString()
        {
            return Id.ToString();
        }
    }
}
