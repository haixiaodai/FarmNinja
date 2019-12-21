using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRiver : MonoBehaviour
{
    public GameObject river;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 3; i++)

        {

            Instantiate(river, new Vector3(Random.Range(0f, 20f), 0.02f, Random.Range(0f, 20f)), Quaternion.identity);

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
