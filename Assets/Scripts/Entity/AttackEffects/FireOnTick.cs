using UnityEngine;
using Utils;
using System.Linq;

namespace Entities.AttackEffects {

    [CreateAssetMenu(fileName = "SpawnOnTick", menuName = "AttackEffects/SpawnOnTick")]
    public class FireOnTick : Effect {

        [SerializeField]
        protected float fireTime;

        [SerializeField]
        protected GameObject firePrefab;
        
        protected bool checkFire = false;

        protected float time = 0;

        public override bool AddEffect(Effectable eff) {
            if(eff.gameObject.CompareTag("Fire")) {
                eff.effects.Remove(this);
            }
            return true;
        }

        public override bool PerTick(Effectable eff) {
            var dt = Time.deltaTime;
            foreach(Effect e in eff.effects) {
                e.ChangeTime(eff, ref dt);
            }

            time += dt;

            if(time > fireTime) {
                time = 0;
                var spawned = GameObject.Instantiate(firePrefab);
                spawned.transform.position = eff.transform.position;
                var proj = spawned.GetComponent<Effectable>();
                if(proj != null) {
                    var effectsToTransfer = eff.GetEffectSublist(1);
                    for(int i = effectsToTransfer.Count - 1; i >= 0; i--) {
                        if(effectsToTransfer[i] is FireOnTick) {
                            effectsToTransfer.RemoveAt(i);
                        }
                    }

                    proj.AddEffects(eff.GetEffectSublist(1));
                    proj.doNotHit = eff.GetComponent<Resources>();
                }
                
                return true;
            }

            return false;
        }

        public override Effect GenerateCopy() {
            return Instantiate(this);
        }
    }

}
