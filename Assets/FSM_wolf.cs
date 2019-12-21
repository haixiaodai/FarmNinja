using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FSM_wolf : MonoBehaviour
{
    public enum FSMState                                  //the base states
    {
        LookingForSheep,
        RunningAway,
    }

    public enum FSMState_I                                //first child states
    {
        Patorlling,
        Chasing,
        Attacking,
    }

    public struct Transitions                           //4 transitions
    {
        public bool see_a_sheep;
        public bool in_attack_range;
        public bool kill_a_sheep;
        public bool farmer_close;
    }

 
    public FSMState curState;
    public FSMState_I curState_I;
    public float wolfSpeed;
    private wolf wolfScript;
    private Transitions transitions;
    private static float KILL_SPEED = 2;
    private static float PATROLLING_SPEED = 1;
    private static float CHASING_SPEED = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        wolfScript = transform.GetComponent<wolf>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(curState.ToString());
        //Debug.Log(curState_I.ToString());

    }

    public void updateFSM()                                            //update base states
    {

        switch (curState)
        {
            case FSMState.LookingForSheep:
                if (transitions.farmer_close)
                {
                    curState = FSMState.RunningAway;
                    break;
                }
                transform.GetComponent<Animator>().SetBool("see_a_sheep", true);
                updateLookingFor();
                
                break;
            case FSMState.RunningAway:
                if (!transitions.farmer_close)
                {
                    curState = FSMState.LookingForSheep;
                    break;
                }
                transform.GetComponent<Animator>().SetBool("see_a_sheep", false);
                wolfScript.fleeFarmer();
               
                
                break;

        }


    }


    public void updateLookingFor()                                           //update first child states
    {
        switch (curState_I)
        {
            case FSMState_I.Patorlling:
                if (transitions.see_a_sheep)
                {
                    curState_I = FSMState_I.Chasing;
                    break;
                }
                else if (transitions.in_attack_range)
                {
                    curState_I = FSMState_I.Attacking;
                    break;
                }
                transform.GetComponent<Animator>().SetBool("see_a_sheep", false);
                transform.GetComponent<Animator>().SetBool("in_attack_range", false);
                wolfScript.patrolling();
                setWolfSpeed(PATROLLING_SPEED);
                break;
            case FSMState_I.Chasing:
                if (!transitions.see_a_sheep)
                {
                    curState_I = FSMState_I.Patorlling;
                    break;
                }
                else if (transitions.in_attack_range)
                {
                    curState_I = FSMState_I.Attacking;
                    break;
                }
                transform.GetComponent<Animator>().SetBool("see_a_sheep", true);
                transform.GetComponent<Animator>().SetBool("in_attack_range", false);
                setWolfSpeed(CHASING_SPEED);
                wolfScript.chasing();
                break;
            case FSMState_I.Attacking:
                if (transitions.kill_a_sheep)
                {
                    curState_I = FSMState_I.Patorlling;
                    break;
                }
                else if (!transitions.in_attack_range)
                {
                    curState_I = FSMState_I.Chasing;
                    break;
                }
                wolfScript.attacking();
                //setWolfSpeed(CHASING_SPEED);
                transform.GetComponent<Animator>().SetBool("in_attack_range", true);
                transform.GetComponent<Animator>().SetBool("kill_a_sheep", false);
                break;

        }
    }

    public void updateTransitions()                                        //update transitions 
    {
        if (wolfScript.getSheepDistance() <= wolf.MIN_SHEEP_DISTANCE)
            transitions.see_a_sheep = true;
        else
            transitions.see_a_sheep = false;

        if (wolfScript.getSheepDistance() <= wolf.MIN_ATTACK_DISTANCE)
            transitions.in_attack_range = true;
        else
            transitions.in_attack_range = false;
     
        if(wolfScript.slider != null)                          //after call wolfScript.attacking()
        {
            if (wolfScript.slider.value <= 0  )
                transitions.kill_a_sheep = true;
            else
                transitions.kill_a_sheep = false;
        }                  

        if (wolfScript.getFarmerDistance() <= wolf.MIN_FARMER_DISTANCE)
            transitions.farmer_close = true;
        else
            transitions.farmer_close = false;

        
    }

    void setWolfSpeed(float speed)
    {
        if (wolfScript.slider.value > 50)
        {
            wolfSpeed = speed;
            transform.GetComponent<Animator>().speed = speed;
        }
        else
        {
            wolfSpeed = KILL_SPEED;
            transform.GetComponent<Animator>().speed = KILL_SPEED;
        }
    }
}
