using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour {

    public enum DriveType {
        frontwheel,
        rearwheel,
        fourwheel
    }

    [Header("Wheels")]
    [SerializeField]
    private Wheel frontLeftWheel;
    [SerializeField]
    private Wheel frontRightWheel;
    [SerializeField]
    private Wheel backLeftWheel;
    [SerializeField]
    private Wheel backRightWheel;

    [Header("Car Specs")]
    [SerializeField]
    private float topSpeed = 30;
    [SerializeField]
    private DriveType driveType;
    [SerializeField]
    private float acceleration = 4f;
    [SerializeField]
    private float torque = 200;

    [Header("Steering")]
    [SerializeField]
    private float turnRadius = 10;
    [SerializeField]
    private float steerTime = 10f;
    [SerializeField]
    [Range(0, 1)]
    private float maxDriftValue = 0.8f;

    private float steerInput;
    private float wheelbase;
    private float rearTrack;
    private float ackmAngleLeft;
    private float ackmAngleRight;
    private float brakeValue;

    private Rigidbody rb;

    void Start() {
        rb = GetComponent<Rigidbody>();
        GetCarSpecs();
    }

    void GetCarSpecs() {
        wheelbase = Mathf.Abs(frontLeftWheel.transform.position.z - backLeftWheel.transform.position.z);
        rearTrack = Mathf.Abs(backLeftWheel.transform.position.x - backRightWheel.transform.position.x);
    }

    void Update() {
        Steer();
    }

    void Steer() {
        if (steerInput > 0) {
            ackmAngleLeft = Mathf.Rad2Deg * Mathf.Atan(wheelbase / (turnRadius + (rearTrack / 2))) * steerInput;
            ackmAngleRight = Mathf.Rad2Deg * Mathf.Atan(wheelbase / (turnRadius - (rearTrack / 2))) * steerInput;
        } else if (steerInput < 0) {
            ackmAngleLeft = Mathf.Rad2Deg * Mathf.Atan(wheelbase / (turnRadius - (rearTrack / 2))) * steerInput;
            ackmAngleRight = Mathf.Rad2Deg * Mathf.Atan(wheelbase / (turnRadius + (rearTrack / 2))) * steerInput;
        } else {
            ackmAngleLeft = ackmAngleRight = 0;
        }

        frontLeftWheel.Steer(ackmAngleLeft, steerTime);
        frontRightWheel.Steer(ackmAngleRight, steerTime);
    }

    void FixedUpdate() {
        Drive();

        rb.velocity = Vector3.ClampMagnitude(rb.velocity, topSpeed);
    }

    void Drive() {
        float currentAcceleration = acceleration;// * Mathf.Clamp01(topSpeed - rb.velocity.magnitude);
        float driftInput = Mathf.Clamp(Mathf.Abs(1 - Mathf.Abs(steerInput)), maxDriftValue, 1);

        switch (driveType) {
            case DriveType.frontwheel:
                frontLeftWheel.Drive(currentAcceleration, torque);
                frontRightWheel.Drive(currentAcceleration, torque);
                break;
            case DriveType.rearwheel:
                backLeftWheel.Drive(currentAcceleration, torque);
                backRightWheel.Drive(currentAcceleration, torque);
                break;
            case DriveType.fourwheel:
                frontLeftWheel.Drive(currentAcceleration, torque);
                frontRightWheel.Drive(currentAcceleration, torque);
                backLeftWheel.Drive(currentAcceleration, torque);
                backRightWheel.Drive(currentAcceleration, torque);
                break;
        }

        backLeftWheel.Drift(driftInput);
        backRightWheel.Drift(driftInput);
    }

    void OnMove(InputValue value) {
        steerInput = value.Get<float>();
    }
}
