using UnityEngine;
using System.Collections.Generic;
using Utils;

public class BasicEnemySpawner : MonoBehaviour {

    [SerializeField]
    protected List<GameObject> enemies;
    [SerializeField]
    protected List<int> enemyIndex;
    [SerializeField]
    protected List<float> enemyWeights;

    [SerializeField]
    protected List<Entities.SteeringEffect> steerings;
    [SerializeField]
    protected List<float> steeringWeights;

    [SerializeField]
    protected List<Entities.Effect> effects;
    [SerializeField]
    protected List<float> effectWeights;

    [SerializeField]
    protected AnimationCurve steeringsPerDepth;

    [SerializeField]
    protected AnimationCurve effectsPerDepth;

    [SerializeField]
    protected AnimationCurve spawnChancePerSecondPerDepth;

    public GameObject holder;

    [SerializeField]
    protected TilemapHardness hardness;

    private SRandom rand;

    private Utils.GrabBag<Entities.SteeringEffect> steeringBag;
    private Utils.GrabBag<Entities.Effect> effectBag;
    private Utils.GrabBag<int> enemyBag;

    protected void Start() {
        rand = new SRandom((uint) System.DateTime.Now.Millisecond);
        enemyBag = new GrabBag<int>();
        enemyBag.AddItems(enemyIndex, enemyWeights);
    }

    protected void Update() {
        if(GameManager.Instance.inGame == false) {
            return;
        }

        if(rand.RandomChance(spawnChancePerSecondPerDepth.Evaluate(GameManager.Instance.depth) * Time.deltaTime * 5)) {
            TrySpawn();
        }
    }

    protected void TrySpawn() {
        if(holder == null) {
            return;
        }

        var player = GameManager.Instance.player;
        for(int i = 0; i < 5; i++) {
            var chosenX = rand.RandomIntInRange(10, hardness.width - 10);
            var chosenY = rand.RandomIntInRange(10, hardness.height - 10);

            if(Vector2.Distance(player.transform.position, new Vector2(chosenX, chosenY)) < 20) {
                continue;
            }

            var numOcc = 0;
            for(int j = -1; j < 1; j++) {
                for(int k = -1; k < 1; k++) {
                    numOcc += hardness.hardness[chosenX + j + (chosenY + k) * hardness.width];
                }
            }

            if(numOcc == 0) {
                DoSpawn(new Vector3(chosenX, chosenY, -4f));
                return;
            }
        }
    }

    protected void DoSpawn(Vector3 pos) {
        var effectBag = new GrabBag<Entities.Effect>(true);
        effectBag.AddItems(effects, effectWeights);
        steeringBag = new GrabBag<Entities.SteeringEffect>(true);
        steeringBag.AddItems(steerings, steeringWeights);

        var maxEnemyIndex = (int) (GameManager.Instance.depth / 100f);
        maxEnemyIndex = Mathf.Clamp(maxEnemyIndex, 0, enemies.Count - 1);

        var enemy = enemies[Mathf.Clamp(enemyBag.GetItem(), 0, maxEnemyIndex)];
        var numSteerings = rand.RandomIntLessThan((int) steeringsPerDepth.Evaluate(GameManager.Instance.depth));
        var numEffects = rand.RandomIntLessThan((int)effectsPerDepth.Evaluate(GameManager.Instance.depth));

        var spawned = Instantiate(enemy);
        var enem = spawned.GetComponent<Entities.BasicEnemy>();

        for(int i = 0; i < numSteerings; i++) {
            if(steeringBag.IsEmpty()) {
                break;
            }

            enem.AddSteering(steeringBag.GetItem());
        }

        for(int i = 0; i < numEffects; i++) {
            if(effectBag.IsEmpty()) {
                break;
            }

            enem.AddEffect(effectBag.GetItem());
        }

        spawned.transform.position = pos;
        spawned.transform.parent = holder.transform;
    }

}
