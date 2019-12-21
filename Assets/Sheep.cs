using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep : MonoBehaviour
{

    //public GameObject sheep;
    public GameObject farmer;
    public float max_velocity;
    public Vector3 velocity;
    public bool farmer_close;
    public static float MIN_FARMER_DISTANCE = 3f;
    // Start is called before the first frame update
    void Start()
    {
        max_velocity = 8;
    }

    // Update is called once per frame
    void Update()
    {

        if (getWolfDistance() <= MIN_FARMER_DISTANCE)
            fleeFarmer();
        //transform.position = Vector3.MoveTowards(farmer.GetComponent<Transform>().position,transform.position,1f);
    }

    public float getWolfDistance()
    {
        float distance = (farmer.transform.position - transform.position).magnitude;

        return distance;
    }

    public void fleeFarmer()
    {
        Vector3 desiredVelocity = (transform.position - farmer.transform.position).normalized * max_velocity;
        Vector3 steeringForce = desiredVelocity - velocity;
        Vector3 acc = steeringForce / 1;
        velocity += acc * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        if (velocity.magnitude > 0.01f)
        {
            Vector3 newForward = Vector3.Slerp(transform.forward, velocity, Time.deltaTime);
            newForward.y = 0;
            transform.forward = newForward;
        }
    }
}
