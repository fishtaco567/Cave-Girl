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

        [SerializeField]
        protected float dieAfter;

        [SerializeField]
        protected float time;

        public Projectile DaughterOf() {
            var spawned = Instantiate(this.gameObject);
            var newProj = spawned.GetComponent<Projectile>();
            newProj.effects = GetEffectSublist(1);
            newProj.doNotHit = doNotHit;
            return newProj;
        }

        public override void Start() {
            base.Start();
            time = 0;
            OnSpawn();
        }

        protected void Update() {
            flags.Clear();
            var dt = Time.deltaTime;
            foreach(Effect e in effects) {
                e.ChangeTime(this, ref dt);
            }

            time += dt;

            if(time > dieAfter) {
                Destroy();
            }

            foreach(Effect effect in effects) {
                effect.PerTick(this);
            }

            var angle = Vector2.SignedAngle(Vector2.up, velocity);

            transform.localRotation = Quaternion.Euler(0, 0, angle);
        }

        protected void FixedUpdate() {
            var dt = Time.fixedDeltaTime;
            foreach(Effect effect in effects) {
                effect.VelocityTick(this, ref velocity);
                effect.ChangeTime(this, ref dt);
            }

            var hit = Physics2D.Raycast(transform.position, velocity, velocity.magnitude * dt  , hitMask);
        
            if(hit.collider != null) {
                foreach(Effect effect in effects) {
                    effect.OnHit(this, hit.normal, null);
                }

                Destroy();
            }

            transform.position += new Vector3(velocity.x, velocity.y, 0) * dt;
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
