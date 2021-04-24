using UnityEngine;
using System;

namespace SharpNoise.Generators
{

    public class GeneratorBlend : IGenerator
    {

        private IGenerator baseGen1;
        private IGenerator baseGen2;
        private IGenerator blender;

        /// <summary>
        /// Blends two generators together
        /// </summary>
        /// <param name="baseGen1">"From" Generator</param>
        /// <param name="baseGen2">"To" Generator</param>
        /// <param name="blender">Blend factor, normalized to (0, 1)</param>
        public GeneratorBlend(IGenerator baseGen1, IGenerator baseGen2, IGenerator blender)
        {
            this.baseGen1 = baseGen1;
            this.baseGen2 = baseGen2;
            this.blender = blender;
        }

        public float GetNoise1D(float x)
        {
            return Mathf.Lerp(baseGen1.GetNoise1D(x), baseGen2.GetNoise1D(x), blender.GetNoise1D(x));
        }

        public float GetNoise2D(Vector2 x)
        {
            return Mathf.Lerp(baseGen1.GetNoise2D(x), baseGen2.GetNoise2D(x), blender.GetNoise2D(x));
        }

        public float GetNoise3D(Vector3 x)
        {
            return Mathf.Lerp(baseGen1.GetNoise3D(x), baseGen2.GetNoise3D(x), blender.GetNoise3D(x));
        }

        public float GetNoise4D(Vector4 x)
        {
            return Mathf.Lerp(baseGen1.GetNoise4D(x), baseGen2.GetNoise4D(x), blender.GetNoise4D(x));
        }

    }

}