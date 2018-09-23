using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace Gamekit3D
{
    public class CameraSettings : MonoBehaviour
    {
        public enum InputChoice
        {
            KeyboardAndMouse, Controller,
        }

        [Serializable]
        public struct InvertSettings
        {
            public bool invertX;
            public bool invertY;
        }

        public Transform follow;
        public Transform lookAt;
        public CinemachineFreeLook keyboardAndMouseCamera;
        public CinemachineFreeLook controllerCamera;
        public InputChoice inputChoice;
        public InvertSettings keyboardAndMouseInvertSettings;
        public InvertSettings controllerInvertSettings;
        public bool allowRuntimeCameraSettingsChanges;


        public CinemachineFreeLook Current
        {
            get { return inputChoice == InputChoice.KeyboardAndMouse ? keyboardAndMouseCamera : controllerCamera; }
        }

        public bool fixCamera = false;
        private string m_inputXAxisName;
        private string m_inputYAxisName;
        bool m_updateFix = false;
        void Reset()
        {
            Transform keyboardAndMouseCameraTransform = transform.Find("KeyboardAndMouseFreeLookRig");
            if (keyboardAndMouseCameraTransform != null)
                keyboardAndMouseCamera = keyboardAndMouseCameraTransform.GetComponent<CinemachineFreeLook>();

            Transform controllerCameraTransform = transform.Find("ControllerFreeLookRig");
            if (controllerCameraTransform != null)
                controllerCamera = controllerCameraTransform.GetComponent<CinemachineFreeLook>();

            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null && playerController.name == "Ellen")
            {
                follow = playerController.transform;

                lookAt = follow.Find("HeadTarget");

                if (playerController.cameraSettings == null)
                    playerController.cameraSettings = this;
            }
        }

        void Awake()
        {
            UpdateCameraSettings();
            m_inputXAxisName = keyboardAndMouseCamera.m_XAxis.m_InputAxisName;
            m_inputYAxisName = keyboardAndMouseCamera.m_YAxis.m_InputAxisName;
        }

        void Update()
        {
            if (allowRuntimeCameraSettingsChanges)
            {
                UpdateCameraSettings();
            }
        }

        public void FixCamera()
        {
            if (fixCamera)
                return;

            keyboardAndMouseCamera.m_XAxis.m_InputAxisName = "";
            keyboardAndMouseCamera.m_YAxis.m_InputAxisName = "";
            controllerCamera.m_XAxis.m_InputAxisName = "";
            controllerCamera.m_YAxis.m_InputAxisName = "";
            fixCamera = true;
        }

        public void UnFixCamera()
        {
            if (!fixCamera)
                return;

            keyboardAndMouseCamera.m_XAxis.m_InputAxisName = m_inputXAxisName;
            keyboardAndMouseCamera.m_YAxis.m_InputAxisName = m_inputYAxisName;
            controllerCamera.m_XAxis.m_InputAxisName = m_inputXAxisName;
            controllerCamera.m_YAxis.m_InputAxisName = m_inputYAxisName;
            fixCamera = false;
        }

        void UpdateCameraSettings()
        {
            if (fixCamera)
            {
                return;
            }
            keyboardAndMouseCamera.Follow = follow;
            keyboardAndMouseCamera.LookAt = lookAt;
            keyboardAndMouseCamera.m_XAxis.m_InvertInput = keyboardAndMouseInvertSettings.invertX;
            keyboardAndMouseCamera.m_YAxis.m_InvertInput = keyboardAndMouseInvertSettings.invertY;

            controllerCamera.m_XAxis.m_InvertInput = controllerInvertSettings.invertX;
            controllerCamera.m_YAxis.m_InvertInput = controllerInvertSettings.invertY;
            controllerCamera.Follow = follow;
            controllerCamera.LookAt = lookAt;

            keyboardAndMouseCamera.Priority = inputChoice == InputChoice.KeyboardAndMouse ? 1 : 0;
            controllerCamera.Priority = inputChoice == InputChoice.Controller ? 1 : 0;
        }
    }
}
