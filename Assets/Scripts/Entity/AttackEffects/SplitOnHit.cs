using UnityEngine;
using Utils;

namespace Entities.AttackEffects {

    [CreateAssetMenu(fileName = "SplitOnHit", menuName = "AttackEffects/SplitOnHit")]
    public class SplitOnHit : Effect {

        [SerializeField]
        protected int number;

        public override bool OnHit(Projectile proj, Vector2 normal, Resources resources) {
            if(proj.flags.TryGetValue("split", out bool hasSplit)) {
                if(hasSplit) {
                    return false;
                }
            }
            
            proj.flags.Add("split", true);

            for(int i = 0; i < number; i++) {
                var angle = Mathf.PI / 4 + (i / (float)number) * 2 * Mathf.PI;
                var velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

                var newProj = proj.DaughterOf();
                newProj.effects.Remove(this);
                if(newProj != null) {
                    newProj.doNotHit = proj.GetComponent<Resources>();
                    newProj.SetVelocity(velocity * proj.GetVelocity().magnitude);
                }

                newProj.transform.parent = GameManager.Instance.holder.transform;
                newProj.transform.position = proj.transform.position + new Vector3(velocity.x, velocity.y, 0) * 0.1f;
            }
            return false;
        }

        public void Hit(Effectable eff) {
        }

        public override Effect GenerateCopy() {
            return Instantiate(this);
        }
    }

}
