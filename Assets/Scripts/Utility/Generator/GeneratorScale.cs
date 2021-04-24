using UnityEngine;
using System;

namespace SharpNoise.Generators
{

    public class GeneratorScale : IGenerator
    {

        private IGenerator baseGen;
        private IGenerator scaleGenX;
        private IGenerator scaleGenY;
        private IGenerator scaleGenZ;
        private IGenerator scaleGenW;

        /// <summary>
        /// Scales a generator
        /// </summary>
        /// <param name="baseGen">The base generator</param>
        /// <param name="scaleGen">The scale factor</param>
        public GeneratorScale(IGenerator baseGen, IGenerator scaleGenX, IGenerator scaleGenY, IGenerator scaleGenZ, IGenerator scaleGenW)
        {
            this.baseGen = baseGen;
            this.scaleGenX = scaleGenX;
            this.scaleGenY = scaleGenY;
            this.scaleGenZ = scaleGenZ;
            this.scaleGenW = scaleGenW;
        }

        public float GetNoise1D(float x)
        {
            return baseGen.GetNoise1D(x * scaleGenX.GetNoise1D(x));
        }

        public float GetNoise2D(Vector2 x)
        {
            return baseGen.GetNoise2D(new Vector2(x.x * scaleGenX.GetNoise2D(x), x.y * scaleGenY.GetNoise2D(x)));
        }

        public float GetNoise3D(Vector3 x)
        {
            return baseGen.GetNoise3D(new Vector3(x.x * scaleGenX.GetNoise3D(x), x.y * scaleGenY.GetNoise3D(x), x.z * scaleGenZ.GetNoise3D(x)));
        }

        public float GetNoise4D(Vector4 x)
        {
            return baseGen.GetNoise4D(new Vector4(x.x * scaleGenX.GetNoise4D(x), x.y * scaleGenY.GetNoise4D(x), x.z * scaleGenZ.GetNoise4D(x), x.w * scaleGenW.GetNoise4D(x)));
        }

    }

}