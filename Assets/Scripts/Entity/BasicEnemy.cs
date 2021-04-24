using UnityEngine;
using System.Collections.Generic;
using Utils;

namespace Entities {
    public class BasicEnemy : Effectable {

        [SerializeField]
        protected Collider2D hitbox;
        [SerializeField]
        public CharacterController2D controller;

        [SerializeField]
        protected float damage;

        [SerializeField]
        protected List<SteeringEffect> steerings;

        [SerializeField]
        protected int defaultSteeringState;

        [SerializeField]
        protected GameObject spawnOnDeath;

        public Vector2 velocity;

        [SerializeField]
        protected int currentSteeringState;

        protected SRandom rand;

        public override void Start() {
            base.Start();

            var instSteerings = new List<SteeringEffect>(steerings.Count);

            foreach(SteeringEffect s in steerings) {
                instSteerings.Add(Instantiate(s));
            }

            steerings = instSteerings;

            velocity = Vector2.zero;

            rand = new SRandom((uint) System.DateTime.Now.Millisecond);
        }

        protected virtual void FixedUpdate() {
            steerings[currentSteeringState].DoState(this, steerings, controller, effects);
        }

        protected virtual void Update() {
            foreach(Effect e in effects) {
                e.PerTick(this);
            }
        }

        public void ChangeToDefaultState() {
            ChangeToState(defaultSteeringState);
        }

        public void ChangeToState(int i) {
            if(i >= steerings.Count) {
                return;
            }

            steerings[currentSteeringState].OnLeaveState(this);
            currentSteeringState = i;
            steerings[currentSteeringState].OnEnterState(this);
        }

        public void ProbablisticState() {
            List<float> chances = new List<float>(steerings.Count);
            List<bool> canChangeTo = new List<bool>(steerings.Count);
            float curChance = 0;

            for(int i = 0; i < steerings.Count; i++) {
                chances.Add(curChance);
                var thisChance = steerings[i].GetChanceForState(this);
                curChance += thisChance;
                if(thisChance == 0) {
                    canChangeTo.Add(false);
                } else {
                    canChangeTo.Add(true);
                }
            }

            var randNum = rand.RandomFloatInRange(0, curChance);

            for(int i = steerings.Count - 1; i > 0; i--) {
                if(i == currentSteeringState) {
                    continue;
                }

                if(chances[i] < randNum && canChangeTo[i]) {
                    ChangeToState(i);
                    break;
                }
            }
        }

        public void AddSteering(SteeringEffect effect) {
            steerings.Add(effect);
        }

        public virtual void OnSpawned() {
            foreach(Effect e in effects) {
                e.OnSpawn(this);
            }
        }

        public override void Destroy() {
            var spawned = Instantiate(spawnOnDeath);
            spawned.transform.position = transform.position;

            foreach(Effect e in effects) {
                e.OnDestroyed(this);
            }

            Destroy(this.gameObject);
        }
    }

}