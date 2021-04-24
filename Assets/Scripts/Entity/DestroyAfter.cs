using UnityEngine;
using System.Collections;

public class DestroyAfter : MonoBehaviour {

    [SerializeField]
    protected float timeToDestroy;

    protected float time;

    public void OnEnable() {
        time = 0;
    }

    public void Update() {
        time += Time.deltaTime;
        if(time > timeToDestroy) {
            Destroy(this.gameObject);
        }
    }

}
