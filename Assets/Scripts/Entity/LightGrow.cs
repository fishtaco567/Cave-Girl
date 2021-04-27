using UnityEngine;
using System.Collections;

public class LightGrow : MonoBehaviour {

    [SerializeField]
    protected GameObject lightObj;

    protected Entities.Effectable eff;

    // Use this for initialization
    void Start() {
        eff = GetComponentInParent<Entities.Effectable>();
    }

    // Update is called once per frame
    void Update() {
        if(eff != null) {
            float scale = 1;
            foreach(Entities.Effect e in eff.effects) {
                e.ChangeRange(eff, ref scale);
            }
            lightObj.transform.localScale = new Vector3(scale, scale, scale);
        }
    }
}
