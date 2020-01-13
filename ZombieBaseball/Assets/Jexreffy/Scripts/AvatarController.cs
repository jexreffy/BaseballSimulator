using System;
using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;
using VRTK;

[RequireComponent(typeof(VRIK))]
public class AvatarController : MonoBehaviour {

    public GameObject leftHandMount;
    public Vector3    leftMountPosition;
    public Vector3    leftMountRotation;
    public Vector3    leftMountScale;
    
    public GameObject rightHandMount;
    public Vector3    rightMountPosition;
    public Vector3    rightMountRotation;
    public Vector3    rightMountScale;

    public VRIKCalibrator.Settings settings;
    private VRIK _ik;
    

    void Awake() {
        _ik = GetComponent<VRIK>();
        _ik.AutoDetectReferences();
        
        _ik.solver.leftArm.shoulderRotationMode  = IKSolverVR.Arm.ShoulderRotationMode.FromTo;
        _ik.solver.rightArm.shoulderRotationMode = IKSolverVR.Arm.ShoulderRotationMode.FromTo;
    }

    void OnEnable() {
        VRTK_SDKManager.instance.LoadedSetupChanged += OnLoadedSetupChanged;
    }

    void OnDisable() {
        VRTK_SDKManager.instance.LoadedSetupChanged += OnLoadedSetupChanged;
    }
    
    private void OnLoadedSetupChanged(VRTK_SDKManager sender, VRTK_SDKManager.LoadedSetupChangeEventArgs e) {
        VRIKCalibrator.Calibrate(_ik, settings,
                                 VRTK_DeviceFinder.HeadsetTransform(), null,
                                 VRTK_DeviceFinder.DeviceTransform(VRTK_DeviceFinder.Devices.LeftController),
                                 VRTK_DeviceFinder.DeviceTransform(VRTK_DeviceFinder.Devices.RightController));
        
        if (leftHandMount != null) {
            leftHandMount.transform.SetParent(_ik.references.leftHand, false);
            leftHandMount.transform.localPosition = leftMountPosition;
            leftHandMount.transform.localRotation = Quaternion.Euler(leftMountRotation);
            leftHandMount.transform.localScale    = leftMountScale;
        }
        
        if (rightHandMount != null) {
            rightHandMount.transform.SetParent(_ik.references.rightHand, false);
            rightHandMount.transform.localPosition = rightMountPosition;
            rightHandMount.transform.localRotation = Quaternion.Euler(rightMountRotation);
            rightHandMount.transform.localScale    = rightMountScale;
        }
    }
}
