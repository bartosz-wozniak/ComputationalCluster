using System;

namespace DvrpTaskSolverCommon.DvrpObjects
{
    [Serializable]
    public class Depot : IDvrpObject
    {
        public int Id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
    }
}
