using UnityEngine;
using System.Collections;

namespace Entities.Steering {

    [CreateAssetMenu(fileName = "AvoidWalls", menuName = "Steering/AvoidWalls")]
    public class AvoidWalls : SteeringEffect {

        [SerializeField]
        protected float moveAwaySpeed;

        [SerializeField]
        protected float lookAhead;

        protected float currentStrength = 0;

        public override Vector2 GetSteering(BasicEnemy enemy, Vector2 currentSteering) {
            var hit = Physics2D.Raycast(enemy.transform.position, currentSteering, lookAhead, enemy.controller.platformMask);
            
            if(hit.collider != null) {
                currentStrength = 1 - hit.distance / lookAhead;
                return (hit.normal * moveAwaySpeed * 0.5f + new Vector2(-hit.normal.y, hit.normal.x) * moveAwaySpeed * 0.5f) * currentStrength;
            } else {
                currentStrength = 0;
                return Vector2.zero;
            }
        }

        public override float GetWeight(BasicEnemy enemy, int numSteerables) {
            return currentStrength;
        }

    }

}