using UnityEngine;
using Utils;

namespace Entities.AttackEffects {

    [CreateAssetMenu(fileName = "ChangeRange", menuName = "AttackEffects/ChangeRange")]
    public class RangeChanger : Effect {

        [SerializeField]
        protected float scale;

        public override bool ChangeRange(Effectable eff, ref float strength) {
            strength *= scale;
            return true;
        }

        public override Effect GenerateCopy() {
            return Instantiate(this);
        }
    }

}
