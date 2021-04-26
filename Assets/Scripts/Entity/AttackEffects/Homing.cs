using UnityEngine;
using Utils;

namespace Entities.AttackEffects {

    [CreateAssetMenu(fileName = "Homing", menuName = "AttackEffects/Homing")]
    public class Homing : Effect {

        [SerializeField]
        protected float range;

        [SerializeField]
        protected ContactFilter2D filter;

        protected Collider2D[] results;

        public void Awake() {
            results = new Collider2D[2];
        }

        public override bool VelocityTick(Projectile proj, ref Vector2 velocity) {
            var num = Physics2D.OverlapCircle(proj.transform.position, range, filter, results);

            for(int i = 0; i < num; i++) {
                var resources = results[i].GetComponent<Resources>();
                if(resources != null && resources == proj.doNotHit) {
                    continue;
                }

                var vectorTo = results[i].transform.position - proj.transform.position;
                vectorTo.z = 0;
                vectorTo = vectorTo.normalized * velocity.magnitude;

                velocity.x = vectorTo.x;
                velocity.y = vectorTo.y;
            }

            return true;
        }

        public override Effect GenerateCopy() {
            return Instantiate(this);
        }
    }
}
