using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    //private PostProcessVolume postVolume;
    //private PostProcessProfile postProfile;

    public Transform kartModel;
    public Transform kartNormal;
    public Rigidbody sphere;

    //public List<ParticleSystem> primaryParticles = new List<ParticleSystem>();
    //public List<ParticleSystem> secondaryParticles = new List<ParticleSystem>();

    float speed, currentSpeed;
    float rotate, currentRotate;
    int driftDirection;
    float driftPower;
    int driftMode = 0;
    bool grounded = false;
    //Color c;

    [Header("Bools")]
    public bool drifting;

    [Header("Parameters")]

    public float acceleration = 30f;
    public float steering = 80f;
    public float gravity = 10f;
    public LayerMask layerMask;

    //[Header("Model Parts")]

    //public Transform frontWheels;
    //public Transform backWheels;
    //public Transform steeringWheel;

    //[Header("Particles")]
    //public Transform wheelParticles;
    //public Transform flashParticles;
    //public Color[] turboColors;

    void Start()
    {
        //postVolume = Camera.main.GetComponent<PostProcessVolume>();
        //postProfile = postVolume.profile;

        //for (int i = 0; i < wheelParticles.GetChild(0).childCount; i++)
        //{
        //    primaryParticles.Add(wheelParticles.GetChild(0).GetChild(i).GetComponent<ParticleSystem>());
        //}

        //for (int i = 0; i < wheelParticles.GetChild(1).childCount; i++)
        //{
        //    primaryParticles.Add(wheelParticles.GetChild(1).GetChild(i).GetComponent<ParticleSystem>());
        //}

        //foreach(ParticleSystem p in flashParticles.GetComponentsInChildren<ParticleSystem>())
        //{
        //    secondaryParticles.Add(p);
        //}
    }

    void Update()
    {
        //Follow Collider
        transform.position = sphere.transform.position - new Vector3(0, 0.2f, 0);

        //Accelerate
        if (Input.GetButton("Jump"))
            speed = acceleration;

        //Steer
        if (Input.GetAxis("Horizontal") != 0)
        {
            int dir = Input.GetAxis("Horizontal") > 0 ? 1 : -1;
            float amount = Mathf.Abs((Input.GetAxis("Horizontal")));
            Steer(dir, amount);
        }

        currentSpeed = Mathf.SmoothStep(currentSpeed, speed, Time.deltaTime * 12f); speed = 0f;
        currentRotate = Mathf.Lerp(currentRotate, rotate, Time.deltaTime * 4f); rotate = 0f;

        //Animations    

        //a) Kart
        if (!drifting)
        {
            kartModel.localEulerAngles = Vector3.Lerp(kartModel.localEulerAngles, new Vector3(0, Input.GetAxis("Horizontal") * 10, kartModel.localEulerAngles.z), 0.2f);
        }
    }

    private bool GetAlignment()
    {
        bool onGround = false;

        RaycastHit hit;
        Debug.DrawRay(transform.position + new Vector3(0, -0.03f, 0), -transform.up, Color.red);

        int layerMask = ~LayerMask.GetMask("Player");

        if (Physics.Raycast(transform.position + new Vector3(0, -0.03f, 0), -transform.up, out hit, 1.4f, layerMask))
        {
            if (!hit.collider.isTrigger)
            {
                transform.GetChild(0).up -= (transform.GetChild(0).up - hit.normal) * 0.1f;
                onGround = true;
            }
        }
        else
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, new Quaternion(0, transform.rotation.y, 0, transform.rotation.w), 1);

            if (transform.rotation.x > 0.98f)
            {
                transform.rotation = new Quaternion(0.9f, transform.rotation.y, transform.rotation.z, transform.rotation.w);
            }
        }

        return onGround;
    }

    private void FixedUpdate()
    {
        //Forward Acceleration
        if (!drifting)
            sphere.AddForce(kartModel.transform.forward * currentSpeed, ForceMode.Acceleration);

        //Gravity
        sphere.AddForce(Vector3.down * gravity, ForceMode.Acceleration);

        //Steering
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, transform.eulerAngles.y + currentRotate, 0), Time.deltaTime * 5f);

        GetAlignment();
    }

    public void Steer(int direction, float amount)
    {
        rotate = (steering * direction) * amount;
    }


    private void Speed(float x)
    {
        currentSpeed = x;
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawLine(transform.position + transform.up, transform.position - (transform.up * 2));
    //}
}