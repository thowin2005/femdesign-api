﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FemDesign.Samples
{
    public partial class SampleProgram
    {
        private static void TestGetResults()
        {
            // Create batch file to extract the results with
            string bscPath = "Local/Nodal displacements.bsc";
            var bsc = new FemDesign.Calculate.Bsc(Calculate.ResultType.NodalDisplacementsLoadCombination, bscPath);

            // Run FEM-Design script to extract results
            string modelPath = System.IO.Path.GetFullPath("Local/Sample.str");
            var fdscript = FemDesign.Calculate.FdScript.ReadStr(modelPath, new List<string>() { bsc });

            var app = new FemDesign.Calculate.Application();
            bool hasExited = app.RunFdScript(fdscript, false, true, true);

            // Read results from generated results files
            string resultsPath = fdscript.CmdListGen[0].OutFile;
            var results = Results.ResultsReader.Parse(resultsPath);

            // Print all results
            for (int i = 0; i < Math.Min(results.Count, 100); i++)
            {
                Console.WriteLine($"{i}: {results[i]}");
            }

            //var liveloads = results.Cast<FemDesign.Results.NodalDisplacement>().Where(r => r.CaseIdentifier == "Liveload" && r.Id == "S.1").ToList();
            var liveloads = results.Cast<FemDesign.Results.NodalDisplacement>().Where(r => r.CaseIdentifier == "SLS" && r.Id == "S.1").ToList();
            for (int i = 0; i < liveloads.Count; i++)
            {
                Console.WriteLine($"{i}: {liveloads[i]}");
            }
        }
    }
}