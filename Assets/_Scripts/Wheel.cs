using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour {

    private float currentSteerAngle;

    private Transform wheelTransform;
    private WheelCollider wc;

    void Start() {
        wc = GetComponent<WheelCollider>();
        wheelTransform = transform.GetChild(0).GetComponent<Transform>();
    }

    void Update() {
        RotateWheel();
    }

    void RotateWheel() {
        wc.GetWorldPose(out Vector3 position, out Quaternion rotation);

        wheelTransform.position = position;
        wheelTransform.rotation = rotation;
    }

    public void Drive(float acceleration, float torque) {
        wc.motorTorque = acceleration * torque;
    }

    public void Steer(float steerAngle, float steerTime) {
        currentSteerAngle = Mathf.Lerp(currentSteerAngle, steerAngle, steerTime * Time.deltaTime);
        wc.steerAngle = currentSteerAngle;
    }

    public void Drift(float driftInput) {
        WheelFrictionCurve curve = wc.sidewaysFriction;
        curve.stiffness = driftInput;
        wc.sidewaysFriction = curve;
    }

}
