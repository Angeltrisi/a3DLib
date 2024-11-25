global using Microsoft.Xna.Framework;
using ReLogic.Content.Sources;
using Terraria;
using Terraria.ModLoader;

namespace a3DLib
{
    // This library uses Krafs.Publicizer, so it will not build correctly if built from the Mod Development tab inside tModLoader. You must build it from your IDE.
    public class a3DLib : Mod
	{
        public const string BlankTexture = "a3DLib/BlankTexture";
        public override IContentSource CreateDefaultContentSource()
        {
            if (!Main.dedServ)
            {
                //AddContent(new ObjReader(Main.instance.GraphicsDevice));
            }
            return base.CreateDefaultContentSource();
        }
    }
}
