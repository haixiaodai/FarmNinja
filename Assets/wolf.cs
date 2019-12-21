using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class wolf : MonoBehaviour
{
    
    public GameObject pathfind;
    public GameObject farmer;
    public float max_velocity;
    public Vector3 velocity;
    public static GameObject targetSheep;
    public static GameObject targetCube;

    public static float MIN_FARMER_DISTANCE = 5f;
    public static float MIN_SHEEP_DISTANCE = 10f;
    public static float MIN_ATTACK_DISTANCE = 2f;

    public Slider slider;
    private FSM_wolf wolfFSM;
    private float attckTimer =0.5f;
    private PathFindingV3 pathfinding;
    //private Patrolling patrolling;

    private float chasingSpeed = 2f;
    private float patrollingSpeed = 1f;
    private wolfRandomWalking wolfRD;
    private Patrolling wolfPatroll;
    public GameObject[] targets;
    private bool refTarget;
    System.Random random = new System.Random();
    int targetNumber;
  
    // Start is called before the first frame update
    void Start()
    {
        targetNumber = random.Next(0, 8);
        wolfFSM = transform.GetComponent<FSM_wolf>();
        //wolfRD  = transform.GetComponent<wolfRandomWalking>();
        max_velocity = 8;
        wolfPatroll = transform.GetComponent<Patrolling>();
        pathfinding = pathfind.transform.GetComponent<PathFindingV3>();
         
    }

    // Update is called once per frame
    void Update()
    {
         
        findAsheep();
        if(targetSheep!=null)
            slider = targetSheep.GetComponentInChildren<Slider>();
        wolfFSM.updateTransitions();
        wolfFSM.updateFSM();
           
    }

    public void fleeFarmer()
    {
        pathfind.GetComponent<PathFindingV3>().enabled = false;
        
        Vector3 desiredVelocity = ( transform.position - farmer.transform.position ).normalized * max_velocity;
        Vector3 steeringForce = desiredVelocity - velocity;
        Vector3 acc = steeringForce / 1;
        velocity += acc * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        if(velocity.magnitude > 0.01f)
        {
            Vector3 newForward = Vector3.Slerp(transform.forward, velocity, Time.deltaTime);
            newForward.y = 0;
            transform.forward = newForward;
        }
    }

    public void patrolling()
    {
        
        targetCube = targets[targetNumber];
        if (getCubeDistance() <= 3)
        {
            targetNumber = random.Next(0, 8);
            //refTarget = true;
        }
        pathfinding.wolfSpeed = wolfFSM.wolfSpeed;
        pathfind.GetComponent<PathFindingV3>().enabled = true;
        
    }

    public void chasing()
    {

        //transform.LookAt(targetSheep.transform);
        pathfinding.wolfSpeed = wolfFSM.wolfSpeed;
        pathfind.GetComponent<PathFindingV3>().enabled = true;
        
    }

    public void attacking()
    {
        transform.LookAt(targetSheep.transform);
       
        pathfind.GetComponent<PathFindingV3>().enabled = false;
        
        attckTimer -= Time.deltaTime;
        if(attckTimer <= 0)
        {
            //slider.value -= 20;
            targetSheep.GetComponent<flocking>().hp -=20;
            attckTimer = 0.5f;
        }
        if (slider.value <= 0)
        {
            flocking.sheeps.Remove(targetSheep);
            //targetSheep = null;
        }

    }

    public void findAsheep()
    {
        float minDist = 1000f;
        if (flocking.sheeps == null)
            return;
        foreach (GameObject sheep in flocking.sheeps)
        {
            float distance = (sheep.transform.position - transform.position).magnitude;

            if (distance < minDist)
            {
                minDist = distance;
                targetSheep = sheep;
            }
        }
    }

    public float getFarmerDistance()
    {
        float distance = (farmer.transform.position - transform.position).magnitude;

        return distance;
    }

    public float getSheepDistance()
    {
        if (targetSheep == null)
            return 100f;
        float distance = (targetSheep.transform.position - transform.position).magnitude;

        return distance;
    }

    public float getCubeDistance()
    {
        if (targetCube == null)
            return 100f;
        float distance = (targetCube.transform.position - transform.position).magnitude;

        return distance;
    }

    
}
