using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {

    public enum DriveType {
        frontwheel,
        rearwheel,
        fourwheel
    }

    [Header("Wheels")]
    [SerializeField]
    private WheelPoint frontLeftWheel;
    [SerializeField]
    private WheelPoint frontRightWheel;
    [SerializeField]
    private WheelPoint backLeftWheel;
    [SerializeField]
    private WheelPoint backRightWheel;

    [Header("Car Specs")]
    [SerializeField]
    private float turnRadius = 10;
    [SerializeField]
    private float topSpeed = 160; //100mph
    [SerializeField]
    private DriveType driveType;

    private float steerInput;
    public float wheelbase;
    public float rearTrack;
    public float ackmAngleLeft;
    public float ackmAngleRight;

    private Vector3 playerInput;

    private Rigidbody rb;

    void Start() {
        rb = GetComponent<Rigidbody>();
        GetCarSpecs();
        SetDriveWheels();
    }

    void GetCarSpecs() {
        wheelbase = Mathf.Abs(frontLeftWheel.transform.position.z - backLeftWheel.transform.position.z);
        rearTrack = Mathf.Abs(backLeftWheel.transform.position.x - backRightWheel.transform.position.x);
    }

    void Update() {
        GetSteerDirection();
        DoSteering();
    }

    void SetDriveWheels() {
        switch (driveType) {
            case DriveType.frontwheel:
                frontLeftWheel.driveWheel = frontRightWheel.driveWheel = true;
                break;
            case DriveType.rearwheel:
                backLeftWheel.driveWheel = backRightWheel.driveWheel = true;
                break;
            case DriveType.fourwheel:
                frontLeftWheel.driveWheel = frontRightWheel.driveWheel = backLeftWheel.driveWheel = backRightWheel.driveWheel = true;
                break;
        }
    }

    void GetSteerDirection() {
        Vector3 carDirection = Vector3.ProjectOnPlane(transform.right, Vector3.up);
        Vector3 otherCarDirection = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
        float angle = Vector3.SignedAngle(otherCarDirection, playerInput, Vector3.up) / 180f;
        steerInput = angle;
    }

    void DoSteering() {
        if (steerInput > 0) {
            ackmAngleLeft = Mathf.Rad2Deg * Mathf.Atan(wheelbase / (turnRadius + (rearTrack / 2))) * steerInput;
            ackmAngleRight = Mathf.Rad2Deg * Mathf.Atan(wheelbase / (turnRadius - (rearTrack / 2))) * steerInput;
        } else if (steerInput < 0) {
            ackmAngleLeft = Mathf.Rad2Deg * Mathf.Atan(wheelbase / (turnRadius - (rearTrack / 2))) * steerInput;
            ackmAngleRight = Mathf.Rad2Deg * Mathf.Atan(wheelbase / (turnRadius + (rearTrack / 2))) * steerInput;
        } else {
            ackmAngleLeft = ackmAngleRight = 0;
        }

        frontLeftWheel.steerAngle = ackmAngleLeft;
        frontRightWheel.steerAngle = ackmAngleRight;
    }

    void FixedUpdate() {
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, topSpeed);
    }

    void OnMove(InputValue value) {

        Vector2 v2Value = value.Get<Vector2>();

        playerInput = new Vector3(v2Value.x, 0, v2Value.y);
    }
}
