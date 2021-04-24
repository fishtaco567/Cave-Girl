using System;
using UnityEngine;

namespace SharpNoise
{
    public class Voronoi
    {

        public enum DistanceMetric
        {
            EUCLIDEAN,
            TAXICAB,
            CHEBYSHEV
        }

        private const int REPITITION_SIZE = 255;

        private int[] perm;

        private const float PERM_TO_0_1 = 1f / 255f;

        private DistanceMetric metric;

        public Voronoi(int seed, DistanceMetric metric, float falloffDistance)
        {
            System.Random rand = new System.Random(seed);
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

            this.metric = metric;
        }

        public float GetNoise1D(float x)
        {
            float highestFraction = 0;

            int xGrid = FastFloor(x);

            const int len = 3;

            float[] xPos = new float[len];

            int index = 0;
            for(int i = -1; i <= 1; i++) {
                xPos[index] = xGrid + i + perm[(xGrid + i) % REPITITION_SIZE];
                index++;
            }

            for(int i = 0; i < len; i++) {
                float d1 = Distance1D(x, xPos[i]);
                for(int j = 0; j < len; j++) {
                    float d2 = Distance1D(x, xPos[j]);
                    highestFraction = Mathf.Max((2 * d1) / (d1 + d2), (2 * d2) / (d1 + d2), highestFraction);
                }
            }

            return highestFraction;
        }

        public float GetNoise2D(float x, float y)
        {
            float highestFraction = 0;

            int xGrid = FastFloor(x);
            int yGrid = FastFloor(y);

            const int len = 9;

            float[] xPos = new float[len];
            float[] yPos = new float[len];

            int index = 0;
            for (int i = -1; i <= 1; i++) {
                for (int j = -1; j <= 1; j++) {
                    xPos[index] = xGrid + i + perm[(xGrid + i) % REPITITION_SIZE + perm[(yGrid + j) % REPITITION_SIZE]];
                    yPos[index] = yGrid + j + perm[(yGrid + j) % REPITITION_SIZE + perm[(xGrid + i) % REPITITION_SIZE]];
                    index++;
                }
            }

            for (int i = 0; i < len; i++) {
                float d1 = Distance2D(x, y, xPos[i], yPos[i]);
                for (int j = 0; j < len; j++) {
                    float d2 = Distance2D(x, y, xPos[i], yPos[i]);
                    highestFraction = Mathf.Max((2 * d1) / (d1 + d2), (2 * d2) / (d1 + d2), highestFraction);
                }
            }

            return highestFraction;
        }

        public float GetNoise3D(float x, float y, float z)
        {
            float highestFraction = 0;

            int xGrid = FastFloor(x);
            int yGrid = FastFloor(y);
            int zGrid = FastFloor(z);

            const int len = 27;

            float[] xPos = new float[len];
            float[] yPos = new float[len];
            float[] zPos = new float[len];

            int index = 0;
            for (int i = -1; i <= 1; i++) {
                for (int j = -1; j <= 1; j++) {
                    for (int k = -1; k <= 1; k++) {
                        xPos[index] = xGrid + i + perm[(xGrid + i) % REPITITION_SIZE + perm[(yGrid + j) % REPITITION_SIZE + perm[(zGrid + k) % REPITITION_SIZE]]];
                        yPos[index] = yGrid + i + perm[(yGrid + j) % REPITITION_SIZE + perm[(zGrid + k) % REPITITION_SIZE + perm[(xGrid + i) % REPITITION_SIZE]]];
                        zPos[index] = zGrid + i + perm[(zGrid + k) % REPITITION_SIZE + perm[(xGrid + i) % REPITITION_SIZE + perm[(yGrid + j) % REPITITION_SIZE]]];
                        index++;
                    }
                }
            }

            for (int i = 0; i < len; i++) {
                float d1 = Distance2D(x, y, xPos[i], yPos[i]);
                for (int j = 0; j < len; j++) {
                    float d2 = Distance2D(x, y, xPos[i], yPos[i]);
                    highestFraction = Mathf.Max((2 * d1) / (d1 + d2), (2 * d2) / (d1 + d2), highestFraction);
                }
            }

            return highestFraction;
        }

        public float GetNoise4D(float x, float y, float z, float w)
        {
            float highestFraction = 0;

            int xGrid = FastFloor(x);
            int yGrid = FastFloor(y);
            int zGrid = FastFloor(z);
            int wGrid = FastFloor(w);

            const int len = 81;

            float[] xPos = new float[len];
            float[] yPos = new float[len];
            float[] zPos = new float[len];
            float[] wPos = new float[len];

            int index = 0;
            for (int i = -1; i <= 1; i++) {
                for (int j = -1; j <= 1; j++) {
                    for (int k = -1; k <= 1; k++) {
                        for (int l = -1; l <= 1; l++) {
                            xPos[index] = xGrid + i + perm[(xGrid + i) % REPITITION_SIZE
                                + perm[(yGrid + j) % REPITITION_SIZE
                                + perm[(zGrid + k) % REPITITION_SIZE
                                + perm[(wGrid + l) % REPITITION_SIZE]]]];
                            yPos[index] = yGrid + i + perm[(yGrid + j) % REPITITION_SIZE
                                + perm[(zGrid + k) % REPITITION_SIZE
                                + perm[(wGrid + l) % REPITITION_SIZE
                                + perm[(xGrid + i) % REPITITION_SIZE]]]];
                            zPos[index] = zGrid + i + perm[(zGrid + k) % REPITITION_SIZE
                                + perm[(wGrid + l) % REPITITION_SIZE
                                + perm[(xGrid + i) % REPITITION_SIZE
                                + perm[(yGrid + j) % REPITITION_SIZE]]]];
                            wPos[index] = wGrid + i + perm[(wGrid + l) % REPITITION_SIZE
                                + perm[(xGrid + i) % REPITITION_SIZE
                                + perm[(yGrid + j) % REPITITION_SIZE
                                + perm[(zGrid + k) % REPITITION_SIZE]]]];
                            index++;
                        }
                    }
                }
            }

            for (int i = 0; i < len; i++) {
                float d1 = Distance2D(x, y, xPos[i], yPos[i]);
                for (int j = 0; j < len; j++) {
                    float d2 = Distance2D(x, y, xPos[i], yPos[i]);
                    highestFraction = Mathf.Max((2 * d1) / (d1 + d2), (2 * d2) / (d1 + d2), highestFraction);
                }
            }

            return highestFraction;
        }

        private float Distance1D(float x, float y)
        {
            return Math.Abs(x - y);
        }

        private float Distance2D(float x1, float y1, float x2, float y2)
        {
            switch(metric) {
                case DistanceMetric.EUCLIDEAN:
                    return Mathf.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
                case DistanceMetric.TAXICAB:
                    return Mathf.Abs(x1 - x2) + Mathf.Abs(y1 - y2);
                case DistanceMetric.CHEBYSHEV:
                    return Mathf.Max(x1 - x2, y1 - y2);
                default:
                    return 0;
            }
        }

        private float Distance3D(float x1, float y1, float z1, float x2, float y2, float z2)
        {
            switch (metric) {
                case DistanceMetric.EUCLIDEAN:
                    return Mathf.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2) + (z1 - z2) * (z1 - z2));
                case DistanceMetric.TAXICAB:
                    return Mathf.Abs(x1 - x2) + Mathf.Abs(y1 - y2) + Mathf.Abs(z1 - z2);
                case DistanceMetric.CHEBYSHEV:
                    return Mathf.Max(x1 - x2, y1 - y2, z1 - z2);
                default:
                    return 0;
            }
        }

        private float Distance4D(float x1, float y1, float z1, float w1, float x2, float y2, float z2, float w2)
        {
            switch (metric) {
                case DistanceMetric.EUCLIDEAN:
                    return Mathf.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2) + (z1 - z2) * (z1 - z2) + (w1 - w2) * (w1 - w2));
                case DistanceMetric.TAXICAB:
                    return Mathf.Abs(x1 - x2) + Mathf.Abs(y1 - y2) + Mathf.Abs(z1 - z2) + Mathf.Abs(w1 - w2);
                case DistanceMetric.CHEBYSHEV:
                    return Mathf.Max(x1 - x2, y1 - y2, z1 - z2, w1 - w2);
                default:
                    return 0;
            }
        }

        private static int FastFloor(float x)
        {
            return x > 0 ? (int)x : (int)x - 1;
        }

    }
}
