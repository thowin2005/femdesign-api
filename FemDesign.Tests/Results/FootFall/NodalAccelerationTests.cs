﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using FemDesign.Results;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FemDesign.Results
{
    [TestClass()]
    public class NodalAccelerationTests
    {
        [TestMethod]
        public void Parse()
        {
            string modelPath = "Results\\Assets\\SimpleFrame.str";

            var (resultLines, headers, results) = UtilTestMethods.GetCsvParseData<NodalAcceleration>(modelPath);


            // Check parsed data
            Assert.IsTrue(results.First().GetType() == typeof(NodalAcceleration), $"{typeof(NodalAcceleration).Name} should be parsed");
            Assert.IsTrue(results.Last().GetType() == typeof(NodalAcceleration), $"{typeof(NodalAcceleration).Name} should be parsed");
            Assert.IsTrue(results.Count == resultLines.Sum(), "Should read all results.");

            foreach (var header in headers)
            {
                // Check header
                foreach (var line in header)
                {
                    var headerMatch = NodalAcceleration.HeaderExpression.Match(line);
                    Assert.IsTrue(headerMatch.Success, $"Should identify \"{line}\" as header");
                }

                // Check identification
                var identifier = header[0];
                var match = NodalAcceleration.IdentificationExpression.Match(identifier);
                Assert.IsTrue(match.Success, $"Should identify type of \"{identifier}\" as {typeof(NodalAcceleration).Name}");
                Assert.IsTrue(match.Groups["type"].Success);
                Assert.IsTrue(match.Groups["result"].Success);
                Assert.IsTrue(match.Groups["casename"].Success);
            }
        }

    }
}
