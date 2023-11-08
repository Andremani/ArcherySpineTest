using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Andremani.ArcherySpineTest
{
    public class ArcherRotationController : MonoBehaviour
    {
        [Header("Scene references")]
        [SerializeField] private InputSystem input;
        [SerializeField] private ArcherAnimationSkeleton archerAnimationSkeleton;

        private void Update()
        {
            RotateArcher();
        }

        private void RotateArcher()
        {
            Vector3 directionVector3 = input.WorldMousePosition - archerAnimationSkeleton.CenterSkeletonLocation;
            Vector2 directionVector2 = new Vector2(directionVector3.x, directionVector3.y);
            directionVector2 = -directionVector2;
            directionVector2.Normalize();

            float angle = Vector2.SignedAngle(Vector2.right, directionVector2);
            archerAnimationSkeleton.SetSkeletonAimingRotation(angle);
        }
    }
}