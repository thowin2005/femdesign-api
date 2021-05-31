﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;

namespace FemDesign.Results
{
    /// <summary>
    /// Reads FEM-Design results from list tables text files.
    /// </summary>
    public class ResultsReader : CsvReader
    {
        private Regex HeaderExpression = new Regex(@"(?'type'[\w\ ]+), (?'result'[\w\ ]+), (?'loadcasetype'[\w\ ]+) - Load (?'casecomb'[\w\ ]+): (?'casename'[\w\ ]+)|(ID)|(\[.*\])");
        public ResultsReader(string filePath) : base(filePath, delimiter: '\t')
        {

        }

        private PointSupportReaction parsePointSupportReaction(string[] row, CsvReader reader)
        {
            string supportname = row[0];
            double x = Double.Parse(row[1], CultureInfo.InvariantCulture);
            double y = Double.Parse(row[2], CultureInfo.InvariantCulture);
            double z = Double.Parse(row[3], CultureInfo.InvariantCulture);
            int nodeId = int.Parse(row[4], CultureInfo.InvariantCulture);
            double fx = Double.Parse(row[5], CultureInfo.InvariantCulture);
            double fy = Double.Parse(row[6], CultureInfo.InvariantCulture);
            double fz = Double.Parse(row[7], CultureInfo.InvariantCulture);
            double mx = Double.Parse(row[8], CultureInfo.InvariantCulture);
            double my = Double.Parse(row[9], CultureInfo.InvariantCulture);
            double mz = Double.Parse(row[10], CultureInfo.InvariantCulture);
            double fr = Double.Parse(row[11], CultureInfo.InvariantCulture);
            double mr = Double.Parse(row[12], CultureInfo.InvariantCulture);
            string lc = HeaderData["casename"];
            return new PointSupportReaction(supportname, x, y, z, nodeId, fx, fy, fz, mx, my, mz, fr, mr, lc);
        }

        private LineSupportReaction parseLineSupportReaction(string[] row, CsvReader reader)
        {
            string supportname = row[0];
            int elementId = int.Parse(row[1], CultureInfo.InvariantCulture);
            int nodeId = int.Parse(row[2], CultureInfo.InvariantCulture);
            double fx = Double.Parse(row[3], CultureInfo.InvariantCulture);
            double fy = Double.Parse(row[4], CultureInfo.InvariantCulture);
            double fz = Double.Parse(row[5], CultureInfo.InvariantCulture);
            double mx = Double.Parse(row[6], CultureInfo.InvariantCulture);
            double my = Double.Parse(row[7], CultureInfo.InvariantCulture);
            double mz = Double.Parse(row[8], CultureInfo.InvariantCulture);
            double fr = Double.Parse(row[9], CultureInfo.InvariantCulture);
            double mr = Double.Parse(row[10], CultureInfo.InvariantCulture);
            string lc = HeaderData["casename"];
            return new LineSupportReaction(supportname, elementId, nodeId, fx, fy, fz, mx, my, mz, fr, mr, lc);
        }

        private bool parseHeaderDefault(string line, CsvReader reader)
        {
            var match = HeaderExpression.Match(line);

            if (match.Success)
            {
                foreach (Group group in match.Groups)
                {
                    if (!string.IsNullOrEmpty(group.Value))
                        HeaderData[group.Name] = group.Value;
                }
            }

            return HeaderExpression.IsMatch(line);
        }

        protected sealed override void BeforeParse(Type type)
        {
            HeaderParser = parseHeaderDefault;

            if (type == typeof(Results.PointSupportReaction))
                RowParser = parsePointSupportReaction;
            else if (type == typeof(Results.PointSupportReaction))
                RowParser = parseLineSupportReaction;
            else
                throw new NotImplementedException($"Parser for {type} has not yet been implemented.");

            base.BeforeParse(type);
        }
    }

    /// <summary>
    /// CSV reader. Reads a comma (or other char) delimited file, line by line and parses each line to a new object. Header lines can be handled with the HeaderParser. 
    /// </summary>
    public class CsvReader
    {
        public StreamReader File;
        private char Delimiter;
        /// <summary>
        /// Returns a boolean representing wether the current line is a Header.
        /// </summary>
        protected Func<string, CsvReader, bool> HeaderParser;
        /// <summary>
        /// Parses a split line to a new object. Applied on every line not a header.
        /// </summary>
        protected Func<string[], CsvReader, object> RowParser;
        /// <summary>
        /// Last header row read by the header parser.
        /// </summary>
        internal string Header;
        internal Dictionary<string, string> HeaderData = new Dictionary<string, string>();
        public bool IsDone { get { return File.Peek() == -1; } }

        protected CsvReader(string filePath, char delimiter = ',', Func<string[], CsvReader, object> rowParser = null, Func<string, CsvReader, bool> headerParser = null)
        {
            File = new System.IO.StreamReader(filePath);
            Delimiter = delimiter;
            RowParser = rowParser;
            HeaderParser = headerParser;
        }

        protected string[] ReadRow()
        {
            string line = File.ReadLine();
            bool isHeader = false;
            if (HeaderParser != null)
            {
                isHeader = HeaderParser(line, this);
                if (isHeader)
                    Header = line;
            }
            return isHeader || string.IsNullOrEmpty(line) ? null : line.Split(new char[] { Delimiter });
        }

        protected T ParseRow<T>()
        {
            var row = ReadRow();
            return row == null ? default(T) : (T)RowParser(row, this);
        }

        public List<T> ParseAll<T>(bool skipNull = true)
        {
            BeforeParse(typeof(T));

            List<T> results = new List<T>();
            while (!this.IsDone)
            {
                T parsed = ParseRow<T>();
                if (skipNull && parsed == null)
                    continue;
                results.Add(parsed);
            }
            return results;
        }

        protected virtual void BeforeParse(Type type)
        {
            if (RowParser == null)
                throw new ApplicationException("Row parser was not initialized properly.");
            if (HeaderParser == null)
                throw new ApplicationException("Header parser was not initialized properly.");
        }
    }
}
