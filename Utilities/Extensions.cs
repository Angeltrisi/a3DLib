using Microsoft.Xna.Framework.Graphics;
using System;
using a3DLib.Tests;

namespace a3DLib.Utilities
{
    public static class Extensions
    {
        public static Vector3 ToVector3(this Vector2 v) => new(v.X, v.Y, 0);
        /// <summary>
        /// <para>This is a simplified version of the native <see cref="Model.Draw(Matrix, Matrix, Matrix)"/>, though you can use either (with some changes, check <see cref="CubeProjectile"/>'s comments.</para>
        /// Also, added flexibility through ChangeEffectParams.
        /// Don't know what to use for the matrices? Check the <see cref="MatrixCreation"/> class for some helpers designed for use with Terraria.
        /// You may also check the usage examples such as <see cref="CubeItem"/> for UI usage, and <see cref="CubeProjectile"/> for world usage.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="world"></param>
        /// <param name="view"></param>
        /// <param name="projection"></param>
        /// <param name="ChangeEffectParams"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void Draw(this Model m, Matrix world, Matrix view, Matrix projection, Action<BasicEffect> ChangeEffectParams = null)
        {
            foreach (ModelMesh mesh in m.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    IEffectMatrices obj = (effect as IEffectMatrices) ?? throw new InvalidOperationException();
                    obj.World = world;
                    obj.View = view;
                    obj.Projection = projection;
                    if (effect is BasicEffect be)
                        ChangeEffectParams(be);
                }
                mesh.Draw();
            }
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
