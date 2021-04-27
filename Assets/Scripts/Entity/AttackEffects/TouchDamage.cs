using UnityEngine;
using System.Collections;

namespace Entities.AttackEffects {

    [CreateAssetMenu(fileName = "TouchDamage", menuName = "AttackEffects/TouchDamage")]
    public class TouchDamage : Effect {

        [SerializeField]
        protected bool destroy;

        [System.NonSerialized]
        public bool destroyOnTouch;

        protected Collider2D collider;

        protected Collider2D[] results;

        [SerializeField]
        protected ContactFilter2D filter;

        [SerializeField]
        public int damage;

        protected void Awake() {
            destroyOnTouch = destroy;
            results = new Collider2D[3];
        }

        public override bool AddEffect(Effectable eff) {
            bool found = false;
            foreach(Effect e in eff.effects) {
                if(e is TouchDamage && e != this) {
                    (e as TouchDamage).damage += this.damage;
                    found = true;
                }
            }

            if(found) {
                eff.effects.Remove(this);
            }
            return true;
        }

        public override bool PerTick(Effectable eff) {
            if(collider == null) {
                collider = eff.GetComponent<Collider2D>();
            }

            if(collider == null) {
                return false;
            }

            var num = collider.OverlapCollider(filter, results);
            bool hasHit = false;

            for(int i = 0; i < num; i++) {
                if(results[i].gameObject.layer == eff.doNotHitLayers) {
                    continue;
                }
                if(results[i].gameObject == eff.gameObject) {
                    continue;
                }

                if(results[i].gameObject.transform.position.z > 1) {
                    continue;
                }

                var res = results[i].GetComponent<Resources>();
                
                if(res != null && res != eff.doNotHit) {
                    float curDamage = damage;
                    foreach(Effect e in eff.effects) {
                        e.ChangeStrength(eff, ref curDamage);
                    }

                    if(res.Damage((int) curDamage)) {
                        hasHit = true;

                        if(eff is Projectile) {
                            var proj = eff as Projectile;
                            OnHitEffects(proj, -proj.GetVelocity(), res);
                        } else {
                            OnHitEffects(eff, res);
                        }
                    }
                }
            }

            if(hasHit && destroyOnTouch) {
                eff.Destroy();
            }

            return hasHit;
        }

        private void OnHitEffects(Effectable eff, Resources res) {
            foreach(Effect e in eff.effects) {
                if(e == this) {
                    continue;
                }

                e.OnHit(eff, res);
            }
        }

        private void OnHitEffects(Projectile proj, Vector2 norm, Resources res) {
            foreach(Effect e in proj.effects) {
                if(e == this) {
                    continue;
                }

                e.OnHit(proj, norm, res);
            }
        }

        public override Effect GenerateCopy() {
            var inst = Instantiate(this);
            inst.damage = 1;
            return inst;
        }
    }

}