using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Content.Readers;
using ReLogic.Utilities;
using System.IO;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace a3DLib.Core
{
    // i wanted to have an IAssetReader for this to be compatible with ModContent.Request but i am too dum to do it :death:
    /*
    [Autoload(false)]
    public class ObjReader(GraphicsDevice graphicsDevice) : IAssetReader, ILoadable
    {
        private static readonly string modelExtension = ".obj";
        private readonly GraphicsDevice _graphicsDevice = graphicsDevice;
        public static AssetReaderCollection Collection { get { return Main.instance.Services.Get<AssetReaderCollection>(); } }
        public async ValueTask<T> FromStream<T>(Stream stream, MainThreadCreationContext mainThreadCtx) where T : class
        {
            if (typeof(T) != typeof(LibMesh))
            {
                throw AssetLoadException.FromInvalidReader<ObjReader, T>();
            }

            await mainThreadCtx;

            var result = CreateModel(stream);

            return result as T;
        }
        private LibMesh CreateModel(Stream stream)
        {
            var parser = new ObjParser();
            parser.LoadFromStream(stream);
            LibMesh newMesh = new(_graphicsDevice, parser);
            return newMesh;
        }
        void ILoadable.Load(Mod mod)
        {
            var assetReaderCollection = Collection;
            if (!assetReaderCollection.TryGetReader(modelExtension, out IAssetReader reader) || reader != this)
            {
                assetReaderCollection.RegisterReader(this, modelExtension);
            }
        }
        void ILoadable.Unload()
        {
            var assetReaderCollection = Collection;
            if (assetReaderCollection.TryGetReader(modelExtension, out var reader) && reader == this)
            {
                assetReaderCollection.UnregisterReader(this, modelExtension);
            }
        }
    }
    public static class AssetReaderCollectionUtils
    {
        public static void UnregisterReader(this AssetReaderCollection collection, IAssetReader reader, params string[] extensions)
        {
            // does the opposite of registerreader
            var dict = collection._readersByExtension;
            foreach (string text in extensions)
            {
                dict.Remove(text.ToLower());
            }
            collection._extensions = [.. dict.Keys];
        }
    }
    */
}
