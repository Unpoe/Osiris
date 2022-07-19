using UnityEngine;

namespace Osiris
{
    public class ProjectileFactory : MonoBehaviour
    {
        [SerializeField] private Projectile prefab = default;

        private Pool<Projectile> pool = null;

        public void Initialize() {
            pool = new Pool<Projectile>(CreateInstance);
        }

        public Projectile Get() {
            return pool.Get();
        }

        public void Reclaim(Projectile projectile) {
            pool.Return(projectile);
        }

        private Projectile CreateInstance() {
            return Instantiate(prefab, transform);
        }
    }
}