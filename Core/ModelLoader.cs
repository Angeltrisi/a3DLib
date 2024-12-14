using a3DLib.Tests;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
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
        private static Func<ContentReader, object> readAsset;
        public static readonly Dictionary<string, Model> ModelRegistry = [];
        /// <summary>
        /// <para>Loads a model that has been compiled into .xnb onto the registry.</para>
        /// <para>Don't know how to compile into .xnb? One easy way is to use <see href="https://github.com/SuperAndyHero/EasyXnb/releases">EasyXnb</see>, though it only supports .fbx compilation. Another way is to compile your model using XNA (or any offshoot of XNA) using the Content Pipeline.</para>
        /// <para>If the provided model's name (the final part of the path) is already in the registry, it won't attempt to register it again.</para>
        /// This method must be called using <see cref="Main.RunOnMainThread(Action)"/> or <see cref="Main.QueueMainThreadAction(Action)"/>, otherwise an error will be thrown.
        /// Check <see cref="CubeProjectile"/> for an example.
        /// </summary>
        /// <param name="pathNoExtension"></param>
        /// <returns></returns>
        public static Model LoadModel(string pathNoExtension)
        {
            string modelName = Path.GetFileName(pathNoExtension);
            string modName = pathNoExtension.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries)[0];
            string fullModelName = $"{modName}:{modelName}";
            if (ModelRegistry.TryGetValue(fullModelName, out Model value))
                return value;

            byte[] file = ModContent.GetFileBytes(pathNoExtension + ".xnb");

            using MemoryStream stream = new(file);
            // this part is taken from Dom's WoTM!!!
            stream.Seek(10, SeekOrigin.Begin);
            Type[] constructorParams = [typeof(ContentManager), typeof(Stream), typeof(string), typeof(int), typeof(char), typeof(Action<IDisposable>)];
            ConstructorInfo constructor = typeof(ContentReader).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, constructorParams);
            object[] parameters = [Main.ShaderContentManager, stream, modelName, 0, 'w', null];
            using ContentReader cr = (ContentReader)constructor.Invoke(parameters);
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
    /*
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
