using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

public class TilemapHardness : MonoBehaviour {

    public int width;
    public int height;
    public int[] hardness;

    public Tile underWallTile;

    public int GetHardness(int i, int j) {
        return hardness[i + j * width];
    }

}
