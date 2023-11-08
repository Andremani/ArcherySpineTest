using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Andremani.ArcherySpineTest
{
    public class InputSystem : MonoBehaviour
    {
        [SerializeField] private Camera mouseCamera;

        private Vector3 clickStartPosition;

        public Vector3 WorldMousePosition { get; private set; }

        public event System.Action OnMainPointerDown;
        public event System.Action OnMainPointerUp;

        private void Update()
        {
            WorldMousePosition = mouseCamera.ScreenToWorldPoint(Input.mousePosition);

            if(Input.GetMouseButtonDown(0))
            {
                clickStartPosition = WorldMousePosition;
                OnMainPointerDown?.Invoke();
            }
            if (Input.GetMouseButtonUp(0))
            {
                OnMainPointerUp?.Invoke();
            }
        }

        public Vector3 GetDistanceBetweenStartingAndCurrentPointerPosition()
        {
            return WorldMousePosition - clickStartPosition;
        }
    }
}