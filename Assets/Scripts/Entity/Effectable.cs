using UnityEngine;
using System.Collections.Generic;

namespace Entities {
    public abstract class Effectable : MonoBehaviour {

        [SerializeField]
        public List<Effect> effects;

        [SerializeField]
        public Dictionary<string, bool> flags;

        public Resources doNotHit;

        public LayerMask doNotHitLayers;

        public abstract void Destroy();

        public virtual void Start() {
            flags = new Dictionary<string, bool>();

            var instEffects = new List<Effect>(effects.Count);

            foreach(Effect e in effects) {
                instEffects.Add(e.GenerateCopy());
                e.AddEffect(this);
            }

            effects = instEffects;
        }

        public void AddEffect(Effect newEffect) {
            effects.Insert(0, newEffect.GenerateCopy());
            newEffect.AddEffect(this);
        }

        public void AddEffects(List<Effect> newEffects) {
            foreach(Effect e in newEffects) {
                effects.Insert(0, e);
                e.AddEffect(this);
            }
        }

        public void SetEffects(List<Effect> replaceEffects) {
            effects = replaceEffects;
        }

        public List<Effect> GetEffectSublist(int startIndex) {
            return effects.GetRange(startIndex, effects.Count - startIndex);
        }

        public List<Effect> GetEffectSublist(Effect startEffect) {
            int index = 0;

            for(int i = 0; i < effects.Count; i++) {
                if(effects[i] == startEffect) {
                    index = i;
                    break;
                }
            }

            index += 1;

            if(index >= effects.Count) {
                return new List<Effect>();
            }

            return effects.GetRange(index, effects.Count - index);
        }

    }

}
