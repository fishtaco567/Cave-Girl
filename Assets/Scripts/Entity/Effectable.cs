using UnityEngine;
using System.Collections.Generic;

namespace Entities {
    public abstract class Effectable : MonoBehaviour {

        public List<Effect> effects;

        public abstract void Destroy();

        public void AddEffect(Effect newEffect) {
            effects.Add(newEffect);
        }

        public void AddEffects(List<Effect> newEffects) {
            foreach(Effect e in newEffects) {
                effects.Add(e);
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
