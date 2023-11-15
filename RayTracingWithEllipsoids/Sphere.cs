namespace RayTracingWithEllipsoids
{
    public class Sphere : Ellipsoid
    {  
        //for the sphere, we basically create an ellipsoid with equal axis coordinates
        public Sphere(Vector center, double radius, Material material, Color color) : base(center, new Vector(1, 1, 1), radius, material, color)
        {
        }
        public Sphere(Vector center, double radius, Color color) : base(center, new Vector(1, 1, 1), radius, color)
        {
        }
    }
}