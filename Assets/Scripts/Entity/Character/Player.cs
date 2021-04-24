using Rewired;
using UnityEngine;
using System.Collections.Generic;

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
        protected GameObject sprite;

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

        [Header("State")]
        [SerializeField]
        protected Vector2 velocity;
        protected PState state;
        protected float stateTime;

        protected Vector2 runDirection;

        [SerializeField]
        protected float playerY;
        protected float playerYVelocity;
        protected bool grounded;

        protected Rewired.Player rePlayer;

        protected Collider2D[] results;
        protected ContactFilter2D interactFilter;
        protected ContactFilter2D hittableFilter;

        protected float timeSinceLastBomb;

        protected Direction curDir;

        protected void Start() {
            velocity = Vector2.zero;
            state = PState.Normal;
            stateTime = 0;

            runDirection = Vector2.right;

            playerY = 0;
            playerYVelocity = 0;

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
        }

        protected void Update() {
            foreach(Effect effect in effects) {
                effect.PerTick(this);
            }

            sprite.transform.localPosition = new Vector3(0, WorldConstants.upAspectRatio * playerY, sprite.transform.localPosition.z);
            dropShadow.transform.localPosition = new Vector3(0, -WorldConstants.shadowAspectRatio * playerY - 0.5f, dropShadow.transform.localPosition.z);
            dropShadow.transform.localScale = Vector3.one * (WorldConstants.shadowAspectRatio * playerY) + Vector3.one;

            var interactPressed = rePlayer.GetButtonDown("Interact");
            if(interactPressed) {
                var num = interactBox.OverlapCollider(interactFilter, results);
                for(int i = 0; i < num; i++) {
                    results[i].GetComponent<Interactable>()?.OnInteract(this);
                }
            }

            var bombPressed = rePlayer.GetButtonDown("Bomb");
            timeSinceLastBomb += Time.deltaTime;
            if(bombPressed && timeSinceLastBomb > bombDelay) {
                timeSinceLastBomb = 0;
                var spawned = Instantiate(bombPrefab);
                spawned.transform.position = transform.position;
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
                    directionalThings.transform.rotation = Quaternion.Euler(0, 0, 0);
                    break;
                case Direction.Down:
                    directionalThings.transform.rotation = Quaternion.Euler(0, 0, 180);
                    break;
                case Direction.Left:
                    directionalThings.transform.rotation = Quaternion.Euler(0, 0, 90);
                    break;
                case Direction.Right:
                    directionalThings.transform.rotation = Quaternion.Euler(0, 0, 270);
                    break;
            }
        }

        protected void FixedUpdate() {
            var horiz = rePlayer.GetAxis("Horizontal");
            var vert = rePlayer.GetAxis("Vertical");

            var jumpHeld = rePlayer.GetButton("Jump");
            var dashHeld = rePlayer.GetButton("Dash");
            var bowHeld = rePlayer.GetButton("Bow");

            var desVel = Vector2.zero;

            stateTime += Time.fixedDeltaTime;

            playerY += playerYVelocity;

            if(playerY <= 0) {
                grounded = true;
                playerYVelocity = 0;
                playerY = 0;
            } else {
                grounded = false;
                playerYVelocity -= WorldConstants.gravity * Time.fixedDeltaTime;
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

                    CheckChangeToJump(jumpHeld);
                    break;
                }
                case PState.RunCharge: {
                    if(!dashHeld) {
                        ChangeState(PState.Normal);
                    }

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

                    desVel = (runDirection * dashSpeed + input * dashNudgeSpeed).normalized * dashSpeed;

                    CheckChangeToJump(jumpHeld);
                    break;
                }
                case PState.Jump: {
                    if(grounded) {
                        ChangeState(PState.Normal);
                    }

                    var input = new Vector2(horiz, vert).normalized;

                    desVel = runDirection * jumpSpeed + input * dashNudgeSpeed;
                    break;
                }
                case PState.Attack: {
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
            controller.move(velocity * Time.fixedDeltaTime);
        }

        private void SpawnArrow() {
            var spawned = Instantiate(projectilePrefab);
            spawned.transform.position = arrowSpawnAnchor.position;
            var proj = spawned.GetComponent<Projectile>();
            proj.SetVelocity((proj.transform.position - transform.position).normalized * baseArrowSpeed);

            var arrowFx = new List<Effect>(effects.Count);
            foreach(Effect e in effects) {
                arrowFx.Add(e.GenerateCopy());
            }

            proj.SetEffects(arrowFx);
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