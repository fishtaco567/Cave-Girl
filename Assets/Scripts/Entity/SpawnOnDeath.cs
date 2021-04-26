using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Entities;

public class SpawnOnDeath : MonoBehaviour
{

    [SerializeField]
    protected GameObject spawn;

    void Start()
    {
        var res = GetComponent<Entities.Resources>();
        if(res != null) {
            res.OnDeath += SpawnOn;
        }
    }

    protected void SpawnOn() {
        Debug.Log("S");
        var inst = Instantiate(spawn);
        inst.transform.position = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
