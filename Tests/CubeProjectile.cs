using a3DLib.Core;
using a3DLib.Utilities;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace a3DLib.Tests
{
    public class CubeProjectile : ModProjectile
    {
        public override string Texture => a3DLib.BlankTexture;
        public static LibMesh testMesh;
        public override void Load()
        {
            testMesh = LibMesh.Load("a3DLib/Tests/tomatoA", true);
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
            Asset<Texture2D> drawTex = ModContent.Request<Texture2D>("a3DLib/Tests/tomato");

            float xRot = Projectile.Center.X * 0.01f;
            float yRot = 0f;
            float zRot = Projectile.rotation;

            Matrix world = MatrixCreation.World(Projectile.scale * 15f, xRot, yRot, zRot, Projectile.Center, -35f);

            Matrix view = MatrixCreation.CommonCameraView(-40f, 0f);

            Matrix projection = MatrixCreation.CommonOrthographicProjection(-40f, 20f);

            testMesh.Draw(world, view, projection, e =>
            {
                e.AmbientLightColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates()).ToVector3();
                e.Set3DLights(Projectile.Center, 16, 16);
                e.LightingEnabled = true;
                e.TextureEnabled = true;
            });

            return false;
        }
    }
}
