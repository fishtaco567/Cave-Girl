using UnityEngine;
using Utils;

namespace Entities.AttackEffects {

    [CreateAssetMenu(fileName = "ChangeTime", menuName = "AttackEffects/ChangeTime")]
    public class TimeChanger : Effect {

        [SerializeField]
        protected float scale;

        public override bool ChangeTime(Effectable eff, ref float time) {
            time *= scale;
            return true;
        }

        public override Effect GenerateCopy() {
            return Instantiate(this);
        }
    }

}
