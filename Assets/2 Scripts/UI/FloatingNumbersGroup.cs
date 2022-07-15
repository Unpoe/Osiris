using System.Collections.Generic;
using UnityEngine;

namespace Osiris
{
    public class FloatingNumbersGroup : MonoBehaviour
    {
        [SerializeField] private FloatingNumber floatingNumberPrefab = default;
        [SerializeField] private RectTransform floatingNumberHolder = default;

        private Camera mainCamera = null;

        private Pool<FloatingNumber> floatingNumberPool = null;
        private List<FloatingNumber> floatingNumberInstances;

        public void Initialize(Camera mainCamera) {
            this.mainCamera = mainCamera;
            floatingNumberPool = new Pool<FloatingNumber>(CreateFloatingNumberInstance);
            floatingNumberInstances = new List<FloatingNumber>();
        }

        public void AddNumber(float number, Vector3 worldPos) {
            FloatingNumber instance = floatingNumberPool.Get();
            floatingNumberInstances.Add(instance);
            instance.Initialize(number, worldPos, mainCamera);
        }
        
        public void GameUpdate(float dt) {
            for (int i = 0; i < floatingNumberInstances.Count; i++) {
                FloatingNumber floatingNumber = floatingNumberInstances[i];
                if (!floatingNumber.GameUpdate(dt)) {
                    int lastIndex = floatingNumberInstances.Count - 1;
                    floatingNumberInstances[i] = floatingNumberInstances[lastIndex];
                    floatingNumberInstances.RemoveAt(lastIndex);
                    i -= 1;
                    floatingNumberPool.Return(floatingNumber);
                }
            }
        }

        private FloatingNumber CreateFloatingNumberInstance() {
            return Instantiate(floatingNumberPrefab, floatingNumberHolder);
        }
    }
}