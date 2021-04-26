using UnityEngine;
using System.Collections;
using Entities;

public class FloorHazard : MonoBehaviour {

    [SerializeField]
    protected new Collider2D collider;

    [SerializeField]
    protected int damage;

    [SerializeField]
    protected ContactFilter2D filter;

    protected Collider2D[] results;

    [SerializeField]
    protected float minHeight;

    public void Start() {
        results = new Collider2D[5];
    }

    public void Update() {
        var num = collider.OverlapCollider(filter, results);

        for(int i = 0; i < num; i++) {
            var entityInfo = results[i].GetComponent<EntityInfo>();
            var res = results[i].GetComponent<Entities.Resources>();

            if(res != null && entityInfo != null && entityInfo.yHeight < minHeight) {
                res.Damage(damage);
            }
        }
    }

}
