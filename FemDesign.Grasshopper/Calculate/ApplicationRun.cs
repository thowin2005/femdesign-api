﻿// https://strusoft.com/
using System;
using System.Collections.Generic;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Remoting.Messaging;
using FemDesign.Loads;
using FemDesign.Calculate;
using FemDesign.Results;
using System.Data.Common;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using FemDesign.Grasshopper.Extension.ComponentExtension;

namespace FemDesign.Grasshopper
{
    public class ApplicationRun : FEM_Design_API_Component
    {
        public ApplicationRun() : base("Application.RunCalculation", "RunCalculation", "Run calculation for a model. .csv list files and .docx documentation files are saved in the same work directory as StruxmlPath.", CategoryName.Name(), SubCategoryName.Cat7a())
        {
            _minimised = false;
        }

        public bool _minimised { get; set; }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Model", "Model", "Model to open.", GH_ParamAccess.item);
            pManager.AddGenericParameter("Analysis", "Analysis", "Analysis.", GH_ParamAccess.item);
            pManager[pManager.ParamCount - 1].Optional = true;
            pManager.AddGenericParameter("Design", "Design", "Design.", GH_ParamAccess.item);
            pManager[pManager.ParamCount - 1].Optional = true;

            pManager.AddGenericParameter("DesignGroups", "DesignGroups", "DesignGroups.", GH_ParamAccess.list);
            pManager[pManager.ParamCount - 1].Optional = true;

            pManager.AddTextParameter("ResultTypes", "ResultTypes", "Results to be extracted from model. This might require the model to have been analysed. Item or list.", GH_ParamAccess.list);
            pManager[pManager.ParamCount - 1].Optional = true;

            pManager.AddGenericParameter("Units", "Units", "Specify the Result Units for some specific type. \n" +
                "Default Units are: Length.m, Angle.deg, SectionalData.m, Force.kN, Mass.kg, Displacement.m, Stress.Pa", GH_ParamAccess.item);
            pManager[pManager.ParamCount - 1].Optional = true;

            pManager.AddTextParameter("Cfg", "Cfg", "Cfg file path with design parameters for structural materials. \nYou can use the 'cfg.xml' file in located package manager library folder as a starting point..\n%AppData%\\McNeel\\Rhinoceros\\packages\\7.0\\FemDesign\\", GH_ParamAccess.item);
            pManager[pManager.ParamCount - 1].Optional = true;

            pManager.AddTextParameter("GlobalCfg", "GlobalCfg", "GlobalCfg file path. You can use the 'cmdglobalcfg.xml' file in located package manager library folder as a starting point.\n%AppData%\\McNeel\\Rhinoceros\\packages\\7.0\\FemDesign\\", GH_ParamAccess.item);
            pManager[pManager.ParamCount - 1].Optional = true;

            pManager.AddTextParameter("DocxTemplatePath", "DocxTemplatePath", "File path to documentation template file (.dsc) to run. Optional parameter.", GH_ParamAccess.item);
            pManager[pManager.ParamCount - 1].Optional = true;

            pManager.AddBooleanParameter("RunNode", "RunNode", "If true node will execute. If false node will not execute.", GH_ParamAccess.item, true);
            pManager[pManager.ParamCount - 1].Optional = true;

        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Model", "Model", "Model.", GH_ParamAccess.item);
            pManager.Register_GenericParam("FiniteElement", "FiniteElement", "FemDesign Finite Element Geometries(nodes, bars, shells).");
            pManager.AddGenericParameter("Results", "Results", "Results.", GH_ParamAccess.tree);
        }


        public dynamic _getResults(FemDesignConnection connection, Type resultType, Results.UnitResults units = null, Options options = null, List<FemDesign.GenericClasses.IStructureElement> elements = null)
        {
            var method = nameof(FemDesign.FemDesignConnection.GetResults);
            List<Results.IResult> mixedResults = new List<Results.IResult>();
            MethodInfo genericMethod = typeof(FemDesign.FemDesignConnection).GetMethod(method).MakeGenericMethod(resultType);
            dynamic result = genericMethod.Invoke(connection, new object[] { units, options, elements });
            mixedResults.AddRange(result);
            return mixedResults;
        }


        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            // Append the item to the menu, making sure it's always enabled and checked if Absolute is True.
            ToolStripMenuItem item = Menu_AppendItem(menu, "Minimise FEM-Design", Menu_AbsoluteClicked, null, true, _minimised);
        }

        private void Menu_AbsoluteClicked(object sender, EventArgs e)
        {
            _minimised = !_minimised;
            ExpireSolution(true);
        }


 

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool runNode = true;
            DA.GetData("RunNode", ref runNode);

            if (runNode == false)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "RunNode set to false!");
                return;
            }

            dynamic _model = null;
            DA.GetData("Model", ref _model);

            FemDesign.Calculate.Analysis analysis = null;
            DA.GetData("Analysis", ref analysis);

            FemDesign.Calculate.Design design = null;
            DA.GetData("Design", ref design);

            if(analysis == null && design == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Specify 'Analysis' or 'Design' to run the calculation!");
                return;
            }


            FemDesign.Results.UnitResults units = UnitResults.Default();
            DA.GetData("Units", ref units);

            List<FemDesign.Calculate.CmdDesignGroup> designGroups = new List<Calculate.CmdDesignGroup>();
            DA.GetDataList("DesignGroups", designGroups);

            List<string> _resultType = new List<string>();
            DA.GetDataList("ResultTypes", _resultType);

            string cfg = null;
            DA.GetData("Cfg", ref cfg);

            string globalCfg = null;
            DA.GetData("GlobalCfg", ref globalCfg);

            // Collect Outputs
            Model model = null;
            List<Results.IResult> results = new List<Results.IResult>();
            FemDesign.Results.FiniteElement finiteElement = null;
            var resultsTree = new DataTree<object>();


            var types = new List<Type>();
            foreach (var resType in _resultType)
            {
                var _type = $"FemDesign.Results.{resType}, FemDesign.Core";
                Type type = Type.GetType(_type);
                types.Add(type);
            }


            // check if .gh file exist to store the .fdscript/.csv/.struxml file in the same folder
            bool fileExist = OnPingDocument().IsFilePathDefined;
            if (!fileExist)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Save your .gh script or specfy a FilePath.");
                return;
            }
            string filePath = OnPingDocument().FilePath;
            var outputDir = System.IO.Path.GetDirectoryName(filePath);



            // Create Task
            var t = Task.Run((Action)(() =>
            {
                var connection = new FemDesign.FemDesignConnection(minimized: _minimised, outputDir: outputDir);

                connection.Open(_model.Value);

                if (cfg != null)
                    connection.SetConfig( new Calculate.CmdConfig(cfg) );

                if (globalCfg != null)
                    connection.SetGlobalConfig(CmdGlobalCfg.DeserializeCmdGlobalCfgFromFilePath(globalCfg));

                if (analysis != null)
                    connection.RunAnalysis(analysis);


                // run design

                if (design != null)
                {
                    CmdUserModule userModule = design.Mode;
                    if (designGroups.Count() == 0)
                        connection.RunDesign(userModule, design);
                    else
                        connection.RunDesign(userModule, design, designGroups);

                    if (design.ApplyChanges == true)
                    {
                        connection.ApplyDesignChanges();
                    }

                    if(design.ApplyChanges == true && design.Check == true)
                    {
                        connection.RunAnalysis(analysis);
                        var _design = new Design(check: true);

                        connection.RunDesign(userModule, _design);
                    }
                }


                finiteElement = connection.GetFeaModel(units.Length);

                if(types.Count != 0)
                {
                    int i = 0;
                    foreach (var type in types)
                    {
                        var res = _getResults(connection, type, units);
                        resultsTree.AddRange(res, new GH_Path(i));
                        i++;
                    }
                }

                // return the new model
                model = connection.GetModel();
                

                connection.Dispose();
            }));

            
            t.ConfigureAwait(false);
            try
            {
                t.Wait();
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }

            // Set output
            DA.SetData("Model", model);
            DA.SetData("FiniteElement", finiteElement);
            DA.SetDataTree(2, resultsTree);
        }
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return FemDesign.Properties.Resources.ModelRunAnalysis;
            }
        }
        public override Guid ComponentGuid
        {
            get { return new Guid("{81ECF200-1217-4CE8-B2F3-9C35CC49D3CD}"); }
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

    }
}
