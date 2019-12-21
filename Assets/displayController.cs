using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class displayController : MonoBehaviour
{
    public Camera mainCamera;
    public Camera camera2;

    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
      

        if (Input.GetKeyDown("space"))                         //use space to switch camera and audioListener
        {
            mainCamera.enabled = !mainCamera.enabled;
            camera2.enabled = !camera2.enabled;

            mainCamera.GetComponent<AudioListener>().enabled = !mainCamera.GetComponent<AudioListener>().enabled;
            camera2.GetComponent<AudioListener>().enabled = !camera2.GetComponent<AudioListener>().enabled;
        }

        if (mainCamera.enabled)
        {
            CameraFOV(mainCamera);
        }
        if (camera2.enabled)
        {
            CameraFOV(camera2);
        }
    }


    public void CameraFOV(Camera camera)                             //function to translate camera
    {
        //mouse scrollwheel to control camera
        float wheel = Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * 100;

        //change camera position
        camera.transform.Translate(Vector3.forward * wheel);
    }
}
