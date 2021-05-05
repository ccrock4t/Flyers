using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowWinners : MonoBehaviour
{
    public GameObject prefabFlyer;
    // Start is called before the first frame update
    void Start()
    {
        float[] firstplace = new float[] {
            2.282935f,
            3.598958f,
            3.829222f,
            3.606087f,
            3.510516f,
            2.643354f,
            3.431449f
        }; //48.64 m
        float[] secondplace = new float[] {
            3.960963f,
            1.672601f,
            3.808132f,
            3.023949f,
            3.996559f,
            3.079279f,
            3.635968f

        }; //47.28 m

        float[] thirdplace = new float[] {
            2.066612f,
            3.048501f,
            3.530656f,
            3.340074f,
            3.545026f,
            3.830632f,
            3.551865f
        }; //45.38 m

        for (int i=0; i<3; i++)
        {
            GameObject flyerObject = Instantiate(prefabFlyer);
            Flyer flyer = flyerObject.GetComponent<Flyer>();

            float[] chromosome = null;
            if(i == 0)
            {
                chromosome = secondplace;
            }
            else if (i == 1)
            {
                chromosome = firstplace;
            }
            else if(i == 2)
            {
                chromosome = thirdplace;
            }

            flyer.init(chromosome[(int)Flyer.Gene.LeftWingLength],
                chromosome[(int)Flyer.Gene.RightWingLength],
                chromosome[(int)Flyer.Gene.LeftWingWidth],
                chromosome[(int)Flyer.Gene.BodyDiameter],
                chromosome[(int)Flyer.Gene.RightWingWidth],
                chromosome[(int)Flyer.Gene.LeftWingThickness],
                chromosome[(int)Flyer.Gene.RightWingThickness]);

            flyer.dead = true;

            flyerObject.transform.position = new Vector3(i*5,5);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
