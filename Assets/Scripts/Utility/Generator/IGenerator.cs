using UnityEngine;

namespace SharpNoise.Generators
{

    public interface IGenerator
    {

        float GetNoise1D(float x);

        float GetNoise2D(Vector2 x);

        float GetNoise3D(Vector3 x);

        float GetNoise4D(Vector4 x);

    }

}
