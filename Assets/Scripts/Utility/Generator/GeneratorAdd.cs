using UnityEngine;
using System;

namespace SharpNoise.Generators
{

    public class GeneratorAdd : IGenerator
    {

        private IGenerator[] parents;

        /// <summary>
        /// Adds all given generators
        /// </summary>
        /// <param name="parents">Generators to add</param>
        public GeneratorAdd(params IGenerator[] parents)
        {
            this.parents = parents;
        }
        
        public float GetNoise1D(float x)
        {
            float signal = 0;

            for(int i = 0; i < parents.Length; i++) {
                signal += parents[i].GetNoise1D(x);
            }

            return signal;
        }

        public float GetNoise2D(Vector2 x)
        {
            float signal = 0;

            for (int i = 0; i < parents.Length; i++) {
                signal += parents[i].GetNoise2D(x);
            }

            return signal;
        }

        public float GetNoise3D(Vector3 x)
        {
            float signal = 0;

            for (int i = 0; i < parents.Length; i++) {
                signal += parents[i].GetNoise3D(x);
            }

            return signal;
        }
        
        public float GetNoise4D(Vector4 x)
        {
            float signal = 0;

            for (int i = 0; i < parents.Length; i++) {
                signal += parents[i].GetNoise4D(x);
            }

            return signal;
        }

    }

}
