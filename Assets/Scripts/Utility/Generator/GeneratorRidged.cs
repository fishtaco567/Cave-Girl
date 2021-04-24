using UnityEngine;
using System;
using Utils;

namespace SharpNoise.Generators
{

    public class GeneratorRidged : IGenerator
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

        // 1 / sqrt(0.5)
        const float NORMALIZATION_2D = 1.414213562373095f;

        // 1 / sqrt(0.75)
        const float NORMALIZATION_3D = 1.154700538379251f;

        public GeneratorRidged(int seed, float amplitude = 1, float persistence = 0.5f,
            float frequency = 1f, float freqMulti = 2f, int octaves = 1)
        {
            this.perlin = new PerlinNoise((uint) seed);
            this.Amplitude = amplitude;
            this.Persistence = persistence;
            this.Frequency = frequency;
            this.FreqMulti = freqMulti;
            this.Octaves = octaves;

            var rand = new SRandom((uint) seed);

            offsetX = (float)rand.RandomFloatZeroToOne();
            offsetY = (float)rand.RandomFloatZeroToOne();
            offsetZ = (float)rand.RandomFloatZeroToOne();
            offsetW = (float)rand.RandomFloatZeroToOne();
        }

        /// <summary>
        /// 1D Ridged Noise
        /// </summary>
        /// <param name="x">Location</param>
        /// <returns>Ridged Noise Normalized to -1, 1</returns>
        public float GetNoise1D(float x)
        {
            float curAmp = Amplitude;
            float curFreq = Frequency;
            float signal = perlin.GetNoise1D(x * Frequency + offsetX);
            signal = (Mathf.Abs(signal)) * Amplitude;

            float sumAmplitude = Amplitude;

            for (int i = 1; i < Octaves; i++) {
                curAmp *= Persistence;
                curFreq *= FreqMulti;
                float curSig = perlin.GetNoise1D(x * curFreq + offsetX);
                signal += (Mathf.Abs(curSig)) * curAmp;
                sumAmplitude += curAmp;
            }

            return signal * (Amplitude / sumAmplitude);
        }

        /// <summary>
        /// 2D Ridged Noise
        /// </summary>
        /// <param name="x">Location</param>
        /// <returns>Ridged Noise Normalized to -1, 1</returns>
        public float GetNoise2D(Vector2 x)
        {
            float curAmp = Amplitude;
            float curFreq = Frequency;
            float signal = perlin.GetNoise2D(x.x * Frequency + offsetX, x.y * Frequency + offsetY) * NORMALIZATION_2D;
            signal = (Mathf.Abs(signal)) * Amplitude;

            float sumAmplitude = Amplitude;

            for (int i = 1; i < Octaves; i++) {
                curAmp *= Persistence;
                curFreq *= FreqMulti;
                float curSig = perlin.GetNoise2D(x.x * curFreq + offsetX, x.y * curFreq + offsetY) * NORMALIZATION_2D;
                signal += (Mathf.Abs(curSig)) * curAmp;
                sumAmplitude += curAmp;
            }

            return signal * (Amplitude / sumAmplitude);
        }

        /// <summary>
        /// 3D Ridged Noise
        /// </summary>
        /// <param name="x">Location</param>
        /// <returns>Ridged Noise Normalized to -1, 1</returns>
        public float GetNoise3D(Vector3 x)
        {
            float curAmp = Amplitude;
            float curFreq = Frequency;
            float signal = perlin.GetNoise3D(x.x * Frequency + offsetX, x.y * Frequency + offsetY, x.z * Frequency + offsetZ) * NORMALIZATION_3D;
            signal = (Mathf.Abs(signal)) * Amplitude;

            float sumAmplitude = Amplitude;

            for (int i = 1; i < Octaves; i++) {
                curAmp *= Persistence;
                curFreq *= FreqMulti;
                float curSig = perlin.GetNoise3D(x.x * curFreq + offsetX, x.y * curFreq + offsetY, x.z * curFreq + offsetZ) * NORMALIZATION_3D;
                signal += (Mathf.Abs(curSig)) * curAmp;
                sumAmplitude += curAmp;
            }

            return signal * (Amplitude / sumAmplitude);
        }

        /// <summary>
        /// 4D Ridged Noise
        /// </summary>
        /// <param name="x">Location</param>
        /// <returns>Ridged Noise Normalized to -1, 1</returns>
        public float GetNoise4D(Vector4 x)
        {
            float curAmp = Amplitude;
            float curFreq = Frequency;
            float signal = perlin.GetNoise4D(x.x * Frequency + offsetX, x.y * Frequency + offsetY, x.z * Frequency + offsetZ, x.w * Frequency + offsetW);
            signal = (Mathf.Abs(signal)) * Amplitude;

            float sumAmplitude = Amplitude;

            for (int i = 1; i < Octaves; i++) {
                curAmp *= Persistence;
                curFreq *= FreqMulti;
                float curSig = perlin.GetNoise4D(x.x * curFreq + offsetX, x.y * curFreq + offsetY, x.z * curFreq + offsetZ, x.z * curFreq + offsetW);
                signal += (Mathf.Abs(curSig)) * curAmp;
                sumAmplitude += curAmp;
            }

            return signal * (Amplitude / sumAmplitude);
        }

    }

}
