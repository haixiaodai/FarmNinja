using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{
    public GameObject grass;
    public GameObject ground;
    
    // Start is called before the first frame update
    void Start()
    {
        System.Random rand = new System.Random();
        
        float groundSizeX = ground.transform.localScale.x;
        float groundSizeZ = ground.transform.localScale.z;

        for (float x = -groundSizeX; x < groundSizeZ; x=x+1)
        {
            
            for (float z = -groundSizeZ; z < groundSizeZ; z=z+1)
            {
                if (x == 0 && z == 0)
                    continue;
                GameObject go = Instantiate(grass);
                
                go.transform.SetParent(ground.transform);
                go.transform.localScale = new Vector3(0.1f, 1, 0.1f);
                go.transform.localPosition = new Vector3(x, 0, z);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
