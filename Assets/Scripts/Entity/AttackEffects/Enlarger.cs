using UnityEngine;
using Utils;

namespace Entities.AttackEffects {

    [CreateAssetMenu(fileName = "Enlarger", menuName = "AttackEffects/Enlarger")]
    public class Enlarger : Effect {

        [SerializeField]
        protected float scale;

        [SerializeField]
        protected bool addOthers;

        public override bool PerTick(Effectable eff) {
            float curScale = scale;
            if(addOthers) {
                foreach(Effect e in eff.effects) {
                    e.ChangeRange(eff, ref curScale);
                }
            }
            eff.transform.localScale = new Vector3(curScale, curScale, curScale);

            return base.PerTick(eff);
        }

        public override Effect GenerateCopy() {
            return Instantiate(this);
        }
    }

}
