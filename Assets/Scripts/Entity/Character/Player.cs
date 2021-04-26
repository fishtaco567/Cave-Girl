using Rewired;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

namespace Entities.Character {
    public class Player : Effectable {

        public enum PState {
            Normal,
            RunCharge,
            Run,
            Jump,
            Attack,
            HoldingBow
        }

        public enum Direction {
            Up,
            Down,
            Left,
            Right
        }

        [Header("Objects")]
        [SerializeField]
        protected CharacterController2D controller;

        [SerializeField]
        protected SpriteRenderer sprite;

        [SerializeField]
        protected GameObject dropShadow;

        [SerializeField]
        protected Collider2D interactBox;

        [SerializeField]
        protected GameObject bombPrefab;

        [SerializeField]
        protected Animator swordAnimator;

        [SerializeField]
        protected Animator bowAnimator;

        [SerializeField]
        protected Animator playerAnimator;

        [SerializeField]
        protected GameObject directionalThings;

        [SerializeField]
        protected GameObject projectilePrefab;

        [Header("Movement")]
        [SerializeField]
        protected float moveSpeed;

        [SerializeField]
        protected float dashSpeed;

        [SerializeField]
        protected float dashNudgeSpeed;

        [SerializeField]
        protected float jumpSpeed;
        [SerializeField]
        protected float jumpYSpeed;

        [SerializeField]
        protected float maxAcceleration;

        [SerializeField]
        protected float minHeldDash;

        [Header("Attacks")]
        [SerializeField]
        protected float bombDelay;

        [SerializeField]
        protected float attackTime;

        [SerializeField]
        protected float bowHoldTime;

        [SerializeField]
        protected Collider2D swordCollider;

        [SerializeField]
        protected LayerMask hittable;

        [SerializeField]
        protected int swordDamage;

        [SerializeField]
        protected Transform arrowSpawnAnchor;

        [SerializeField]
        protected float baseArrowSpeed;

        [SerializeField]
        protected float chanceLosePowerupOnHit;

        [Header("State")]
        [SerializeField]
        protected Vector2 velocity;
        protected PState state;
        protected float stateTime;

        protected Vector2 runDirection;

        protected float playerYVelocity;
        protected bool grounded;

        public Tilemap tilemap;
        public TilemapHardness hardness;

        protected Rewired.Player rePlayer;

        protected Collider2D[] results;
        protected ContactFilter2D interactFilter;
        protected ContactFilter2D hittableFilter;

        protected float timeSinceLastBomb;

        protected Direction curDir;
        public bool isDead;

        public EntityInfo info;

        [SerializeField]
        protected GameObject swordObj;

        protected Utils.SRandom rand;

        public int maxBombs;

        public int curBombs;


        public override void Start() {
            base.Start();

            rand = new Utils.SRandom((uint)System.DateTime.Now.Millisecond);

            velocity = Vector2.zero;
            state = PState.Normal;
            stateTime = 0;

            runDirection = Vector2.right;

            info.yHeight = 0;
            playerYVelocity = 0;

            curBombs = maxBombs;

            grounded = true;

            rePlayer = ReInput.players.GetPlayer(0);

            results = new Collider2D[5];

            interactFilter = new ContactFilter2D();
            interactFilter.SetLayerMask(LayerMask.GetMask("Interact"));

            hittableFilter = new ContactFilter2D();
            hittableFilter.SetLayerMask(hittable);

            timeSinceLastBomb = bombDelay;

            curDir = Direction.Up;

            effects = new List<Effect>();

            isDead = false;

            var res = GetComponent<Resources>();
            res.OnDeath += OnDeath;
            res.OnHit += OnHit;
        }

        public void OnHit(int damage) {          
            if(damage > 0) {
                return;
            }

            if(rand.RandomChance(chanceLosePowerupOnHit * Mathf.Abs(damage)) && effects.Count != 0) {
                effects.RemoveAt(rand.RandomIntLessThan(effects.Count));
            }
        }

        public void OnDeath() {
            GameManager.Instance.OnDeath();
        }

        protected void Update() {
            if(isDead) {
                return;
            }

            flags.Clear();

            float scale = 1;
            foreach(Effect effect in effects) {
                effect.PerTick(this);
                effect.ChangeRange(this, ref scale);
            }

            swordObj.transform.localScale = new Vector3(scale, scale, scale);

            sprite.transform.localPosition = new Vector3(0, WorldConstants.upAspectRatio * info.yHeight, sprite.transform.localPosition.z);
            dropShadow.transform.localPosition = new Vector3(0, -WorldConstants.shadowAspectRatio * info.yHeight - 0.5f, dropShadow.transform.localPosition.z);
            dropShadow.transform.localScale = Vector3.one * (WorldConstants.shadowAspectRatio * info.yHeight) + new Vector3(0.75f, 0.75f, 0.75f);

            var interactPressed = rePlayer.GetButtonDown("Interact");
            if(interactPressed) {
                var num = interactBox.OverlapCollider(interactFilter, results);
                for(int i = 0; i < num; i++) {
                    results[i].GetComponent<Interactable>()?.OnInteract(this);
                }
            }

            var bombPressed = rePlayer.GetButtonDown("Bomb");
            timeSinceLastBomb += Time.deltaTime;
            if(bombPressed && timeSinceLastBomb > bombDelay && curBombs > 0) {
                curBombs--;
                timeSinceLastBomb = 0;
                var spawned = Instantiate(bombPrefab);
                spawned.transform.position = transform.position;
                var bomb = spawned.GetComponent<Bomb>();
                bomb.SetTilemap(tilemap, hardness);
                bomb.AddEffects(effects);
            }

            var swordPressed = rePlayer.GetButtonDown("Sword");
            if(swordPressed && state != PState.Attack) {
                ChangeState(PState.Attack);
                swordAnimator.SetTrigger("Attack");
            }

            var bowPressed = rePlayer.GetButtonDown("Bow");
            if(bowPressed && state != PState.HoldingBow) {
                ChangeState(PState.HoldingBow);
                bowAnimator.SetBool("HeldBack", true);
            }
            
            if(Mathf.Abs(velocity.x) > 0.1f || Mathf.Abs(velocity.y) > 0.1f) {
                if(Mathf.Abs(velocity.x) > Mathf.Abs(velocity.y)) {
                    curDir = velocity.x < 0 ? Direction.Left : Direction.Right;
                } else {
                    curDir = velocity.y < 0 ? Direction.Down : Direction.Up;
                }
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

        protected void FixedUpdate() {
            if(isDead) {
                return;
            }

            float dt = Time.fixedDeltaTime;
            foreach(Effect e in effects) {
                e.ChangeTime(this, ref dt);
            }

            var horiz = rePlayer.GetAxis("Horizontal");
            var vert = rePlayer.GetAxis("Vertical");

            var jumpHeld = rePlayer.GetButton("Jump");
            var dashHeld = rePlayer.GetButton("Dash");
            var bowHeld = rePlayer.GetButton("Bow");

            var desVel = Vector2.zero;

            stateTime += Time.fixedDeltaTime;

            info.yHeight += playerYVelocity;

            if(info.yHeight <= 0) {
                grounded = true;
                playerYVelocity = 0;
                info.yHeight = 0;
            } else {
                grounded = false;
                playerYVelocity -= WorldConstants.gravity * dt;
            }

            switch(state) {
                case PState.Normal: {
                    if(Mathf.Abs(horiz) > 0.1f || Mathf.Abs(vert) > 0.1f) {
                        var input = new Vector3(horiz, vert).normalized;

                        desVel = input * moveSpeed;
                    }

                    if(dashHeld) {
                        ChangeState(PState.RunCharge);
                    }

                    if(velocity.sqrMagnitude > 0.5f) {
                        TrySetAnimator("Run");
                    } else {
                        TrySetAnimator("Idle");
                    }

                    CheckChangeToJump(jumpHeld);
                    break;
                }
                case PState.RunCharge: {
                    if(!dashHeld) {
                        ChangeState(PState.Normal);
                    }

                    TrySetAnimator("DashCharge");

                    if(stateTime > minHeldDash) {
                        ChangeState(PState.Run);

                        runDirection = new Vector2(horiz, vert).normalized;
                    }

                    CheckChangeToJump(jumpHeld);
                    break;
                }
                case PState.Run: {
                    if(!dashHeld || controller.collisionState.above || controller.collisionState.below || controller.collisionState.left || controller.collisionState.right) {
                        ChangeState(PState.Normal);
                    }
                    var input = new Vector2(horiz, vert).normalized;

                    TrySetAnimator("Run");

                    desVel = (runDirection * dashSpeed + input * dashNudgeSpeed).normalized * dashSpeed;

                    CheckChangeToJump(jumpHeld);
                    CheckSwordAttack();
                    break;
                }
                case PState.Jump: {
                    if(grounded) {
                        ChangeState(PState.Normal);
                    }

                    if(playerYVelocity > 0) {
                        TrySetAnimator("JumpUp");
                    } else {
                        TrySetAnimator("Fall");
                    }

                    var input = new Vector2(horiz, vert).normalized;

                    desVel = runDirection * jumpSpeed + input * dashNudgeSpeed;
                    break;
                }
                case PState.Attack: {
                    TrySetAnimator("Idle");

                    if(stateTime > attackTime) {
                        ChangeState(PState.Normal);
                        CheckSwordAttack();
                    }
                    break;
                }
                case PState.HoldingBow: {
                    if(Mathf.Abs(horiz) > 0.1f || Mathf.Abs(vert) > 0.1f) {
                        if(Mathf.Abs(horiz) > Mathf.Abs(vert)) {
                            curDir = horiz < 0 ? Direction.Left : Direction.Right;
                        } else {
                            curDir = vert < 0 ? Direction.Down : Direction.Up;
                        }
                    }

                    if(stateTime > bowHoldTime && !bowHeld) {
                        ChangeState(PState.Normal);
                        bowAnimator.SetTrigger("Shoot");
                        bowAnimator.SetBool("HeldBack", false);
                        SpawnArrow();
                    }

                    if(!bowHeld) {
                        bowAnimator.SetBool("HeldBack", false);
                        ChangeState(PState.Normal);
                    }

                    break;
                }
            }

            var delta = desVel - velocity;
            if(delta.magnitude > maxAcceleration) {
                delta = delta.normalized * maxAcceleration;
            }

            velocity += delta;
            controller.move(velocity * dt);
        }

        private void TrySetAnimator(string name) {
            var dir = "_Front";

            switch(curDir) {
                case Direction.Up:
                    dir = "_Back";
                    break;
                case Direction.Down:
                    dir = "_Front";
                    break;
                case Direction.Left:
                case Direction.Right:
                    dir = "_Side";
                    break;
            }

            name = name + dir;
            var animState = playerAnimator.GetCurrentAnimatorStateInfo(0);
            if(animState.IsName(name)) {
                return;
            }

            playerAnimator.Play(name, 0, animState.normalizedTime);
        }

        private void SpawnArrow() {
            var spawned = Instantiate(projectilePrefab);
            spawned.transform.position = arrowSpawnAnchor.position;
            spawned.transform.parent = GameManager.Instance.holder.transform;
            var proj = spawned.GetComponent<Projectile>();
            var arrowDir = Vector2.up;

            switch(curDir) {
                case Direction.Down:
                    arrowDir = Vector2.down;
                    break;
                case Direction.Left:
                    arrowDir = Vector2.left;
                    break;
                case Direction.Right:
                    arrowDir = Vector2.right;
                    break;
            }
            proj.SetVelocity(arrowDir * baseArrowSpeed);

            var arrowFx = new List<Effect>(effects.Count);
            foreach(Effect e in effects) {
                arrowFx.Add(e.GenerateCopy());
            }

            proj.doNotHit = this.GetComponent<Resources>();

            proj.AddEffects(arrowFx);
        }

        private void CheckSwordAttack() {
            foreach(Effect e in effects) {
                e.DuringAttack(this, swordCollider.bounds.center);
            }

            var num = swordCollider.OverlapCollider(hittableFilter, results);
            for(int i = 0; i < num; i++) {
                var health = results[i].GetComponent<Resources>();

                if(health != null) {
                    foreach(Effect e in effects) {
                        e.OnHit(this, health);
                    }

                    health.Damage(swordDamage);
                }
            }
        }

        private void CheckChangeToJump(bool jump) {
            if(jump) {
                ChangeState(PState.Jump);
                playerYVelocity = jumpYSpeed;

                var horiz = rePlayer.GetAxis("Horizontal");
                var vert = rePlayer.GetAxis("Vertical");
                runDirection = new Vector2(horiz, vert).normalized;
            }
        }

        private void ChangeState(PState newState) {
            state = newState;
            stateTime = 0;
        }

        public override void Destroy() {
            foreach(Effect e in effects) {
                e.OnDestroyed(this);
            }
        }
    }

}