using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header ("Wheel Colliders")]
    [SerializeField] WheelCollider flWheelCollider;
    [SerializeField] WheelCollider frWheelCollider;
    [SerializeField] WheelCollider blWheelCollider;
    [SerializeField] WheelCollider brWheelCollider;

    [Header("Wheels")]
    [SerializeField] Transform flWheel;
    [SerializeField] Transform frWheel;
    [SerializeField] Transform blWheel;
    [SerializeField] Transform brWheel;

    float vMove;
    float hMove;

    [SerializeField] float motorSpeed = 100f;
    [SerializeField] float turnAngle = 30f;
    [SerializeField] float breakForce = 2000f;
    [SerializeField] Transform carCentreOfMass;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = carCentreOfMass.transform.position;
    }

    private void FixedUpdate()
    {
        CarMove();
        CarTurn();
        WheelRotatate();
        ApplyBreaks();
    }

    void CarMove()
    {
        vMove = Input.GetAxisRaw("Vertical") * motorSpeed * Time.fixedDeltaTime;
        flWheelCollider.motorTorque = vMove;
        frWheelCollider.motorTorque = vMove;
    }
    void CarTurn()
    {
        hMove = Input.GetAxisRaw("Horizontal") * turnAngle;
        flWheelCollider.steerAngle = hMove;
        frWheelCollider.steerAngle = hMove;
    }

    void WheelRotatate()
    {
        RotateWheels(flWheel, flWheelCollider);
        RotateWheels(frWheel, frWheelCollider);
        RotateWheels(blWheel, blWheelCollider);
        RotateWheels(brWheel, brWheelCollider);
    }

    void RotateWheels(Transform wheelTransform, WheelCollider wheelCollider)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.position = pos;
        wheelTransform.rotation = rot;
    }

    void ApplyBreaks()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            flWheelCollider.brakeTorque = breakForce;
            frWheelCollider.brakeTorque = breakForce;
            blWheelCollider.brakeTorque = breakForce;
            brWheelCollider.brakeTorque = breakForce;
        }
        else
        {
            flWheelCollider.brakeTorque = 0f;
            frWheelCollider.brakeTorque = 0f;
            blWheelCollider.brakeTorque = 0f;
            brWheelCollider.brakeTorque = 0f;
        }
    }

}
