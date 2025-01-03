﻿using a3DLib.Core;
using a3DLib.Utilities;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace a3DLib.Tests
{
    /// <summary>
    /// An example on how to display a model inside the game world.
    /// Check <see cref="CubeItem"/> for a UI example.
    /// </summary>
    public class CubeProjectile : ModProjectile
    {
        public override string Texture => a3DLib.BlankTexture;
        public static Model testMesh;
        public override void Load()
        {
            Main.QueueMainThreadAction(() =>
            {
                testMesh = ModelLoader.LoadModel("a3DLib/Tests/cube");
                // Now we can use this model in two ways. Look at PreDraw for both of them (one of them is commented out).
            });
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.hostile = false;
        }
        public override void AI()
        {
            Projectile.velocity.Y += 0.1f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.spriteDirection = Projectile.direction = Projectile.velocity.X > 0 ? 1 : -1;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // By default, the spriteBatch's RasterizerState is CullCounterClockwise, which prevents proper rendering in the world.
            // TO-DO: Find a way around this.

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            float xRot = Projectile.Center.X * 0.01f;
            float yRot = 0f;
            float zRot = Projectile.rotation;

            Matrix world = MatrixCreation.World(Projectile.scale, xRot, yRot, zRot, Projectile.Center, -35f);

            Matrix view = MatrixCreation.CommonCameraView(-40f, 0f);

            Matrix projection = MatrixCreation.CommonOrthographicProjection(-40f, 20f);

            // The first way we can reference and use the model is storing it in a static variable and using it this way.
            // Make sure to check for null before running anything on it.
            /*
            testMesh?.Draw(world, view, projection, be =>
            {
                be.LightingEnabled = true;
                be.Set3DLights(Projectile.Center, 8, 8);
                be.AmbientLightColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates()).ToVector3();
            });
            */

            // We can also use the model directly from the registry.
            // Since our path is "a3DLib/Tests/cube", the name of this model in the registry will be our mod name, then "cube".

            if (ModelLoader.ModelRegistry.TryGetValue("a3DLib:cube", out Model m))
            {
                // If you're having trouble using the native Model.Draw() implementation, you might need to scale the world matrix by some factor like I do here:

                /*
                Matrix rescaled = Matrix.CreateScale(0.01f);
                m.Draw(rescaled * world, view, projection);
                */
                
                // My implementation should work just fine in most places.

                m.Draw(world, view, projection, e =>
                {
                    e.LightingEnabled = true;
                    e.Set3DLights(Projectile.Center, 8, 8);
                    e.AmbientLightColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates()).ToVector3();
                });
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}
