using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour {

    NeuralNetwork network;
    bool initialized = false;
    Rigidbody rb;

    [SerializeField]
    Transform sensorRight;
    [SerializeField]
    Transform sensorLeft;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        if (initialized)
        {
            var input = new float[4];

            var angle = SignedAngleBetween(this.transform.forward, (Manager.Instance.PosOfClosestRes(this.transform.position) - this.transform.position).normalized, Vector3.up);

            input[0] = angle;
            input[1] = 1;
            input[2] = 1;
            input[3] = 1;

            RaycastHit hit;
            Ray ray = new Ray(this.transform.position + (Vector3.up * 0.25f), sensorRight.position - (this.transform.position + (Vector3.up * 0.25f)));
            Debug.DrawRay(this.transform.position + (Vector3.up * 0.25f), (sensorRight.position - (this.transform.position + (Vector3.up * 0.25f))).normalized, Color.blue);
            if (Physics.Raycast(ray, out hit, 1f))
            {
                if(hit.transform.gameObject != this.gameObject && hit.transform.GetComponent<Res>() != null)
                {
                    input[1] = -1;
                } 
            }
            
            ray = new Ray(this.transform.position + (Vector3.up * 0.25f), sensorLeft.position - (this.transform.position + (Vector3.up * 0.25f)));
            Debug.DrawRay(this.transform.position + (Vector3.up * 0.25f), (sensorLeft.position - (this.transform.position + (Vector3.up * 0.25f))).normalized, Color.red);
            if (Physics.Raycast(ray, out hit, 1f))
            {
                if (hit.transform.gameObject != this.gameObject && hit.transform.GetComponent<Res>() != null)
                {
                    input[2] = -1;
                }
            }
            
            ray = new Ray(this.transform.position + (Vector3.up * 0.25f), this.transform.forward);
           // Debug.DrawRay(this.transform.position + (Vector3.up * 0.25f), this.transform.forward, Color.cyan);
            if (Physics.Raycast(ray, out hit, 1f))
            {
                if (hit.transform.gameObject != this.gameObject && hit.transform.GetComponent<Res>() != null)
                {
                    input[3] = -1;
                }
            }

            var output = network.FeedForward(input);

            var vel = output[0];

            if (vel < -0.2f)
                vel = -0.2f;

            rb.velocity = this.transform.forward * vel * Time.deltaTime * 500f;
            rb.AddTorque(this.transform.up * Time.deltaTime * output[1] * 500f, ForceMode.VelocityChange);

            ray = new Ray(this.transform.position + (Vector3.up * 0.25f), this.transform.forward);
            Debug.DrawRay(this.transform.position + (Vector3.up * 0.25f), this.transform.forward * 0.75f, Color.cyan);
            if (Physics.Raycast(ray, out hit, 0.75f))
            {
                if (hit.transform.gameObject != this.gameObject && hit.transform.GetComponent<Res>() != null)
                {
                    network.AddFitness(-0.1f);
                }
            }

            //var fitness = 1.0f - (Mathf.Min(10.0f, Manager.Instance.DistanceToClosestRes(this.transform.position)) / 10f);
            //network.AddFitness(fitness);
        }
        /*
        if (this.transform.position.x > 50.0f)
            this.transform.position = new Vector3(0f, this.transform.position.y, 0f);
        if (this.transform.position.x < -50.0f)
            this.transform.position = new Vector3(0f, this.transform.position.y, 0f);

        if (this.transform.position.z > 50.0f)
            this.transform.position = new Vector3(0f, this.transform.position.y, 0f);

        if (this.transform.position.z < -50.0f)
            this.transform.position = new Vector3(0f, this.transform.position.y, 0f);
            */
    }

    public void Init(NeuralNetwork n)
    {
        network = n;
        initialized = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Res>() != null)
        {
            other.GetComponent<Res>().Delete();
            network.AddFitness(1f);
        }
    }

    public void SetNeuralNetwork(NeuralNetwork n)
    {
        network = n;
    }

    float SignedAngleBetween(Vector3 a, Vector3 b, Vector3 n)
    {
        // angle in [0,180]
        float angle = Vector3.Angle(a, b);
        float sign = Mathf.Sign(Vector3.Dot(n, Vector3.Cross(a, b)));

        // angle in [-179,180]
        float signed_angle = angle * sign;

        // angle in [0,360] (not used but included here for completeness)
        //float angle360 =  (signed_angle + 180) % 360;

        return signed_angle;
    }
}
