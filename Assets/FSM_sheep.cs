using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FSM_sheep : MonoBehaviour
{
    
    private Slider slider;
    public enum FSMState                                  //the base states
    {
        Alive,  
        Dead,
    }

    public enum FSMState_I                                //first child states
    {
        Move,
        //FleeWolf,   
    }

    public enum FSMState_II                              //second child states
    {
        //RandomWalking,
        Flocking,
        //FleeFarmer_RandomWalking,
        FleeFarmer_Flocking,
        FleeFarmer,
    }


    public struct Transitions                           //6 transitions
    {
        public bool alone;
        public bool farmer_close;
        public bool farmer_closer;
        public bool dead;
        public bool wolf_close;
        public bool wolf_far;
    }

   

    public FSMState curState;
    public FSMState_I curState_I;
    public FSMState_II curState_II;

    public float sheepSpeed;
    private flocking flockingScript;
    private Transitions transitions;

    private float RANDOM_SPEED = 1.0f;
    private float FLOCKING_SPEED = 1.0f;
    private float FF_RANDOM_SPEED = 1.1f;
    private float FF_FLOCKING_SPEED = 1.1f;
    private float FF_SPEED = 1.2f;
    private float FW_SPEED = 1.4f;
    private float LowHP_SPEED = 0.5f;
    private float deadTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        slider = transform.GetComponentInChildren<Slider>();
        flockingScript = transform.GetComponent<flocking>();

        curState = FSMState.Alive;
        curState_I = FSMState_I.Move;
        //curState_II = FSMState_II.RandomWalking;
        curState_II = FSMState_II.Flocking;

        sheepSpeed = 1.0f;
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(curState.ToString());
        //Debug.Log(curState_I.ToString());
        //Debug.Log(curState_II.ToString());

    }

    public void updateFSM()                                            //update base states
    {
        
        switch (curState)
        {
            case FSMState.Alive:
                if (transitions.dead)                                 
                {
                    curState = FSMState.Dead;
                    break;
                }
                updateAlive();
                break;
            case FSMState.Dead:
                flockingScript.target = new Vector3(0, 0, 0);
                transform.GetComponent<Animator>().SetBool("dead", true);
               
                flockingScript.target = new Vector3(0, 0, 0);
                

                deadTimer += Time.deltaTime;
                if (deadTimer > 3f)
                {
                    flocking.sheeps.Remove(gameObject);
                    Destroy(gameObject);
                }

                break;
 
        }

        
    }

    public void updateAlive()                                           //update first child states
    {
        switch (curState_I)
        {
            case FSMState_I.Move:
                if(transitions.wolf_close)                       
                {
                    //curState_I = FSMState_I.FleeWolf;
                    break;
                }
                //transform.GetComponent<Animator>().SetBool("moving", true);
                updateMove();
                break;
            //case FSMState_I.FleeWolf:
            //    if (transitions.wolf_far)                      
            //    {
            //        curState_I = FSMState_I.Move;
            //        break;
            //    }
            //    //transform.GetComponent<Animator>().SetBool("moving", true);
            //    flockingScript.target += flockingScript.fleeWolfTarget;
                
            //    setSheepSpeed(FW_SPEED);



            //    break;
            

        }
    }

    public void updateMove()                                             //update second child states
    {
        float disperseFactor;
        switch (curState_II)
        {

            //case FSMState_II.RandomWalking:
            //    if (!transitions.alone)
            //    {
            //        curState_II = FSMState_II.Flocking;
            //        break;
            //    }
            //    else if (transitions.farmer_close)
            //    {
            //        curState_II = FSMState_II.FleeFarmer_RandomWalking;
            //        break;
            //    }
            //    setSheepSpeed(RANDOM_SPEED);

            //    flockingScript.target += flockingScript.getRandomMovement();              
            //    break;
            case FSMState_II.Flocking:
                if (transitions.alone)
                {
                    //curState_II = FSMState_II.RandomWalking;
                    break;
                }
                else if (transitions.farmer_close)
                {
                    curState_II = FSMState_II.FleeFarmer_Flocking;
                    break;
                }
                setSheepSpeed(FLOCKING_SPEED);

                flockingScript.target += flockingScript.flockingTarget;
                break;
            //case FSMState_II.FleeFarmer_RandomWalking:
            //    if (!transitions.alone)
            //    {
            //        curState_II = FSMState_II.FleeFarmer_Flocking;
            //        break;
            //    }
            //    else if (!transitions.farmer_close)
            //    {
            //        curState_II = FSMState_II.RandomWalking;
            //        break;
            //    }
            //    else if (transitions.farmer_closer)
            //    {
            //        curState_II = FSMState_II.FleeFarmer;
            //        break;
            //    }
            //    setSheepSpeed(FF_RANDOM_SPEED);

            //    disperseFactor = (flockingScript.distanceToShepherdV - flocking.MIN_DISTANCE_TO_SHEPHERD) / flocking.DISPERSE_DISTANCE_TO_SHEPHERD;
            //    flockingScript.target += flockingScript.fleeFarmerTarget * (1 - disperseFactor);
            //    break;
            case FSMState_II.FleeFarmer_Flocking:
                if (transitions.alone)
                {
                    //curState_II = FSMState_II.FleeFarmer_RandomWalking;
                    break;
                }
                else if (!transitions.farmer_close)
                {
                    curState_II = FSMState_II.Flocking;
                    break;
                }
                else if (transitions.farmer_closer)
                {
                    curState_II = FSMState_II.FleeFarmer;
                    break;
                }
                setSheepSpeed(FF_FLOCKING_SPEED);
                disperseFactor = (flockingScript.distanceToShepherdV - flocking.MIN_DISTANCE_TO_SHEPHERD) / flocking.DISPERSE_DISTANCE_TO_SHEPHERD;
                flockingScript.target += flockingScript.flockingTarget * disperseFactor;
                flockingScript.target += flockingScript.fleeFarmerTarget * (1 - disperseFactor);
                break;
            case FSMState_II.FleeFarmer:
                if (!transitions.farmer_closer)
                {
                    if (transitions.alone)
                    {
                        //curState_II = FSMState_II.FleeFarmer_RandomWalking;
                        break;
                    }
                    else
                    {
                        curState_II = FSMState_II.FleeFarmer_Flocking;
                        break;
                    }
                }
                setSheepSpeed(FF_SPEED);
                flockingScript.target += flockingScript.fleeFarmerTarget;
                break;

        }
    }

   

    public void updateTransitions()                                        //update transitions 
    {
        if (flockingScript.getWolfDistance() <= flocking.MIN_DISTANCE_TO_WOLF)
        {
            transitions.wolf_close = true;
            transitions.wolf_far = false;
        }           

        if (flockingScript.getWolfDistance() >= flocking.MAX_DISTANCE_TO_WOLF)
        {
            transitions.wolf_close = false;
            transitions.wolf_far = true;
        }

        if (flockingScript.numberOfSheepWeCanSee > 0)
            transitions.alone = false;
        else
            transitions.alone = true;

        if (slider.value <= 0)
            transitions.dead = true;
        else
            transitions.dead = false;

        if (flockingScript.distanceToShepherdV <= flocking.DISPERSE_DISTANCE_TO_SHEPHERD + flocking.MIN_DISTANCE_TO_SHEPHERD)
            transitions.farmer_close = true;
        else
            transitions.farmer_close = false;

        if (flockingScript.distanceToShepherdV <= flocking.MIN_DISTANCE_TO_SHEPHERD)
            transitions.farmer_closer = true;
        else
            transitions.farmer_closer = false;  
    }

    void setSheepSpeed(float speed)
    {
        if (slider.value > 50)
        {
            sheepSpeed = speed;
            transform.GetComponent<Animator>().speed = speed;
        }
        else
        {
            sheepSpeed = LowHP_SPEED;
            transform.GetComponent<Animator>().speed = LowHP_SPEED;
        }
    }
}
