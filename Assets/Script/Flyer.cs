using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flyer : MonoBehaviour
{
    //attributes
    public float[] chromosome = new float[7];
    public bool dead = false; int flaps = 0; int maxflaps = -1;
    Wing leftwing, rightwing;
    Transform body; 
    float leftwingflaptimer, rightwingflaptimer;
    bool leftflapped = false, rightflapped = false;

    // statistics
    public float maxHeight = 0.0f;

    public enum Gene
    {
        LeftWingLength = 0,
        RightWingLength = 1,
        LeftWingWidth = 2,
        BodyDiameter = 3,
        RightWingWidth = 4,
        LeftWingThickness = 5,
        RightWingThickness = 6
    }

    // Start is called before the first frame update
    void Start()
    {
    }


    public void init(float leftwingLength = 1.0f, float rightwingLength = 1.0f, float leftwingWidth = 1.0f, float bodyDiameter = 1.0f, float rightwingWidth = 1.0f, float leftwingThickness = 2.0f, float rightwingThickness = 2.0f)
    {
        this.dead = false;
        this.leftwing = this.transform.Find("LeftWing").GetComponent<Wing>(); this.leftwing.transform.localPosition = new Vector3(-bodyDiameter / 2, 0, 0);
        //this.leftwing.GetComponent<HingeJoint>().connectedAnchor = new Vector3(-bodyDiameter / 2, 0, 0);
        this.rightwing = this.transform.Find("RightWing").GetComponent<Wing>(); this.rightwing.transform.localPosition = new Vector3(bodyDiameter / 2, 0, 0);
       // this.rightwing.GetComponent<HingeJoint>().connectedAnchor = new Vector3(bodyDiameter / 2, 0, 0);
        body = this.transform.Find("Body");

        //left wing
        this.chromosome[(int)Gene.LeftWingLength] = leftwingLength;
        this.chromosome[(int)Gene.LeftWingWidth] = leftwingWidth;
        this.chromosome[(int)Gene.LeftWingThickness] = leftwingThickness;
        this.leftwing.wingThickness = leftwingThickness;
        this.leftwing.transform.localScale = new Vector3(leftwingLength, leftwingThickness/2.5f, leftwingWidth);

        //right wing
        this.chromosome[(int)Gene.RightWingLength] = rightwingLength;
        this.chromosome[(int)Gene.RightWingWidth] = rightwingWidth;
        this.chromosome[(int)Gene.RightWingThickness] = rightwingThickness;
        this.rightwing.wingThickness = rightwingThickness;
        this.rightwing.transform.localScale = new Vector3(rightwingLength, rightwingThickness/2.5f, rightwingWidth);

        //body
        this.chromosome[(int)Gene.BodyDiameter] = bodyDiameter;
        this.body.localScale = Vector3.one * bodyDiameter; //set Flyer body size
        int flapsPerDiameter = 20;
        maxflaps = (int)(bodyDiameter * flapsPerDiameter);
 

        //calculate and set masses
        this.GetComponent<Rigidbody>().mass = (Mathf.Pow(bodyDiameter/2, 3)*Mathf.PI*4.0f/3.0f) + leftwingLength * leftwingWidth * leftwingThickness/2 + rightwingLength * rightwingWidth * rightwingThickness/2; //calculate body mass

    }

    // Update is called once per frame
    void Update()
    {
        /**
         * STATISTICS
         */
        if(this.transform.position.y > maxHeight)
        {
            maxHeight = this.transform.position.y;
        }

        if (dead) return;

        /**
         * FLAP WINGS
         */
        leftwingflaptimer += Time.deltaTime;
        rightwingflaptimer += Time.deltaTime;

        //flap left wing
        if (!leftflapped && leftwingflaptimer >= 3*this.leftwing.GetFlapPeriod() / 4f)
        {
            this.leftwing.Flap(this.GetLeftWingLength() * this.GetLeftWingWidth()); //Flap force proportional to wing area
            flaps++;
            if(flaps >= maxflaps)
            {
                dead = true;
                EvolutionManager.singleton.FlyerDied();
            }
            leftflapped = true;
        }

        if (leftwingflaptimer >= this.leftwing.GetFlapPeriod())
        {
            leftwingflaptimer = 0.0f;
            leftflapped = false;
        }

        //flap right wing
        if (!rightflapped && rightwingflaptimer >= 3 * this.rightwing.GetFlapPeriod() / 4f)
        {
            this.rightwing.Flap(this.GetRightWingLength() * this.GetRightWingWidth()); //Flap force proportional to wing area
            flaps++;
            if (flaps >= maxflaps)
            {
                dead = true;
                EvolutionManager.singleton.FlyerDied();
            }
            rightflapped = true;
        }


        if (rightflapped && rightwingflaptimer >= this.rightwing.GetFlapPeriod())
        {
            rightwingflaptimer = 0.0f;
            rightflapped = false;
        }

    }

    /**
     * CHROMOSOME GETTER FUNCTIONS
     */
    public float GetLeftWingLength()
    {   
        return(this.chromosome[(int)Gene.LeftWingLength]);
    }
    public float GetRightWingLength()
    {   
        return(this.chromosome[(int)Gene.RightWingLength]);
    }   
    public float GetLeftWingWidth()
    {   
        return(this.chromosome[(int)Gene.LeftWingWidth]);
    }
    public float GetBodyDiameter()
    {   
        return(this.chromosome[(int)Gene.BodyDiameter]);
    }
    public float GetRightWingWidth()
    {   
        return(this.chromosome[(int)Gene.RightWingWidth]);
    }
    public float GetLeftWingThickness()
    {   
        return(this.chromosome[(int)Gene.LeftWingThickness]);
    }
     public float GetRightWingThickness()
    {   
        return(this.chromosome[(int)Gene.RightWingThickness]);
    }
}

