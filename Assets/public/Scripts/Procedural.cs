using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PUBLIC{
    public class Procedural : MonoBehaviour
    {

        public Material TerrainMaterial;

        // Use this for initialization
        void Start()
        {

            //generate a random seed
            int seed = (int)Random.Range(0, 100f);

            //generate 100 chunks
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Chunk c = new Chunk(i, j, seed, TerrainMaterial);
                    c.Generate();
                    c.Render();
                }
            }

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

    public class Chunk
    {

        public int x, y, seed;
        public static readonly int SIZE = 32;
        public Vector3[,] blocks;
        public GameObject chunkObj;
        private Material mt;

        public Chunk(int x, int y, int seed, Material m)
        {
            this.x = x;
            this.y = y;
            this.seed = seed;
            /*
             * Making the size SIZE + 1 to make sure the terrain is continuous
             * When rendering only go through 0 to SIZE 
             */
            blocks = new Vector3[SIZE + 1, SIZE + 1];
            mt = m;
        }

        // Generate ofc
        public void Generate()
        {
            for (int i = 0; i < blocks.GetLength(0); i++)
            {
                for (int j = 0; j < blocks.GetLength(1); j++)
                {
                    blocks[i, j] = GenerateBlock(i, j, seed);
                }
            }
        }

        private Vector3 GenerateBlock(int i, int j, int seed)
        {

            // Try commenting out one of these below to see what will happen (::
            float ocean = NoiseLib.ExpNoise(SIZE * x + i, SIZE * y + j, seed, 1000, 1 * Mathf.PerlinNoise((SIZE * x + i + seed + 2) / 60f, (SIZE * y + j + seed + 2) / 60f), 1.0f, 1.0f, 0.5f);

            float noise = NoiseLib.ExpNoise(SIZE * x + i, SIZE * y + j, seed, 50, 10 * Mathf.PerlinNoise((SIZE * x + i + seed + 2) / 60f, (SIZE * y + j + seed + 2) / 60f), 1.0f, 1.0f, 0.5f);

            // Calculate the height map based on ocean and noise (mountains)
            float height = 30 + 40 * (Mathf.Sign(ocean - 0.45f) - 1) * Mathf.Pow(Mathf.Abs(ocean - 0.45f), 0.9f) + 50 * Mathf.Pow(noise, 10 * Mathf.PerlinNoise((SIZE * x + i + seed + 2) / 100f, (SIZE * y + j + seed + 2) / 100f));

            // Coordinates of the vertex within the block
            float dx = (float)NoiseLib.Range(SIZE * x + i - seed, SIZE * y + j + seed, 0.2f, 0.8f, seed);
            float dz = (float)NoiseLib.Range(SIZE * x + i + seed, SIZE * y + j - seed, 0.2f, 0.8f, 10 * seed);

            return new Vector3(dx, height, dz);

        }

        // Generate the mesh (see docs for mesh class)
        private Mesh GetMesh()
        {
            Debug.Log("Generating Mesh");
            List<Vector2> UV = new List<Vector2>();
            List<int> tris = new List<int>();
            List<Vector3> verts = new List<Vector3>();
            int count = 0;
            for (int x = 0; x < blocks.GetLength(0) - 1; x++)
            {
                for (int z = 0; z < blocks.GetLength(1) - 1; z++)
                {
                    tris.Add(count * 4);
                    tris.Add(count * 4 + 2);
                    tris.Add(count * 4 + 3);
                    tris.Add(count * 4 + 1);
                    tris.Add(count * 4 + 2);
                    tris.Add(count * 4);
                    count++;

                    UV.Add(new Vector2(0, 0));
                    UV.Add(new Vector2(1, 0));
                    UV.Add(new Vector2(1, 1));
                    UV.Add(new Vector2(0, 1));

                    verts.Add(new Vector3(x + blocks[x, z + 1].x, blocks[x, z + 1].y, z + 1 + blocks[x, z + 1].z));
                    verts.Add(new Vector3(x + 1 + blocks[x + 1, z + 1].x, blocks[x + 1, z + 1].y, z + 1 + blocks[x + 1, z + 1].z));
                    verts.Add(new Vector3(x + 1 + blocks[x + 1, z].x, blocks[x + 1, z].y, z + blocks[x + 1, z].z));
                    verts.Add(new Vector3(x + blocks[x, z].x, blocks[x, z].y, z + blocks[x, z].z));
                }
            }
            Mesh m = new Mesh()
            {
                vertices = verts.ToArray(),
                uv = UV.ToArray(),
                triangles = tris.ToArray()
            };
            return m;
        }

        // Render ofc
        public void Render()
        {
            Debug.Log("Rendering");
            chunkObj = new GameObject("Chunk");
            chunkObj.transform.position = new Vector3(SIZE * x, 0, SIZE * y);
            MeshRenderer mr = chunkObj.AddComponent<MeshRenderer>();
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
            MeshFilter mf = chunkObj.AddComponent<MeshFilter>();
            mf.mesh = GetMesh();
            mr.material = mt;
            mf.mesh.RecalculateNormals();
        }


    }

    // Noise Library 
    public static class NoiseLib
    {
        // Exponential Noise
        // For more info just Google you lazy ass
        public static float ExpNoise(int x, int y, int seed, float scale, params float[] nums)
        {
            float e = 0;
            float sum = 0;
            for (int i = 0; i < nums.Length; i++)
            {
                sum += nums[i];
                e += nums[i] * Mathf.PerlinNoise(Mathf.Pow(2, i + 1) * (x + seed) / scale, Mathf.Pow(2, i + 1) * (y + seed) / scale);
            }
            e /= sum;
            return e;
        }

        // System.Random(int seed) is a fake random function
        // For more info just Google you lazy ass
        public static int Range(int x, int y, int min, int max, int seed)
        {
            System.Random random = new System.Random(1521134295 * x + y + seed);
            return random.Next(min, max);
        }

        public static double Range(int x, int y, float min, float max, int seed)
        {
            System.Random random = new System.Random(1521134295 * x + y + seed);
            return random.NextDouble() * (max - min) + min;
        }

    }
}





