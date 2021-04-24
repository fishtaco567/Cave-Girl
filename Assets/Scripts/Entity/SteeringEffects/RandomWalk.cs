using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utils;

namespace Entities.Steering {

    [CreateAssetMenu(fileName = "RandomWalkSteer", menuName = "Steering/RandomWalkSteer")]
    public class RandomWalk : SteeringEffect {

        [SerializeField]
        protected Vector2 minMaxWalkTime;

        [SerializeField]
        protected float idleChance;

        [SerializeField]
        protected float moveSpeed;

        protected Vector2 currentWalkDir;
        protected float currentWalkTime;
        protected float nextWalkTime;

        protected SRandom rand;

        public void Awake() {
            rand = new SRandom((uint)System.DateTime.Now.Millisecond);
            currentWalkTime = 0;
            nextWalkTime = rand.RandomFloatInRange(minMaxWalkTime.x, minMaxWalkTime.y);
            currentWalkDir = rand.RandomDirection2D();
        }

        public override Vector2 GetSteering(BasicEnemy enemy, Vector2 currentSteering) {
            currentWalkTime += Time.deltaTime;
            if(currentWalkTime > nextWalkTime || enemy.controller.collisionState.hasCollision() || (Vector2.Dot(enemy.velocity.normalized, currentWalkDir.normalized) < 0.6f && currentWalkDir.sqrMagnitude != 0)) {
                currentWalkTime = 0;
                nextWalkTime = rand.RandomFloatInRange(minMaxWalkTime.x, minMaxWalkTime.y);

                if(rand.RandomChance(idleChance)) {
                    currentWalkDir = Vector2.zero;
                } else {
                    currentWalkDir = rand.RandomDirection2D();
                }
            }

            return currentWalkDir * moveSpeed;
        }

        public override float GetWeight(BasicEnemy enemy, int numSteerables) {
            return 0.1f;
        }

        public override void OnEnterState(BasicEnemy enemy) {

        }

        public override float GetChanceForState(BasicEnemy enemy) {
            return 0.1f;
        }

        public override void DoState(BasicEnemy enemy, List<SteeringEffect> steerings, CharacterController2D controller, List<Effect> effects) {
            var currentSteering = Vector2.zero;
            var totalWeight = 0.0f;

            foreach(SteeringEffect s in steerings) {
                currentSteering += s.GetSteering(enemy, currentSteering) * s.GetWeight(enemy, steerings.Count);
                totalWeight += s.GetWeight(enemy, steerings.Count);
            }

            currentSteering /= totalWeight;

            enemy.velocity = currentSteering * Time.deltaTime;

            enemy.ProbablisticState();

            controller.move(enemy.velocity);
        }

        public override void OnLeaveState(BasicEnemy enemy) {
        }
    }

}