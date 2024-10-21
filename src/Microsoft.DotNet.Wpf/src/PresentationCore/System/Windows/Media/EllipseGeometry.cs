// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MS.Internal;
using System.Windows.Media.Composition;

using SR = MS.Internal.PresentationCore.SR;

namespace System.Windows.Media 
{
    /// <summary>
    /// This is the Geometry class for Circles and Ellipses 
    /// </summary>
    public sealed partial class EllipseGeometry : Geometry 
    {
        #region Constructors

        /// <summary>
        ///
        /// </summary>
        public EllipseGeometry() { }

        /// <summary>
        /// Constructor - sets the ellipse to the paramters with the given transformation
        /// </summary>
        public EllipseGeometry(Rect rect)
        {
            if (rect.IsEmpty) 
            {
                throw new ArgumentException(SR.Format(SR.Rect_Empty, nameof(rect)));
            }

            RadiusX = (rect.Right - rect.X) * (1.0 / 2.0);
            RadiusY = (rect.Bottom - rect.Y) * (1.0 / 2.0);
            Center = new Point(rect.X + RadiusX, rect.Y + RadiusY);
        }
        
        /// <summary>
        /// Constructor - sets the ellipse to the parameters
        /// </summary>
        public EllipseGeometry(Point center, double radiusX, double radiusY)
        {
            Center = center;
            RadiusX = radiusX;
            RadiusY = radiusY;
        }
        
        /// <summary>
        /// Constructor - sets the ellipse to the parameters
        /// </summary>
        public EllipseGeometry(Point center, double radiusX, double radiusY, Transform transform) : this(center, radiusX, radiusY)
        {
            Transform = transform;
        }

        #endregion

        /// <summary>
        /// Gets the bounds of this Geometry as an axis-aligned bounding box
        /// </summary>
        public override Rect Bounds
        {
            get
            {
                ReadPreamble();

                Rect boundsRect;

                Transform transform = Transform;

                if (transform == null || transform.IsIdentity) 
                {
                    Point currentCenter = Center;
                    double currentRadiusX = RadiusX;
                    double currentRadiusY = RadiusY;

                    boundsRect = new Rect(currentCenter.X - Math.Abs(currentRadiusX),
                                          currentCenter.Y - Math.Abs(currentRadiusY),
                                          2.0 * Math.Abs(currentRadiusX),
                                          2.0 * Math.Abs(currentRadiusY));
                }
                else
                {
                    //
                    // If at sometime in the
                    // future this code gets exercised enough, we can
                    // handle the general case in managed code. Until then,
                    // it's easier to let unmanaged code do the work for us.
                    //

                    Transform.GetTransformValue(transform, out Matrix geometryMatrix);

                    boundsRect = EllipseGeometry.GetBoundsHelper(
                        null /* no pen */,
                        Matrix.Identity,
                        Center,
                        RadiusX,
                        RadiusY,
                        geometryMatrix,
                        StandardFlatteningTolerance, 
                        ToleranceType.Absolute);
                }

                return boundsRect;
            }
}

        /// <summary>
        /// Returns the axis-aligned bounding rectangle when stroked with a pen, after applying
        /// the supplied transform (if non-null).
        /// </summary>
        internal override Rect GetBoundsInternal(Pen pen, Matrix matrix, double tolerance, ToleranceType type)
        {
            Transform.GetTransformValue(Transform, out Matrix geometryMatrix);

            return EllipseGeometry.GetBoundsHelper(pen, matrix, Center, RadiusX, RadiusY, geometryMatrix, tolerance, type);
        }
        
        internal static unsafe Rect GetBoundsHelper(Pen pen, Matrix worldMatrix, Point center, double radiusX, double radiusY,
                                                    Matrix geometryMatrix, double tolerance, ToleranceType type)
        {
            if ((pen == null || pen.DoesNotContainGaps) && worldMatrix.IsIdentity && geometryMatrix.IsIdentity)
            {
                double strokeThickness = Pen.ContributesToBounds(pen) ? Math.Abs(pen.Thickness) : 0.0;

                return new Rect(center.X - Math.Abs(radiusX) - 0.5 * strokeThickness,
                                center.Y - Math.Abs(radiusY) - 0.5 * strokeThickness,
                                2.0 * Math.Abs(radiusX) + strokeThickness,
                                2.0 * Math.Abs(radiusY) + strokeThickness);
            }

            Point* ptrPoints = stackalloc Point[(int)PointCount];
            EllipseGeometry.InitializePointList(ptrPoints, (int)PointCount, center, radiusX, radiusY);

            fixed (byte* ptrTypes = RoundedPathTypes) // Merely retrieves the pointer to static PE data, no actual pinning occurs
            {
                return Geometry.GetBoundsHelper(pen, &worldMatrix, ptrPoints, ptrTypes, PointCount, SegmentCount, &geometryMatrix,
                                                tolerance, type, false); // skip hollows - meaningless here, this is never a hollow
            }
        }

        internal override bool ContainsInternal(Pen pen, Point hitPoint, double tolerance, ToleranceType type)
        {
            unsafe
            {
                Point* ptrPoints = stackalloc Point[(int)PointCount];
                EllipseGeometry.InitializePointList(ptrPoints, (int)PointCount, Center, RadiusX, RadiusY);

                fixed (byte* ptrTypes = RoundedPathTypes) // Merely retrieves the pointer to static PE data, no actual pinning occurs
                {
                    return ContainsInternal(pen, hitPoint, tolerance, type, ptrPoints, PointCount, ptrTypes, SegmentCount);
                }
            }
        }

        #region Public Methods

        /// <summary>
        /// Returns true if this geometry is empty
        /// </summary>
        public override bool IsEmpty()
        {
            return false;
        }

        /// <summary>
        /// Returns true if this geometry may have curved segments
        /// </summary>
        public override bool MayHaveCurves()
        {
            return true;
        }

        /// <summary>
        /// Gets the area of this geometry
        /// </summary>
        /// <param name="tolerance">The computational error tolerance</param>
        /// <param name="type">The way the error tolerance will be interpreted - realtive or absolute</param>
        public override double GetArea(double tolerance, ToleranceType type)
        {
            ReadPreamble();

            double area = Math.Abs(RadiusX * RadiusY) * Math.PI;

            // Adjust to internal transformation
            Transform transform = Transform;
            if (transform != null && !transform.IsIdentity)
            {
                area *= Math.Abs(transform.Value.Determinant);
            }

            return area;
        }

        #endregion Public Methods

        internal override PathFigureCollection GetTransformedFigureCollection(Transform transform)
        {
            // Initialize the point list
            Span<Point> points = stackalloc Point[(int)PointCount];
            InitializePointList(points);

            // Get the combined transform argument with the internal transform
            Matrix matrix = GetCombinedMatrix(transform);
            if (!matrix.IsIdentity)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    points[i] *= matrix;
                }
            }

            return new PathFigureCollection(1) { new PathFigure(points[0], [new BezierSegment(points[1], points[2], points[3], true, true),
                                                                            new BezierSegment(points[4], points[5], points[6], true, true),
                                                                            new BezierSegment(points[7], points[8], points[9], true, true),
                                                                            new BezierSegment(points[10], points[11], points[12], true, true)],
                                                                            closed: true) };
        }

        /// <summary>
        /// GetAsPathGeometry - return a PathGeometry version of this Geometry
        /// </summary>
        internal override PathGeometry GetAsPathGeometry()
        {
            PathStreamGeometryContext ctx = new(FillRule.EvenOdd, Transform);
            PathGeometry.ParsePathGeometryData(GetPathGeometryData(), ctx);

            return ctx.GetPathGeometry();
        }

        /// <summary>
        /// GetPathGeometryData - returns a byte[] which contains this Geometry represented
        /// as a path geometry's serialized format.
        /// </summary>
        internal override PathGeometryData GetPathGeometryData()
        {
            if (IsObviouslyEmpty())
            {
                return Geometry.GetEmptyPathGeometryData();
            }

            PathGeometryData data = new() { FillRule = FillRule.EvenOdd, Matrix = CompositionResourceManager.TransformToMilMatrix3x2D(Transform) };

            // Initialize the point list
            Span<Point> points = stackalloc Point[(int)PointCount];
            InitializePointList(points);

            ByteStreamGeometryContext ctx = new();
            ctx.BeginFigure(points[0], true /* is filled */, true /* is closed */);

            // i == 0, 3, 6, 9
            for (int i = 0; i < 12; i += 3)
            {
                ctx.BezierTo(points[i + 1], points[i + 2], points[i + 3], true /* is stroked */, true /* is smooth join */);
            }

            ctx.Close();
            data.SerializedData = ctx.GetData();

            return data;
        }

        /// <summary>
        /// Initializes the point list into <paramref name="destination"/>. Optionally pins the source if not stack-allocated.
        /// </summary>
        private unsafe void InitializePointList(Span<Point> destination)
        {
            fixed (Point* ptrPoints = destination) // In case this is stackallocated, it's a no-op
            {
                EllipseGeometry.InitializePointList(ptrPoints, destination.Length, Center, RadiusX, RadiusY);
            }
        }

        /// <summary>
        /// Initializes the point list specified by <paramref name="points"/>. The pointer must be pinned.
        /// </summary>
        private unsafe static void InitializePointList(Point* points, int pointsCount, Point center, double radiusX, double radiusY)
        {
            Invariant.Assert((uint)pointsCount >= PointCount);

            radiusX = Math.Abs(radiusX);
            radiusY = Math.Abs(radiusY);
              
            // Set the X coordinates
            double mid = radiusX * ArcAsBezier;

            points[0].X = points[1].X = points[11].X = points[12].X = center.X + radiusX;
            points[2].X = points[10].X = center.X + mid;
            points[3].X = points[9].X = center.X;
            points[4].X = points[8].X = center.X - mid;
            points[5].X = points[6].X = points[7].X = center.X - radiusX;

            // Set the Y coordinates
            mid = radiusY * ArcAsBezier;

            points[2].Y = points[3].Y = points[4].Y = center.Y + radiusY;
            points[1].Y = points[5].Y = center.Y + mid;
            points[0].Y = points[6].Y = points[12].Y = center.Y;
            points[7].Y = points[11].Y = center.Y - mid;
            points[8].Y = points[9].Y = points[10].Y = center.Y - radiusY;
        }
        
        #region Static Data
        
        // Approximating a 1/4 circle with a Bezier curve                _
        internal const double ArcAsBezier = 0.5522847498307933984; // =( \/2 - 1)*4/3

        private const uint SegmentCount = 4;
        private const uint PointCount = 13;

        private const byte SmoothBezier = (byte)MILCoreSegFlags.SegTypeBezier  |
                                          (byte)MILCoreSegFlags.SegIsCurved    |
                                          (byte)MILCoreSegFlags.SegSmoothJoin;

        private static ReadOnlySpan<byte> RoundedPathTypes => [(byte)MILCoreSegFlags.SegTypeBezier | 
                                                               (byte)MILCoreSegFlags.SegIsCurved   |
                                                               (byte)MILCoreSegFlags.SegSmoothJoin | 
                                                               (byte)MILCoreSegFlags.SegClosed,
                                                               SmoothBezier,
                                                               SmoothBezier,
                                                               SmoothBezier];

        #endregion
    }
}

