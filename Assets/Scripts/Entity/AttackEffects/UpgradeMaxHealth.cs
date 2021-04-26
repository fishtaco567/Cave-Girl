using UnityEngine;
using System.Collections;

namespace Entities.AttackEffects {

    [CreateAssetMenu(fileName = "MaxHealth", menuName = "AttackEffects/MaxHealth")]
    public class UpgradeMaxHealth : Effect {

        [SerializeField]
        protected Vector2Int minMaxAmount;

        public override bool OnSpawn(Effectable eff) {
            foreach(Effect e in eff.effects) {
                var td = e as TouchDamage;
                if(td != null) {
                    td.destroyOnTouch = false;
                    return true;
                }
            }

            return false;
        }

        public override bool AddEffect(Effectable eff) {
            var res = eff.GetComponent<Resources>();
            if(res != null) {
                var amount = GameManager.Instance.rand.RandomIntInRange(minMaxAmount.x, minMaxAmount.y);
                res.maxHealth += amount;
                res.Health = res.maxHealth;
            }
            eff.effects.Remove(this);
            return true;
        }

        public override bool ShouldPickup(Effectable eff) {
            var res = eff.GetComponent<Resources>();
            if(res != null) {
                return true;
            }

            return false;
        }

        public override Effect GenerateCopy() {
            return Instantiate(this);
        }
    }

}