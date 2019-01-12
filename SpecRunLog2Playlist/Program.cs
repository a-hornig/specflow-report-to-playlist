using NDesk.Options;
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
            bool showHelp = false;
            string inputFile;
            string inputStatus = string.Empty;
            string filter = string.Empty;
            var p = new OptionSet {
                { "h|help",  "show this message and exit", v => showHelp = v != null },
                { "s|status",  "filter on test status Succeeded|Failed|Pending", v => filter = v }
            };
            List<string> optionValues = p.Parse(args);

            if (showHelp)
            {
                ShowHelpAndExit(p);
            }

            if (!args.Any())
            {
                ShowHelpAndExit(p, "ERROR: A SpecRun log file path is required as input.\n");
            }

            if (filter.IsNullOrEmpty())
            {
                inputFile = optionValues[0];
            }
            else
            {
                inputFile = optionValues[1];
                inputStatus = optionValues[0];
            }

            if (File.Exists(inputFile))
            {
                input = File.OpenText(inputFile);
            }
            else
            {
                ShowHelpAndExit(p, "ERROR: The SpecRun log file does not exist.\n");
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
                        Feature = line.GetRegExCaptureGroup(@"Feature:([^/]+)").Replace("+",""),
                    });
                }
                else if (line.IsRegExMatch(@"Test #\d+/\d is finished"))
                {
                    var id = line.GetRegExCaptureGroup(@"Test (#\d+)");
                    tests[id].Status = line.GetRegExCaptureGroup(@"(\S+)\swithin");
                    // with retries only the last status will be used
                }
            }

            var filteredTests = inputStatus.IsNullOrEmpty() ? tests : tests.Where(t => t.Value.Status.Equals(inputStatus)).ToDictionary(x => x.Key, x => x.Value);

            Console.WriteLine("<Playlist Version=\"1.0\">");
            foreach (var test in filteredTests)
            {
                var p1 = test.Value.Assembly;
                var p2 = test.Value.Feature;
                var p3 = test.Value.Text;
                Console.WriteLine($"<Add Test=\"{p1}.{p2}.#()::{p3}\" />");
            }
            Console.WriteLine("</Playlist>");
        }

        static void ShowHelpAndExit(OptionSet p, string errorMessage = "")
        {
            Console.WriteLine(errorMessage);
            Console.WriteLine("Usage: specRunLog2Playlist [-s <testStatusValue>] <specrunlog>");
            Console.WriteLine("Create a VS test playlist from a SpecRun log file.\n");
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
            Environment.Exit(1);
        }
    }
}


