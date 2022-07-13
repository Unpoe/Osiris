using System;
using System.Collections.Generic;
using UnityEngine;

namespace Osiris
{
    public class Pool<T> where T : IPoolable
    {
        private List<T> allInstances; // A poolable object is never removed from this list (used in the ReturnAll function)
        private List<T> savedInstances; // A poolable object is removed from this list when you use the Get function
        private Func<T> createInstanceFunc;

        public Pool(Func<T> createInstanceFunc) {
            allInstances = new List<T>();
            savedInstances = new List<T>();
            this.createInstanceFunc = createInstanceFunc;
        }

        public T Get() {
            T instance;

            if(savedInstances.Count > 0) {
                instance = savedInstances[0];
                savedInstances.Remove(instance);
            } else {
                instance = createInstanceFunc.Invoke();
                allInstances.Add(instance);
            }

            instance.OnGetFromPool();
            return instance;
        }

        public void Return(T instance) {
            if (savedInstances.Contains(instance)) {
                Debug.LogWarning("(Pool) Attempting to return an instance that is already saved in the pool.");
                return;
            }

            instance.OnReturnToPool();
            savedInstances.Add(instance);
        }

        public void ReturnAll() {
            for(int i = 0; i < allInstances.Count; i++) {
                Return(allInstances[i]);
            }
        }
    }
}