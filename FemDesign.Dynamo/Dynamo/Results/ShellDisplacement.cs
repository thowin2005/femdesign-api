﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.DesignScript.Runtime;


namespace FemDesign.Results
{
    [IsVisibleInDynamoLibrary(false)]
    public partial class ShellDisplacement : IResult
    {
        /// <summary>
        /// Deconstruct the Shell Displacement Results
        /// </summary>
        /// <param name="Result">Result to be Parse</param>
        /// <param name="LoadCase">Name of Load Case for which to return the results. Default value returns the displacement for the first load case</param>
        [IsVisibleInDynamoLibrary(true)]
        [MultiReturn(new[] { "CaseIdentifier", "ElementId", "NodeId", "Translation", "Rotation" })]
        public static Dictionary<string, object> Deconstruct(List<FemDesign.Results.ShellDisplacement> Result, [DefaultArgument("null")] string LoadCase)
        {
            // Read Result from Abstract Method
            Dictionary<string, object> result;

            try
            {
                result = FemDesign.Results.ShellDisplacement.DeconstructShellDisplacement(Result, LoadCase);
            }
            catch (ArgumentException ex)
            {
                throw new Exception(ex.Message);
            }

            // Extract Results from the Dictionary
            var loadCases = (List<string>)result["CaseIdentifier"];
            var elementId = (List<int>)result["ElementId"];
            var nodeId = (List<string>)result["NodeId"];
            var iTranslation = (List<FemDesign.Geometry.FdVector3d>)result["Translation"];
            var iRotation = (List<FemDesign.Geometry.FdVector3d>)result["Rotation"];

            // Convert the FdVector to Dynamo
            var oTranslation = iTranslation.Select(x => x.ToDynamo());
            var oRotation = iRotation.Select(x => x.ToDynamo());

            var uniqueLoadCase = loadCases.Distinct().ToList();
            var uniqueId = elementId.Distinct().ToList();


            // Convert Data in DataTree structure
            var elementIdTree = new List<int>();
            var nodeIdTree = new List<List<string>>();
            var oTranslationTree = new List<List<Autodesk.DesignScript.Geometry.Vector>>();
            var oRotationTree = new List<List<Autodesk.DesignScript.Geometry.Vector>>();


            foreach (var id in uniqueId)
            {
                // temporary List to append
                var nodeIdTreeTemp = new List<string>();
                var oTranslationTreeTemp = new List<Autodesk.DesignScript.Geometry.Vector>();
                var oRotationTreeTemp = new List<Autodesk.DesignScript.Geometry.Vector>();

                // indexes where the uniqueId matches in the list
                var indexes = elementId.Select((value, index) => new { value, index })
                  .Where(a => string.Equals(a.value, id))
                  .Select(a => a.index);


                foreach (int index in indexes)
                {
                    //loadCasesTree.Add(loadCases.ElementAt(index), new GH_Path(i));
                    //elementIdTree.Add(elementId.ElementAt(index), new GH_Path(ghPath, i));
                    nodeIdTreeTemp.Add(nodeId.ElementAt(index));
                    oTranslationTreeTemp.Add(oTranslation.ElementAt(index));
                    oRotationTreeTemp.Add(oRotation.ElementAt(index));
                }

                elementIdTree.Add(id);
                nodeIdTree.Add(nodeIdTreeTemp);
                oTranslationTree.Add(oTranslationTreeTemp);
                oRotationTree.Add(oRotationTreeTemp);
            }

            return new Dictionary<string, dynamic>
            {
                {"CaseIdentifier", uniqueLoadCase},
                {"ElementId", elementIdTree},
                {"NodeId",nodeIdTree},
                {"Translation", oTranslationTree},
                {"Rotation", oRotationTree}
            };
        }
    }
}