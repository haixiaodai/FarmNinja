using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deadGrass : MonoBehaviour
{

    public float grassHealth = 1.0f;
    public List<GameObject> sheep;

    public static float GRASS_WEAR_PER_SECOND = 0.05f; // no matter the amount of sheep (this just balances the game more)
    public static float GRASS_REGEN_PER_SECOND = 0.05f; // only if no sheeps are on the grass

    // Start is called before the first frame update
    void Start()
    {
        //sheep = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        // update grass health
        if (sheep.Count > 0)
        {
            grassHealth -= GRASS_WEAR_PER_SECOND * Time.deltaTime;
        }
        else
        {
            grassHealth += GRASS_REGEN_PER_SECOND * Time.deltaTime;
        }

        // cap grass health
        grassHealth = Mathf.Min(1, Mathf.Max(0, grassHealth));

        if (grassHealth >= 0.001f)
        {
            foreach (GameObject sheepSingular in sheep)
            {
                sheepSingular.GetComponent<flocking>().hp += Time.deltaTime * 20;
                sheepSingular.GetComponent<flocking>().hp = Mathf.Min(flocking.MAX_HEALTH, Mathf.Max(0, sheepSingular.GetComponent<flocking>().hp));
            }
        }

        for (int i = 0; i < sheep.Count; i++)
        {
            if (sheep[i] == null)
            {
                sheep.RemoveAt(i);
                i--;
            }
        }

        GetComponent<Renderer>().material.color = getDeadGrassColor(1 - grassHealth);
    }

    Color getDeadGrassColor(float deadness) // deadness between 1 and 0 (1 all dead)
    {
        return new Color(.55f * deadness + 0.08f, .32f * deadness + .62f * (1 - deadness), .06f * deadness + .12f);
    }




    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<flocking>() != null)
        {
            sheep.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<flocking>() != null)
        {
            sheep.Remove(other.gameObject);
        }
    }
}

