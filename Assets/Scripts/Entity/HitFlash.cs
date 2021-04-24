using UnityEngine;
using System.Collections;

namespace Entities {

    public class HitFlash : MonoBehaviour {

        [SerializeField]
        float timeToFlash = 0.5f;
        [SerializeField]
        float timeEachFlash = 0.125f;

        float currentTime;
        float flashTime;

        bool on;

        [SerializeField]
        Color tint = default;

        Color original;

        new SpriteRenderer renderer;

        // Use this for initialization
        void Start() {
            currentTime = timeToFlash + 1;
            flashTime = 0;
            on = false;
            renderer = GetComponentInChildren<SpriteRenderer>();
            original = renderer.color;
            GetComponent<Resources>().OnHit += OnHit;
        }

        // Update is called once per frame
        void Update() {
            currentTime += Time.deltaTime;

            if(currentTime < timeToFlash) {
                flashTime += Time.deltaTime;
                if(flashTime > timeEachFlash) {
                    flashTime = 0;
                    on = !on;
                }
            } else {
                on = false;
            }

            if(on) {
                renderer.color = tint;
            } else {
                renderer.color = original;
            }
        }

        public void OnHit(int i) {
            currentTime = 0;
        }
    }

}