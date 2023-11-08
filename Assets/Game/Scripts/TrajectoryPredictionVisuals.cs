using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Andremani.ArcherySpineTest
{
    public class TrajectoryPredictionVisuals : MonoBehaviour
    {
        [Header("Scene references")]
        [SerializeField] private List<Transform> trajectoryPoints;
        [SerializeField] private Transform visualsParent;
        [SerializeField] private ArcherAttackSystem archerAttackSystem;
        [Header("Assets references")]
        [SerializeField] private Transform trajectoryPointPrefab;
        [Header("Options")]
        [SerializeField] private int trajectoryPointsAmount;
        [SerializeField] private float timeBetweenTrajectoryPoints;
        [SerializeField] private float pointBiggestScale;
        [SerializeField] private float pointSmallestScale;

        private void Start()
        {
            while (trajectoryPoints.Count < trajectoryPointsAmount)
            {
                Transform newTrajectoryPoint = Instantiate(trajectoryPointPrefab, visualsParent);
                trajectoryPoints.Add(newTrajectoryPoint);
            }
            //distribute scale within points
            for (int i = 0; i < trajectoryPoints.Count; i++)
            {
                trajectoryPoints[i].gameObject.SetActive(true);
                float interpolationValue = (float)i / (trajectoryPoints.Count - 1);
                float pointScale = Mathf.Lerp(pointBiggestScale, pointSmallestScale, interpolationValue);
                trajectoryPoints[i].localScale = new Vector3(pointScale, pointScale, pointScale);
            }
            archerAttackSystem.OnStartShooting += Hide;
            archerAttackSystem.OnEndReloading += Show;
        }

        private void Update()
        {
            if (visualsParent.gameObject.activeSelf)
            {
                UpdateTrajectory();
            }
        }

        public void Show()
        {
            UpdateTrajectory();
            visualsParent.gameObject.SetActive(true);
        }

        public void Hide()
        {
            visualsParent.gameObject.SetActive(false);
        }

        private void UpdateTrajectory()
        {
            float totalTime = 0;
            foreach (Transform point in trajectoryPoints)
            {
                totalTime += timeBetweenTrajectoryPoints;
                point.position = archerAttackSystem.GetProjectilePredictedPosition(totalTime);
                //TODO: circleCasts or Raycasts for non-draw circles after colliders
            }
        }
    }
}