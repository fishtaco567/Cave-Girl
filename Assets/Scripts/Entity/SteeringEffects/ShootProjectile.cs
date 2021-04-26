using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Entities.Steering {

    [CreateAssetMenu(fileName = "ShootProjectile", menuName = "Steering/ShootProjectile")]
    public class ShootProjectile : SteeringEffect {

        [SerializeField]
        protected float attackInTime;

        [SerializeField]
        protected float attackRange;

        [SerializeField]
        protected LayerMask toAttack;
        protected ContactFilter2D filter;

        [SerializeField]
        protected float attackChance;
        [SerializeField]
        protected float offChance;

        [SerializeField]
        protected GameObject[] projectiles;

        protected float currentTime;
        protected Collider2D[] results;

        protected Utils.SRandom rand;

        [SerializeField]
        protected float speed;

        protected void Awake() {
            filter = new ContactFilter2D();
            filter.SetLayerMask(toAttack);

            rand = new Utils.SRandom((uint)System.DateTime.Now.Millisecond);

            results = new Collider2D[2];
            currentTime = 0;
        }

        public override Vector2 GetSteering(BasicEnemy enemy, Vector2 currentSteering) {
            return Vector2.zero;
        }

        public override float GetWeight(BasicEnemy enemy, int numSteerables) {
            return 0;
        }

        public override void DoState(BasicEnemy enemy, List<SteeringEffect> steerings, CharacterController2D controller, List<Effect> effects) {
            enemy.velocity = Vector2.zero;
            currentTime += Time.deltaTime;

            if(currentTime > attackInTime) {
                enemy.ProbablisticState();
                SpawnProjectile(enemy);
            }
        }

        protected void SpawnProjectile(BasicEnemy enemy) {
            var player = FindPlayer(enemy.transform.position);

            var direction = Vector2.zero;
            if(player == null) {
                direction = rand.RandomDirection2D();
            } else {
                direction = player.transform.position - enemy.transform.position;
                direction = direction.normalized;
            }

            var spawned = Instantiate(projectiles[rand.RandomIntLessThan(projectiles.Length)]);
            spawned.transform.position = enemy.basicSpawnAnchor.transform.position;
            spawned.transform.parent = GameManager.Instance.holder.transform;
            var proj = spawned.GetComponent<Projectile>();
            if(proj != null) {
                proj.SetVelocity(direction * speed);
                proj.doNotHit = enemy.GetComponent<Resources>();
            }
        }

        public override float GetChanceForState(BasicEnemy enemy) {
            var player = FindPlayer(enemy.transform.position);

            if(player == null) {
                return offChance;
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