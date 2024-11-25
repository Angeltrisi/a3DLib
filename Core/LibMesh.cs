using a3DLib.Tests;
using a3DLib.Utilities;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stubble.Core.Parser;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace a3DLib.Core
{
    public class LibMesh : IDisposable
    {
        private static readonly RasterizerState CullClockwiseState = RasterizerState.CullClockwise;
        private readonly VertexBuffer vertexBuffer;
        private readonly IndexBuffer indexBuffer;
        private readonly BasicEffect effect;
        private readonly GraphicsDevice graphicsDevice;
        private readonly Asset<Texture2D> texture;
        public LibMesh(GraphicsDevice gd, ObjParser parser, Asset<Texture2D> texture = null)
        {
            graphicsDevice = gd;

            var vertices = new List<VertexPositionNormalTexture>();
            var indexMap = new Dictionary<(int pos, int uv, int norm), int>();
            var indices = new List<int>();
            if (texture != null)
                this.texture = texture;

            // TODO: Fix UVs. they just don't work and appear distorted
            foreach (var faceIndex in parser.VertexIndices)
            {
                int posIndex = faceIndex;
                int uvIndex = faceIndex >= 0 && faceIndex < parser.UVIndices.Count ? parser.UVIndices[faceIndex] : -1;
                int normIndex = faceIndex >= 0 && faceIndex < parser.NormalIndices.Count ? parser.NormalIndices[faceIndex] : -1;

                var pos = parser.Vertices[posIndex];
                var uv = uvIndex >= 0 ? parser.UVs[uvIndex] : Vector2.Zero;
                var norm = normIndex >= 0 ? parser.Normals[normIndex] : Vector3.Zero;

                var key = (posIndex, uvIndex, normIndex);
                // deduplication: you can't register the same vertex two times
                if (!indexMap.TryGetValue(key, out var vertexIndex))
                {
                    vertexIndex = vertices.Count;
                    vertices.Add(new VertexPositionNormalTexture(pos, norm, uv));
                    indexMap[key] = vertexIndex;
                }
                indices.Add(vertexIndex);
            }

            vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionNormalTexture), vertices.Count, BufferUsage.WriteOnly);
            vertexBuffer.SetData(vertices.ToArray());

            indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, indices.Count, BufferUsage.WriteOnly);
            indexBuffer.SetData(indices.ToArray());

            effect = new BasicEffect(graphicsDevice)
            {
                TextureEnabled = texture != null,
                Texture = texture.Value,
            };
        }
        public static LibMesh Load(string pathNoExtension, bool hasTexture = false)
        {
            LibMesh result = null;
            Main.QueueMainThreadAction(() =>
            {
                var parser = new ObjParser();
                parser.LoadFromStream(ModContent.GetFileBytes(pathNoExtension + ".obj").ToMemoryStream());
                Asset<Texture2D> tex = hasTexture ? ModContent.Request<Texture2D>(pathNoExtension + "_Texture", AssetRequestMode.ImmediateLoad) : null;
                result = new LibMesh(Main.instance.GraphicsDevice, parser, tex);
            });
            while (result is null)
            {

            }
            return result;
        }
        void IDisposable.Dispose()
        {
            GC.SuppressFinalize(this);
            vertexBuffer.Dispose();
            indexBuffer.Dispose();
            effect.Dispose();
        }
        /// <summary>
        /// Draws the model with the provided matrices. Using the <see cref="MatrixCreation"/> helpers is recommended.
        /// If the model is clipping, you may need to provide different values for the Z planes and camera positions.
        /// If the model is being culled incorrectly, restart the <see cref="SpriteBatch"/> with a different <see cref="RasterizerState"/>. Look at <see cref="CubeItem"/> for an example.
        /// </summary>
        /// <param name="world">World matrix. See <see cref="MatrixCreation.World(float, float, float, float, Vector2, float)"/> and its overloads for common ways of creating it.</param>
        /// <param name="view">View matrix. If this model is being drawn on the UI, use <see cref="Matrix.Identity"/>, otherwise, use <see cref="MatrixCreation.CommonCameraView(float, float)"/>.</param>
        /// <param name="projection">Projection matrix. If this model is being drawn on the game world, use <see cref=""/></param>
        /// <param name="ChangeEffectParams">A function called right before drawing the model, which allows you to change the effect parameters.</param>
        public void Draw(Matrix world, Matrix view, Matrix projection, Action<BasicEffect> ChangeEffectParams = null)
        {
            effect.World = world;
            effect.View = view;
            effect.Projection = projection;

            graphicsDevice.RasterizerState = CullClockwiseState;

            graphicsDevice.SetVertexBuffer(vertexBuffer);
            graphicsDevice.Indices = indexBuffer;

            ChangeEffectParams?.Invoke(effect);

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexBuffer.VertexCount, 0, indexBuffer.IndexCount / 3);
            }
        }
    }
}
