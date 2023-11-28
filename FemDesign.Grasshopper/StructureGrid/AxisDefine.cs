﻿// https://strusoft.com/
using System;
using Grasshopper.Kernel;

namespace FemDesign.Grasshopper
{
    public class AxisDefine : FEM_Design_API_Component
    {
        public AxisDefine() : base("Axis.Define", "Define", "Define an Axis.", "FEM-Design", "ModellingTools")
        {

        }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Line", "Line", "Line of axis. Line will be projected onto XY-plane.", GH_ParamAccess.item);
            pManager.AddGenericParameter("Id", "Id", "Number or letters of axis identifier.", GH_ParamAccess.item);
            pManager.AddTextParameter("Prefix", "Prefix", "Prefix of axis identifier.", GH_ParamAccess.item, "");
            pManager[pManager.ParamCount - 1].Optional = true;

        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Axis", "Axis", "Axis.", GH_ParamAccess.item);
        }
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // get data
            Rhino.Geometry.Curve line = null;
            if (!DA.GetData(0, ref line))
            {
                return;
            }

            dynamic id = 0;
            if (!DA.GetData(1, ref id))
            {
                return;
            }

            string prefix = "";
            DA.GetData(2, ref prefix);

            // convert geometry
            if (line.GetType() != typeof(Rhino.Geometry.LineCurve))
            {
                throw new System.ArgumentException("Curve must be a LineCurve");
            }
            FemDesign.Geometry.Point3d p0 = Convert.FromRhino(line.PointAtStart);
            FemDesign.Geometry.Point3d p1 = Convert.FromRhino(line.PointAtEnd);

            //


            if (int.TryParse( id.Value, out int _number))
            {
                FemDesign.StructureGrid.Axis obj = new StructureGrid.Axis(p0, p1, _number, prefix);
                DA.SetData(0, obj);
            }
            else
            {
                FemDesign.StructureGrid.Axis obj = new StructureGrid.Axis(p0, p1, id.Value, prefix);
                DA.SetData(0, obj);
            }

        }
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return FemDesign.Properties.Resources.AxisDefine;
            }
        }
        public override Guid ComponentGuid
        {
            get { return new Guid("{FC66D368-4649-4C43-ABDE-AA85D9AD251A}"); }
        }
        public override GH_Exposure Exposure => GH_Exposure.quarternary;

    }
}