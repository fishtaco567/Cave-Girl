using UnityEngine;
using System;

namespace SharpNoise.Generators
{

    public class GeneratorSubtract : IGenerator
    {

        private IGenerator minuend;
        private IGenerator subtrahend;
        
        /// <summary>
        /// Generates the difference of the two generators
        /// </summary>
        /// <param name="minuend">The number to subtract from</param>
        /// <param name="subtrahend">The amount to subtract</param>
        public GeneratorSubtract(IGenerator minuend, IGenerator subtrahend)
        {
            this.minuend = minuend;
            this.subtrahend = subtrahend;
        }

        public float GetNoise1D(float x)
        {
            return minuend.GetNoise1D(x) - subtrahend.GetNoise1D(x);
        }

        public float GetNoise2D(Vector2 x)
        {
            return minuend.GetNoise2D(x) - subtrahend.GetNoise2D(x);
        }

        public float GetNoise3D(Vector3 x)
        {
            return minuend.GetNoise3D(x) - subtrahend.GetNoise3D(x);
        }

        public float GetNoise4D(Vector4 x)
        {
            return minuend.GetNoise4D(x) - subtrahend.GetNoise4D(x);
        }

    }

}
