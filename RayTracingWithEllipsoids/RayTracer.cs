using System;

namespace RayTracingWithEllipsoids
{
    class RayTracer
    {
        private Geometry[] geometries;
        private Light[] lights;

        public RayTracer(Geometry[] geometries, Light[] lights)
        {
            this.geometries = geometries;
            this.lights = lights;
        }

        private double ImageToViewPlane(int n, int imgSize, double viewPlaneSize)
        {
            return -n * viewPlaneSize / imgSize + viewPlaneSize / 2;
        }

        private Intersection FindFirstIntersection(Line ray, double minDist, double maxDist)
        {
            var intersection = new Intersection();

            foreach (var geometry in geometries)
            {
                var intr = geometry.GetIntersection(ray, minDist, maxDist);

                if (!intr.Valid || !intr.Visible) continue;

                if (!intersection.Valid || !intersection.Visible)
                {
                    intersection = intr;
                }
                else if (intr.T < intersection.T)
                {
                    intersection = intr;
                }
            }
            return intersection;
        }

        private bool IsLit(Vector point, Light light)
        {
            // TODO: ADD CODE HERE
            var ray = new Line(light.Position, point); //a ray pointing from our light source towards our point
            var inter = FindFirstIntersection(ray, 0, 100000); //first inter between the ray and any object in a range of [0,100000] surface
            if (inter.Valid == false)
            {
                return true; //we have a clear line of sight
            }
            if (inter.Visible == false)
            {
                return true; //the object is either transparent or doesn't exist
            }
            if (inter.T > (light.Position - point).Length() - 0.001) // we check wether the dist between the source of light and the inter point is greater than the one between the light and the given point
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Render(Camera camera, int width, int height, string filename)
        {
            var background = new Color();
            var viewParallel = (camera.Up ^ camera.Direction).Normalize();
            var image = new Image(width, height);

            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    var semiAxes = camera.SemiAxes;
                   var pointOnViewPlane = camera.Position + camera.Direction * camera.ViewPlaneDistance + 
                                          (camera.Up ^ camera.Direction) * ImageToViewPlane(i, width, camera.ViewPlaneWidth) + 
                                          camera.Up * ImageToViewPlane(j, height, camera.ViewPlaneHeight);//the ray starting from the camera position towards the point we just calculated
                    var ray = new Line(camera.Position, pointOnViewPlane);//the ray starting from the camera position towards the point we just calculated
                    var intersection = FindFirstIntersection(ray, camera.FrontPlaneDistance, camera.BackPlaneDistance);// determines the first intersection point between the ray and other potential objects
                    if (intersection.Valid && intersection.Visible)
                    {
                        var pixelColor = new Color();//color of current pixel
                        foreach (var light in lights)
                        {
                            var lightColor = new Color(); //the shade of the lights that affects the color of the current pixel
                            lightColor += intersection.Geometry.Material.Ambient * light.Ambient;
                            if (IsLit(intersection.Position, light))
                            {
                                var interPoint = intersection.Position; // the position of the intersection point
                                var vCamInter = (camera.Position - interPoint).Normalize(); // normal vector pointing from camera to intersection point
                                var dirLightInter = ((Ellipsoid) intersection.Geometry).Normal(intersection.Position); // surface normal vector pointing from light source to inter point
                                var vLightInter = (light.Position - interPoint).Normalize(); //  normal vector pointing from light source to inter point
                                var reflLight = (dirLightInter * (dirLightInter * vLightInter) * 2 - vLightInter).Normalize(); // unit vector of reflected light
                                
                                if (dirLightInter * vLightInter > 0) //we check to see if the vectors are not in opposite directions; if true, we have diffuse light
                                    lightColor += intersection.Geometry.Material.Diffuse * light.Diffuse * (dirLightInter * vLightInter);

                                if (vCamInter * reflLight > 0) //same here, but with specular
                                    lightColor += intersection.Geometry.Material.Specular * light.Specular * Math.Pow(vCamInter * reflLight, intersection.Geometry.Material.Shininess);

                                lightColor *= light.Intensity; //amplifying the color by intensity
                            }

                            pixelColor += lightColor;//we add the colors from the light sources to the overall color of the pixel
                        }

                        image.SetPixel(i, j, pixelColor);
                    }
                    else
                    {
                        image.SetPixel(i, j, background);//if there is no intersection with an object, the pixel will just get the bg color
                    }
                }
            }

            image.Store(filename);
        }
    }
}