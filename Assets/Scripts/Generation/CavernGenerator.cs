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
    protected Tilemap bad;

    [SerializeField]
    protected int width;

    [SerializeField]
    protected int height;

    [SerializeField]
    protected TileBase grass;

    [SerializeField]
    protected TileBase stone;

    [SerializeField]
    protected TileBase path;

    [SerializeField]
    protected TileBase moss;

    [SerializeField]
    protected TileBase[] decor;

    [SerializeField]
    protected TileBase[] deadly;

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

    [SerializeField]
    protected GameObject upgradeShrinePrefab;

    [SerializeField]
    protected AnimationCurve numUpgradeShrinesPerDepth;

    [SerializeField]
    protected Effect[] possibleEffects;
    [SerializeField]
    protected float[] possibleEffectWeights;

    [SerializeField]
    protected GameObject holder;

    [SerializeField]
    protected BasicEnemySpawner spawner;

    [SerializeField]
    protected GameObject bombsPickup;

    [SerializeField]
    protected Vector2Int numBombPickup;

    [SerializeField]
    protected GameObject waterPickup;

    [SerializeField]
    protected AnimationCurve numWaterPickups;
    [SerializeField]
    protected AnimationCurve chanceWaterPickup;

    [SerializeField]
    protected Color[] colorsPer200;

    [SerializeField]
    protected AnimationCurve perlinRoughness;
    [SerializeField]
    protected AnimationCurve ridgedRoughness;
    [SerializeField]
    protected AnimationCurve bpRoughness;

    [SerializeField]
    protected AnimationCurve perlinFreqSc;
    [SerializeField]
    protected AnimationCurve ridgedFreqSc;
    [SerializeField]
    protected AnimationCurve bpFreqSc;
    [SerializeField]
    protected AnimationCurve blend1FreqSc;
    [SerializeField]
    protected AnimationCurve blend2FreqSc;

    protected Utils.GrabBag<Effect> powerupBag;

    protected void Start() {
        hardness = tilemap.GetComponent<TilemapHardness>();
        rand = new SRandom((uint)System.DateTime.Now.Millisecond);
    }

    protected void Update() {
        if(Input.GetKeyDown(KeyCode.P)) {
        //    GenerateMap();
        }
    }

    public void GenerateMap() {
        if(holder != null) {
            GameObject.Destroy(holder);
        }
        holder = new GameObject("Holder");
        spawner.holder = holder;
        GameManager.Instance.holder = holder;

        powerupBag = new Utils.GrabBag<Effect>();
        powerupBag.AddItems(possibleEffects, possibleEffectWeights);

        undermap.ClearAllTiles();
        tilemap.ClearAllTiles();
        bad.ClearAllTiles();

        int colorIndex = (int) (GameManager.Instance.depth / 200f);
        colorIndex = Mathf.Min(colorIndex, colorsPer200.Length - 1);
        var color = colorsPer200[colorIndex];

        tilemap.color = color;
        undermap.color = color;

        player = GameManager.Instance.player;
        var genPerlinMoss = new GeneratorPerlin(System.DateTime.Now.Millisecond + 12, 1, 0.3f, 0.05f, 2, 2);
        var genPerlinDecorger = new GeneratorPerlin(System.DateTime.Now.Millisecond + 12, 1, 0.3f, 0.05f, 2, 2);

        var depth = GameManager.Instance.depth;

        float perlinFreq = 0.05f * perlinFreqSc.Evaluate(depth);
        var pr = perlinRoughness.Evaluate(depth);
        float perlinFreqMulti = 2f * pr;
        float perlinPersistence = 0.5f * pr;

        float ridgedFreq = 0.05f * ridgedFreqSc.Evaluate(depth);
        var rr = ridgedRoughness.Evaluate(depth);
        float ridgedFreqMulti = 2f * rr;
        float ridgedPersistence = 0.5f * rr;

        float bpFreq = 0.05f * bpFreqSc.Evaluate(depth);
        var bpr = bpRoughness.Evaluate(depth);
        float bpFreqMulti = 2f * bpr;
        float bpPersistence = 0.5f * bpr;

        float blend1Freq = 0.02f * blend1FreqSc.Evaluate(depth);

        float blend2Freq = 0.03f * blend2FreqSc.Evaluate(depth);

        var genPerlin = new GeneratorPerlin(System.DateTime.Now.Millisecond, 1, perlinPersistence, perlinFreq, perlinFreqMulti, 4);
        var genRidged = new GeneratorRidged(System.DateTime.Now.Millisecond + 1, 1, ridgedPersistence, ridgedFreq, ridgedFreqMulti, 4);
        var genBPBil = new GeneratorBillowed(System.DateTime.Now.Millisecond + 2, 1, bpPersistence, bpFreq, bpFreqMulti, 4);
        var genBPRidge = new GeneratorRidged(System.DateTime.Now.Millisecond + 3, 1, bpPersistence, bpFreq, bpFreqMulti, 4);

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

                var pos = new Vector2(i, j);
                var signal = generator.GetNoise2D(pos) - circleFunction;

                var index = i + j * width;

                if(genPerlinMoss.GetNoise2D(pos) < -0.22f) {
                    if(hardness.hardness[index] == 0)
                        undermap.SetTile(new Vector3Int(i, j, 1), moss);
                    if(hardness.hardness[index + 1] == 0)
                        undermap.SetTile(new Vector3Int(i + 1, j, 1), moss);
                    if(hardness.hardness[index + width] == 0)
                        undermap.SetTile(new Vector3Int(i, j + 1, 1), moss);
                    if(hardness.hardness[index + width + 1] == 0)
                        undermap.SetTile(new Vector3Int(i + 1, j + 1, 1), moss);
                }

                if(signal < stoneThreshhold) {
                    tilesCollision[i + j * width] = stone;
                    tilesCollision[(i + 1) + j * width] = stone;
                    tilesCollision[i + (j + 1) * width] = stone;
                    tilesCollision[(i + 1) + (j + 1) * width] = stone;
                    undermap.SetTile(new Vector3Int(i, j, 1), null);
                    undermap.SetTile(new Vector3Int(i + 1, j, 1), null);
                    undermap.SetTile(new Vector3Int(i, j + 1, 1), null);
                    undermap.SetTile(new Vector3Int(i + 1, j + 1, 1), null);
                    hardness.hardness[index] = 1;
                    hardness.hardness[index + 1] = 1;
                    hardness.hardness[index + width] = 1;
                    hardness.hardness[index + width + 1] = 1;
                }

                tilesUnder[i + j * width] = grass;

                var decorChance = genPerlinDecorger.GetNoise2D(pos);
                if(decorChance < 0f) {
                    if(rand.RandomChance(Mathf.Abs(decorChance) * 0.1f) && hardness.hardness[index] == 0) {
                        undermap.SetTile(new Vector3Int(i, j, 2), decor[rand.RandomIntLessThan(decor.Length)]);
                    }
                }
                if(decorChance > 0f) {
                    if(rand.RandomChance(Mathf.Abs(decorChance) * 0.05f) && hardness.hardness[index] == 0) {
                        bad.SetTile(new Vector3Int(i, j, 0), deadly[rand.RandomIntLessThan(deadly.Length)]);
                        undermap.SetTile(new Vector3Int(i, j, 1), null);
                    }
                }
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

                int col = 0;

                for(int ii = -1; ii <= 1; ii++) {
                    for(int jj = -1; jj <= 1; jj++) {
                        col += hardness.hardness[x + ii + (y + jj) * width];
                    }
                }

                if(bad.GetTile(new Vector3Int(x, y, 0)) == null && col == 0) {
                    player.transform.position = new Vector3(x + 0.5f, y + 0.5f, -5f);

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

        var numDepth = numUpgradeShrinesPerDepth.Evaluate(GameManager.Instance.depth);
        for(int i = 0; i < numDepth; i++) {
            TryPlaceShrine();
        }

        var numBomb = rand.RandomIntInRange(numBombPickup.x, numBombPickup.y);
        for(int i = 0; i < numBomb; i++) {
            TryPlaceBomb();
        }

        var numWater = Mathf.CeilToInt(numWaterPickups.Evaluate(GameManager.Instance.depth));
        for(int i = 0; i < numWater; i++) {
            if(rand.RandomChance(chanceWaterPickup.Evaluate(GameManager.Instance.depth))) {
                TryPlaceWater();
            }
        }
    }

    public void TryPlaceShrine() {
        for(int i = 0; i < 30; i++) {
            var chosenX = rand.RandomIntInRange(10, width - 10);
            var chosenY = rand.RandomIntInRange(10, height - 10);

            var numOcc = 0;
            for(int j = -2; j < 2; j++) {
                for(int k = -2; k < 0; k++) {
                    numOcc += hardness.hardness[chosenX + j + (chosenY + k) * width];
                }
            }

            if(numOcc == 0) {
                var spawned = Instantiate(upgradeShrinePrefab);
                spawned.transform.parent = holder.transform;
                spawned.transform.position = new Vector3(chosenX, chosenY, -3);
                var upgrade = spawned.GetComponentInChildren<Powerup>();
                upgrade.Setup(powerupBag.GetItem());
                return;
            }
        }
    }

    public void TryPlaceBomb() {
        for(int i = 0; i < 30; i++) {
            var chosenX = rand.RandomIntInRange(10, width - 10);
            var chosenY = rand.RandomIntInRange(10, height - 10);

            var numOcc = hardness.hardness[chosenX - 1 + (chosenY) * width];

            if(numOcc == 0) {
                var spawned = Instantiate(bombsPickup);
                spawned.transform.parent = holder.transform;
                spawned.transform.position = new Vector3(chosenX, chosenY + 0.5f, -3);
                return;
            }
        }
    }

    public void TryPlaceWater() {
        for(int i = 0; i < 30; i++) {
            var chosenX = rand.RandomIntInRange(10, width - 10);
            var chosenY = rand.RandomIntInRange(10, height - 10);

            var numOcc = hardness.hardness[chosenX - 1 + (chosenY) * width];

            if(numOcc == 0) {
                var spawned = Instantiate(waterPickup);
                spawned.transform.parent = holder.transform;
                spawned.transform.position = new Vector3(chosenX, chosenY + 0.5f, -3);
                return;
            }
        }
    }

    public void Walk(Vector2Int playerPos, Vector2Int walkPos) {
        var genPerlin = new GeneratorPerlin(System.DateTime.Now.Millisecond + 20, 1, 0.5f, 0.08f, 2, 3);

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

            if(genPerlin.GetNoise2D(walkPos) < -0.2f) {
                undermap.SetTile(new Vector3Int(walkPos.x, walkPos.y, 1), path);
            }
        }

        Instantiate(stairs).transform.position = new Vector3(furtherLocation.x, furtherLocation.y, -0.1f);
        tilemap.SetTile(new Vector3Int(furtherLocation.x, furtherLocation.y, 0), null);
        tilemap.SetTile(new Vector3Int(furtherLocation.x - 1, furtherLocation.y, 0), null);
        tilemap.SetTile(new Vector3Int(furtherLocation.x - 1, furtherLocation.y - 1, 0), null);
        tilemap.SetTile(new Vector3Int(furtherLocation.x, furtherLocation.y - 1, 0), null);
        undermap.SetTile(new Vector3Int(furtherLocation.x, furtherLocation.y, 1), null);
        undermap.SetTile(new Vector3Int(furtherLocation.x - 1, furtherLocation.y, 1), null);
        undermap.SetTile(new Vector3Int(furtherLocation.x - 1, furtherLocation.y - 1, 1), null);
        undermap.SetTile(new Vector3Int(furtherLocation.x, furtherLocation.y - 1, 1), null);
        undermap.SetTile(new Vector3Int(furtherLocation.x, furtherLocation.y, 2), null);
        undermap.SetTile(new Vector3Int(furtherLocation.x - 1, furtherLocation.y, 2), null);
        undermap.SetTile(new Vector3Int(furtherLocation.x - 1, furtherLocation.y - 1, 2), null);
        undermap.SetTile(new Vector3Int(furtherLocation.x, furtherLocation.y - 1, 2), null);
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
