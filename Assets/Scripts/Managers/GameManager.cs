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

    [SerializeField]
    protected UnityEngine.UI.Slider healthBar;
    [SerializeField]
    protected TMPro.TMP_Text healthText;
    [SerializeField]
    protected TMPro.TMP_Text bombText;

    public Entities.Character.Player player;
    public Entities.Resources playerRes;

    [SerializeField]
    protected GameObject playerPrefab;

    bool hasShownScreen;
    bool hasGenerated;
    float currentLevelTransitionTime;

    public float depth;

    public SRandom rand;

    protected bool inDeathScene;

    protected bool shouldRespawnPlayer;

    [SerializeField]
    protected TMPro.TMP_Text inGameDepthText;

    [SerializeField]
    protected UnityEngine.UI.Image[] upgrades;

    public GameObject holder;

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
        inGameDepthText.text = string.Format("Depth: {0, 0:F2} meters", depth);
        for(int i = 0; i < upgrades.Length; i++) {
            if(i < player.effects.Count) {
                upgrades[i].sprite = player.effects[i].sprite;
                upgrades[i].color = new Color(1, 1, 1, 1);
            } else {
                upgrades[i].color = new Color(1, 1, 1, 0);
            }
        }

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
                    player.transform.position = new Vector3(0, 0, -5f);
                    playerRes = player.GetComponent<Entities.Resources>();
                }
            }

            if(currentLevelTransitionTime > delayToGenerate) {
                hasGenerated = true;
                generator.GenerateMap();
                loadingScreen.SetActive(false);
            }
        }
        
        if(playerRes != null) {
            healthBar.value = playerRes.Health / (float)playerRes.maxHealth;
            healthText.text = playerRes.Health + " / " + playerRes.maxHealth;
            bombText.text = "Bombs: " + player.curBombs + " / " + player.maxBombs;
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
