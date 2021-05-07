
using System;
using System.Xml.Serialization;

#region dynamo
using Autodesk.DesignScript.Runtime;
#endregion

namespace FemDesign.Geometry
{
    [IsVisibleInDynamoLibrary(false)]
    public partial class FdPoint3d
    {
        #region dynamo
        /// <summary>
        /// Create FdPoint3d from Dynamo point.
        /// </summary>
        public static FdPoint3d FromDynamo(Autodesk.DesignScript.Geometry.Point point)
        {
            FdPoint3d newPoint = new FdPoint3d(point.X, point.Y, point.Z);
            return newPoint;
        }

        /// <summary>
        /// Create Dynamo point from FdPoint3d.
        /// </summary>
        public Autodesk.DesignScript.Geometry.Point ToDynamo()
        {
            return Autodesk.DesignScript.Geometry.Point.ByCoordinates(this.X, this.Y, this.Z);
        }

        #endregion
    }
}