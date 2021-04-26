using UnityEngine;
using System.Collections;

namespace Entities.AttackEffects {

    [CreateAssetMenu(fileName = "MaxBombs", menuName = "AttackEffects/MaxBombs")]
    public class MaxBombs : Effect {

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
            var res = eff.GetComponent<Entities.Character.Player>();
            if(res != null) {
                var amount = GameManager.Instance.rand.RandomIntInRange(minMaxAmount.x, minMaxAmount.y);
                res.maxBombs += amount;
            }
            eff.effects.Remove(this);
            return true;
        }

        public override Effect GenerateCopy() {
            return Instantiate(this);
        }
    }

}