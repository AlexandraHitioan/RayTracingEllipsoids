namespace rt
{
    public class Camera
    {
        public Vector Position { get; set; }
        public Vector Direction { get; set; }
        public Vector Up { get; set; }
        
        public double ViewPlaneDistance { get; set; }
        public double ViewPlaneWidth { get; set; }
        public double ViewPlaneHeight { get; set; }
        public Vector SemiAxes { get; set; } //newly added property, to help it work with the ellipsoid class (Look for further explanation when it's all done)
        
        public double FrontPlaneDistance { get; set; }
        public double BackPlaneDistance { get; set; }

        public Camera(Vector position, Vector direction, Vector up, double viewPlaneDistance, double viewPlaneWidth, double viewPlaneHeight, double frontPlaneDistance, double backPlaneDistance, Vector semiAxes)
        {
            Position = position;
            Direction = direction;
            Up = up;
            ViewPlaneDistance = viewPlaneDistance;
            ViewPlaneWidth = viewPlaneWidth;
            ViewPlaneHeight = viewPlaneHeight;
            FrontPlaneDistance = frontPlaneDistance;
            BackPlaneDistance = backPlaneDistance;
            SemiAxes = semiAxes; // Initialize the SemiAxes property
        }

        public void Normalize()
        {
            Direction.Normalize();
            Up.Normalize();
            Up = (Direction ^ Up) ^ Direction;
        }
    }
}