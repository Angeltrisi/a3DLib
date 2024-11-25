using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace a3DLib.Utilities
{
    public static class Lighting3D
    {
        public static DirectionalLights GetLights(Vector2 worldCenter, int queryWidth, int queryHeight)
        {
            Point tileCoords = worldCenter.ToTileCoordinates();
            return GetLights(tileCoords, queryWidth, queryHeight);
        }
        public static DirectionalLights GetLights(Point tileCoords, int queryWidth, int queryHeight)
        {
            int halfWidth = queryWidth / 2;
            int halfHeight = queryHeight / 2;

            Point topLeft = new(tileCoords.X - halfWidth, tileCoords.Y - halfHeight);
            Point bottomRight = new(tileCoords.X + halfWidth, tileCoords.Y + halfHeight);

            List<(DirectionalLight light, float score)> lights = [];

            for (int x = topLeft.X; x <= bottomRight.X; x++)
            {
                for (int y = topLeft.Y; y <= bottomRight.Y; y++)
                {
                    var lightColor = Lighting.GetColor(x, y);

                    float brightness = (lightColor.R + lightColor.G + lightColor.B) / 3f / 255f;

                    if (brightness > 0)
                    {
                        Vector3 direction = Vector3.Normalize(new Vector3(x - tileCoords.X, y - tileCoords.Y, 0));

                        float distance = Vector2.Distance(new Vector2(tileCoords.X, tileCoords.Y), new Vector2(x, y));

                        float score = brightness / (1f + distance);

                        DirectionalLight light = new()
                        {
                            Direction = direction,
                            DiffuseColor = lightColor.ToVector3(),
                            SpecularColor = new Vector3(1f, 1f, 1f)
                        };

                        lights.Add((light, score));
                    }
                }
            }

            var topLights = lights.OrderByDescending(l => l.score).Take(3).ToList();

            while (topLights.Count < 3)
            {
                topLights.Add((new DirectionalLight
                {
                    Direction = Vector3.Zero,
                    DiffuseColor = Vector3.Zero,
                    SpecularColor = Vector3.Zero
                }, 0f));
            }

            return new DirectionalLights(topLights[0].light, topLights[1].light, topLights[2].light);
        }

    }
    public struct DirectionalLights (DirectionalLight light0, DirectionalLight light1, DirectionalLight light2)
    {
        public DirectionalLight Light0 { get; set; } = light0;
        public DirectionalLight Light1 { get; set; } = light1;
        public DirectionalLight Light2 { get; set; } = light2;
    }
    public struct DirectionalLight
    {
        public Vector3 Direction {  get; set; }
        public Vector3 DiffuseColor { get; set; }
        public Vector3 SpecularColor { get; set; }
    }
}
