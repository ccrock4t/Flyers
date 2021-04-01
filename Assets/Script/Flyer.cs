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


    //attribute values are between 0 and EvolutionManager.maxGeneValue
    public void init(float leftwingLength = 1.0f, float rightwingLength = 1.0f, float leftwingWidth = 1.0f, float bodyDiameter = 1.0f, float rightwingWidth = 1.0f, float leftwingThickness = 2.0f, float rightwingThickness = 2.0f)
    {
        this.dead = false;
        float wingScaleOffset = 0.1f;
        float wingScale = 0.5f;

        float bodyScaleOffset = 0.5f;
        float bodyScale = 0.05f;
        float scaledBodyDiameter = (bodyDiameter * bodyScale + bodyScaleOffset);

        this.leftwing = this.transform.Find("LeftWing").GetComponent<Wing>(); this.leftwing.transform.localPosition = new Vector3(-scaledBodyDiameter / 2, 0, 0);
        this.rightwing = this.transform.Find("RightWing").GetComponent<Wing>(); this.rightwing.transform.localPosition = new Vector3(scaledBodyDiameter / 2, 0, 0);
        body = this.transform.Find("Body");
        body.transform.gameObject.GetComponent<Renderer>().material.color = new Color((bodyDiameter + leftwingLength) / (EvolutionManager.maxGeneValue*2), (bodyDiameter + rightwingLength) / (EvolutionManager.maxGeneValue*2), (rightwingThickness + leftwingThickness + bodyDiameter) / (EvolutionManager.maxGeneValue*3));


        //left wing
        this.chromosome[(int)Gene.LeftWingLength] = leftwingLength;
        this.chromosome[(int)Gene.LeftWingWidth] = leftwingWidth;
        this.chromosome[(int)Gene.LeftWingThickness] = leftwingThickness;
        this.leftwing.wingThickness = leftwingThickness;
        
        this.leftwing.transform.localScale = new Vector3(leftwingLength * 0.5f * wingScale + wingScaleOffset, leftwingThickness * 0.1f * wingScale + wingScaleOffset, leftwingWidth * wingScale + wingScaleOffset);
        this.leftwing.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = new Color((leftwingLength/10), (leftwingThickness / 10), (leftwingWidth / 10));

        //right wing
        this.chromosome[(int)Gene.RightWingLength] = rightwingLength;
        this.chromosome[(int)Gene.RightWingWidth] = rightwingWidth;
        this.chromosome[(int)Gene.RightWingThickness] = rightwingThickness;
        this.rightwing.wingThickness = rightwingThickness;
        this.rightwing.transform.localScale = new Vector3(rightwingLength * 0.5f * wingScale + wingScaleOffset, rightwingThickness * 0.1f * wingScale + wingScaleOffset, rightwingWidth * wingScale + wingScaleOffset);
        this.rightwing.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = new Color((rightwingLength / EvolutionManager.maxGeneValue), (rightwingThickness / EvolutionManager.maxGeneValue), (rightwingWidth / EvolutionManager.maxGeneValue));

        //body
        this.chromosome[(int)Gene.BodyDiameter] = bodyDiameter;
        this.body.localScale = Vector3.one * (bodyDiameter *2* bodyScale + bodyScaleOffset); ; //set Flyer body size
        int flapsPerDiameter = 2;
        maxflaps = (int)(bodyDiameter * flapsPerDiameter) + 25;
 

        //calculate and set masses
        this.GetComponent<Rigidbody>().mass = (Mathf.Pow(bodyDiameter, 3)*Mathf.PI*4.0f/3.0f)/20f
            + (leftwingLength) 
            * (leftwingWidth )
            * (leftwingThickness)
            + (rightwingLength)
            * (rightwingWidth)
            * (rightwingThickness); //calculate body mass
        this.GetComponent<Rigidbody>().mass = this.GetComponent<Rigidbody>().mass / (40 * EvolutionManager.maxGeneValue) + .5f;

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

