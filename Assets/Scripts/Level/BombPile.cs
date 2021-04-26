using UnityEngine;
using System.Collections;
using Entities;

public class BombPile : MonoBehaviour {

    [SerializeField]
    protected new Collider2D collider;

    [SerializeField]
    protected ContactFilter2D filter;

    protected Collider2D[] results;

    public Vector2Int minMaxNum;

    public void Start() {
        results = new Collider2D[3];
    }

    public void Update() {
        var num = collider.OverlapCollider(filter, results);

        for(int i = 0; i < num; i++) {
            var res = results[i].GetComponent<Entities.Character.Player>();
            res.curBombs += GameManager.Instance.rand.RandomIntInRange(minMaxNum.x, minMaxNum.y);
            res.curBombs = Mathf.Min(res.maxBombs, res.curBombs);

            Destroy(this.gameObject);
        }
    }

}
