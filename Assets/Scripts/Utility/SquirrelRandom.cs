using System;
using UnityEngine;

namespace Utils {
    public class SRandom {

        private uint seed;
        private int position;

        public SRandom(uint seed, int position = 0) {
            this.seed = seed;
            this.position = position;
        }

        public void ResetSeed(uint seed, int position = 0) {
            this.seed = seed;
            this.position = position;
        }

        public uint GetSeed() {
            return seed;
        }

        public void SetCurrentPosition(int position) {
            this.position = position;
        }

        public int GetCurrentPosition() {
            return position;
        }

        public uint RandomUInt() {
            return SquirrelNoise.Get1DNoiseUint(position++, seed);
        }

        public int RandomInt() {
            return unchecked((int) SquirrelNoise.Get1DNoiseUint(position++, seed));
        }

        public int RandomIntLessThan(int lessThan) {
            return (int) Math.Floor(SquirrelNoise.Get1DNoiseZeroToOne(position++, seed) * lessThan);
        }

        public int RandomIntInRange(int lowerInclusive, int upperInclusive) {
            return (int)Math.Floor(SquirrelNoise.Get1DNoiseZeroToOne(position++, seed) * (upperInclusive - lowerInclusive + 1) + lowerInclusive);
        }

        public float RandomFloatZeroToOne() {
            return SquirrelNoise.Get1DNoiseZeroToOne(position++, seed);
        }

        public float RandomFloatNegativeOneToOne() {
            return SquirrelNoise.Get1DNoiseNegativeOneToOne(position++, seed);
        }

        public float RandomFloatInRange(float lowerInclusive, float upperInclusive) {
            return (SquirrelNoise.Get1DNoiseNegativeOneToOne(position++, seed) + 1) * ((upperInclusive - lowerInclusive) / 2) + lowerInclusive;
        }

        public bool RandomChance(float probabilityTrue) {
            return SquirrelNoise.Get1DNoiseZeroToOne(position++, seed) < probabilityTrue;
        }

        public Vector2 RandomDirection2D() {
            var theta = RandomFloatInRange(0, 2 * (float) Math.PI);
            return new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));
        }

    }
}
