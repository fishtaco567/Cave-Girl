using UnityEngine;
using System.Collections;

public class SnapToPixel : MonoBehaviour {

    

    // Update is called once per frame
    void Update() {
        transform.localPosition = new Vector3(0, 0, transform.localPosition.z);

        var lockedX = Mathf.Floor(transform.position.x * 16) / 16;
        var lockedY = Mathf.Floor(transform.position.y * 16) / 16;

        transform.position = new Vector3(lockedX, lockedY, transform.position.z);
    }
}
