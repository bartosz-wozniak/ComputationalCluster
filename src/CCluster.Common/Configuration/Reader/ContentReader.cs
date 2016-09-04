using System.IO;

namespace CCluster.Common.Configuration.Reader
{
    public class ContentReader : IContentReader
    {
        public string Read(string fileName)
        {
            return File.ReadAllText(fileName);
        }
    }
}
