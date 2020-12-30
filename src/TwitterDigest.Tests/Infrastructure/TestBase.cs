using System.IO;

namespace TwitterDigest.Tests.Infrastructure
{
    public abstract class TestBase
    {
        public static class TestContext
        {
            public static string GetStringFromFile(string filename)
            {
                var executingPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                var path = Path.GetFullPath(Path.Combine(executingPath, @$"..\..\..\..\Resources\{filename}"));
                return File.ReadAllText(path);
            }
        }
    }
}
