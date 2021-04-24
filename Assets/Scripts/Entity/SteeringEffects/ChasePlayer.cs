using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Entities.Steering {

    [CreateAssetMenu(fileName = "ChasePlayer", menuName = "Steering/ChasePlayer")]
    public class ChasePlayer : SteeringEffect {

        [SerializeField]
        protected float targetAcquireRange;

        [SerializeField]
        protected float aggressiveRange;

        [SerializeField]
        protected AnimationCurve weightByDistance;

        [SerializeField]
        protected LayerMask layerMask;

        [SerializeField]
        protected float aggroSpeed;
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
                return (player.transform.position - enemy.transform.position).normalized * aggroSpeed;
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
            player = FindPlayer(enemy.transform.position);

            if(player == null) {
                return 0;
            } else {
                var dist = Vector2.Distance(enemy.transform.position, player.position);
                
                if(dist > aggressiveRange) {
                    return 0;
                }
                
                return weightByDistance.Evaluate(dist);
            }
        }

        public override void OnEnterState(BasicEnemy enemy) {

        }

        public override void DoState(BasicEnemy enemy, List<SteeringEffect> steerings, CharacterController2D controller, List<Effect> effects) {
            var currentSteering = Vector2.zero;
            var totalWeight = 0.0f;

            foreach(SteeringEffect s in steerings) {
                var curWeight = s.GetWeight(enemy, steerings.Count);
                if(s == this) {
                    curWeight *= 2;
                }
                currentSteering += s.GetSteering(enemy, currentSteering) * curWeight;
                totalWeight += curWeight;
            }

            currentSteering /= totalWeight;

            enemy.velocity = currentSteering * Time.deltaTime;

            enemy.ProbablisticState();

            if(player == null) {
                enemy.ChangeToDefaultState();
            }

            controller.move(enemy.velocity);
        }

        public override void OnLeaveState(BasicEnemy enemy) {

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