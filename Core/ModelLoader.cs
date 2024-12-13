using a3DLib.Tests;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Content.Readers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace a3DLib.Core
{
    // this is extremely fucked for the record but it works
    public class ModelLoader : ILoadable
    {
        private static readonly string modelExtension = ".xnbfbx";
        private readonly GraphicsDevice _graphicsDevice = Main.instance.GraphicsDevice;
        private static Func<ContentReader, object> readAsset;
        public static readonly Dictionary<string, Model> ModelRegistry = [];
        /// <summary>
        /// Loads a model that has been compiled into .xnb onto the registry.
        /// If the provided model's name (the final part of the path) is already in the registry, it won't attempt to register it again.
        /// This method must be called using <see cref="Main.RunOnMainThread(Action)"/> or <see cref="Main.QueueMainThreadAction(Action)"/>, otherwise an error will be thrown.
        /// Check <see cref="CubeProjectile"/> for an example.
        /// </summary>
        /// <param name="pathNoExtension"></param>
        /// <returns></returns>
        public static Model LoadModel(string pathNoExtension)
        {
            string modelName = Path.GetFileName(pathNoExtension);
            string modName = pathNoExtension.Split([ '/', '\\' ], StringSplitOptions.RemoveEmptyEntries)[0];
            string fullModelName = $"{modName}:{modelName}";
            if (ModelRegistry.TryGetValue(fullModelName, out Model value))
                return value;

            byte[] file = ModContent.GetFileBytes(pathNoExtension + ".xnb");

            using MemoryStream stream = new(file);
            // this part is taken from Dom's WoTM!!!
            stream.Seek(10, SeekOrigin.Begin);
            using ContentReader cr = new(Main.ShaderContentManager, stream, modelName, 0, 'w', null);
            Model asset = (Model)readAsset(cr);
            ModelRegistry[fullModelName] = asset;

            return asset;
        }
        void ILoadable.Load(Mod mod)
        {
            var readAssetMethod = typeof(ContentReader).GetMethod("ReadAsset", BindingFlags.NonPublic | BindingFlags.Instance)!.MakeGenericMethod(typeof(object));
            readAsset = (Func<ContentReader, object>)Delegate.CreateDelegate(typeof(Func<ContentReader, object>), readAssetMethod);
        }
        void ILoadable.Unload()
        {
            readAsset = null;
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
}
