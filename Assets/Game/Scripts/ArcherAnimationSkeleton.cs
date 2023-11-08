using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

namespace Andremani.ArcherySpineTest
{
    public class ArcherAnimationSkeleton : MonoBehaviour
    {
        [Header("Scene asset references")]
        [SerializeField] private SkeletonAnimation skeletonAnimation;
        private Spine.AnimationState animationState;

        [Header("Bones")]
        [SpineBone(dataField: "skeletonAnimation")] [SerializeField] private string controlBoneName;
        [SpineBone(dataField: "skeletonAnimation")] [SerializeField] private string centralBoneName;
        [SpineBone(dataField: "skeletonAnimation")] [SerializeField] private string projectileSourceBoneName;
        private Bone controlBone;
        private Bone centralBone;
        private Bone projectileSourceBone;

        [Header("Animations")]
        [SpineAnimation] [SerializeField] private string shootAnimationName;
        [SpineAnimation] [SerializeField] private string reloadAnimationName;
        [SpineAnimation] [SerializeField] private string targetAnimationName;

        [Header("Animation events")]
        [SpineEvent(dataField: "skeletonAnimation", fallbackToTextField: true)] [SerializeField] private string shootEventName;
        private Spine.EventData shootEventData;

        public Vector3 CenterSkeletonLocation { get; private set; }

        public event System.Action OnReleaseProjectile;

        void OnValidate()
        {
            if (skeletonAnimation == null)
            {
                skeletonAnimation = GetComponent<SkeletonAnimation>();
            }
        }

        private void Start()
        {
            if (!skeletonAnimation.valid)
            {
                return;
            }

            animationState = skeletonAnimation.AnimationState;

            controlBone = skeletonAnimation.Skeleton.FindBone(controlBoneName);
            centralBone = skeletonAnimation.Skeleton.FindBone(centralBoneName);
            projectileSourceBone = skeletonAnimation.Skeleton.FindBone(projectileSourceBoneName);

            CenterSkeletonLocation = centralBone.GetWorldPosition(skeletonAnimation.transform);

            shootEventData = skeletonAnimation.Skeleton.Data.FindEvent(shootEventName);
            skeletonAnimation.AnimationState.Event += HandleAnimationStateEvent;
        }

        private void HandleAnimationStateEvent(TrackEntry trackEntry, Spine.Event e)
        {
            if (shootEventData == e.Data)
            {
                OnReleaseProjectile?.Invoke();
            }
        }

        public void SetSkeletonAimingRotation(float angle)
        {
            controlBone.Rotation = angle;
        }

        public Vector3 GetProjectileSpawnLocation()
        {
            return projectileSourceBone.GetWorldPosition(skeletonAnimation.transform);
        }

        public float GetProjectileSpawnAngle()
        {
            return projectileSourceBone.LocalToWorldRotation(projectileSourceBone.Rotation);
        }

        public void PlayShootAnimation()
        {
            animationState.SetAnimation(0, shootAnimationName, false);
        }

        public void PlayReloadAnimation(System.Action OnReloadEnd)
        {
            animationState.AddAnimation(0, reloadAnimationName, false, 0);
            TrackEntry currentTrackEntry = animationState.AddAnimation(0, targetAnimationName, false, 0);
            currentTrackEntry.Start += OnReloadAnimationTrackEnd;

            void OnReloadAnimationTrackEnd(TrackEntry track)
            {
                track.Start -= OnReloadAnimationTrackEnd;
                OnReloadEnd?.Invoke();
            }
        }
    }
}