using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class flocking : MonoBehaviour
{

    public static readonly int SHEEP_STATE_RANDOM_WALKING = 0;
    public static readonly int SHEEP_STATE_FLOCKING = 1;
    public static readonly int SHEEP_STATE_RANDOM_WALKING_AND_FLEE_FARMER = 2;
    public static readonly int SHEEP_STATE_FLOCKING_AND_FLEE_FARMER = 3;
    public static readonly int SHEEP_STATE_FLEE_FARMER = 4;
    public static readonly int SHEEP_STATE_FLEE_WOLF = 5;
    public static readonly int SHEEP_STATE_DEAD = 6;
 
    private int state = SHEEP_STATE_RANDOM_WALKING;

    


    public GameObject wolf;
    public Rigidbody rb;
    public float randomWalkAngle = 0.0f; // angle acording to the imagenary circle that is sqrt(2) infront of the sheep
    public int id;
    public float timeToNextRandomMovement;
    public float deadTime = 0;
    public Vector3 randomTarget;
    public Vector3 flockingTarget;
    public Vector3 fleeFarmerTarget;
    public Vector3 fleeWolfTarget;
    public Vector3 target;

    // state changing variales
    public float hp = MAX_HEALTH;
    public int numberOfSheepWeCanSee = 0;
    public float distanceToShepherdV = 0.0f;
    public float distanceToWolfV = 100.0f; // TODO keep this up to date when the wolf is importated


    private static int ID_COUNT = 0;

    public static ArrayList sheeps = null;

    public static float MAX_HEALTH = 100.0f;
    public static float SPEED = 5f;
    public static float BACKWARDS_SPEED_FRACTION = 0.5f;
    public static float RANDOM_WALK_ANGLE_CHANGE_PER_SEC = 2.0f;
    public static float RANDOM_WALK_CIRCLE_OFFSET = 1 + Mathf.Sqrt(2);
    public static float MAX_ANGULAR_VELOCITY = Mathf.PI;
    public static float SEPERATION_MAX_DISTANCE = 10;
    public static float MAX_DISTANCE_TO_TARGET_WITH_FULL_SPEED = 1.0f; // plus MIN_DISTANCE_BEFORE_PURSUING_TARGET
    public static float MAX_ANGLE_TO_TARGET_WITH_FULL_ANGULAR_SPEED = Mathf.PI / 4;
    public static float MIN_DISTANCE_TO_SHEPHERD = 2.0f;
    public static float DISPERSE_DISTANCE_TO_SHEPHERD = 9.0f; // this is added to the MIN_DISTANCE_TO_SHEPHERD

    public static float SEPERATION_WEIGHT = 3f;
    public static float COHESION_WEIGHT = 10.0f;
    public static float ALIGNMENT_WEIGHT = 0.03f;
    public static float SHEPHERD_WEIGHT = 5.0f;
    public static float RANDOM_MOVEMENT_WEIGHT = 0.0f;

    public static float MIN_DISTANCE_BEFORE_PURSUING_TARGET = 1.5f;
    public static float MIN_TIME_BETWEEN_RANDOM_MOVEMENTS = 4.5f;

    public static float MIN_DISTANCE_TO_WOLF = 10.0f;
    public static float MAX_DISTANCE_TO_WOLF = 15.0f;

    public static float TIME_DEAD_BEFORE_DESTRUCTION = 10.0f;


    private readonly float EPSILON = 0.001f;
    private FSM_sheep fsm_sheep;
    private float speedFactor;
    public static Vector3 shepherdPosition = new Vector3(100, 100, 100); // dummy location for now

    public GameObject shepherd;



    // Start is called before the first frame update
    void Start()
    {
        fsm_sheep = transform.GetComponent<FSM_sheep>();
        id = ID_COUNT++;
        rb = GetComponent<Rigidbody>();
        rb.velocity = new Vector3(0, 0, 0);
        if (sheeps == null)
        {
            sheeps = new ArrayList();
        }
        else
        {
            // FISRT SHEEP SHOULD NOT BE ADDED TO THE LIST
            sheeps.Add(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        shepherdPosition = shepherd.GetComponent<Transform>().position;

        Vector3 seperation = new Vector3(0, 0, 0);
        Vector3 cohesion = new Vector3(0, 0, 0);
        Vector3 alignment = new Vector3(0, 0, 0);
        int numberOfSheepsWeCanSee = 0;
        foreach (GameObject sheep in sheeps)
        {
            if (canWeSeeIt(sheep))
            {
                numberOfSheepsWeCanSee++;
                Vector3 vectorToOtherSheep = sheep.GetComponent<Rigidbody>().position - rb.position;
                float distance = vectorToOtherSheep.magnitude;
                if (System.Math.Abs(distance) < EPSILON)
                {
                    continue; // its ourself 
                }
                if (distance < SEPERATION_MAX_DISTANCE)
                {
                    // to close run away                   
                    Vector3 directionAway = vectorToOtherSheep;
                    directionAway.Normalize();
                    directionAway.Scale(new Vector3(SEPERATION_MAX_DISTANCE - distance, SEPERATION_MAX_DISTANCE - distance, SEPERATION_MAX_DISTANCE - distance));
                    directionAway.Scale(new Vector3(1.0f / SEPERATION_MAX_DISTANCE, 1.0f / SEPERATION_MAX_DISTANCE, 1.0f / SEPERATION_MAX_DISTANCE));
                    seperation -= directionAway;
                }
                cohesion += sheep.GetComponent<Rigidbody>().position - rb.position;
                alignment += sheep.GetComponent<Rigidbody>().velocity - rb.velocity;
            }
        }

        // update state changing variables
        this.numberOfSheepWeCanSee = numberOfSheepsWeCanSee;
        this.distanceToShepherdV = (rb.position - shepherdPosition).magnitude;
        // update hp and distance to wolf when they get implementet

        //stateMachineUpdate();



        target = new Vector3(0, 0, 0);

        target += getRandomMovement(); // to add some randomness to the sheep

        flockingTarget = getFlockingTarget(seperation, cohesion, alignment);
        //flockingTarget = (transform.position - ledersheep.GetComponent<Transform>().position);
        fleeFarmerTarget = getFleeFarmerTarget();
        fleeWolfTarget = new Vector3(0, 0, 0); // TODO update accordingly

        fsm_sheep.updateTransitions();

        fsm_sheep.updateFSM();
        speedFactor = fsm_sheep.sheepSpeed;
        
/*
        if (state == SHEEP_STATE_RANDOM_WALKING)
        {
            // randomness already added, since it is always added
        }
        else if (state == SHEEP_STATE_FLOCKING)
        {
            target += flockingTarget;
        }
        else if (state == SHEEP_STATE_RANDOM_WALKING_AND_FLEE_FARMER)
        {
            // randomness already added, since it is always added
            float disperseFactor = (distanceToShepherdV - MIN_DISTANCE_TO_SHEPHERD) / DISPERSE_DISTANCE_TO_SHEPHERD;
            target += fleeFarmerTarget * (1 - disperseFactor);
        }
        else if (state == SHEEP_STATE_FLOCKING_AND_FLEE_FARMER)
        {
            float disperseFactor = (distanceToShepherdV - MIN_DISTANCE_TO_SHEPHERD) / DISPERSE_DISTANCE_TO_SHEPHERD;
            target += flockingTarget * disperseFactor;
            target += fleeFarmerTarget * (1 - disperseFactor);
        }
        else if (state == SHEEP_STATE_FLEE_FARMER)
        {
            target += fleeFarmerTarget;
        }
        else if (state == SHEEP_STATE_FLEE_WOLF)
        {
            target = fleeWolfTarget;
            speedFactor = 2.0f;
        }
        else if (state == SHEEP_STATE_DEAD)
        {
            // DONT MOVE (ghost sheep are scary!!!)
            target = new Vector3(0, 0, 0);
            transform.GetComponent<Animator>().SetBool("dead", true);

            deadTime += Time.deltaTime;
            if (deadTime > TIME_DEAD_BEFORE_DESTRUCTION)
            {
                sheeps.Remove(gameObject);
                Destroy(gameObject);
            }
        }
*/


        goToTarget(target, speedFactor);


        hp -= Time.deltaTime * 1;

        GetComponentInChildren<Slider>().value = hp;


        return;



        // OLD RANDOM WALK CODE
        /*

        float angle = transform.eulerAngles.y;
        angle = angle / 180 * Mathf.PI;

        randomWalkAngle += Random.Range(-RANDOM_WALK_ANGLE_CHANGE_PER_SEC * Time.deltaTime, RANDOM_WALK_ANGLE_CHANGE_PER_SEC * Time.deltaTime);
        randomWalkAngle = Mathf.Min(randomWalkAngle, Mathf.PI / 4);
        randomWalkAngle = Mathf.Max(randomWalkAngle, -Mathf.PI / 4);

        Vector2 pointOnRandomWalkCircle = new Vector2(Mathf.Cos(randomWalkAngle), Mathf.Sin(randomWalkAngle)); // point on circle
        Vector2 circleOffset = new Vector2(RANDOM_WALK_CIRCLE_OFFSET, 0); // offset infront of the animal
        pointOnRandomWalkCircle += circleOffset;

        float angleToRandomWalkCircle = Mathf.Atan2(pointOnRandomWalkCircle.y, pointOnRandomWalkCircle.x);

        Vector3 angularVelocity = new Vector3(0, angleToRandomWalkCircle, 0);
        angularVelocity.Normalize();
        angularVelocity.Scale(new Vector3(0, MAX_ANGULAR_VELOCITY, 0));
        rb.angularVelocity = angularVelocity;

        Vector3 vel = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
        vel.Scale(new Vector3(SPEED / vel.magnitude, 0, SPEED / vel.magnitude));
        rb.velocity = vel;
        */

    }

    bool canWeSeeIt(GameObject otherSheep)
    {
        Vector3 vectorToOtherSheep = otherSheep.GetComponent<Rigidbody>().position - rb.position;
        float distance = vectorToOtherSheep.magnitude;
        return distance < 15;
    }

    Vector3 getFleeFarmerTarget()
    {
        Vector3 shepherdDirection = rb.position - shepherdPosition;

        float distanceToShepherd = shepherdDirection.magnitude;
        shepherdDirection.Normalize();
        shepherdDirection *= SHEPHERD_WEIGHT;

        return shepherdDirection;
    }

    Vector3 getFlockingTarget(Vector3 seperation, Vector3 cohesion, Vector3 alignment)
    {
        int localNumberOfSheepWeCanSee = numberOfSheepWeCanSee;
        localNumberOfSheepWeCanSee = Mathf.Max(localNumberOfSheepWeCanSee, 1);
        cohesion *= 1.0f / localNumberOfSheepWeCanSee;
        alignment *= 1.0f / localNumberOfSheepWeCanSee;

        Vector3 seperationDirection = seperation;
        Vector3 cohesionDirection = cohesion;
        Vector3 alignmentDirection = alignment;

        seperationDirection.Scale(new Vector3(SEPERATION_WEIGHT, SEPERATION_WEIGHT, SEPERATION_WEIGHT));
        cohesionDirection.Scale(new Vector3(COHESION_WEIGHT, COHESION_WEIGHT, COHESION_WEIGHT));
        alignmentDirection.Scale(new Vector3(ALIGNMENT_WEIGHT, ALIGNMENT_WEIGHT, ALIGNMENT_WEIGHT));

        return seperationDirection + cohesionDirection + alignmentDirection;
    }

    public Vector3 getRandomMovement()
    {
        timeToNextRandomMovement -= Time.deltaTime;
        if (timeToNextRandomMovement < -2.0)
        {
            timeToNextRandomMovement = Random.Range(100.0f * MIN_TIME_BETWEEN_RANDOM_MOVEMENTS, 200.0f * MIN_TIME_BETWEEN_RANDOM_MOVEMENTS) / 100.0f;
            float randomAngle = Random.value * Mathf.PI * 2;
            randomTarget = new Vector3(Mathf.Cos(randomAngle), 0, Mathf.Sin(randomAngle));
            randomTarget *= MIN_DISTANCE_BEFORE_PURSUING_TARGET + MAX_DISTANCE_TO_TARGET_WITH_FULL_SPEED / 2.0f;
        }
        if (timeToNextRandomMovement < 0)
        {
            return randomTarget;
        }
        return new Vector3(0, 0, 0);
    }

    void goToTarget(Vector3 target, float speedFactorWolf)
    {

        Vector3 realTarget = target;
        target = getTargetWithSimpleObstacleAvoidance(rb.position, target);

        float angle = transform.eulerAngles.y;
        angle = angle / 180 * Mathf.PI;



        Vector3 angularVelocity = new Vector3(0, 0, 0);

        float angleToTarget = Mathf.Atan2(target.x, target.z) - angle;
        while (angleToTarget > Mathf.PI)
        {
            angleToTarget -= Mathf.PI * 2;
        }
        while (angleToTarget < -Mathf.PI)
        {
            angleToTarget += Mathf.PI * 2;
        }


        if (angleToTarget > 0)
        {
            angularVelocity = new Vector3(0, MAX_ANGULAR_VELOCITY, 0);
        }
        else
        {
            angularVelocity = new Vector3(0, -MAX_ANGULAR_VELOCITY, 0);
        }

        if (Mathf.Abs(angleToTarget) < MAX_ANGLE_TO_TARGET_WITH_FULL_ANGULAR_SPEED)
        {
            float angleSpeedFactor = Mathf.Abs(angleToTarget) / MAX_ANGLE_TO_TARGET_WITH_FULL_ANGULAR_SPEED;
            angularVelocity.Scale(new Vector3(angleSpeedFactor, angleSpeedFactor, angleSpeedFactor));
        }

        Vector3 vel = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
        vel.Normalize();
        bool walkingForward = true;
        if ((target - vel).magnitude > (target + vel).magnitude)
        {
            vel *= -BACKWARDS_SPEED_FRACTION * speedFactorWolf;
            walkingForward = false;
        }



        float distanceToTarget = realTarget.magnitude;
        if (distanceToTarget > MAX_DISTANCE_TO_TARGET_WITH_FULL_SPEED + MIN_DISTANCE_BEFORE_PURSUING_TARGET)
        {
            vel *= SPEED * speedFactorWolf;
        }
        else
        {
            float speedFactor = (SPEED * speedFactorWolf) / MAX_DISTANCE_TO_TARGET_WITH_FULL_SPEED * (distanceToTarget - MIN_DISTANCE_BEFORE_PURSUING_TARGET);
            vel.Scale(new Vector3(speedFactor, speedFactor, speedFactor));
        }

        float oldVelMagnitude = rb.velocity.magnitude;
        float newVelMagnitude = vel.magnitude;

        if (distanceToTarget > MIN_DISTANCE_BEFORE_PURSUING_TARGET)
        {
            rb.velocity = vel;
            rb.angularVelocity = angularVelocity;
        }
        else
        {
            rb.velocity = new Vector3(0, 0, 0);
            rb.angularVelocity = new Vector3(0, 0, 0);
        }



        if (rb.velocity.magnitude > 0.2)
        {
            transform.GetComponent<Animator>().SetBool("sheepIsWalking", true);
            if (walkingForward)
            {
                transform.GetComponent<Animator>().SetBool("sheepIsWalkingBackwards", true);
            }
            else
            {
                transform.GetComponent<Animator>().SetBool("sheepIsWalkingBackwards", false);
            }
            transform.GetComponent<Animator>().speed = oldVelMagnitude / 2.5f;
        }
        else
        {
            transform.GetComponent<Animator>().SetBool("sheepIsWalking", false);
            transform.GetComponent<Animator>().speed = 1;
        }

    }











    Vector3 getTargetWithSimpleObstacleAvoidance(Vector3 position, Vector3 target)
    {
        Vector2 pos = new Vector2(position.x, position.z);
        Vector2 tar = new Vector2(target.x, target.z) + pos;
        int obsticale = whatDoIHit(pos, tar);
        if (obsticale == -1)
        {
            return target;
        }
        else
        {
            Vector2[] path = getPathAround(pos, tar, fenceGenerator.fenses[obsticale, 0], fenceGenerator.fenses[obsticale, 1]);
            if (path.Length > 0)
            {
                Vector3 newTarget = new Vector3(path[1].x, 0, path[1].y);
                return newTarget - position;
            }
            else
            {
                // cannot reach target with simple obsticle advodiance
                return target;
            }
        }
    }

    public static int whatDoIHit(Vector2 p1, Vector2 p2)
    {
        int intersectId = -1;
        for (int i = 0; i < fenceGenerator.fenses.Length / 2; i++)
        {
            Vector2 dummy = new Vector2();
            if (LineSegmentsIntersection(p1, p2, fenceGenerator.fenses[i, 0], fenceGenerator.fenses[i, 1], out dummy))
            {
                //if (fenceGenerator.fenceList[i].GetComponent<Transform>().position.y > .3)
                //{
                    intersectId = i;
                //}
            }
        }
        return intersectId;
    }

    public static bool canIWalkLine(Vector2 p1, Vector2 p2)
    {
        return whatDoIHit(p1, p2) == -1;
    }

    Vector2[] getPathAround(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        Vector2[] path1 = new Vector2[] { p1, getPointAround(p3, p4, 1, 0), p2 };
        Vector2[] path2 = new Vector2[] { p1, getPointAround(p4, p3, 1, 0), p2 };
        Vector2[] path3 = new Vector2[] { p1, getPointAround(p3, p4, 2, 0), getPointAround(p3, p4, 2, 1), p2 };
        Vector2[] path4 = new Vector2[] { p1, getPointAround(p4, p3, 2, 0), getPointAround(p4, p3, 2, 1), p2 };

        Vector2[][] paths = new Vector2[][] { path1, path2, path3, path4 };

        float shortestDistanceWithoutObsticals = 10000.0f;
        Vector2[] shortestPathWithoutObsticals = new Vector2[0];
        foreach (Vector2[] path in paths)
        {
            float totalLength = 0;
            bool success = true;
            for (int i = 0; i < path.Length - 1; i++)
            {
                if (!canIWalkLine(path[i], path[i + 1]))
                {
                    success = false;
                    break;
                }
                totalLength += (path[i] - path[i + 1]).magnitude;
            }
            if (success && totalLength < shortestDistanceWithoutObsticals)
            {
                shortestDistanceWithoutObsticals = totalLength;
                shortestPathWithoutObsticals = path;
            }
        }
        return shortestPathWithoutObsticals;
    }

    Vector2 getPointAround(Vector2 p1, Vector2 p2, int count, int index)
    {
        Vector2 direction = (p1 - p2).normalized * 2;
        //direction
        if (count == 1)
        {
            return p1 + direction;
        }
        else
        {
            if (index == 0)
            {
                return p1 + Rotate(direction, -Mathf.PI / 4);
            }
            else
            {
                return p1 + Rotate(direction, Mathf.PI / 4);
            }
        }

    }

    Vector2 Rotate(Vector2 v_in, float angle)
    {
        Vector2 v = new Vector2(0, 0);
        float sin = Mathf.Sin(angle);
        float cos = Mathf.Cos(angle);

        float tx = v_in.x;
        float ty = v_in.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }



    static bool LineSegmentsIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector3 p4, out Vector2 intersection)
    {
        intersection = Vector2.zero;

        var d = (p2.x - p1.x) * (p4.y - p3.y) - (p2.y - p1.y) * (p4.x - p3.x);

        if (d == 0.0f)
        {
            return false;
        }

        var u = ((p3.x - p1.x) * (p4.y - p3.y) - (p3.y - p1.y) * (p4.x - p3.x)) / d;
        var v = ((p3.x - p1.x) * (p2.y - p1.y) - (p3.y - p1.y) * (p2.x - p1.x)) / d;

        if (u < 0.0f || u > 1.0f || v < 0.0f || v > 1.0f)
        {
            return false;
        }

        intersection.x = p1.x + u * (p2.x - p1.x);
        intersection.y = p1.y + u * (p2.y - p1.y);

        return true;
    }

    public float getWolfDistance()
    {
        float distance = (wolf.transform.position - transform.position).magnitude;

        return distance;
    }

  /*  void stateMachineUpdate()
    {
        if (state == SHEEP_STATE_RANDOM_WALKING)
        {
            if (numberOfSheepWeCanSee > 0)
            {
                state = SHEEP_STATE_FLOCKING;
            }
            else if (distanceToShepherdV < DISPERSE_DISTANCE_TO_SHEPHERD + MIN_DISTANCE_TO_SHEPHERD)
            {
                state = SHEEP_STATE_RANDOM_WALKING_AND_FLEE_FARMER;
            }
        }
        else if (state == SHEEP_STATE_FLOCKING)
        {

            if (numberOfSheepWeCanSee == 0)
            {
                state = SHEEP_STATE_RANDOM_WALKING;
            }
            else if (distanceToShepherdV < DISPERSE_DISTANCE_TO_SHEPHERD + MIN_DISTANCE_TO_SHEPHERD)
            {
                state = SHEEP_STATE_FLOCKING_AND_FLEE_FARMER;
            }
        }
        else if (state == SHEEP_STATE_RANDOM_WALKING_AND_FLEE_FARMER)
        {
            if (numberOfSheepWeCanSee > 0)
            {
                state = SHEEP_STATE_FLOCKING_AND_FLEE_FARMER;
            }
            else if (distanceToShepherdV > DISPERSE_DISTANCE_TO_SHEPHERD + MIN_DISTANCE_TO_SHEPHERD)
            {
                state = SHEEP_STATE_RANDOM_WALKING;
            }
            else if (distanceToShepherdV < MIN_DISTANCE_TO_SHEPHERD)
            {
                state = SHEEP_STATE_FLEE_FARMER;
            }
        }
        else if (state == SHEEP_STATE_FLOCKING_AND_FLEE_FARMER)
        {
            if (numberOfSheepWeCanSee == 0)
            {
                state = SHEEP_STATE_RANDOM_WALKING_AND_FLEE_FARMER;
            }
            else if (distanceToShepherdV > DISPERSE_DISTANCE_TO_SHEPHERD + MIN_DISTANCE_TO_SHEPHERD)
            {
                state = SHEEP_STATE_FLOCKING;
            }
            else if (distanceToShepherdV < MIN_DISTANCE_TO_SHEPHERD)
            {
                state = SHEEP_STATE_FLEE_FARMER;
            }

        }
        else if (state == SHEEP_STATE_FLEE_FARMER)
        {
            if (distanceToShepherdV > MIN_DISTANCE_TO_SHEPHERD)
            {
                if (numberOfSheepWeCanSee == 0)
                {
                    state = SHEEP_STATE_RANDOM_WALKING_AND_FLEE_FARMER;
                }
                else
                {
                    state = SHEEP_STATE_FLOCKING_AND_FLEE_FARMER;
                }
            }
        }
        else if (state == SHEEP_STATE_FLEE_WOLF)
        {
            if (distanceToWolfV > MAX_DISTANCE_TO_WOLF)
            {
                state = SHEEP_STATE_RANDOM_WALKING;
            }
        }
        else if (state == SHEEP_STATE_DEAD)
        {
            // stay in this state forever
        }

        if (hp <= 0)
        {
            state = SHEEP_STATE_DEAD;
        }
        else if (distanceToWolfV < MIN_DISTANCE_TO_WOLF)
        {
            state = SHEEP_STATE_FLEE_WOLF;
        }
    }*/
}

