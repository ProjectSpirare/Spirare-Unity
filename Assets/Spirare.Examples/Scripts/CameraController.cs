using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spirare.Examples
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private float cameraTranslateSpeed = 3f;

        [SerializeField]
        private float cameraRotationSpeed = 0.1f;

        private Camera mainCamera;

        private Vector3 startMousePosition;
        private Vector3 startCameraAngles;

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        void Update()
        {
            RotateCamera();
            MoveCamera();
        }

        private void RotateCamera()
        {
            // right click
            var button = 1;
            if (Input.GetMouseButtonDown(button))
            {
                startMousePosition = Input.mousePosition;
                startCameraAngles = mainCamera.transform.eulerAngles;
            }

            if (Input.GetMouseButton(button))
            {
                var delta = Input.mousePosition - startMousePosition;
                var x = startCameraAngles.x - cameraRotationSpeed * delta.y;
                var y = startCameraAngles.y + cameraRotationSpeed * delta.x;
                mainCamera.transform.eulerAngles = new Vector3(x, y, startCameraAngles.z);
            }
        }

        private void MoveCamera()
        {
            var z = 0;
            if (Input.GetKey(KeyCode.W))
            {
                z += 1;
            }
            if (Input.GetKey(KeyCode.S))
            {
                z -= 1;
            }

            var x = 0;
            if (Input.GetKey(KeyCode.A))
            {
                x -= 1;
            }
            if (Input.GetKey(KeyCode.D))
            {
                x += 1;
            }

            var direction = new Vector3(x, 0, z);
            var translation = cameraTranslateSpeed * Time.deltaTime * direction;
            mainCamera.transform.Translate(translation);
        }
    }
}
