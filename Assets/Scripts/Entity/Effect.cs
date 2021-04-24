using UnityEngine;
using System.Collections;

namespace Entities {

    public abstract class Effect : ScriptableObject {

        public virtual bool PerTick(Effectable eff) {
            return false;
        }

        public virtual bool OnHit(Projectile proj, Vector2 normal, Resources resources) {
            return false;
        }

        public virtual bool OnHit(Effectable eff, Resources resources) {
            return false;
        }

        public virtual bool DuringAttack(Effectable eff, Vector2 pos) {
            return false;
        }

        public virtual bool OnDestroyed(Effectable eff) {
            return false;
        }

        public virtual bool OnSpawn(Effectable eff) {
            return false;
        }

        public virtual bool VelocityTick(Projectile proj, ref Vector2 velocity) {
            return false;
        }

        public abstract Effect GenerateCopy();

    }

}