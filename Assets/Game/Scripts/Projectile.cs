using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

namespace Andremani.ArcherySpineTest
{
    public class Projectile : MonoBehaviour
    {
        [Header("Scene references")]
        [SerializeField] private Rigidbody2D rgb;
        [SerializeField] private Collider2D projectileCollider;
        [SerializeField] private SkeletonAnimation skeletonAnimation;
        [Header("Animation options")]
        [SpineAnimation] [SerializeField] private string hitAnimationName;

        private Spine.AnimationState animationState;

        public void Initialize(Vector2 velocity)
        {
            rgb.velocity = transform.TransformVector(velocity);
        }

        private void Start()
        {
            animationState = skeletonAnimation.AnimationState;
        }

        private void Update()
        {
            if (rgb.velocity.magnitude > 0.01f)
            {
                Vector3 directionVector3 = rgb.velocity;
                Vector2 directionVector2 = new Vector2(directionVector3.x, directionVector3.y);
                directionVector2.Normalize();
                float angle = Vector2.SignedAngle(Vector2.right, directionVector2);
                rgb.rotation = angle;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            projectileCollider.enabled = false;
            rgb.velocity = Vector2.zero;
            rgb.gravityScale = 0;
            rgb.position = collision.GetContact(0).point;

            animationState.SetAnimation(0, hitAnimationName, false);
            animationState.Complete += HitEnd;
        }

        private void HitEnd(TrackEntry trackEntry)
        {
            Destroy(gameObject);
        }
    }
}