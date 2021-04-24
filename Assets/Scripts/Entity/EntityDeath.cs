using UnityEngine;
using System.Collections;

namespace Entities
{
    public class EntityDeath : MonoBehaviour
    {

        public GameObject smokePrefab;

        protected Resources resources;

        // Use this for initialization
        void Start()
        {
            resources = GetComponent<Resources>();
            resources.OnDeath += OnDeath;
        }

        protected void OnDeath()
        {
            if(smokePrefab != null) {
                var smoke = Instantiate(smokePrefab);
                smoke.transform.position = transform.position;
            }
            Destroy(this.gameObject);
        }

        // Update is called once per frame
        void Update()
        {

        }

        protected void OnDestroy() {
            if(resources != null) {
                resources.OnDeath -= OnDeath;
            }
        }

    }
}
