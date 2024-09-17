using UnityEngine;

namespace HyperCasual.Runner
{
    /// <summary>
    ///     A class used to generate terrain for
    ///     Runner levels.
    /// </summary>
    public class TerrainGenerator : MonoBehaviour
    {
        /// <summary>
        ///     Creates and instantiates a Terrain GameObject, built
        ///     with the specified width, length, and thickness.
        /// </summary>
        /// <param name="terrainDimensions">
        ///     The dimensions of the terrain to generate
        /// </param>
        /// <param name="terrainMaterial">
        ///     The material to apply to the terrain.
        /// </param>
        /// <param name="terrainGameObject">
        ///     A new GameObject that is created to hold the terrain.
        /// </param>
        public static void CreateTerrain(TerrainDimensions terrainDimensions, Material terrainMaterial,
            ref GameObject terrainGameObject)
        {
            var width = terrainDimensions.Width;
            var length = terrainDimensions.Length;
            var startBuffer = terrainDimensions.StartBuffer;
            var endBuffer = terrainDimensions.EndBuffer;
            var thickness = terrainDimensions.Thickness;

            var mesh = new Mesh();
            mesh.name = "Terrain";
            var vertices = new Vector3[24];
            var normals = new Vector3[24];
            var uvs = new Vector2[24];
            var triangles = new int[36];

            var halfWidth = width * 0.5f;
            var beginningOfStartBuffer = -startBuffer;
            var endOfEndBuffer = length + endBuffer;

            var upperStartLeft = new Vector3(-halfWidth, 0.0f, beginningOfStartBuffer);
            var upperStartRight = new Vector3(halfWidth, 0.0f, beginningOfStartBuffer);
            var lowerStartLeft = new Vector3(-halfWidth, -thickness, beginningOfStartBuffer);
            var lowerStartRight = new Vector3(halfWidth, -thickness, beginningOfStartBuffer);

            var upperEndLeft = new Vector3(-halfWidth, 0.0f, endOfEndBuffer);
            var upperEndRight = new Vector3(halfWidth, 0.0f, endOfEndBuffer);
            var lowerEndLeft = new Vector3(-halfWidth, -thickness, endOfEndBuffer);
            var lowerEndRight = new Vector3(halfWidth, -thickness, endOfEndBuffer);

            var upNormal = Vector3.up;
            var rightNormal = Vector3.right;
            var forwardNormal = Vector3.forward;
            var downNormal = -upNormal;
            var leftNormal = -rightNormal;
            var backwardNormal = -forwardNormal;

            var vertexIndex = 0;
            var triangleIndex = 0;

            // Top Plane

            vertices[vertexIndex + 0] = upperStartLeft;
            vertices[vertexIndex + 1] = upperEndLeft;
            vertices[vertexIndex + 2] = upperEndRight;
            vertices[vertexIndex + 3] = upperStartRight;

            normals[vertexIndex + 0] = upNormal;
            normals[vertexIndex + 1] = upNormal;
            normals[vertexIndex + 2] = upNormal;
            normals[vertexIndex + 3] = upNormal;

            uvs[vertexIndex + 0] = new Vector2(0.0f, beginningOfStartBuffer);
            uvs[vertexIndex + 1] = new Vector2(0.0f, endOfEndBuffer);
            uvs[vertexIndex + 2] = new Vector2(1.0f, endOfEndBuffer);
            uvs[vertexIndex + 3] = new Vector2(1.0f, beginningOfStartBuffer);

            triangles[triangleIndex + 0] = vertexIndex;
            triangles[triangleIndex + 1] = vertexIndex + 1;
            triangles[triangleIndex + 2] = vertexIndex + 2;

            triangles[triangleIndex + 3] = vertexIndex;
            triangles[triangleIndex + 4] = vertexIndex + 2;
            triangles[triangleIndex + 5] = vertexIndex + 3;

            vertexIndex += 4;
            triangleIndex += 6;

            // Bottom Plane

            vertices[vertexIndex + 0] = lowerStartLeft;
            vertices[vertexIndex + 1] = lowerEndLeft;
            vertices[vertexIndex + 2] = lowerEndRight;
            vertices[vertexIndex + 3] = lowerStartRight;

            normals[vertexIndex + 0] = downNormal;
            normals[vertexIndex + 1] = downNormal;
            normals[vertexIndex + 2] = downNormal;
            normals[vertexIndex + 3] = downNormal;

            uvs[vertexIndex + 0] = new Vector2(0.0f, beginningOfStartBuffer);
            uvs[vertexIndex + 1] = new Vector2(0.0f, endOfEndBuffer);
            uvs[vertexIndex + 2] = new Vector2(1.0f, endOfEndBuffer);
            uvs[vertexIndex + 3] = new Vector2(1.0f, beginningOfStartBuffer);

            triangles[triangleIndex + 0] = vertexIndex;
            triangles[triangleIndex + 1] = vertexIndex + 2;
            triangles[triangleIndex + 2] = vertexIndex + 1;

            triangles[triangleIndex + 3] = vertexIndex;
            triangles[triangleIndex + 4] = vertexIndex + 3;
            triangles[triangleIndex + 5] = vertexIndex + 2;

            vertexIndex += 4;
            triangleIndex += 6;

            // Right Plane

            vertices[vertexIndex + 0] = upperStartRight;
            vertices[vertexIndex + 1] = upperEndRight;
            vertices[vertexIndex + 2] = lowerEndRight;
            vertices[vertexIndex + 3] = lowerStartRight;

            normals[vertexIndex + 0] = rightNormal;
            normals[vertexIndex + 1] = rightNormal;
            normals[vertexIndex + 2] = rightNormal;
            normals[vertexIndex + 3] = rightNormal;

            uvs[vertexIndex + 0] = new Vector2(1.0f, beginningOfStartBuffer);
            uvs[vertexIndex + 1] = new Vector2(1.0f, endOfEndBuffer);
            uvs[vertexIndex + 2] = new Vector2(1.0f - thickness, endOfEndBuffer);
            uvs[vertexIndex + 3] = new Vector2(1.0f - thickness, beginningOfStartBuffer);

            triangles[triangleIndex + 0] = vertexIndex;
            triangles[triangleIndex + 1] = vertexIndex + 1;
            triangles[triangleIndex + 2] = vertexIndex + 2;

            triangles[triangleIndex + 3] = vertexIndex;
            triangles[triangleIndex + 4] = vertexIndex + 2;
            triangles[triangleIndex + 5] = vertexIndex + 3;

            vertexIndex += 4;
            triangleIndex += 6;

            // Left Plane

            vertices[vertexIndex + 0] = lowerStartLeft;
            vertices[vertexIndex + 1] = lowerEndLeft;
            vertices[vertexIndex + 2] = upperEndLeft;
            vertices[vertexIndex + 3] = upperStartLeft;

            normals[vertexIndex + 0] = leftNormal;
            normals[vertexIndex + 1] = leftNormal;
            normals[vertexIndex + 2] = leftNormal;
            normals[vertexIndex + 3] = leftNormal;

            uvs[vertexIndex + 0] = new Vector2(-thickness, beginningOfStartBuffer);
            uvs[vertexIndex + 1] = new Vector2(-thickness, endOfEndBuffer);
            uvs[vertexIndex + 2] = new Vector2(0.0f, endOfEndBuffer);
            uvs[vertexIndex + 3] = new Vector2(0.0f, beginningOfStartBuffer);

            triangles[triangleIndex + 0] = vertexIndex;
            triangles[triangleIndex + 1] = vertexIndex + 1;
            triangles[triangleIndex + 2] = vertexIndex + 2;

            triangles[triangleIndex + 3] = vertexIndex;
            triangles[triangleIndex + 4] = vertexIndex + 2;
            triangles[triangleIndex + 5] = vertexIndex + 3;

            vertexIndex += 4;
            triangleIndex += 6;

            // Start Plane

            vertices[vertexIndex + 0] = lowerStartLeft;
            vertices[vertexIndex + 1] = upperStartLeft;
            vertices[vertexIndex + 2] = upperStartRight;
            vertices[vertexIndex + 3] = lowerStartRight;

            normals[vertexIndex + 0] = backwardNormal;
            normals[vertexIndex + 1] = backwardNormal;
            normals[vertexIndex + 2] = backwardNormal;
            normals[vertexIndex + 3] = backwardNormal;

            uvs[vertexIndex + 0] = new Vector2(0.0f, 0.0f);
            uvs[vertexIndex + 1] = new Vector2(0.0f, thickness);
            uvs[vertexIndex + 2] = new Vector2(1.0f, thickness);
            uvs[vertexIndex + 3] = new Vector2(1.0f, 0.0f);

            triangles[triangleIndex + 0] = vertexIndex;
            triangles[triangleIndex + 1] = vertexIndex + 1;
            triangles[triangleIndex + 2] = vertexIndex + 2;

            triangles[triangleIndex + 3] = vertexIndex;
            triangles[triangleIndex + 4] = vertexIndex + 2;
            triangles[triangleIndex + 5] = vertexIndex + 3;

            vertexIndex += 4;
            triangleIndex += 6;

            // End Plane

            vertices[vertexIndex + 0] = lowerEndRight;
            vertices[vertexIndex + 1] = upperEndRight;
            vertices[vertexIndex + 2] = upperEndLeft;
            vertices[vertexIndex + 3] = lowerEndLeft;

            normals[vertexIndex + 0] = forwardNormal;
            normals[vertexIndex + 1] = forwardNormal;
            normals[vertexIndex + 2] = forwardNormal;
            normals[vertexIndex + 3] = forwardNormal;

            triangles[triangleIndex + 0] = vertexIndex;
            triangles[triangleIndex + 1] = vertexIndex + 1;
            triangles[triangleIndex + 2] = vertexIndex + 2;

            uvs[vertexIndex + 0] = new Vector2(0.0f, 0.0f);
            uvs[vertexIndex + 1] = new Vector2(0.0f, thickness);
            uvs[vertexIndex + 2] = new Vector2(1.0f, thickness);
            uvs[vertexIndex + 3] = new Vector2(1.0f, 0.0f);

            triangles[triangleIndex + 3] = vertexIndex;
            triangles[triangleIndex + 4] = vertexIndex + 2;
            triangles[triangleIndex + 5] = vertexIndex + 3;

            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.triangles = triangles;
            mesh.uv = uvs;

            if (terrainGameObject != null)
            {
                if (Application.isPlaying)
                    Destroy(terrainGameObject);
                else
                    DestroyImmediate(terrainGameObject);
            }

            terrainGameObject = new GameObject("Terrain");
            var meshFilter = terrainGameObject.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = mesh;
            var meshRenderer = terrainGameObject.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = terrainMaterial;
        }

        /// <summary>
        ///     Contains all the measurements needed for the
        ///     TerrainGenerator to create a new piece of Terrain.
        /// </summary>
        public struct TerrainDimensions
        {
            /// <summary>
            ///     Width of the terrain to generate.
            /// </summary>
            public float Width;

            /// <summary>
            ///     Length of the terrain to generate.
            /// </summary>
            public float Length;

            /// <summary>
            ///     Length of terrain to add before the start of the level.
            /// </summary>
            public float StartBuffer;

            /// <summary>
            ///     Length of terrain to add after the end of the level.
            /// </summary>
            public float EndBuffer;

            /// <summary>
            ///     Thickness of the terrain to generate.
            /// </summary>
            public float Thickness;
        }
    }
}