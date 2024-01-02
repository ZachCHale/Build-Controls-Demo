using System;
using System.Collections;
using System.Collections.Generic;
using PlasticGui.WorkspaceWindow.Diff;
using UnityEngine;

namespace Demo
{
    public class CameraControls : MonoBehaviour
    {
        private Transform _focusPointTransform;
        private Transform _armYawTransform;
        private Transform _armPitchTransform;
        private Transform _armZoomTransform;
        private Transform _cameraTransform;
        
        [Header("Constant Settings")]
        [SerializeField] private Vector3 _focusPointSpawnPosition;
        [SerializeField] [Range(0,1)]private float _rotationSensitivity = 0.5f;
        [SerializeField] [Range(0,1)]private float _scrollSensitivity = 0.5f;

        private float RotationScale => (50f * _rotationSensitivity) + 10f;
        private float ScrollScale => (0.09f * _scrollSensitivity) + 0.01f;


        
        [Header("When Zoomed In:")]
        [SerializeField] private float _minZoomDistance = 5f;
        [SerializeField] private float _minZoomArmAngle = 20f;
        [SerializeField] private float _minZoomCameraAngle = 40f;
        [SerializeField] private float _minZoomPanSpeed = 0.03f;
        [Header("When Zoomed Out:")]
        [SerializeField] private float _maxZoomDistance = 20f;
        [SerializeField] private float _maxZoomArmAngle = 60f;
        [SerializeField] private float _maxZoomCameraAngle = 40f;
        [SerializeField] private float _maxZoomPanSpeed = 0.1f;


        
        private float FocusPointHeight => _focusPointSpawnPosition.y;


        private float _currentZoomT = .25f;

        private float _edgePanMargin = 0.02f;
        private float EdgePanPixels => Screen.height * _edgePanMargin;

        
        private Vector3 _prevMousePosition;
        private Vector3 MouseDelta => _prevMousePosition - Input.mousePosition;

        private void Awake()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
                mainCamera = new Camera();
            _focusPointTransform = new GameObject("CameraFocusPoint").transform;
            _armYawTransform = new GameObject("CameraArmYaw").transform;
            _armPitchTransform = new GameObject("CameraArmPitch").transform;
            _armZoomTransform = new GameObject("CameraArmZoom").transform;
            _cameraTransform = mainCamera.transform;

            _focusPointTransform.position = Vector3.zero;

            _armYawTransform.parent = _focusPointTransform;
            _armYawTransform.localPosition = Vector3.zero;
            _armYawTransform.localRotation = Quaternion.identity;
            
            _armPitchTransform.parent = _armYawTransform;
            _armPitchTransform.localPosition = Vector3.zero;
            _armPitchTransform.localRotation = Quaternion.AngleAxis(_minZoomArmAngle, Vector3.right);
            
            
            _armZoomTransform.parent = _armPitchTransform;
            _armZoomTransform.localPosition = Vector3.zero + Vector3.back * _minZoomDistance;
            _armZoomTransform.localRotation = Quaternion.identity;

            _cameraTransform.parent = _armZoomTransform;
            _cameraTransform.localPosition = Vector3.zero;
            _cameraTransform.localRotation = Quaternion.AngleAxis(_minZoomCameraAngle, Vector3.right);

            _prevMousePosition = Vector3.zero;
        }

        private void Update()
        {
            AdjustArm();
            AdjustPosition();
            AdjustRotation();
            _prevMousePosition = Input.mousePosition;
        }

        private void AdjustArm()
        {
            float scrollValue = Input.mouseScrollDelta.y * ScrollScale;
            _currentZoomT = Mathf.Clamp01(_currentZoomT - scrollValue);
            _armPitchTransform.localRotation = Quaternion.AngleAxis(Mathf.Lerp(_minZoomArmAngle, _maxZoomArmAngle, _currentZoomT), Vector3.right);
            _armZoomTransform.localPosition = Vector3.zero + Vector3.back * Mathf.Lerp(_minZoomDistance, _maxZoomDistance, _currentZoomT);
            _cameraTransform.localRotation = Quaternion.AngleAxis(Mathf.Lerp(_minZoomCameraAngle,_maxZoomCameraAngle,_currentZoomT), Vector3.right);
        }

        private void AdjustPosition()
        {
            float keyX = Input.GetKey(KeyCode.LeftArrow) ? -1 : Input.GetKey(KeyCode.RightArrow) ? 1 : 0;
            float keyY = Input.GetKey(KeyCode.DownArrow) ? -1 : Input.GetKey(KeyCode.UpArrow) ? 1 : 0;

            if (keyX != 0 || keyY != 0)
            {
                Vector2 dir = new Vector2(keyX, keyY).normalized;
                _focusPointTransform.position = _focusPointTransform.position + _armYawTransform.TransformDirection(new (dir.x , 0, dir.y)) * (Time.deltaTime * Mathf.Lerp(_minZoomPanSpeed, _maxZoomPanSpeed , _currentZoomT));
            }
            else
            {
                float mouseX = Input.mousePosition.x;
                float mouseY = Input.mousePosition.y;
                Vector2 mousePosition = new(mouseX, mouseY);

                bool isMouseAtEdge = mouseX < EdgePanPixels || mouseX > Screen.width - EdgePanPixels ||
                                     mouseY < EdgePanPixels || mouseY > Screen.height - EdgePanPixels;
                if (!isMouseAtEdge)
                    return;


                Vector2 screenCenter = new(Screen.width / 2, Screen.height / 2);

                Vector2 dif = screenCenter - mousePosition;
                Vector2 dir = dif.normalized;
                _focusPointTransform.position = _focusPointTransform.position - _armYawTransform.TransformDirection(new (dir.x , 0, dir.y)) * (Time.deltaTime * Mathf.Lerp(_minZoomPanSpeed, _maxZoomPanSpeed , _currentZoomT));

            }
        }

        private void AdjustRotation()
        {
            if (!Input.GetKey(KeyCode.Mouse1)) return;
            float deltaX = MouseDelta.x * RotationScale * Time.deltaTime;
            
            _armYawTransform.eulerAngles = new(0, _armYawTransform.localEulerAngles.y - deltaX, 0);
        }
    }
}
