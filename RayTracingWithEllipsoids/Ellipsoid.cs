using System;


namespace rt
{
    public class Ellipsoid : Geometry
    {
        private Vector Center { get; }
        private Vector SemiAxesLength { get; }
        private double Radius { get; }
        
        
        public Ellipsoid(Vector center, Vector semiAxesLength, double radius, Material material, Color color) : base(material, color)
        {
            Center = center;
            SemiAxesLength = semiAxesLength;
            Radius = radius;
        }

        public Ellipsoid(Vector center, Vector semiAxesLength, double radius, Color color) : base(color)
        {
            Center = center;
            SemiAxesLength = semiAxesLength;
            Radius = radius;
        }

        public override Intersection GetIntersection(Line ray, double minDist, double maxDist)
        {
            //TODO
            // Translate the ray into ellipsoid's local coordinate system
            Vector localRayOrigin = ray.X0 - Center;
            Vector localRayDirection = ray.Dx;

            // Calculate coefficients of the intersection equation
            double a = SemiAxesLength.X * SemiAxesLength.X;
            double b = SemiAxesLength.Y * SemiAxesLength.Y;
            double c = SemiAxesLength.Z * SemiAxesLength.Z;

            double dx = localRayDirection.X;
            double dy = localRayDirection.Y;
            double dz = localRayDirection.Z;

            double A = a * dx * dx + b * dy * dy + c * dz * dz;
            double B = 2 * (a * localRayOrigin.X * dx + b * localRayOrigin.Y * dy + c * localRayOrigin.Z * dz);
            double C = a * localRayOrigin.X * localRayOrigin.X + b * localRayOrigin.Y * localRayOrigin.Y + c * localRayOrigin.Z * localRayOrigin.Z - Radius * Radius;

            // Solve the quadratic equation
            double discriminant = B * B - 4 * A * C;

            if (discriminant < 0)
            {
                // No real intersections
                return new Intersection();
            }

            // Calculate the t values for intersections
            double t1 = (-B - Math.Sqrt(discriminant)) / (2 * A);
            double t2 = (-B + Math.Sqrt(discriminant)) / (2 * A);

            Intersection intersection1 = null;
            Intersection intersection2 = null;

            if (t1 >= minDist && t1 <= maxDist)
            {
                // t1 is a valid intersection
                Vector intersectionPoint = localRayOrigin + localRayDirection * t1;
                Vector normal = Normal(intersectionPoint);

                intersection1 = new Intersection(true, true, this, ray, t1, intersectionPoint);
            }

            if (t2 >= minDist && t2 <= maxDist)
            {
                // t2 is a valid intersection
                Vector intersectionPoint = localRayOrigin + localRayDirection * t2;
                Vector normal = Normal(intersectionPoint);

                intersection2 = new Intersection(true, true, this, ray, t2, intersectionPoint);
            }

            if (intersection1 != null && intersection2 != null)
            {
                // Both intersections are valid; return the one closest to the ray origin
                if (intersection1.T < intersection2.T)
                {
                    return intersection1;
                }
                else
                {
                    return intersection2;
                }
            }
            else if (intersection1 != null)
            {
                return intersection1;
            }
            else if (intersection2 != null)
            {
                return intersection2;
            }
            else
            {
                // No valid intersections within the specified range
                return new Intersection();
            }
        }
        public Vector Normal(Vector v)
        { 
            //todo implement the Normal function
            // Calculate the normalized gradient of the ellipsoid's surface equation
            double a2 = SemiAxesLength.X * SemiAxesLength.X;
            double b2 = SemiAxesLength.Y * SemiAxesLength.Y;
            double c2 = SemiAxesLength.Z * SemiAxesLength.Z;

            double x = v.X;
            double y = v.Y;
            double z = v.Z;

            double nx = 2 * x / a2;
            double ny = 2 * y / b2;
            double nz = 2 * z / c2;

            Vector normal = new Vector(nx, ny, nz);
            normal.Normalize(); // Ensure the normal vector is unit length

            return normal;
        }
    }
}
