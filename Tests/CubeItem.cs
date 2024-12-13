using a3DLib.Utilities;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace a3DLib.Tests
{
    public class CubeItem : ModItem
    {
        public override string Texture => "a3DLib/Tests/tomato";
        public override void SetDefaults()
        {
            Item.width = Item.height = 32;
            Item.DefaultToRangedWeapon(ModContent.ProjectileType<CubeProjectile>(), AmmoID.None, 16, 8f);
        }
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullClockwise, null, Main.UIScaleMatrix);

            float div = 10f;
            Quaternion upwardRotation = Quaternion.CreateFromAxisAngle(Vector3.Right, Main.GameUpdateCount / div);
            Quaternion rightwardRotation = Quaternion.CreateFromAxisAngle(Vector3.Up, Main.GameUpdateCount / div);
            Quaternion targetRotation = Quaternion.Concatenate(upwardRotation, rightwardRotation);

            Matrix world = MatrixCreation.World(scale, targetRotation, position, 10f);
            Matrix view = Matrix.Identity;
            Matrix projection = MatrixCreation.UIProjection(-30f, 10f);

            Asset<Texture2D> cubeTex = ModContent.Request<Texture2D>("a3DLib/Tests/tomatoA_Texture");

            CubeProjectile.testMesh.Draw(world, Matrix.Identity, projection, e =>
            {
                e.textureEnabled = true;
                e.Texture = cubeTex.Value;
                e.LightingEnabled = false;
            });

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);
            return false;
        }
    }
}
