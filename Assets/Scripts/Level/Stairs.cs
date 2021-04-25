using UnityEngine;
using System.Collections;

public class Stairs : MonoBehaviour {

    [SerializeField]
    protected ContactFilter2D filter;
    [SerializeField]
    protected Collider2D collider;

    protected Collider2D[] results;

    protected bool hasTriggered;

    // Use this for initialization
    void Start() {
        results = new Collider2D[2];
        hasTriggered = false;
    }

    // Update is called once per frame
    void Update() {
        var num = collider.OverlapCollider(filter, results);

        if(num > 0 && !hasTriggered) {
            hasTriggered = true;
            GameManager.Instance.NextLevel();
            Destroy(this.gameObject, 0.5f);
        }
    }
}
