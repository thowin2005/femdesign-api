﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Text.RegularExpressions;
using FemDesign.GenericClasses;


namespace FemDesign.Results
{
    /// <summary>
    /// FemDesign "RC design: Shell, utilization" result
    /// </summary>
    public class ShellUtilization : IResult
    {
        /// <summary>
        /// Shell name identifier
        /// </summary>
        public string Id { get; }
        /// <summary>
        /// Max utilization of bottom/top and x'/y' reinforcement and crack width on top/bottom face.
        /// </summary>
        public double Max { get
            {
                return (new double[] { RBX, RBY, RTX, RTY, CWB, CWT }).Max();
            }
        }
        /// <summary>
        /// Utilization of bottom x'/r reinforcement
        /// </summary>
        public double RBX { get; }
        /// <summary>
        /// Utilization of bottom y'/t reinforcement
        /// </summary>
        public double RBY { get; }
        /// <summary>
        /// Utilization of top x'/r reinforcement
        /// </summary>
        public double RTX { get; }
        /// <summary>
        /// Utilization of top y'/t reinforcement
        /// </summary>
        public double RTY { get; }
        /// <summary>
        /// Utilization for shell buckling
        /// </summary>
        public double BU { get; }
        /// <summary>
        /// Shear capacity ok?
        /// </summary>
        public bool SC { get; }
        /// <summary>
        /// Utilization for crack width on the bottom face
        /// </summary>
        public double CWB { get; }
        /// <summary>
        /// Utilization for crack width on the top face
        /// </summary>
        public double CWT { get; }
        /// <summary>
        /// Load case or combination name
        /// </summary>
        public string CaseIdentifier { get; }

        internal ShellUtilization(string id, double rbx, double rby, double rtx, double rty, double bu, bool sc, double cwb, double cwt, string resultCase)
        {
            Id = id;
            RBX = rbx;
            RBY = rby;
            RTX = rtx;
            RTY = rty;
            BU = bu;
            SC = sc;
            CWB = cwb;
            CWT = cwt;
            CaseIdentifier = resultCase;
        }

        public override string ToString()
        {
            return $"{base.ToString()}, {Id}, {CaseIdentifier}, {Max:###}";
        }

        internal static Regex IdentificationExpression
        {
            get
            {
                return new Regex(@"(?'max'Max. of load combinations, Shell, Utilization)|(?'type'Shell, Utilization), ((?'loadcasetype'[\w\s]+)? - )?Load (?'casecomb'case|comb.+): (?'casename'[ -#%'-;=?A-\ufffd]{1,79})");
            }
        }

        internal static Regex HeaderExpression
        {
            get
            {
                return new Regex(@"(?'max'Max. of load combinations, Shell, Utilization)|(?'type'Shell, Utilization), ((?'loadcasetype'[\w\s]+)? - )?Load (?'casecomb'case|comb.+): (?'casename'[ -#%'-;=?A-\ufffd]{1,79})|Shell\t|\[.*\]");
            }
        }

        internal static ShellUtilization Parse(string[] row, CsvParser reader, Dictionary<string, string> HeaderData)
        {
            if (HeaderData.ContainsKey("max"))
            {
                string id = row[0];
                double rbx = double.Parse(row[3], CultureInfo.InvariantCulture);
                double rby = double.Parse(row[4], CultureInfo.InvariantCulture);
                double rtx = double.Parse(row[5], CultureInfo.InvariantCulture);
                double rty = double.Parse(row[6], CultureInfo.InvariantCulture);
                double bu = double.Parse(row[7], CultureInfo.InvariantCulture);
                bool sc = row[8] == "OK";
                double cwb = double.Parse(row[9], CultureInfo.InvariantCulture);
                double cwt = double.Parse(row[10], CultureInfo.InvariantCulture);
                string lc = row[2];
                return new ShellUtilization(id, rbx, rby, rtx, rty, bu, sc, cwb, cwt, lc);
            }

            {
                string id = row[0];
                double rbx = double.Parse(row[2], CultureInfo.InvariantCulture);
                double rby = double.Parse(row[3], CultureInfo.InvariantCulture);
                double rtx = double.Parse(row[4], CultureInfo.InvariantCulture);
                double rty = double.Parse(row[5], CultureInfo.InvariantCulture);
                double bu = double.Parse(row[6], CultureInfo.InvariantCulture);
                bool sc = row[7] == "OK";
                double cwb = double.Parse(row[8], CultureInfo.InvariantCulture);
                double cwt = double.Parse(row[9], CultureInfo.InvariantCulture);
                string lc = HeaderData["casename"];
                return new ShellUtilization(id, rbx, rby, rtx, rty, bu, sc, cwb, cwt, lc);
            }

        }
    }
}
