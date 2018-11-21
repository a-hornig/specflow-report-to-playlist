using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SpecFlowReport2Playlist
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

            // use `input` for all input operations
            var lines = new List<string>();
            for (string line; (line = input.ReadLine()) != null;)
            {
                var newline = line.GetRegExCaptureGroup(@"(?:<h3>Scenario|<li>Status): (.*)</");
                if (newline.IsNullOrEmpty())
                    continue;
                lines.Add(newline);
            }

            var cleanLines = new List<string>();
            for (int i = 0; i < lines.Count; i=i+2)
            {
                var linePart1 = lines[i].ReplaceRegExMatch(@"\s*\(in (\S+)\, (.*)\)", ":$1:$2");
                var linePart2 = lines[i + 1];
                cleanLines.Add($"{linePart1}:{linePart2}");
            }

            foreach (var cl in cleanLines)
            {
                var pArr = cl.Split(':');
                var p1 = pArr[1]; // assembly
                var p2 = pArr[2]; // feature
                var p3 = p2.Replace(" ", "+");
                var p4 = pArr[0].Replace(" ", "+").ReplaceRegExMatch(@"[:,\'\[\]/]", (m) => Uri.HexEscape(Convert.ToChar(m.Value)).ToLower());
                
                Console.WriteLine($"<Add Test=\"{p1}.{p2}.#()::TestAssembly:{p1}/Feature:{p3}/Scenario:{p4}\" />");
            }

        }
    }
}
