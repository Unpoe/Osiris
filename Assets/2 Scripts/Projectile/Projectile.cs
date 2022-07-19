using UnityEngine;

namespace Osiris
{
    public class Projectile : MonoBehaviour, IPoolable
    {
        [SerializeField] private float speed = 5f;
        [SerializeField] private float distanceToCollide = 0.2f;

        private Actor target = null;
        private float damage = 0f;

        public void Initialize(Vector3 startingPoint, Actor target, float damage) {
            this.target = target;
            this.damage = damage;

            transform.position = startingPoint;
        }

        public bool GameUpdate(float dt) {
            if(target == null || !target.gameObject.activeInHierarchy) {
                return false;
            }

            Transform cachedTransform = transform;
            Vector3 targetPos = target.GetChestPosition();

            Vector3 dir = targetPos - cachedTransform.position;

            float distance = dir.magnitude;
            if(distance <= distanceToCollide) {
                target.ApplyDamage(damage);
                return false;
            }

            dir.Normalize();
            cachedTransform.position += dir * speed * dt;
            transform.forward = dir;

            return true;
        }

        public void OnGetFromPool() {
            gameObject.SetActive(true);
        }

        public void OnReturnToPool() {
            gameObject.SetActive(false);
        }
    }
}