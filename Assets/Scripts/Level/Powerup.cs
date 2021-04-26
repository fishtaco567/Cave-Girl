using UnityEngine;
using System.Collections;
using Entities;

public class Powerup : MonoBehaviour {

    [SerializeField]
    protected new Collider2D collider;

    [SerializeField]
    protected ContactFilter2D filter;

    protected Collider2D[] results;

    [SerializeField]
    protected Effect effect;

    [SerializeField]
    protected SpriteRenderer sr;

    public void Start() {
        results = new Collider2D[3];
    }

    public void Setup(Effect e) {
        effect = e;
        sr.sprite = e.sprite;
    }

    public void Update() {
        var num = collider.OverlapCollider(filter, results);

        for(int i = 0; i < num; i++) {
            var eff = results[i].GetComponent<Effectable>();
            if(eff != null) {
                eff.AddEffect(effect);

                Destroy(this.gameObject); 
            }
        }
    }

}
