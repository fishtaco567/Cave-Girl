using UnityEngine;
using Utils;

namespace Entities.AttackEffects {

    [CreateAssetMenu(fileName = "SpawnProjectileConstant", menuName = "AttackEffects/SpawnProjectileConstant")]
    public class SpawnProjectileConstant : Effect {

        [SerializeField]
        protected GameObject spawnOnHit;

        [SerializeField]
        protected float delayBetween;

        [SerializeField]
        protected float number;

        [SerializeField]
        protected float speed;

        protected float time;

        protected void Awake() {
            time = 0;
        }

        public override bool PerTick(Effectable eff) {
            var dt = Time.deltaTime;
            foreach(Effect e in eff.effects) {
                e.ChangeTime(eff, ref dt);
            }

            time += dt;

            if(time < delayBetween) {
                return false;
            }
            time = 0;

            var effectsToTransfer = eff.GetEffectSublist(1);
            effectsToTransfer.Remove(this);
            for(int i = effectsToTransfer.Count - 1; i >= 0; i--) {
                if(effectsToTransfer[i] is SpawnProjectileConstant) {
                    effectsToTransfer.RemoveAt(i);
                }
            }

            for(int i = 0; i < number; i++) {
                var angle = Mathf.PI / 4 + (i / (float)number) * 2 * Mathf.PI;
                var velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

                var spawned = Instantiate(spawnOnHit);
                var newProj = spawned.GetComponent<Projectile>();
                if(newProj != null) {
                    newProj.AddEffects(effectsToTransfer);
                    newProj.doNotHit = eff.GetComponent<Resources>();
                    newProj.doNotHitLayers = eff.gameObject.layer;
                    newProj.SetVelocity(velocity * speed);
                }

                spawned.transform.parent = GameManager.Instance.holder.transform;
                newProj.transform.position = eff.transform.position + new Vector3(velocity.x, velocity.y, 0) * 0.1f;
            }

            return true;
        }

        public override Effect GenerateCopy() {
            return Instantiate(this);
        }
    }

}
