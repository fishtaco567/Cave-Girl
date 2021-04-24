using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Entities.Steering {

    [CreateAssetMenu(fileName = "DistanceFromPlayer", menuName = "Steering/DistanceFromPlayer")]
    public class DistanceFromPlayer : SteeringEffect {

        [SerializeField]
        protected float targetAcquireRange;

        [SerializeField]
        protected AnimationCurve weightByDistance;

        [SerializeField]
        protected LayerMask layerMask;

        [SerializeField]
        protected float awaySpeed;

        [SerializeField]
        protected float circleSpeed;

        protected ContactFilter2D filter;

        protected Transform player = null;
        protected Collider2D[] results;

        protected void Awake() {
            results = new Collider2D[2];
            filter = new ContactFilter2D();
            filter.SetLayerMask(layerMask);
        }

        public override Vector2 GetSteering(BasicEnemy enemy, Vector2 currentSteering) {
            player = FindPlayer(enemy.transform.position);

            if(player == null) {
                return Vector2.zero;
            } else {
                Vector2 toPlayer = (player.transform.position - enemy.transform.position).normalized;
                return toPlayer * -awaySpeed + new Vector2(-toPlayer.y, toPlayer.x) * circleSpeed;
            }
        }

        public override float GetWeight(BasicEnemy enemy, int numSteerables) {
            if(player == null) {
                return 0;
            } else {
                var dist = Vector2.Distance(enemy.transform.position, player.position);
                return weightByDistance.Evaluate(dist);
            }
        }

        public override float GetChanceForState(BasicEnemy enemy) {
            return 0;
        }

        private Transform FindPlayer(Vector2 position) {
            int num = Physics2D.OverlapCircle(position, targetAcquireRange, filter, results);

            if(num != 0) {
                return results[0].transform;
            }

            return null;
        }

    }

}