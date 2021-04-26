using UnityEngine;
using System.Collections;
using Entities;

public class HealthPickup : MonoBehaviour {

    [SerializeField]
    protected new Collider2D collider;

    [SerializeField]
    protected ContactFilter2D filter;

    protected Collider2D[] results;

    [SerializeField]
    protected GameObject toSpawn;


    public void Start() {
        results = new Collider2D[3];
    }

    public void Update() {
        var num = collider.OverlapCollider(filter, results);

        for(int i = 0; i < num; i++) {
            var res = results[i].GetComponent<Entities.Resources>();
            res.Health += 1;

            Instantiate(toSpawn).transform.position = transform.position;

            Destroy(this.gameObject);
        }
    }

}
