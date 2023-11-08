using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Andremani.ArcherySpineTest
{
    public class ArcherAttackSystem : MonoBehaviour
    {
        [Header("Scene references")]
        [SerializeField] private InputSystem input;
        [SerializeField] private ArcherAnimationSkeleton archerAnimationSkeleton;
        [SerializeField] private Transform projectilesParent;
        [Header("Assets references")]
        [SerializeField] private Projectile projectilePrefab;
        [Header("Options")]
        [SerializeField] private float initialProjectileSpeed;
        [SerializeField] private float minProjectileSpeed;
        [SerializeField] private float maxProjectileSpeed;
        [SerializeField] private float minProjectileSpeedDistance;
        [SerializeField] private float maxProjectileSpeedDistance;

        private bool canShoot = true;

        public event System.Action OnStartShooting;
        public event System.Action OnEndReloading;

        private void Start()
        {
            input.OnMainPointerUp += TryShoot;
            archerAnimationSkeleton.OnReleaseProjectile += ReleaseProjectile;
        }

        private void Update()
        {
            InitialProjectileSpeedAdjusting();
        }

        private void InitialProjectileSpeedAdjusting()
        {
            Vector3 directionVector3 = input.WorldMousePosition - archerAnimationSkeleton.CenterSkeletonLocation;
            Vector2 directionVector2 = new Vector2(directionVector3.x, directionVector3.y);
            float speedControlDistance = Mathf.Clamp(directionVector2.magnitude, minProjectileSpeedDistance, maxProjectileSpeedDistance);
            float speedFactor = Mathf.InverseLerp(minProjectileSpeedDistance, maxProjectileSpeedDistance, speedControlDistance);
            initialProjectileSpeed = Mathf.Lerp(minProjectileSpeed, maxProjectileSpeed, speedFactor);
        }

        private void TryShoot()
        {
            if (canShoot)
            {
                canShoot = false;
                StartShooting();
            }
        }

        private void StartShooting()
        {
            OnStartShooting?.Invoke();
            archerAnimationSkeleton.PlayShootAnimation();
            //catching shooting animation event (OnReleaseProjectile) will lead to projectile spawn
        }

        private void ReleaseProjectile()
        {
            Vector3 projectileSpawnPoint = archerAnimationSkeleton.GetProjectileSpawnLocation();
            Quaternion projectileSpawnOrientation = Quaternion.Euler(0, 0, archerAnimationSkeleton.GetProjectileSpawnAngle());
            Projectile projectile = Instantiate(projectilePrefab, projectileSpawnPoint, projectileSpawnOrientation, projectilesParent);

            projectile.Initialize(Vector2.right * initialProjectileSpeed);

            Reload();
        }

        private void Reload()
        {
            archerAnimationSkeleton.PlayReloadAnimation(OnReloadEnd);
            //wait until this animation ends, it will call "OnReloadEnd" then (by delegate)
        }

        private void OnReloadEnd()
        {
            OnEndReloading?.Invoke();
            canShoot = true;
        }

        public Vector3 GetProjectilePredictedPosition(float time)
        {
            Vector3 projectileSpawnPoint = archerAnimationSkeleton.GetProjectileSpawnLocation();
            float angle = archerAnimationSkeleton.GetProjectileSpawnAngle();
            Vector2 gravity = Physics2D.gravity;

            Vector3 predictedPosition;
            predictedPosition.x = projectileSpawnPoint.x + Mathf.Cos(Mathf.Deg2Rad * angle) * initialProjectileSpeed * time + gravity.x * time * time / 2;
            predictedPosition.y = projectileSpawnPoint.y + Mathf.Sin(Mathf.Deg2Rad * angle) * initialProjectileSpeed * time + gravity.y * time * time / 2;
            predictedPosition.z = 0;

            return predictedPosition;
        }
    }
}