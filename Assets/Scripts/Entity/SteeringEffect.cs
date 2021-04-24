using UnityEngine;
using System.Collections.Generic;

namespace Entities {
    public abstract class SteeringEffect : ScriptableObject {

        public string type;

        public abstract Vector2 GetSteering(BasicEnemy enemy, Vector2 currentSteering);

        public abstract float GetWeight(BasicEnemy enemy, int numSteerables);

        public virtual float GetChanceForState(BasicEnemy enemy) {
            return 0;
        }

        public virtual void OnEnterState(BasicEnemy enemy) {

        }

        public virtual void DoState(BasicEnemy enemy, List<SteeringEffect> steerings, CharacterController2D controller, List<Effect> effects) {
            enemy.ChangeToDefaultState();
        }

        public virtual void OnLeaveState(BasicEnemy enemy) {
            
        }

    }
}
