using a3DLib.Core;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;

namespace a3DLib.Utilities
{
    public static class Extensions
    {
        public static Vector3 ToVector3(this Vector2 v) => new(v.X, v.Y, 0);
        public static void Draw(this Asset<LibMesh> m, Matrix world, Matrix view, Matrix projection, Action<BasicEffect> ChangeEffectParams = null)
        {
            if (!m.IsLoaded)
                return;
            m.Value.Draw(world, view, projection, ChangeEffectParams);
        }
        /// <summary>
        /// Looks for the brightest and nearest 3 lights to the given world position, and applies them to this BasicEffect.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="worldPosition"></param>
        /// <param name="queryWidth"></param>
        /// <param name="queryHeight"></param>
        public static void Set3DLights(this BasicEffect e, Vector2 worldPosition, int queryWidth, int queryHeight)
        {
            DirectionalLights dirLights = Lighting3D.GetLights(worldPosition, queryWidth, queryHeight);

            e.DirectionalLight0.Enabled = true;
            e.DirectionalLight1.Direction = dirLights.Light0.Direction;
            e.DirectionalLight0.DiffuseColor = dirLights.Light0.DiffuseColor;
            e.DirectionalLight0.SpecularColor = dirLights.Light0.SpecularColor;

            e.DirectionalLight1.Enabled = true;
            e.DirectionalLight1.Direction = dirLights.Light1.Direction;
            e.DirectionalLight1.DiffuseColor = dirLights.Light1.DiffuseColor;
            e.DirectionalLight1.SpecularColor = dirLights.Light1.SpecularColor;

            e.DirectionalLight2.Enabled = true;
            e.DirectionalLight2.Direction = dirLights.Light2.Direction;
            e.DirectionalLight2.DiffuseColor = dirLights.Light2.DiffuseColor;
            e.DirectionalLight2.SpecularColor = dirLights.Light2.SpecularColor;
        }
    }
}
