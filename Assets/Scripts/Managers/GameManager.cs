using UnityEngine;
using System.Collections;
using Utils;

public class GameManager : Singleton<GameManager> {

    [SerializeField]
    protected CavernGenerator generator;

    [SerializeField]
    protected GameObject loadingScreen;

    [SerializeField]
    protected GameObject deathScreen;
    [SerializeField]
    protected TMPro.TMP_Text deathDepthText;

    [SerializeField]
    protected float delayToGenerate;
    [SerializeField]
    protected float delayToLoadingScreen;

    public Entities.Character.Player player;

    [SerializeField]
    protected GameObject playerPrefab;

    bool hasShownScreen;
    bool hasGenerated;
    float currentLevelTransitionTime;

    public float depth;

    public SRandom rand;

    protected bool inDeathScene;

    protected bool shouldRespawnPlayer;

    // Use this for initialization
    void Start() {
        hasShownScreen = true;
        hasGenerated = true;
        currentLevelTransitionTime = delayToGenerate + 1;
        depth = 0;
        rand = new SRandom((uint)System.DateTime.Now.Millisecond);
        inDeathScene = false;
        shouldRespawnPlayer = false;
    }

    // Update is called once per frame
    void Update() {
        if(!hasGenerated) {
            currentLevelTransitionTime += Time.deltaTime;

            if(currentLevelTransitionTime > delayToLoadingScreen && !hasShownScreen) {
                loadingScreen.SetActive(true);
                deathScreen.SetActive(false);
                hasShownScreen = true;

                if(shouldRespawnPlayer) {
                    shouldRespawnPlayer = false;
                    Destroy(player.gameObject);
                    player = Instantiate(playerPrefab).GetComponent<Entities.Character.Player>();
                }
            }

            if(currentLevelTransitionTime > delayToGenerate) {
                hasGenerated = true;
                generator.GenerateMap();
                loadingScreen.SetActive(false);
            }
        }
    }

    public void NextLevel() {
        hasShownScreen = false;
        hasGenerated = false;
        currentLevelTransitionTime = 0;
        depth += rand.RandomFloatInRange(13, 76);
    }

    public void OnDeath() {
        player.isDead = true;
        inDeathScene = true;
        deathScreen.SetActive(true);
        deathDepthText.text = string.Format("Depth: {0, 0:F2} meters", depth);
    }

    public void Retry() {
        depth = 0;
        shouldRespawnPlayer = true;
        NextLevel();
    }

    public void Exit() {
        Debug.Log("Exit Pressed WIP");
    }

}
