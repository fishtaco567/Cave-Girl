using UnityEngine;
using Utils;

namespace Entities.AttackEffects {

    [CreateAssetMenu(fileName = "SpawnOnHit", menuName = "AttackEffects/SpawnOnHit")]
    public class SpawnOnHit : Effect {

        [SerializeField]
        protected GameObject spawnOnHit;

        public override bool OnHit(Effectable eff, Resources resources) {
            Hit(eff);
            return false;
        }

        public override bool OnHit(Projectile proj, Vector2 normal, Resources resources) {
            Hit(proj);
            return false;
        }

        public void Hit(Effectable eff) {
            var effectsToTransfer = eff.GetEffectSublist(1);
            effectsToTransfer.Remove(this);

            var spawned = GameObject.Instantiate(spawnOnHit);
            var newEff = spawned.GetComponent<Effectable>();
            if(newEff != null) {
                newEff.AddEffects(effectsToTransfer);
                newEff.doNotHit = eff.GetComponent<Resources>();
            }

            spawned.transform.position = eff.transform.position;
        }

        public override Effect GenerateCopy() {
            return Instantiate(this);
        }
    }

}
