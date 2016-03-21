using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mano.Parser.Tests
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logs = GitLog.CloneAndGetLogs("https://github.com/Kagamine/EasySense.git", @"c:\projects\test\");
            Console.WriteLine("Finished");
            Console.Read();
        }
    }
}
