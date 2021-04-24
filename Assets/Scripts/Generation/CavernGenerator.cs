using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using SharpNoise;
using Utils;
using SharpNoise.Generators;

public class CavernGenerator : MonoBehaviour {

    [SerializeField]
    protected Tilemap tilemap;

    [SerializeField]
    protected int width;

    [SerializeField]
    protected int height;

    [SerializeField]
    protected Tile grass;

    [SerializeField]
    protected Tile stone;

    [SerializeField]
    protected float stoneThreshhold;

    [SerializeField]
    protected float circleFunctionStrength;

    [SerializeField]
    protected GameObject player;

    protected void Start() {
        GenerateMap();
    }

    protected void Update() {
        if(Input.GetKeyDown(KeyCode.P)) {
            GenerateMap();
        }
    }

    protected void GenerateMap() {
        var genPerlin = new GeneratorPerlin(System.DateTime.Now.Millisecond, 1, 0.5f, 0.05f, 2, 4);
        var genRidged = new GeneratorRidged(System.DateTime.Now.Millisecond + 1, 1, 0.5f, 0.05f, 2, 4);
        var genBPBil = new GeneratorBillowed(System.DateTime.Now.Millisecond + 2, 1, 0.5f, 0.05f, 2, 4);
        var genBPRidge = new GeneratorRidged(System.DateTime.Now.Millisecond + 3, 1, 0.5f, 0.05f, 2, 4);

        var genBP = new GeneratorDivide(new GeneratorAdd(genBPBil, genBPRidge), new GeneratorConstant(2f));

        var genBlend1 = new GeneratorAdd(new GeneratorPerlin(System.DateTime.Now.Millisecond + 4, 0.5f, 0.5f, 0.02f, 2, 1), new GeneratorConstant(1));
        var genBlend2 = new GeneratorAdd(new GeneratorPerlin(System.DateTime.Now.Millisecond + 5, 0.5f, 0.5f, 0.03f, 2, 1), new GeneratorConstant(1));

        var genBlended1 = new GeneratorBlend(genRidged, genBP, genBlend2);

        var generator = new GeneratorBlend(genBlended1, genPerlin, genBlend1);

        for(int i = 0; i < width; i++) {
            for(int j = 0; j < width; j++) {
                var circleFunction = ((i - width / 2) / (float) width) * ((i - width / 2) / (float) width) + ((j - height / 2) / (float) height) * ((j - height / 2) / (float) height);
                circleFunction *= circleFunctionStrength;

                var signal = generator.GetNoise2D(new Vector2(i, j)) - circleFunction;

                if(signal < stoneThreshhold) {
                    tilemap.SetTile(new Vector3Int(i, j, 0), stone);
                } else {
                    tilemap.SetTile(new Vector3Int(i, j, 0), grass);
                }
            }
        }

        for(int i = 0; i < 10; i++) {
            int x = 100 + i;
            int y = 100 + i;
            
            if(tilemap.GetTile(new Vector3Int(x, y, 0)) == grass) {
                player.transform.position = new Vector3(x + 0.5f, y + 0.5f, player.transform.position.z);
                break;
            }
        }
    }

}
