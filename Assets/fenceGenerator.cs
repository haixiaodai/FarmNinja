using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class fenceGenerator : MonoBehaviour
{
    public bool fencesdown;

    public string bossposition;

    public bool hassheepinside = false;

    public string fencestatus = "up";

    public string sheepposition;

    public float sheepdistance;

    public float wolfdistance;

    public GameObject sheep;

    public GameObject deadGrass;

    public GameObject wolf;

    public GameObject boss;

    public LayerMask grass;

    public GameObject grassinit;

    public float grasshealth;

    public bool hassheepoutside = false;

    public static float fenceWidth = 12.0f;

    public static float fenceModelScale = 0.25f;

    private float timer = 0f;

    private float timer2 = 0f;

    private string path = Directory.GetCurrentDirectory() + "/data.arff";

    public static Vector2[,] fenses = new Vector2[,] { {new Vector2( fenceWidth / 2, -fenceWidth / 2), new Vector2(-fenceWidth / 2, -fenceWidth / 2) },
                                         {new Vector2(-fenceWidth / 2, -fenceWidth / 2), new Vector2(-fenceWidth / 2,  fenceWidth / 2) },
                                         {new Vector2(-fenceWidth / 2,  fenceWidth / 2), new Vector2( fenceWidth / 2,  fenceWidth / 2) },
                                        // {new Vector2( fenceWidth / 2,  fenceWidth / 2), new Vector2( fenceWidth / 2,  fenceWidth * .25f) },
                                        // {new Vector2( fenceWidth / 2,  fenceWidth * .25f), new Vector2( fenceWidth / 2, 0) },
                                        // {new Vector2( fenceWidth / 2, 0), new Vector2( fenceWidth / 2, -fenceWidth * .25f) },
                                        // {new Vector2( fenceWidth / 2, -fenceWidth * .25f), new Vector2( fenceWidth / 2, -fenceWidth / 2) } };
                                         {new Vector2(fenceWidth / 2, fenceWidth / 2), new Vector2(fenceWidth / 2, -fenceWidth / 2) } };

    Vector3[] fenseOffsets = new Vector3[] { new Vector3(0, -fenceWidth, Mathf.PI / 2),
        //new Vector3(fenceWidth * 1.8f, 0, Mathf.PI / 2 + Mathf.PI / 8) };
    //new Vector3(fenceWidth * -1.8f, 0, Mathf.PI / 2 - Mathf.PI / 8),
    //new Vector3(0, fenceWidth * 3.2f, -Mathf.PI / 2) };
    new Vector3(fenceWidth * 1.8f, fenceWidth * 2.2f, -Mathf.PI / 2 - Mathf.PI / 8) };
    //};
    //new Vector3(fenceWidth *- 1.8f, fenceWidth * 2.2f, -Mathf.PI / 2 + Mathf.PI / 8) };

    public static List<GameObject> fenceList;

    public static ArrayList deadGrasses;

    public static int ID_COUNTER = 0;
    public int id = 0;

    public Vector3 target = new Vector3(0, 0, 0);
    public Vector3 standardPos = new Vector3(0, 0, 0);

    float time = 100.0f;
    Vector3 startPosition;
    float timeToReachTarget;

    public static float fenceDownTime = 4.0f;

    public static float wallThikness = 1f;
    public static float wallHeight = 1;

    public static bool doneGenerating = false;

    // Start is called before the first frame update
    void Start()
    {
         
        //Debug.unityLogger.Log("CSV file exists: "+File.Exists(path));
        id = ID_COUNTER++;
        if (id == 0)
        {
            if (SceneManager.GetActiveScene().name == "PathFindingExample")
            {
                //fenses = new Vector2[,] { { new Vector2(82, 2), new Vector2(162, 2) } };
                fenses = new Vector2[,] { { new Vector2(2, 2), new Vector2(66, 2) }, { new Vector2(82, 2), new Vector2(162, 2) } };
                //fenses = new Vector2[,] { { new Vector2(2, 2), new Vector2(66, 2) }, { new Vector2(82, 2), new Vector2(162, 2) }, { new Vector2(2, 18), new Vector2(18, 18) }, { new Vector2(50, 18), new Vector2(66, 18) }, { new Vector2(114, 18), new Vector2(146, 18) }, { new Vector2(18, 34), new Vector2(34, 34) }, { new Vector2(98, 34), new Vector2(114, 34) }, { new Vector2(130, 34), new Vector2(162, 34) }, { new Vector2(2, 50), new Vector2(50, 50) }, { new Vector2(34, 66), new Vector2(50, 66) }, { new Vector2(66, 66), new Vector2(82, 66) }, { new Vector2(114, 66), new Vector2(130, 66) }, { new Vector2(34, 82), new Vector2(66, 82) }, { new Vector2(82, 82), new Vector2(146, 82) }, { new Vector2(18, 98), new Vector2(82, 98) }, { new Vector2(98, 98), new Vector2(114, 98) }, { new Vector2(34, 114), new Vector2(50, 114) }, { new Vector2(82, 114), new Vector2(130, 114) }, { new Vector2(18, 130), new Vector2(34, 130) }, { new Vector2(130, 130), new Vector2(146, 130) }, { new Vector2(34, 146), new Vector2(50, 146) }, { new Vector2(66, 146), new Vector2(98, 146) }, { new Vector2(114, 146), new Vector2(146, 146) }, { new Vector2(2, 162), new Vector2(82, 162) }, { new Vector2(98, 162), new Vector2(162, 162) }, { new Vector2(2, 2), new Vector2(2, 162) }, { new Vector2(18, 50), new Vector2(18, 66) }, { new Vector2(18, 82), new Vector2(18, 114) }, { new Vector2(18, 130), new Vector2(18, 162) }, { new Vector2(34, 2), new Vector2(34, 34) }, { new Vector2(34, 66), new Vector2(34, 82) }, { new Vector2(34, 114), new Vector2(34, 130) }, { new Vector2(50, 18), new Vector2(50, 50) }, { new Vector2(50, 114), new Vector2(50, 146) }, { new Vector2(66, 34), new Vector2(66, 82) }, { new Vector2(66, 98), new Vector2(66, 146) }, { new Vector2(82, 2), new Vector2(82, 66) }, { new Vector2(82, 82), new Vector2(82, 98) }, { new Vector2(82, 114), new Vector2(82, 130) }, { new Vector2(82, 146), new Vector2(82, 162) }, { new Vector2(98, 18), new Vector2(98, 34) }, { new Vector2(98, 50), new Vector2(98, 82) }, { new Vector2(98, 98), new Vector2(98, 114) }, { new Vector2(98, 130), new Vector2(98, 146) }, { new Vector2(114, 34), new Vector2(114, 66) }, { new Vector2(114, 114), new Vector2(114, 162) }, { new Vector2(130, 18), new Vector2(130, 66) }, { new Vector2(130, 98), new Vector2(130, 114) }, { new Vector2(146, 50), new Vector2(146, 130) }, { new Vector2(162, 2), new Vector2(162, 162) } };
                for (int i = 0; i < fenses.Length / 2; i++)
                {
                    fenses[i, 0] /= 3;
                    fenses[i, 1] /= 3;
                }
                fenseOffsets = new Vector3[] { new Vector3(-25.3f, -25.3f, 0) };
            }

            fenceList = new List<GameObject>();
            //fenceList.Add(gameObject);
            deadGrasses = new ArrayList();
            Vector2[,] finalFenses = new Vector2[fenses.Length / 2 * fenseOffsets.Length, 2];
            for (int j = 0; j < fenseOffsets.Length; j++)
            {
                Vector2 fenseOffsetVector = new Vector2(fenseOffsets[j].x, fenseOffsets[j].y);
                float fenseRotation = fenseOffsets[j].z;
                for (int i = 0; i < fenses.Length / 2; i++)
                {
                    finalFenses[j * fenses.Length / 2 + i, 0] = Rotate(fenses[i, 0], fenseRotation) + fenseOffsetVector;
                    finalFenses[j * fenses.Length / 2 + i, 1] = Rotate(fenses[i, 1], fenseRotation) + fenseOffsetVector;
                }
                GameObject dg = deadGrass;
                if (j != 0)
                {
                    dg = Instantiate(deadGrass);
                }
                dg.GetComponent<Transform>().position = new Vector3(fenseOffsetVector.x, 0.01f, fenseOffsetVector.y);
                dg.GetComponent<Transform>().localScale = new Vector3(fenceWidth / 10.0f, 1, fenceWidth / 10.0f);
                dg.GetComponent<Transform>().eulerAngles = new Vector3(0, -fenseRotation * 180.0f / Mathf.PI, 0);

                deadGrasses.Add(dg);

                if (j % 2 == 0 && !((SceneManager.GetActiveScene().name == "PathFindingExample")))
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            GameObject clone = Instantiate(sheep);
                            Vector2 cloneOffset = Rotate(new Vector3(x, y) * 2, fenseRotation);
                            clone.GetComponent<Rigidbody>().position = dg.GetComponent<Transform>().position + new Vector3(cloneOffset.x, 0, cloneOffset.y);
                            clone.GetComponent<Transform>().eulerAngles = new Vector3(0, fenseRotation * 180.0f / Mathf.PI - 90, 0);
                        }
                    }
                }
            }
            fenses = finalFenses;
            for (int i = 1; i < fenses.Length / 2; i++)
            {
                   Instantiate(gameObject);
            }
        }
        else
        {
            fenceList.Add(gameObject);
        }

        //for (int i = 0; i < fenceList.Count; i++)
        //{
        //    if (fenceList[i].GetComponent<Transform>().position.x.Equals(0) && fenceList[i].GetComponent<Transform>().position.z.Equals(0))
        //    {
        //        Destroy(fenceList[i]);
        //        fenceList.RemoveAt(i);
        //    }
        //}

        Transform t = GetComponent<Transform>();
        target = new Vector3((fenses[id, 0].x + fenses[id, 1].x) / 2.0f, wallHeight / 2.0f, (fenses[id, 0].y + fenses[id, 1].y) / 2.0f);
        t.position = target;
        standardPos = target;
        startPosition = standardPos;
        t.localScale = new Vector3((fenses[id, 0] - fenses[id, 1]).magnitude, wallHeight, (fenses[id, 0] - fenses[id, 1]).magnitude) * fenceModelScale;
        t.eulerAngles = new Vector3(0, (Mathf.PI / 2 - Mathf.Atan2(fenses[id, 0].y - fenses[id, 1].y, fenses[id, 0].x - fenses[id, 1].x)) * 180.0f / Mathf.PI, 0);

        if (SceneManager.GetActiveScene().name != "PathFindingExample")
        {
            Destroy(sheep);
        }
        else
        {
            sheep.GetComponent<Transform>().position = new Vector3(0, 0, -25f);
            t.localScale = new Vector3((fenses[id, 0] - fenses[id, 1]).magnitude * .2f, wallHeight, (fenses[id, 0] - fenses[id, 1]).magnitude * 4) * fenceModelScale;
        }
    }

    // Update is called once per frame
    void Update()
    {
        grassinit = (GameObject)deadGrasses[0];
        if (!doneGenerating)
        {
            //Debug.Log("hhh" + ID_COUNTER);
        }
        doneGenerating = true;
        hassheepinside = false;
        hassheepoutside = false;
        time += Time.deltaTime;
        timer += Time.deltaTime;
        Vector3 downPos = standardPos - new Vector3(0, 0.65f, 0);
        
        timer2 += Time.deltaTime;

        float grassdistance = Vector3.Distance(grassinit.GetComponent<Transform>().position, transform.position);
        foreach (GameObject grassobject in deadGrasses)
        {
            if (Vector3.Distance(grassobject.GetComponent<Transform>().position, transform.position) < grassdistance)
            {
                grassinit = grassobject;
            }
        }
        grasshealth = grassinit.GetComponent<deadGrass>().grassHealth;
        //Get wolf distance
        wolfdistance = Vector3.Distance(wolf.GetComponent<Transform>().position, transform.position);

        //Find nearest sheep
        GameObject sheepinit = (GameObject)flocking.sheeps[0];
        sheepdistance = Vector3.Distance(sheepinit.GetComponent<Transform>().position, transform.position);
        foreach (GameObject sheep in flocking.sheeps)
        {
            //Check if there are sheep outside safe area
            if (!Physics.CheckSphere(sheep.GetComponent<Transform>().position, 1f, grass))
            {
                hassheepoutside = true;
            }
            else
            {
                hassheepinside = true;
            }
            if (hassheepinside && !hassheepoutside)
            {
                sheepposition = "inside";
            }
            else if (!hassheepinside && hassheepoutside)
            {
                sheepposition = "outside";
            }
            else
            {
                sheepposition = "middle";
            }

            //Find nearest sheep
            if (Vector3.Distance(sheep.GetComponent<Transform>().position, transform.position) < sheepdistance)
            {
                sheepinit = sheep;

                sheepdistance = Vector3.Distance(sheep.GetComponent<Transform>().position, transform.position);
            }
        }

        //Get boss position
        if (Physics.CheckSphere(boss.GetComponent<Transform>().position, 1f, grass))
        {
            bossposition = "inside";
        }
        else
        {
            bossposition = "outside";
        }

        decision2();

        //Make decision (method 1)
        //float grasshealth = grassinit.GetComponent<deadGrass>().grassHealth;
        //    if (hassheepoutside)
        //    {
        //        if (grasshealth <= 0)
        //        {
        //            fencedown();
        //        }
        //        else
        //        {
        //            if (grassinit.GetComponent<deadGrass>().grassHealth <= 0.968)
        //            {
        //                fenceup();
        //            }
        //            else
        //            {
        //                fencedown();
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if(grassinit.GetComponent<deadGrass>().grassHealth <= 0.05)
        //        {
        //            fencedown();
        //        }
        //    }

        //Record data
        //if (timer > 2f)
        //{
        //    timer = 0f;
        //    File.AppendAllText(path, grassinit.GetComponent<deadGrass>().grassHealth + "," + wolfdistance + "," + sheppdistance + "," + sheepposition + "," + bossposition + "," + fencestatus + Environment.NewLine);
        //File.AppendAllText(path, grassinit.GetComponent<deadGrass>().grassHealth + "," + wolfdistance + "," + sheppdistance + "," + sheepposition + "," + bossposition + "," + "null" + Environment.NewLine);
        //}
        //if (time < fenceDownTime + 1.0f)
        //{
        //    transform.position = Vector3.Lerp(startPosition, downPos, time);
        //    fencesdown = true;
        //    //Debug.unityLogger.Log("Fences down: " + transform.position);
        //}
        //else
        //{
        //    transform.position = Vector3.Lerp(downPos, startPosition, time - fenceDownTime - 1.0f);
        //    fencesdown = false;
        //}
        //foreach (GameObject gb in fenceList)
        //{
        //    if (gb.GetComponent<fenceGenerator>().fencesdown)
        //    {
        //        gb.layer = LayerMask.NameToLayer("Default");
        //    }
        //}
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


    private void OnMouseDown()
    {
        Vector3 downPos = standardPos - new Vector3(0, 0.65f, 0);

        //Find the correct grass
        //GameObject grassinit = (GameObject)deadGrasses[0];
        //float grassdistance = Vector3.Distance(grassinit.GetComponent<Transform>().position, transform.position);
        //foreach (GameObject grassobject in deadGrasses)
        //{
        //    if (Vector3.Distance(grassobject.GetComponent<Transform>().position, transform.position) < grassdistance)
        //    {
        //        grassinit = grassobject;
        //    }
        //}

        //Get wolf distance
        wolfdistance = Vector3.Distance(wolf.GetComponent<Transform>().position, transform.position);

        //Find nearest sheep
        GameObject sheepinit = (GameObject)flocking.sheeps[0];
        float sheppdistance = Vector3.Distance(sheepinit.GetComponent<Transform>().position, transform.position);
        foreach (GameObject sheep in flocking.sheeps)
        {
            //Check if there are sheep outside safe area
            if (!Physics.CheckSphere(sheep.GetComponent<Transform>().position, 1f, grass))
            {
                hassheepoutside = true;
            }

            //Find nearest sheep
            if (Vector3.Distance(sheep.GetComponent<Transform>().position, transform.position) < sheppdistance)
            {
                sheepinit = sheep;

                sheppdistance = Vector3.Distance(sheep.GetComponent<Transform>().position, transform.position);
            }
        }

        //Record data (down)
        //if (fencestatus.Equals("up"))
        //{
        //    //timer = 0f;
        //    transform.position = Vector3.Lerp(startPosition, downPos, time);
        //    fencesdown = true;
        //    fencestatus = "down";
        //    File.AppendAllText(path, grassinit.GetComponent<deadGrass>().grassHealth + "," + wolfdistance + "," + sheppdistance + "," + sheepposition + "," + bossposition + "," + "down" + Environment.NewLine);
        //    //File.AppendAllText(path, grassinit.GetComponent<deadGrass>().grassHealth + ","+wolfdistance+","+sheppdistance+","+hassheepoutside+","+"down"+Environment.NewLine);
        //    //Debug.unityLogger.Log("Fences down: " + hassheepoutside);
        //}

        //Record data (up)
        //else
        //{
        //    //transform.position = Vector3.Lerp(startPosition, downPos, time);
        //    //fencesdown = true;
        //    transform.position = Vector3.Lerp(downPos, startPosition, time);
        //    fencesdown = false;
        //    fencestatus = "up";
        //    File.AppendAllText(path, grassinit.GetComponent<deadGrass>().grassHealth + "," + wolfdistance + "," + sheppdistance + "," + sheepposition + "," + bossposition + "," + "up" + Environment.NewLine);
        //    //File.AppendAllText(path, grassinit.GetComponent<deadGrass>().grassHealth + "," + wolfdistance + "," + sheppdistance + "," + hassheepoutside +","+ "up" + Environment.NewLine);
        //    //Debug.unityLogger.Log("Fences up: " + transform.position);
        //}
        //if (time > fenceDownTime + 1.0f && time < fenceDownTime + 2.0f) // only if on the way up, go back down
        //{
        //    time = fenceDownTime + 2.0f - time;
        //}
        //else if (time < 1.0f)
        //{
        //    // do not change time, already on the way down
        //}
        //else if (time > fenceDownTime + 2.0f)
        //{
        //    time = 0.0f;
        //}
        //else
        //{
        //    time = 1.0f;
        //}
    }


    public void fenceup()
    {
        if (fencestatus.Equals("down")) { 
        Vector3 downPos = standardPos - new Vector3(0, 0.65f, 0);
        transform.position = Vector3.Lerp(downPos, startPosition, time);
        fencesdown = false;
        fencestatus = "up";
    }
    }

    public void fencedown()
    {
        if (fencestatus.Equals("up"))
        {
            Vector3 downPos = standardPos - new Vector3(0, 0.65f, 0);
            transform.position = Vector3.Lerp(startPosition, downPos, time);
            fencesdown = true;
            fencestatus = "down";
        }
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    if (time > fenceDownTime + 1.0f && time < fenceDownTime + 2.0f) // only if on the way up, go back down
    //    {
    //        time = fenceDownTime + 2.0f - time;
    //    }
    //}

    //Decision tree generated in the second method
    public void decision2()
    {
        if (bossposition.Equals("middle"))
        {
            fencedown();
        }
        else
        {
            if (grasshealth <= 0.15)
            {
                if (sheepposition.Equals("inside"))
                {
                    if (grasshealth <= 0.08)
                    {
                        fencedown();
                    }
                    else
                    {
                        fenceup();
                    }
                }
                else
                {
                    fencedown();
                }
            }
            else
            {
                if (grasshealth <= 0.82)
                {
                    if (sheepposition.Equals("inside"))
                    {
                        fenceup();
                    }
                    else if (sheepposition.Equals("outside"))
                    {
                        if (grasshealth <= 0.32)
                        {
                            fencedown();
                        }
                        else
                        {
                            fenceup();
                        }
                    }
                    else
                    {
                        fencedown();
                    }
                }
                else
                {
                    if (sheepdistance <= 12.42)
                    {
                        fencedown();
                    }
                    else
                    {
                        if (wolfdistance <= 42.06)
                        {
                            fenceup();
                        }
                        else
                        {
                            fencedown();
                        }
                    }
                }
            }
        }
    }
}
