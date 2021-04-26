using UnityEngine;
using System.Collections;

public class ItemBob : MonoBehaviour {

    [SerializeField]
    protected float bobRate;
    [SerializeField]
    protected float bobAmount;

    protected Vector3 basePos;


    public void Start() {
        Initialize();
    }

    public void Initialize() {
        basePos = transform.position;
    }

    protected void Update() {
        transform.position = basePos + new Vector3(0, Mathf.Sin(bobRate * Time.time) * bobAmount, 0);
    }
}
