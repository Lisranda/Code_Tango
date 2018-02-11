using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise {

	int seed;

	float frequency;
	float lacunarity;

	float amplitude;
	float persistance;

	int octaves;


	public Noise (int seed, float frequency, float lacunarity, float amplitude, float persistance, int octaves){
		this.seed = seed;
		this.frequency = frequency;
		this.lacunarity = lacunarity;
		this.amplitude = amplitude;
		this.persistance = persistance;
		this.octaves = octaves;
	}

	public float[,] GetPerlin(int width, int height){
		float[,] perlinValues = new float[width, height];

		for (int i = 0; i < width; i++) {
			for (int o = 0; o < height; o++) {
				perlinValues [i, o] = 0f;
				float tempAmp = amplitude;
				float tempFreq = frequency;

				for (int p = 0; p < octaves; p++) {
					perlinValues [i, o] += Mathf.PerlinNoise (i / (float)width * frequency, o / (float)height * frequency) * amplitude;
					frequency *= lacunarity;
					amplitude *= persistance;
				}

				amplitude = tempAmp;
				frequency = tempFreq;
			}
		}


		return perlinValues;
	}
}
