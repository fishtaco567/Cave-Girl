using UnityEngine;
using System.Collections.Generic;

namespace Utils {
    public class GrabBag<T> {

        public List<T> items;
        public List<float> weights;

        private bool removeAfter;

        public GrabBag(bool removeAfter = false) {
            items = new List<T>();
            weights = new List<float>();
            this.removeAfter = removeAfter;
        }

        public void AddItem(T item, float weight) {
            items.Add(item);
            weights.Add(weight);
        }

        public void AddItems(List<T> items, List<float> weights) {
            if(items.Count != weights.Count) {
                return;
            }

            for(int i = 0; i < items.Count; i++) {
                this.items.Add(items[i]);
                this.weights.Add(weights[i]);
            }
        }

        public void AddItems(T[] items, float[] weights) {
            if(items.Length != weights.Length) {
                return;
            }

            for(int i = 0; i < items.Length; i++) {
                this.items.Add(items[i]);
                this.weights.Add(weights[i]);
            }
        }

        public bool IsEmpty() {
            return items.Count <= 0;
        }

        public T GetItem() {
            List<float> chances = new List<float>(items.Count);
            float curChance = 0;

            for(int i = 0; i < items.Count; i++) {
                chances.Add(curChance);
                var thisChance = weights[i];

                curChance += thisChance;
            }

            var randNum = GameManager.Instance.rand.RandomFloatInRange(0, curChance);

            for(int i = items.Count - 1; i >= 0; i--) {
                if(chances[i] < randNum) {
                    var chosen = items[i];
                    if(removeAfter) {
                        items.RemoveAt(i);
                        weights.RemoveAt(i);
                    }
                    return chosen;
                }
            }

            return items[0];
        }

    }
}
