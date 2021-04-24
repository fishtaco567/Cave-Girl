using System;

namespace SharpNoise
{
    public class ValueNoise
    {

        private const int REPITITION_SIZE = 255;

        private int[] perm;

        private const float PERM_TO_n1_1 = 1f / 128f;

        public ValueNoise(int seed)
        {
            Random rand = new Random(seed);
            perm = new int[(REPITITION_SIZE + 1) * 2];
            for (int i = 0; i < REPITITION_SIZE; i++) {
                perm[i] = i;
            }

            for (int i = 0; i < REPITITION_SIZE; i++) {
                int toSwitch = rand.Next(REPITITION_SIZE);
                int temp = perm[toSwitch];
                perm[toSwitch] = i;
                perm[i] = temp;
                perm[toSwitch + REPITITION_SIZE] = i;
                perm[i + REPITITION_SIZE] = temp;
            }
        }

        public float GetNoise1D(float x)
        {
            int xGridPoint = FastFloor(x); //Find the cell the specified number is in
            x -= xGridPoint;

            xGridPoint &= REPITITION_SIZE; //Wrap the cell to REPITITION_SIZE, our chosen repeat size

            float c0 = perm[xGridPoint] * PERM_TO_n1_1 - 1;
            float c1 = perm[xGridPoint + 1] * PERM_TO_n1_1 - 1;

            return Lerp(c0, c1, Fade(x)); //Interpolate across the X axis
        }

        public float GetNoise2D(float x, float y)
        {
            int xGridPoint = FastFloor(x); //Find the cell the specified number is in
            int yGridPoint = FastFloor(y);
            x -= xGridPoint;
            y -= yGridPoint;

            xGridPoint &= REPITITION_SIZE; //Wrap the cell to REPITITION_SIZE, our chosen repeat size
            yGridPoint &= REPITITION_SIZE;

            float c00 = perm[xGridPoint + perm[yGridPoint]] * PERM_TO_n1_1 - 1; //Dot products
            float c10 = perm[xGridPoint + 1 + perm[yGridPoint]] * PERM_TO_n1_1 - 1;
            float c01 = perm[xGridPoint + perm[yGridPoint + 1]] * PERM_TO_n1_1 - 1;
            float c11 = perm[xGridPoint + 1 + perm[yGridPoint + 1]] * PERM_TO_n1_1 - 1;

            float fadeX = Fade(x); //Pre-compute the fade curve for x because it's used more than once

            float x0 = Lerp(c00, c10, fadeX); //Interpolate across the X axis
            float x1 = Lerp(c01, c11, fadeX);

            return Lerp(x0, x1, Fade(y)); //Interpolate across the Y axis
        }

        public float GetNoise3D(float x, float y, float z)
        {
            int xGridPoint = FastFloor(x); //Find the cell the specified number is in
            int yGridPoint = FastFloor(y);
            int zGridPoint = FastFloor(z);
            x -= xGridPoint;
            y -= yGridPoint;
            z -= zGridPoint;

            xGridPoint &= REPITITION_SIZE; //Wrap the cell to REPITITION_SIZE, our chosen repeat size
            yGridPoint &= REPITITION_SIZE;
            zGridPoint &= REPITITION_SIZE;

            float c000 = perm[xGridPoint + perm[yGridPoint + perm[zGridPoint]]] * PERM_TO_n1_1 - 1; //Dot products
            float c100 = perm[xGridPoint + 1 + perm[yGridPoint + perm[zGridPoint]]] * PERM_TO_n1_1 - 1;
            float c010 = perm[xGridPoint + perm[yGridPoint + 1 + perm[zGridPoint]]] * PERM_TO_n1_1 - 1;
            float c110 = perm[xGridPoint + 1 + perm[yGridPoint + 1 + perm[zGridPoint]]] * PERM_TO_n1_1 - 1;
            float c001 = perm[xGridPoint + perm[yGridPoint + perm[zGridPoint + 1]]] * PERM_TO_n1_1 - 1;
            float c101 = perm[xGridPoint + 1 + perm[yGridPoint + perm[zGridPoint + 1]]] * PERM_TO_n1_1 - 1;
            float c011 = perm[xGridPoint + perm[yGridPoint + 1 + perm[zGridPoint + 1]]] * PERM_TO_n1_1 - 1;
            float c111 = perm[xGridPoint + 1 + perm[yGridPoint + 1 + perm[zGridPoint + 1]]] * PERM_TO_n1_1 - 1;

            float fadeX = Fade(x); //Pre-compute the fade curve for x and y because they're used more than once
            float fadeY = Fade(y);

            float x00 = Lerp(c000, c100, fadeX); //Interpolate across the X axis
            float x10 = Lerp(c010, c110, fadeX);
            float x01 = Lerp(c001, c101, fadeX);
            float x11 = Lerp(c011, c111, fadeX);

            float y0 = Lerp(x00, x10, fadeY); //Interpolate across the Y axis
            float y1 = Lerp(x01, x11, fadeY);

            return Lerp(y0, y1, Fade(z)); //Interpolate across the z axis
        }

        public float GetNoise4D(float x, float y, float z, float w)
        {
            int xGridPoint = FastFloor(x); //Find the cell the specified number is in
            int yGridPoint = FastFloor(y);
            int zGridPoint = FastFloor(z);
            int wGridPoint = FastFloor(w);
            x -= xGridPoint;
            y -= yGridPoint;
            z -= zGridPoint;
            w -= wGridPoint;

            xGridPoint &= REPITITION_SIZE; //Wrap the cell to REPITITION_SIZE, our chosen repeat size
            yGridPoint &= REPITITION_SIZE;
            zGridPoint &= REPITITION_SIZE;
            wGridPoint &= REPITITION_SIZE;

            float c0000 = perm[xGridPoint + perm[yGridPoint + perm[zGridPoint + perm[wGridPoint]]]] * PERM_TO_n1_1 - 1; //Dot products
            float c1000 = perm[xGridPoint + 1 + perm[yGridPoint + perm[zGridPoint + perm[wGridPoint]]]] * PERM_TO_n1_1 - 1;
            float c0100 = perm[xGridPoint + perm[yGridPoint + 1 + perm[zGridPoint + perm[wGridPoint]]]] * PERM_TO_n1_1 - 1;
            float c1100 = perm[xGridPoint + 1 + perm[yGridPoint + 1 + perm[zGridPoint + perm[wGridPoint]]]] * PERM_TO_n1_1 - 1;
            float c0010 = perm[xGridPoint + perm[yGridPoint + perm[zGridPoint + 1 + perm[wGridPoint]]]] * PERM_TO_n1_1 - 1;
            float c1010 = perm[xGridPoint + 1 + perm[yGridPoint + perm[zGridPoint + 1 + perm[wGridPoint]]]] * PERM_TO_n1_1 - 1;
            float c0110 = perm[xGridPoint + perm[yGridPoint + 1 + perm[zGridPoint + 1 + perm[wGridPoint]]]] * PERM_TO_n1_1 - 1;
            float c1110 = perm[xGridPoint + 1 + perm[yGridPoint + 1 + perm[zGridPoint + 1 + perm[wGridPoint]]]] * PERM_TO_n1_1 - 1;
            float c0001 = perm[xGridPoint + perm[yGridPoint + perm[zGridPoint + perm[wGridPoint + 1]]]] * PERM_TO_n1_1 - 1;
            float c1001 = perm[xGridPoint + 1 + perm[yGridPoint + perm[zGridPoint + perm[wGridPoint + 1]]]] * PERM_TO_n1_1 - 1;
            float c0101 = perm[xGridPoint + perm[yGridPoint + 1 + perm[zGridPoint + perm[wGridPoint + 1]]]] * PERM_TO_n1_1 - 1;
            float c1101 = perm[xGridPoint + 1 + perm[yGridPoint + 1 + perm[zGridPoint + perm[wGridPoint + 1]]]] * PERM_TO_n1_1 - 1;
            float c0011 = perm[xGridPoint + perm[yGridPoint + perm[zGridPoint + 1 + perm[wGridPoint + 1]]]] * PERM_TO_n1_1 - 1;
            float c1011 = perm[xGridPoint + 1 + perm[yGridPoint + perm[zGridPoint + 1 + perm[wGridPoint + 1]]]] * PERM_TO_n1_1 - 1;
            float c0111 = perm[xGridPoint + perm[yGridPoint + 1 + perm[zGridPoint + 1 + perm[wGridPoint + 1]]]] * PERM_TO_n1_1 - 1;
            float c1111 = perm[xGridPoint + 1 + perm[yGridPoint + 1 + perm[zGridPoint + 1 + perm[wGridPoint + 1]]]] * PERM_TO_n1_1 - 1;

            float fadeX = Fade(x); //Pre-compute the fade curve for x, y and z because they're used more than once
            float fadeY = Fade(y);
            float fadeZ = Fade(z);

            float x000 = Lerp(c0000, c1000, fadeX); //Interpolate across the X axis
            float x100 = Lerp(c0100, c1100, fadeX);
            float x010 = Lerp(c0010, c1010, fadeX);
            float x110 = Lerp(c0110, c1110, fadeX);
            float x001 = Lerp(c0001, c1001, fadeX);
            float x101 = Lerp(c0101, c1101, fadeX);
            float x011 = Lerp(c0011, c1011, fadeX);
            float x111 = Lerp(c0111, c1111, fadeX);

            float y00 = Lerp(x000, x100, fadeY); //Interpolate across the Y axis
            float y10 = Lerp(x010, x110, fadeY);
            float y01 = Lerp(x001, x101, fadeY);
            float y11 = Lerp(x011, x111, fadeY);

            float z0 = Lerp(y00, y10, fadeZ); //Interpolate across the Z axis
            float z1 = Lerp(y01, y11, fadeZ);

            return Lerp(z0, z1, Fade(w)); //Interpolate across the W axis
        }

        private static float Dot(int[] grad, float x, float y, float z, float w)
        {
            return grad[0] * x + grad[1] * y + grad[2] * z + grad[3] * w;
        }

        private static float Dot(int[] grad, float x, float y, float z)
        {
            return grad[0] * x + grad[1] * y + grad[2] * z;
        }

        private static float Dot(float[] grad, float x, float y)
        {
            return grad[0] * x + grad[1] * y;
        }

        private static float Lerp(float x, float y, float n)
        {
            return x + n * (y - x);
        }

        private static float Fade(float n)
        {
            return n * n * n * (n * (n * 6 - 15) + 10);
        }

        private static int FastFloor(float x)
        {
            return x > 0 ? (int)x : (int)x - 1;
        }

    }
}