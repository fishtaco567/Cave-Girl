using UnityEngine;
using Utils;

namespace Entities.AttackEffects {

    [CreateAssetMenu(fileName = "ChangeStrength", menuName = "AttackEffects/ChangeStrength")]
    public class StrengthChanger : Effect {

        [SerializeField]
        protected float scale;

        public override bool ChangeStrength(Effectable eff, ref float strength) {
            strength *= scale;
            return true;
        }

        public override Effect GenerateCopy() {
            return Instantiate(this);
        }
    }

}
