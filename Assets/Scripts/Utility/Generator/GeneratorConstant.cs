using UnityEngine;
using System;

namespace SharpNoise.Generators
{

    public class GeneratorConstant : IGenerator
    {

        private float constant;

        /// <summary>
        /// Generates a constant number
        /// </summary>
        /// <param name="constant">The number to generate</param>
        public GeneratorConstant(float constant)
        {
            this.constant = constant;
        }

        public float GetNoise1D(float x)
        {
            return constant;
        }

        public float GetNoise2D(Vector2 x)
        {
            return constant;
        }

        public float GetNoise3D(Vector3 x)
        {
            return constant;
        }

        public float GetNoise4D(Vector4 x)
        {
            return constant;
        }

    }

}
