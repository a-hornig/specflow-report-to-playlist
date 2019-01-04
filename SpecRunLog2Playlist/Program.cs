using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SpecRunLog2Playlist
{
    class Program
    {
        static TextReader input = Console.In;
        static void Main(string[] args)
        {
            if (args.Any())
            {
                var path = args[0];
                if (File.Exists(path))
                {
                    input = File.OpenText(path);
                }
            }

            var tests = new Dictionary<string, TestItem>();

            for (string line; (line = input.ReadLine()) != null;)
            {
                if (line.IsRegExMatch(@"^#\d+.*TestAssembly:"))
                {
                    var id = line.GetRegExMatch(@"^#\d+");
                    tests.Add(id, new TestItem
                    {
                        Id = id,
                        Text = line.GetRegExMatch(@"(Target|TestAssembly):.*"),
                        Assembly = line.GetRegExCaptureGroup(@"Assembly:([^/]+)"),
                        Feature = line.GetRegExCaptureGroup(@"Feature:([^/]+)"),
                    });
                }
                else if (line.IsRegExMatch(@"Test #\d+/\d is finished"))
                {
                    var id = line.GetRegExCaptureGroup(@"Test (#\d+)");
                    tests[id].Status = line.GetRegExCaptureGroup(@"(\S+)\swithin");
                    // with retries only the last status will be used
                }
            }


            Console.WriteLine("<Playlist Version=\"1.0\">");
            foreach (var test in tests)
            {
                var p1 = test.Value.Assembly;
                var p2 = test.Value.Feature;
                var p3 = test.Value.Text;
                Console.WriteLine($"<Add Test=\"{p1}.{p2}.#()::{p3}\" />");
            }
            Console.WriteLine("</Playlist>");
        }
    }
}
