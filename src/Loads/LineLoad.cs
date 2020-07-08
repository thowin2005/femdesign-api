// https://strusoft.com/

using System.Collections.Generic;
using System.Xml.Serialization;
#region dynamo
using Autodesk.DesignScript.Runtime;
#endregion

namespace FemDesign.Loads
{
    /// <summary>
    /// line_load_type
    /// </summary>
    [System.Serializable]
    [IsVisibleInDynamoLibrary(false)]
    public class LineLoad: ForceLoadBase
    {
        [XmlAttribute("load_dir")]
        public string _loadDirection; // load_dir_type
        [XmlIgnore]
        public bool constLoadDir
        {
            get
            {
                return RestrictedString.LoadDirTypeToBool(this._loadDirection);
            }
            set
            {
                this._loadDirection = RestrictedString.LoadDirTypeFromBool(value);
            }
        }
        [XmlAttribute("load_projection")]
        public bool loadProjection { get; set; } // bool
        
        // elements
        [XmlElement("edge", Order = 1)]
        public Geometry.Edge edge { get; set; } // edge_type
        [XmlElement("direction", Order = 2)]
        public Geometry.FdVector3d direction { get; set; } // point_type_3d
        [XmlElement("normal", Order = 3)]
        public Geometry.FdVector3d normal { get; set; } // point_type_3d
        [XmlElement("load", Order = 4)]
        public LoadLocationValue[] load = new LoadLocationValue[2];
        // public List<LoadLocationValue> load = new List<LoadLocationValue>(2); // sequence: location_value
        [XmlIgnore]
        public double startLoad
        {
            get
            {
                return this.load[0].val;
            }
            set
            {
                this.load[0] = new LoadLocationValue(this.edge.points[0], value);
            }
        }
        [XmlIgnore]
        public double endLoad
        {
            get
            {
                return this.load[1].val;
            }
            set
            {
                this.load[1] = new LoadLocationValue(this.edge.points[this.edge.points.Count - 1], value);
            }
        }
        [XmlIgnore]
        public Geometry.FdVector3d startForce
        {
            get
            {
                return this.direction.Scale(this.startLoad);
            }
        }
        [XmlIgnore]
        public Geometry.FdVector3d endForce
        {
            get
            {
                return this.direction.Scale(this.endLoad);
            }
        }

        /// <summary>
        /// Parameterless constructor for serialization.
        /// </summary>
        private LineLoad()
        {
            
        }

        /// <summary>
        /// Internal constructor.
        /// </summary>
        internal LineLoad(Geometry.Edge _edge, Geometry.FdVector3d f0, Geometry.FdVector3d f1, LoadCase loadCase, string comment, bool constLoadDir, bool loadProjection, string loadType)
        {
            this.EntityCreated();
            this.loadCase = loadCase.guid;
            this.comment = comment;
            this.constLoadDir = constLoadDir;
            this.loadProjection = loadProjection;
            this.loadType = loadType;
            this.edge = _edge;
            this.normal = _edge.coordinateSystem.localZ; // Note that LineLoad normal and Edge normal are not necessarily the same.
            this.SetStartAndEndForces(f0, f1);
        }

        internal void SetStartAndEndForces(Geometry.FdVector3d startForce, Geometry.FdVector3d endForce)
        {
            Geometry.FdVector3d v0 = startForce.Normalize();
            Geometry.FdVector3d v1 = endForce.Normalize();
            int par = v0.Parallel(v1);
            if (par != 0)
            {
                this.direction = v0;
                this.startLoad = startForce.Length();
                this.endLoad = par * endForce.Length();
            }
            else
            {
                throw new System.ArgumentException($"Forces must be parallel or antiparallel.");
            }
        }

        #region dynamo
        /// <summary>
        /// Create a uniform or non-uniform force line load.
        /// </summary>
        /// <remarks>Create</remarks>
        /// <param name="curve">Curve defining the line load.</param>
        /// <param name="startForce">Force at start.</param>
        /// <param name="endForce">Force at end. Optional. If undefined line load will be uniform with a force of StartForce.</param>
        /// <param name="loadCase">LoadCase.</param>
        /// <param name="constLoadDir">Bool. Constant load direction? If true direction of load will be constant along action line. If false direction will vary along action line - characteristic direction is in the middle point of line. Optional.</param>
        /// <param name="comment">Comment.</param>
        [IsVisibleInDynamoLibrary(true)]
        public static LineLoad Force(Autodesk.DesignScript.Geometry.Curve curve, Autodesk.DesignScript.Geometry.Vector startForce, [DefaultArgument("DynamoNullArgument.GetNull()")] Autodesk.DesignScript.Geometry.Vector endForce, LoadCase loadCase, [DefaultArgument("true")] bool constLoadDir, string comment = "")
        {
            Geometry.Edge edge = Geometry.Edge.FromDynamoLineOrArc1(curve);
            Geometry.FdVector3d _startForce = Geometry.FdVector3d.FromDynamo(startForce);
            Geometry.FdVector3d _endForce;
            if (endForce == null)
            {
                _endForce = _startForce;
            }
            else
            {
                _endForce = Geometry.FdVector3d.FromDynamo(endForce);
            }
            return new LineLoad(edge, _startForce, _endForce, loadCase, comment, constLoadDir, false, "force");
        }

        /// <summary>
        /// Create a uniform or non-uniform moment line load.
        /// </summary>
        /// <remarks>Create</remarks>
        /// <param name="curve">Curve defining the line load.</param>
        /// <param name="startForce">Force (moment) at start.</param>
        /// <param name="endForce">Force (moment) at end. Optional. If undefined line load will be uniform with a force (moment) of StartForce.</param>
        /// <param name="loadCase">LoadCase.</param>
        /// <param name="constLoadDir">Bool. Constant load direction? If true direction of load will be constant along action line. If false direction will vary along action line - characteristic direction is in the middle point of line. Optional.</param>
        /// <param name="comment">Comment.</param>
        [IsVisibleInDynamoLibrary(true)]
        public static LineLoad Moment(Autodesk.DesignScript.Geometry.Curve curve, Autodesk.DesignScript.Geometry.Vector startForce, [DefaultArgument("DynamoNullArgument.GetNull()")] Autodesk.DesignScript.Geometry.Vector endForce, LoadCase loadCase, [DefaultArgument("true")] bool constLoadDir, string comment = "")
        {
            Geometry.Edge edge = Geometry.Edge.FromDynamoLineOrArc1(curve);
            Geometry.FdVector3d _startForce = Geometry.FdVector3d.FromDynamo(startForce);
            Geometry.FdVector3d _endForce;
            if (endForce == null)
            {
                _endForce = _startForce;
            }
            else
            {
                _endForce = Geometry.FdVector3d.FromDynamo(endForce);
            }
            return new LineLoad(edge, _startForce, _endForce, loadCase, comment, constLoadDir, false, "moment");
        }

        /// <summary>
        /// Convert LineLoad edge to Dynamo curve.
        /// </summary>
        internal Autodesk.DesignScript.Geometry.Curve GetDynamoGeometry()
        {
            return this.edge.ToDynamo();
        }

        #endregion
        
        #region grasshopper
        /// <summary>
        /// Convert LineLoad edge to Rhino curve.
        /// </summary>
        internal Rhino.Geometry.Curve GetRhinoGeometry()
        {
            return this.edge.ToRhino();
        }

        #endregion
    }
}