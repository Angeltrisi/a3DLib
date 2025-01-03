﻿using a3DLib.Tests;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;
using System.Runtime.CompilerServices;

namespace a3DLib.Core
{
    public class ModelLoader : ILoadable
    {
        private static Func<ContentReader, object> readAsset;
        public static readonly Dictionary<string, Model> ModelRegistry = [];
        /// <summary>
        /// <para>Loads a model that has been compiled into .xnb onto the registry.</para>
        /// <para>Don't know how to compile into .xnb? One easy way is to use <see href="https://github.com/SuperAndyHero/EasyXnb/releases">EasyXnb</see>, though it only supports .fbx compilation. Another way is to compile your model using XNA (or any offshoot of XNA) using the Content Pipeline.</para>
        /// <para>After testing various models, I found that the ones most likely to compile correctly are models without embedded textures or materials, so you might find more success making models yourself rather than finding them online, though it really depends on the model.</para>
        /// <para>If the provided model's name (the final part of the path) is already in the registry, it won't attempt to register it again.</para>
        /// <para>This method will automatically look for a texture with the given path + "_Texture", and use it. Check the Tests folder for an example.</para>
        /// This method must be called using <see cref="Main.RunOnMainThread(Action)"/> or <see cref="Main.QueueMainThreadAction(Action)"/>, otherwise an error will be thrown.
        /// Check <see cref="CubeProjectile"/> for an example.
        /// </summary>
        /// <param name="pathNoExtension"></param>
        /// <returns></returns>
        public static Model LoadModel(string pathNoExtension)
        {
            if (Main.dedServ)
                return null;
            string modelName = Path.GetFileName(pathNoExtension);
            string modName = pathNoExtension.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries)[0];
            string fullModelName = $"{modName}:{modelName}";
            if (ModelRegistry.TryGetValue(fullModelName, out Model value))
                return value;

            byte[] file = ModContent.GetFileBytes(pathNoExtension + ".xnb");

            using MemoryStream stream = new(file);
            // this part is taken from Dom's WoTM!!!
            stream.Seek(10, SeekOrigin.Begin);

            using ContentReader cr = Accessors.NewContentReader(Main.ShaderContentManager, stream, modelName, 0, 'w', null);
            Model asset = (Model)readAsset(cr);

            string texturePath = pathNoExtension + "_Texture";
            Texture2D fetchedTexture = ModContent.HasAsset(texturePath) ? ModContent.Request<Texture2D>(texturePath, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value : null;

            foreach (ModelMesh mesh in asset.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    if (effect is BasicEffect be)
                    {
                        be.TextureEnabled = fetchedTexture != null;
                        be.Texture = fetchedTexture;
                    }
                }
            }

            ModelRegistry[fullModelName] = asset;

            return asset;
        }
        private static class Accessors
        {
            [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
            public static extern ContentReader NewContentReader(ContentManager contentManager, Stream stream, string modelName, int version, char platform, Action<IDisposable> recordDisposableObject);
            /* this will work when tml is ported to .NET 9
            [UnsafeAccessor(UnsafeAccessorKind.Method)]
            public static extern object CallReadAsset<T>(ContentReader reader);
            */
        }
        void ILoadable.Load(Mod mod)
        {
            // TODO: switch this to unsafeaccessor (when tml is ported to .NET 9)
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
