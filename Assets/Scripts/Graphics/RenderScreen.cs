using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderScreen : MonoBehaviour {

    [SerializeField]
    private RenderTexture texture = default;

    public Material mat = default;

    private Camera cam;

	// Use this for initialization
	void Start () {
        cam = GetComponent<Camera>();
	}

    private void OnPreRender() {
        cam.targetTexture = texture;
    }

    // Update is called once per frame
    void OnPostRender () {
        cam.targetTexture = null;
        if(mat != null) {
            var rt = RenderTexture.GetTemporary(256, 144);
            rt.filterMode = FilterMode.Point;
            Graphics.Blit(texture, rt, mat);
            Graphics.Blit(rt, null as RenderTexture);
            rt.Release();
        } else {
            Graphics.Blit(texture, null as RenderTexture);
        }
	}
}
