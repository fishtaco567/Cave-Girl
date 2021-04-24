using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

namespace Entities {

    public class Bomb : Effectable {

        [SerializeField]
        protected float range;

        [SerializeField]
        protected float timeToExplode;

        [SerializeField]
        protected float currentTime;

        [SerializeField]
        protected Animator anim;

        [SerializeField]
        protected ContactFilter2D filter;

        [SerializeField]
        protected int strength;

        [SerializeField]
        protected GameObject smokeToSpawn;

        [SerializeField]
        protected Collider2D[] results;

        protected Tilemap tilemap;
        protected TilemapHardness hardness;

        public override void Start() {
            currentTime = 0;

            results = new Collider2D[10];
        }

        public void SetTilemap(Tilemap newMap, TilemapHardness newHardness) {
            tilemap = newMap;
            hardness = newHardness;
        }

        protected void Update() {
            var currentDt = Time.deltaTime;
            var timeScale = 1.0f;

            foreach(Effect e in effects) {
                e.ChangeTime(this, ref timeScale);
            }

            currentTime += currentDt * timeScale;

            anim.speed = timeScale;

            if(currentTime > timeToExplode) {
                Explode();
            }
        }

        private void Explode() {
            var currentRange = range;

            foreach(Effect e in effects) {
                e.ChangeRange(this, ref currentRange);
            }

            var num = Physics2D.OverlapCircle(transform.position, range, filter, results);
            for(int i = 0; i < num; i++) {
                var res = results[i].GetComponent<Resources>();
                
                if(res != null) {
                    res.Damage(strength);

                    foreach(Effect e in effects) {
                        e.OnHit(this, res);
                    }
                }
            }

            var intrange = Mathf.RoundToInt(currentRange);
            var rangeSq = currentRange * currentRange;

            for(int i = -intrange; i < intrange; i++) {
                for(int j = -intrange; j < intrange; j++) {
                    if((i * i) + (j * j) > rangeSq) {
                        continue;
                    }

                    var x = i + Mathf.RoundToInt(transform.position.x);
                    var y = j + Mathf.RoundToInt(transform.position.y);

                    Instantiate(smokeToSpawn).transform.position = new Vector3(x, y, -.1f);

                    if(x >= 0 && x < hardness.width && y >= 0 && y < hardness.height) {
                        if(hardness.GetHardness(x, y) <= strength) {
                            tilemap.SetTile(new Vector3Int(x, y, 0), hardness.underWallTile);
                        }
                    }
                }
            }

            Destroy(this.gameObject);
        }

        public override void Destroy() {
            Explode();
        }

    }

}