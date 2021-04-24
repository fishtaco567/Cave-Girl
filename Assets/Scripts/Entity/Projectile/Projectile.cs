using UnityEngine;
using System.Collections.Generic;

namespace Entities {

    public class Projectile : Effectable {

        [SerializeField]
        protected LayerMask hitMask;

        [SerializeField]
        protected Vector2 velocity;

        [SerializeField]
        protected GameObject spawnOnDeath;

        protected void Start() {
            if(effects == null) {
                effects = new List<Effect>();
            }
        }

        protected void Update() {
            foreach(Effect effect in effects) {
                effect.PerTick(this);
            }
        }

        protected void FixedUpdate() {
            foreach(Effect effect in effects) {
                effect.VelocityTick(this, ref velocity);
            }

            var hit = Physics2D.Raycast(transform.position, velocity, velocity.magnitude * Time.fixedDeltaTime, hitMask);
        
            if(hit.collider != null) {
                foreach(Effect effect in effects) {
                    effect.OnHit(this, hit.normal, null);
                }

                Destroy();
            }

            transform.position += new Vector3(velocity.x, velocity.y, 0) * Time.fixedDeltaTime;
        }

        public override void Destroy() {
            foreach(Effect effect in effects) {
                effect.OnDestroyed(this);
            }

            if(spawnOnDeath != null) {
                var spawned = Instantiate(spawnOnDeath);
                spawned.transform.position = transform.position;
            }

            Destroy(this.gameObject);
        }

        public Vector2 GetVelocity() {
            return velocity;
        }

        public void SetVelocity(Vector2 newVelocity) {
            velocity = newVelocity;
        }

        public void OnSpawn() {
            foreach(Effect effect in effects) {
                effect.OnSpawn(this);
            }
        }

    }

}
