using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Entities.Steering {

    [CreateAssetMenu(fileName = "AttackIn", menuName = "Steering/AttackIn")]
    public class AttackIn : SteeringEffect {

        [SerializeField]
        protected float attackInTime;

        [SerializeField]
        protected float attackRange;

        [SerializeField]
        protected LayerMask toAttack;
        protected ContactFilter2D filter;

        [SerializeField]
        protected float attackChance;

        protected float currentTime;
        protected Collider2D[] results;

        protected void Awake() {
            filter = new ContactFilter2D();
            filter.SetLayerMask(toAttack);

            results = new Collider2D[2];
        }

        public override Vector2 GetSteering(BasicEnemy enemy, Vector2 currentSteering) {
            return Vector2.zero;
        }

        public override float GetWeight(BasicEnemy enemy, int numSteerables) {
            return 0;
        }

        public override void DoState(BasicEnemy enemy, List<SteeringEffect> steerings, CharacterController2D controller, List<Effect> effects) {
            var currentSteering = Vector2.zero;
            var totalWeight = 0.0f;
            currentTime += Time.deltaTime;

            foreach(SteeringEffect s in steerings) {
                if(s.type == "distance") {
                    continue;
                }

                currentSteering += s.GetSteering(enemy, currentSteering) * s.GetWeight(enemy, steerings.Count);
                totalWeight += s.GetWeight(enemy, steerings.Count);
            }

            currentSteering /= totalWeight;

            enemy.velocity = currentSteering * Time.deltaTime;

            if(currentTime > attackInTime) {
                enemy.ProbablisticState();
            }

            controller.move(enemy.velocity);
        }

        public override float GetChanceForState(BasicEnemy enemy) {
            var player = FindPlayer(enemy.transform.position);

            if(player == null) {
                return 0;
            } else {
                return attackChance;
            }
        }

        public override void OnEnterState(BasicEnemy enemy) {
            currentTime = 0;
        }

        public override void OnLeaveState(BasicEnemy enemy) {

        }

        private Transform FindPlayer(Vector2 position) {
            int num = Physics2D.OverlapCircle(position, attackRange, filter, results);

            if(num != 0) {
                return results[0].transform;
            }

            return null;
        }
    }

}