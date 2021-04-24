using UnityEngine;
using System;
using Utils;

namespace SharpNoise.Generators {

    public class GeneratorPerlin : IGenerator
    {

        private PerlinNoise perlin;

        public float Amplitude { get; set; }
        public float Persistence { get; set; }
        public float Frequency { get; set; }
        public float FreqMulti { get; set; }
        public int Octaves { get; set; }

        private float offsetX;
        private float offsetY;
        private float offsetZ;
        private float offsetW;

        //Normalization factors, generally 1 / Sqrt(N/4), aside from the one dimensional case

        // 1 / sqrt(0.5)
        const float NORMALIZATION_2D = 1.414213562373095f;

        // 1 / sqrt(0.75)
        const float NORMALIZATION_3D = 1.154700538379251f;

        public GeneratorPerlin(int seed, float amplitude = 1, float persistence = 0.5f,
            float frequency = 1f, float freqMulti = 2f, int octaves = 1)
        {
            this.perlin = new PerlinNoise((uint) seed);
            this.Amplitude = amplitude;
            this.Persistence = persistence;
            this.Frequency = frequency;
            this.FreqMulti = freqMulti;
            this.Octaves = octaves;

            var rand = new SRandom((uint) seed);

            offsetX = (float) rand.RandomFloatZeroToOne();
            offsetY = (float) rand.RandomFloatZeroToOne();
            offsetZ = (float) rand.RandomFloatZeroToOne();
            offsetW = (float) rand.RandomFloatZeroToOne();
        }

        /// <summary>
        /// 1D Perlin Noise
        /// </summary>
        /// <param name="x">Location</param>
        /// <returns>Perlin Noise Normalized to -Amplitude, Amplitude</returns>
        public float GetNoise1D(float x)
        {
            float curAmp = Amplitude;
            float curFreq = Frequency;
            float signal = perlin.GetNoise1D(x * Frequency + offsetX) * Amplitude;

            float sumAmplitude = Amplitude;

            for(int i = 1; i < Octaves; i++) {
                curAmp *= Persistence;
                curFreq *= FreqMulti;
                signal += perlin.GetNoise1D(x * curFreq + offsetX) * curAmp;
                sumAmplitude += curAmp;
            }

            return signal * (Amplitude / sumAmplitude);
        }

        /// <summary>
        /// 2D Perlin Noise
        /// </summary>
        /// <param name="x">Location</param>
        /// <returns>Perlin Noise Normalized to -Amplitude, Amplitude</returns>
        public float GetNoise2D(Vector2 x)
        {
            float curAmp = Amplitude;
            float curFreq = Frequency;
            float signal = perlin.GetNoise2D(x.x * Frequency + offsetX, x.y * Frequency + offsetY) * Amplitude;

            float sumAmplitude = Amplitude;

            for (int i = 1; i < Octaves; i++) {
                curAmp *= Persistence;
                curFreq *= FreqMulti;
                signal += perlin.GetNoise2D(x.x * curFreq + offsetX, x.y * curFreq + offsetY) * curAmp;
                sumAmplitude += curAmp;
            }

            return signal * (Amplitude / sumAmplitude) * NORMALIZATION_2D;
        }

        /// <summary>
        /// 3D Perlin Noise
        /// </summary>
        /// <param name="x">Location</param>
        /// <returns>Perlin Noise Normalized to -Amplitude, Amplitude</returns>
        public float GetNoise3D(Vector3 x)
        {
            float curAmp = Amplitude;
            float curFreq = Frequency;
            float signal = perlin.GetNoise3D(x.x * Frequency + offsetX, x.y * Frequency + offsetY, x.z * Frequency + offsetZ) * Amplitude;

            float sumAmplitude = Amplitude;

            for (int i = 1; i < Octaves; i++) {
                curAmp *= Persistence;
                curFreq *= FreqMulti;
                signal += perlin.GetNoise3D(x.x * curFreq + offsetX, x.y * curFreq + offsetY, x.z * curFreq + offsetZ) * curAmp;
                sumAmplitude += curAmp;
            }

            return signal * (Amplitude / sumAmplitude) * NORMALIZATION_3D;
        }

        /// <summary>
        /// 4D Perlin Noise
        /// </summary>
        /// <param name="x">Location</param>
        /// <returns>Perlin Noise Normalized to -Amplitude, Amplitude</returns>
        public float GetNoise4D(Vector4 x)
        {
            float curAmp = Amplitude;
            float curFreq = Frequency;
            float signal = perlin.GetNoise4D(x.x * Frequency + offsetX, x.y * Frequency + offsetY, x.z * Frequency + offsetZ, x.w * Frequency + offsetW) * Amplitude;

            float sumAmplitude = Amplitude;

            for (int i = 1; i < Octaves; i++) {
                curAmp *= Persistence;
                curFreq *= FreqMulti;
                signal += perlin.GetNoise4D(x.x * curFreq + offsetX, x.y * curFreq + offsetY, x.z * curFreq + offsetZ, x.z * curFreq + offsetW) * curAmp;
                sumAmplitude += curAmp;
            }

            return signal * (Amplitude / sumAmplitude);
        }

    }

}
