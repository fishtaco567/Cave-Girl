using UnityEngine;
using System;

namespace SharpNoise.Generators
{

    public class GeneratorDivide : IGenerator
    {

        private IGenerator dividend;
        private IGenerator divisor;

        /// <summary>
        /// Generates the fraction of the two generators
        /// </summary>
        /// <param name="dividend">The number to subtract from</param>
        /// <param name="divisor">The amount to subtract</param>
        public GeneratorDivide(IGenerator dividend, IGenerator divisor)
        {
            this.dividend = dividend;
            this.divisor = divisor;
        }

        public float GetNoise1D(float x)
        {
            return dividend.GetNoise1D(x) / divisor.GetNoise1D(x);
        }

        public float GetNoise2D(Vector2 x)
        {
            return dividend.GetNoise2D(x) / divisor.GetNoise2D(x);
        }

        public float GetNoise3D(Vector3 x)
        {
            return dividend.GetNoise3D(x) / divisor.GetNoise3D(x);
        }

        public float GetNoise4D(Vector4 x)
        {
            return dividend.GetNoise4D(x) / divisor.GetNoise4D(x);
        }

    }

}
