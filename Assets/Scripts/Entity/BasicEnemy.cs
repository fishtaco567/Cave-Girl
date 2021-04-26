using UnityEngine;
using System.Collections.Generic;
using Utils;

namespace Entities {
    public class BasicEnemy : Effectable {

        public enum Direction {
            Up,
            Down,
            Left,
            Right
        }

        [SerializeField]
        protected Collider2D hitbox;
        [SerializeField]
        public CharacterController2D controller;

        [SerializeField]
        public SpriteRenderer sprite;


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

        [SerializeField]
        protected GameObject directionalThings;

        public GameObject basicSpawnAnchor;

        protected SRandom rand;

        public EntityInfo info;
        protected Animator anim;

        [SerializeField]
        protected Direction curDir;

        public string animName;

        public override void Start() {
            base.Start();

            curDir = Direction.Up;

            anim = GetComponent<Animator>();

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
            flags.Clear();
            foreach(Effect e in effects) {
                e.PerTick(this);
            }

            if(Mathf.Abs(velocity.x) > 0.0001f || Mathf.Abs(velocity.y) > 0.0001f) {
                if(Mathf.Abs(velocity.x) > Mathf.Abs(velocity.y)) {
                    curDir = velocity.x < 0 ? Direction.Left : Direction.Right;
                } else {
                    curDir = velocity.y < 0 ? Direction.Down : Direction.Up;
                }
            }

            if(anim != null) {
                TrySetAnimator(animName);
            }
            switch(curDir) {
                case Direction.Up:
                    sprite.flipX = true;
                    directionalThings.transform.rotation = Quaternion.Euler(0, 0, 0);
                    directionalThings.transform.localPosition = new Vector3(0, 0, 0.1f);
                    break;
                case Direction.Down:
                    sprite.flipX = true;
                    directionalThings.transform.rotation = Quaternion.Euler(0, 0, 180);
                    directionalThings.transform.localPosition = new Vector3(0, -0.1875f, -0.1f);
                    break;
                case Direction.Left:
                    sprite.flipX = true;
                    directionalThings.transform.rotation = Quaternion.Euler(0, 0, 90);
                    directionalThings.transform.localPosition = new Vector3(0, -0.125f, 0.1f);
                    break;
                case Direction.Right:
                    sprite.flipX = false;
                    directionalThings.transform.rotation = Quaternion.Euler(0, 0, 270);
                    directionalThings.transform.localPosition = new Vector3(0, -0.125f, 0.1f);
                    break;
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

            for(int i = steerings.Count - 1; i >= 0; i--) {
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
            if(spawnOnDeath != null) {
                var spawned = Instantiate(spawnOnDeath);
                spawned.transform.position = transform.position;
            }

            foreach(Effect e in effects) {
                e.OnDestroyed(this);
            }

            Destroy(this.gameObject);
        }

        private void TrySetAnimator(string name) {
            var dir = "_Up";

            switch(curDir) {
                case Direction.Up:
                    dir = "_Up";
                    break;
                case Direction.Down:
                    dir = "_Down";
                    break;
                case Direction.Left:
                case Direction.Right:
                    dir = "_Side";
                    break;
            }

            name = name + dir;
            var animState = anim.GetCurrentAnimatorStateInfo(0);
            if(animState.IsName(name)) {
                return;
            }

            anim.Play(name, 0, animState.normalizedTime);
        }
    }

}