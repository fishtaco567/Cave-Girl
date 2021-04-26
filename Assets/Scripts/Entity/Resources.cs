using UnityEngine;
using System;

namespace Entities {
    public class Resources : MonoBehaviour {

        [SerializeField]
        public float iframes;
        public int maxHealth;

        public Action<int> OnHit;
        public Action OnDeath;
        public Action<int> OnHeal;

        protected float timeSinceHit;

        [SerializeField]
        private int _health;
        public int Health {
            get {
                return _health;
            }
            set {
                if(value < 0) {
                    OnDeath?.Invoke();
                    _health = 0;
                    return;
                }

                if(value < _health && timeSinceHit > iframes) {
                    timeSinceHit = 0;
                    OnHit?.Invoke(value - _health);
                } else if(value > _health) {
                    OnHeal?.Invoke(value - _health);
                }

                _health = value;

                if(_health > maxHealth) {
                    _health = maxHealth;
                }
            }
        }

        protected void Start() {
            timeSinceHit = 0;
        }

        protected void Update() {
            timeSinceHit += Time.deltaTime;
        }

        public bool Damage(int damage) {
            if(damage == 0) {
                return false;
            }
            
            if(damage != 0) {
                OnHit?.Invoke(-damage);
            }

            if(timeSinceHit < iframes) {
                return false;
            }

            timeSinceHit = 0;
            _health -= damage;

            if(_health <= 0) {
                _health = 0;
                OnDeath?.Invoke();
            }

            return true;
        }

    }

}