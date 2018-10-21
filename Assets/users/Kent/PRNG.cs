using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generator
{
	public class HeightMap
	{
		//min and max perlinOffset values
		private const int RANDOM_RANGE = 2^16;
		//RNG seed
		public readonly int seed;
		//(largest octave's unit distance per-sample)^-1
		public readonly float frequency;
		//how many octaves
		public readonly int octaves;
		//amplitude of smaller octaves [height += perlin*p^n] where n = octave, p = persistence
		public readonly float persistance;
		//frequency of smaller octaves [frequency = (1/scale)*l^n] where n = octave, l = lacunarity
		public readonly float lacunarity;

		//generated offset values for each octave dependent on the seed
		//i mean, technically, we don't even need the seed var anymore, but fk it
		public readonly Vector2[] perlinOffsets;

		/**
			 Generates a heightmap object given the parameters
			 
			 
			 */
		public HeightMap(int seed, float scale, int octaves, float persistance, float lacunarity)
		{
			this.seed = seed;
			frequency = 1/scale;//NU EQUALS V/LAMBDA REEEEEEE
			this.octaves = octaves;
			this.persistance = persistance;
			this.lacunarity = lacunarity;

			//populate the offset array with random vectors for each octave
			perlinOffsets = new Vector2[octaves];
			System.Random random = new System.Random(seed);
			for(int i = 0; i < octaves; i++)
			{
				perlinOffsets[i] = new Vector2(random.Next(), random.Next());
			}

		}
		/**
		 * Places sampled heights into the given array, across the given rectangle
		 * elements of the array are linearly distributed, with the first element of
		 * each dimension being at x1 or y1, and the last element at x2 or y2.
		 * Note: heights are sampled as height[x][y], not height[y][x].
		 * 
		 */
		public void Populate(ref float[][] heights, float x1, float x2, float y1, float y2)
		{
			//size of the array on each axis, minus one because i feel like it
			//not really, its just because we do the last one manually so that the tiny
			//inevitable difference from floating point doesn't bump the x2 or y2 value
			//basically, i'm stubborn
			int sizeX = heights.Length-1;
			int sizeY = heights[0].Length-1;
			//sidelengths of the given rectangle
			//we trust the user to have their head on straight enough to know not to flip x1 and x2,
			//though no major exceptions will occur, it'll just flip their array but they deserve it
			float distX = x2 - x1;
			float distY = y2 - y1;
			//increment values; distance between each sample
			//note sizeX/sizeY is subtracted, so its the number of spaces, already ;)
			float dx = distX / sizeX;
			float dy = distY / sizeY;

			//fill in each coordinate
			for(int i = 0; i < sizeX; i++)
			{
				for (int j = 0; j < sizeY; j++)
				{
					heights[i][j] = SamplePoint(x1 + i*dx, y1 + j*dy);
				}
			}
			//along y2
			for (int i = 0; i <= sizeX; i++)
			{
				heights[i][sizeY] = SamplePoint(x1 + i * dx, y2);
			}
			//along x2
			for (int j = 0; j < sizeY; j++)
			{
				heights[sizeX][j] = SamplePoint(x2, y1 + j * dy);
			}
		}
		/**
		 * Returns the height value at the given point
		 * 
		 * 
		 */
		public float SamplePoint(float x, float y)
		{
			float sum = 0;
			//per octave amplitude and frequency
			float amp = 1;
			float freq = frequency;
			//iterate per octave
			for (int i = 0; i < octaves; i++)
			{
				//get the perlin sample coords
				float sX = x * freq + perlinOffsets[i].x;//lol im an adult
				float sY = x * freq + perlinOffsets[i].y;
				//adderino
				sum += amp*Mathf.PerlinNoise(sX, sY);

				//update the amplitude and frequency for the next octave
				amp *= persistance;
				freq *= lacunarity;
			}
			return sum;
		}
	}
}
