using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace a3DLib.Core
{
    /// <summary>
    /// Throwaway class for parsing .obj files into workable LibMesh data
    /// </summary>
    public class ObjParser
    {
        public List<Vector3> Vertices { get; private set; } = [];
        public List<Vector3> Normals { get; private set; } = [];
        public List<Vector2> UVs { get; private set; } = [];
        public List<int> VertexIndices { get; private set; } = [];
        public List<int> NormalIndices { get; private set; } = [];
        public List<int> UVIndices { get; private set; } = [];
        public void LoadFromStream(Stream stream)
        {
            using var reader = new StreamReader(stream);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                ParseLine(line);
            }
        }

        private void ParseLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                return; // skip comments and whitespace

            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts[0] == "usemtl" || parts[0] == "s")
                return; // skip these cuz we're not using them

            static int ResolveIndex(int index, int count) => index < 0 ? count + index : index - 1;

            switch (parts[0])
            {
                case "v": // vertex
                    Vertices.Add(new Vector3(
                        float.Parse(parts[1]),
                        float.Parse(parts[2]),
                        float.Parse(parts[3])));
                    break;

                case "vn": // normal
                    Normals.Add(new Vector3(
                        float.Parse(parts[1]),
                        float.Parse(parts[2]),
                        float.Parse(parts[3])));
                    break;

                case "vt": // texcoord or uv
                    var uv = Vector2.Clamp(new Vector2(float.Parse(parts[1]), 1f - float.Parse(parts[2])), Vector2.Zero, Vector2.One);
                    UVs.Add(uv);
                    break;

                case "f": // face

                    var vertIndices = new List<int>();
                    var uvIndices = new List<int>();
                    var normalIndices = new List<int>();

                    foreach (var part in parts.Skip(1))
                    {
                        var indices = part.Split('/');

                        int vertexIndex = ResolveIndex(int.Parse(indices[0]), Vertices.Count);
                        vertIndices.Add(vertexIndex);

                        if (indices.Length > 1 && !string.IsNullOrWhiteSpace(indices[1]))
                        {
                            int uvIndex = ResolveIndex(int.Parse(indices[1]), UVs.Count);
                            uvIndices.Add(uvIndex);
                        }

                        if (indices.Length > 2 && !string.IsNullOrWhiteSpace(indices[2]))
                        {
                            int normalIndex = ResolveIndex(int.Parse(indices[2]), Normals.Count);
                            normalIndices.Add(normalIndex);
                        }
                    }
                    // triangulate
                    // only works for convex polygons but uhhhh a
                    for (int i = 1; i < vertIndices.Count - 1; i++)
                    {
                        VertexIndices.Add(vertIndices[0]);
                        VertexIndices.Add(vertIndices[i]);
                        VertexIndices.Add(vertIndices[i + 1]);
                        if (normalIndices.Count > 0)
                        {
                            NormalIndices.Add(normalIndices[0]);
                            NormalIndices.Add(normalIndices[i]);
                            NormalIndices.Add(normalIndices[i + 1]);
                        }
                        if (uvIndices.Count > 0)
                        {
                            UVIndices.Add(uvIndices[0]);
                            UVIndices.Add(uvIndices[i]);
                            UVIndices.Add(uvIndices[i + 1]);
                        }
                    }
                    break;
            }
        }
    }
}
