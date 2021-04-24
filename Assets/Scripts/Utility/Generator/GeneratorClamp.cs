using UnityEngine;
using System;

namespace SharpNoise.Generators
{

    public class GeneratorClamp : IGenerator
    {

        private IGenerator baseGen;
        private IGenerator min;
        private IGenerator max;
        
        public GeneratorClamp(IGenerator baseGen, IGenerator min, IGenerator max)
        {
            this.baseGen = baseGen;
            this.max = max;
            this.min = min;
        }

        public float GetNoise1D(float x)
        {
            return Mathf.Clamp(baseGen.GetNoise1D(x), min.GetNoise1D(x), max.GetNoise1D(x));
        }

        public float GetNoise2D(Vector2 x)
        {
            return Mathf.Clamp(baseGen.GetNoise2D(x), min.GetNoise2D(x), max.GetNoise2D(x));
        }

        public float GetNoise3D(Vector3 x)
        {
            return Mathf.Clamp(baseGen.GetNoise3D(x), min.GetNoise3D(x), max.GetNoise3D(x));
        }

        public float GetNoise4D(Vector4 x)
        {
            return Mathf.Clamp(baseGen.GetNoise4D(x), min.GetNoise4D(x), max.GetNoise4D(x));
        }

    }

}
