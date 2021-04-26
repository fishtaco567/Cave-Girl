using UnityEngine;
using Utils;

namespace Entities.AttackEffects {

    [CreateAssetMenu(fileName = "Reflect", menuName = "AttackEffects/Reflect")]
    public class Reflect : Effect {

        public override bool OnHit(Projectile proj, Vector2 normal, Resources resources) {
            if(proj.flags.TryGetValue("reflect", out bool hasReflected)) {
                if(hasReflected) {
                    return false;
                }
            }

            proj.flags.Add("reflect", true);

            var reflected = proj.DaughterOf();
            reflected.effects.Remove(this);
            var vel = Vector2.Reflect(proj.GetVelocity(), normal.normalized);
            reflected.SetVelocity(vel);
            reflected.transform.position = proj.transform.position + new Vector3(vel.x, vel.y, 0) * 0.05f;

            return true;
        }

        public override Effect GenerateCopy() {
            return Instantiate(this);
        }
    }
}
