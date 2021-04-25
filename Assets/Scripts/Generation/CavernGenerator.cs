using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using SharpNoise;
using Utils;
using SharpNoise.Generators;
using Entities;
using Entities.Character;

public class CavernGenerator : MonoBehaviour {

    [SerializeField]
    protected Tilemap tilemap;

    [SerializeField]
    protected Tilemap undermap;

    [SerializeField]
    protected int width;

    [SerializeField]
    protected int height;

    [SerializeField]
    protected TileBase grass;

    [SerializeField]
    protected TileBase stone;

    [SerializeField]
    protected float stoneThreshhold;

    [SerializeField]
    protected float circleFunctionStrength;

    [SerializeField]
    protected int randomWalkLength;

    [SerializeField]
    protected float directionChangeChance;

    [SerializeField]
    protected float towardPlayerChance;

    [SerializeField]
    protected Player player;

    [SerializeField]
    protected GameObject stairs;

    [SerializeField]
    protected Tile trailTile;

    protected TilemapHardness hardness;
    protected SRandom rand;

    protected void Start() {
        hardness = tilemap.GetComponent<TilemapHardness>();
        rand = new SRandom((uint)System.DateTime.Now.Millisecond);
        GenerateMap();
    }

    protected void Update() {
        if(Input.GetKeyDown(KeyCode.P)) {
        //    GenerateMap();
        }
    }

    public void GenerateMap() {
        player = GameManager.Instance.player;

        var genPerlin = new GeneratorPerlin(System.DateTime.Now.Millisecond, 1, 0.5f, 0.05f, 2, 4);
        var genRidged = new GeneratorRidged(System.DateTime.Now.Millisecond + 1, 1, 0.5f, 0.05f, 2, 4);
        var genBPBil = new GeneratorBillowed(System.DateTime.Now.Millisecond + 2, 1, 0.5f, 0.05f, 2, 4);
        var genBPRidge = new GeneratorRidged(System.DateTime.Now.Millisecond + 3, 1, 0.5f, 0.05f, 2, 4);

        var genBP = new GeneratorDivide(new GeneratorAdd(genBPBil, genBPRidge), new GeneratorConstant(2f));

        var genBlend1 = new GeneratorAdd(new GeneratorPerlin(System.DateTime.Now.Millisecond + 4, 0.5f, 0.5f, 0.02f, 2, 1), new GeneratorConstant(1));
        var genBlend2 = new GeneratorAdd(new GeneratorPerlin(System.DateTime.Now.Millisecond + 5, 0.5f, 0.5f, 0.03f, 2, 1), new GeneratorConstant(1));

        var genBlended1 = new GeneratorBlend(genRidged, genBP, genBlend2);

        var generator = new GeneratorBlend(genBlended1, genPerlin, genBlend1);

        hardness.hardness = new int[width * height];
        hardness.width = width;
        hardness.height = height;

        TileBase[] tilesCollision = new TileBase[width * height];
        TileBase[] tilesUnder = new TileBase[width * height];

        for(int i = 0; i < width - 1; i++) {
            for(int j = 0; j < height - 1; j++) {
                var circleFunction = ((i - width / 2) / (float) width) * ((i - width / 2) / (float) width) + ((j - height / 2) / (float) height) * ((j - height / 2) / (float) height);
                circleFunction *= circleFunctionStrength;

                var signal = generator.GetNoise2D(new Vector2(i, j)) - circleFunction;

                var index = i + j * width;

                if(signal < stoneThreshhold) {
                    tilesCollision[i + j * width] = stone;
                    tilesCollision[(i + 1) + j * width] = stone;
                    tilesCollision[i + (j + 1) * width] = stone;
                    tilesCollision[(i + 1) + (j + 1) * width] = stone;
                    hardness.hardness[index] = 1;
                    hardness.hardness[index + 1] = 1;
                    hardness.hardness[index + width] = 1;
                    hardness.hardness[index + width + 1] = 1;
                }

                tilesUnder[i + j * width] = grass;
            }
        }

        tilemap.SetTilesBlock(new BoundsInt(0, 0, 0, width, height, 1), tilesCollision);
        undermap.SetTilesBlock(new BoundsInt(0, 0, 0, width, height, 1), tilesUnder);

        var chosenX = 100;
        var chosenY = 100;
        for(int i = 0; i < 20; i++) {
            for(int j = 0; j < 20; j++) {
                int x = 100 + i;
                int y = 100 + j;

                if(tilesCollision[x + y * width] == null) {
                    player.transform.position = new Vector3(x + 0.5f, y + 0.5f, player.transform.position.z);

                    chosenX = x;
                    chosenY = y;
                    break;
                }
            }
        }

        var playerPos = new Vector2Int(chosenX, chosenY);
        var walkPos = new Vector2Int(chosenX, chosenY);

        Walk(playerPos, walkPos);

        player.tilemap = tilemap;
        player.hardness = hardness;
    }

    public void Walk(Vector2Int playerPos, Vector2Int walkPos) {
        Vector2Int walkDir = new Vector2Int(1, 0);

        Vector2Int furtherLocation = walkPos;
        int maxDistance = 0;

        for(int i = 0; i < randomWalkLength; i++) {
            var playerDelta = playerPos - walkPos;
            if(hardness.hardness[(walkPos.x + walkDir.x) + (walkPos.y + walkDir.y) * width] == 0) {
                walkPos += walkDir;
            } else {
                var newWalkDir = RandomDirection(walkPos, playerPos);
                if(newWalkDir == playerDelta || walkDir == newWalkDir || walkDir == -walkDir) {
                    walkDir = RandomDirection(walkPos, playerPos);
                } else {
                    walkDir = newWalkDir;
                }
                continue;
            }

            if(rand.RandomChance(directionChangeChance)) {
                var newWalkDir = RandomDirection(walkPos, playerPos);
                if(newWalkDir == playerDelta || walkDir == newWalkDir || newWalkDir == -walkDir) {
                    walkDir = RandomDirection(walkPos, playerPos);
                }
            }

            if(playerDelta.x + playerDelta.y > maxDistance) {
                maxDistance = playerDelta.x + playerDelta.y;
                furtherLocation = walkPos;
            }
        }

        Instantiate(stairs).transform.position = new Vector3(furtherLocation.x + 0.5f, furtherLocation.y + 0.5f, -0.1f);
    }

    protected Vector2Int RandomDirection(Vector2Int curPos, Vector2Int playerPos) {
        var randChoice = rand.RandomIntLessThan(4);

        switch(randChoice) {
            case 0:
                return new Vector2Int(-1, 0);
            case 1:
                return new Vector2Int(1, 0);
            case 2:
                return new Vector2Int(0, -1);
            case 3:
                return new Vector2Int(0, 1);
        }

        return new Vector2Int(1, 0);
    }

    protected Vector2Int PrimaryDirection(Vector2Int v) {
        if(Mathf.Abs(v.x) > Mathf.Abs(v.y)) {
            return new Vector2Int((int)Mathf.Sign(v.x), 0);
        } else {
            return new Vector2Int(0, (int)Mathf.Sign(v.y));
        }
    } 

}
