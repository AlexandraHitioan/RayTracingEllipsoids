namespace RayTracingWithEllipsoids
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

        public Ellipsoid(Vector center, double radius, Material material, Color color) : base(material, color)
        {
            Center = center;
            Radius = radius;
            SemiAxesLength = new Vector(1,1,1);
        }
        
        public Ellipsoid(Vector center, Vector semiAxesLength, double radius, Color color) : base(color)
        {
            Center = center;
            SemiAxesLength = semiAxesLength;
            Radius = radius;
        }

        public Ellipsoid(Material material, Color color) : base(material,color)
        {
            Material = material;
            Color = color;
        }
        
        public override Intersection GetIntersection(Line line, double minDist, double maxDist)
        { 
            //ADD CODE HERE
            var a = SemiAxesLength.X; 
            var b = SemiAxesLength.Y; 
            var c = SemiAxesLength.Z; //corespondent semi-axes coodrinates for the main axes x,y,z
            var cX = Center.X; 
            var cY = Center.Y;
            var cZ = Center.Z; // x,y,z coordinates of the ellipsoid's center
            
            var a2 = a * a;
            var b2 = b * b;
            var c2 = c * c; //coefficients od the Ellipsoid equation
            
            var x0 = line.X0.X;
            var y0 = line.X0.Y;
            var z0 = line.X0.Z; //coordinates x,y,z of the X0 point of 'line' (ray of light)
        
            var dx = line.Dx.X;
            var dy = line.Dx.Y;
            var dz = line.Dx.Z; //coordinates x,y,z of the direction vector of 'line'
            
            var aE = dx * dx / a2 + dy * dy / b2 + dz * dz / c2;
            var bE = 2.0 * ((x0 - cX) * dx / a2 + (y0 - cY)* dy / b2 + (z0 - cZ) * dz / c2);
            var cE = (x0 - cX) * (x0 - cX) / a2 + (y0 - cY) * (y0 - cY) / b2 + (z0 - cZ) * (z0 - cZ) / c2 -Radius*Radius; //coefficients of quadratic equation
            var delta = bE * bE - 4 * aE * cE; //the discriminant for the equation
            if (delta < 0.001) //there are no valid solutions -> no intersection
            {
                return new Intersection(false, false, this, line, 0, new Vector(0, 0, 0));
            }
            var sol1 = (-bE - Math.Sqrt(delta)) / (2.0 * aE);
            var sol2 = (-bE + Math.Sqrt(delta)) / (2.0 * aE);
            var t1A = sol1 >= minDist && sol1 <= maxDist;
            var t2A = sol2>= minDist && sol2 <= maxDist;
            
            if (!(sol1 >= minDist && sol1 <= maxDist)&& !(sol2>= minDist && sol2 <= maxDist)) //none of the solutions is valid
            {
                return new Intersection(false, false, this, line, 0, new Vector(0,0,0));
            }
        
            if ((sol1 >= minDist && sol1 <= maxDist) && !(sol2>= minDist && sol2 <= maxDist)) //sol1 is valid
            {
                var xS1 = x0 + sol1 * dx;
                var yS1 = y0 + sol1 * dy;
                var zS1 = z0 + sol1 * dz; //coordinates with sol1 replaced
                return new Intersection(true, true, this, line, sol1, this.Normal(new Vector(xS1, yS1, zS1)));
            }
            else if (!(sol1 >= minDist && sol1 <= maxDist) && (sol2>= minDist && sol2 <= maxDist)) //sol2 is valid
            {
                var xS2 = x0 + sol2 * dx;
                var yS2 = y0 + sol2 * dy;
                var zS2 = z0 + sol2 * dz; //coordinates with sol2 replaced
                return new Intersection(true, true, this, line, sol2, this.Normal(new Vector(xS2, yS2, zS2)));
            }

            if (sol1 < sol2)
            {
                return new Intersection(true, true, this, line, sol1, this.Normal(new Vector()));
            }
            return new Intersection(true, true, this, line, sol2, this.Normal(new Vector()));
            
            
        }

        public Vector Normal(Vector v)
        {
            var x = v.X / SemiAxesLength.X;
            var y = v.Y / SemiAxesLength.Y;
            var z = v.Z / SemiAxesLength.Z;

            var h = Center.X / SemiAxesLength.X;
            var j = Center.Y / SemiAxesLength.Y;
            var k = Center.Z / SemiAxesLength.Z;

            var xx = 2 * (x - h) / 1.0;
            var yy = 2 * (y - j) / 1.0;
            var zz = 2 * (z - k) / 1.0;

            return new Vector(xx, yy, zz).Normalize();
        }
    }
}