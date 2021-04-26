using UnityEngine;
using System.Collections;

namespace Entities.AttackEffects {

    [CreateAssetMenu(fileName = "Pierce", menuName = "AttackEffects/Pierce")]
    public class Pierce : Effect {

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

        public override Effect GenerateCopy() {
            return Instantiate(this);
        }
    }

}