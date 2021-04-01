using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float max = 0.0f;
        foreach(Flyer flyer in EvolutionManager.singleton.currentGeneration)
        {
            if(flyer.transform.position.y > max)
            {
                max = flyer.transform.position.y;
            }
        }

        this.transform.position = new Vector3(0,max + 7.5f, this.transform.position.z);
    }
}
