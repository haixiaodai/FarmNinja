using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManMove : MonoBehaviour
{

    float move_X;

    float move_Y;

    float move_Speed = 18f;
    float rotateSpeed = 2.0F;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float angle = transform.eulerAngles.y;
        angle = angle / 180 * Mathf.PI;

        //move_X = Input.GetAxis("Horizontal") * Time.deltaTime * 10;
        move_Y = Input.GetAxis("Vertical") * move_Speed;
        gameObject.GetComponent<Rigidbody>().velocity = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)) * move_Y;
        transform.Rotate(0, Input.GetAxis("Horizontal") * rotateSpeed, 0);
        gameObject.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
        //transform.Rotate(0, Input.GetAxis("Vertical") * rotateSpeed, 0);
        if (Mathf.Abs(move_Y) > 0.001f && move_Speed > 0.5f)
        {
            if (move_Y > 0)
            {
                transform.GetComponent<Animator>().SetBool("WalkingForwards", true);
            }
            else
            {
                transform.GetComponent<Animator>().SetBool("WalkingBackwards", true);
            }
        }
        else
        {
            transform.GetComponent<Animator>().SetBool("WalkingBackwards", false);
            transform.GetComponent<Animator>().SetBool("WalkingForwards", false);
        }
    }

}
