using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Andremani.ArcherySpineTest
{
    public class InputSystem : MonoBehaviour
    {
        [SerializeField] private Camera mouseCamera;
        public Vector3 WorldMousePosition { get; private set; }

        public event System.Action OnMainPointerRelease;

        private void Update()
        {
            WorldMousePosition = mouseCamera.ScreenToWorldPoint(Input.mousePosition);

            if (Input.GetMouseButtonUp(0))
            {
                OnMainPointerRelease?.Invoke();
            }
        }
    }
}