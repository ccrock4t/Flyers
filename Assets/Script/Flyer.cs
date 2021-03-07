using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flyer : MonoBehaviour
{
    Wing leftwing, rightwing;
    Transform body;
    float leftwingLength, leftwingWidth, leftwingflaptimer, leftwingThickness;
    float rightwingLength, rightwingWidth, rightwingflaptimer, rightwingThickness;
    float bodyDiameter;
    bool leftflapped = false, rightflapped = false;

    // Start is called before the first frame update
    void Start()
    {
        leftwing = this.transform.Find("LeftWing").GetComponent<Wing>();
        rightwing = this.transform.Find("RightWing").GetComponent<Wing>();
        body = this.transform.Find("Body");
        init();
    }

    public void init(float leftwingLength = 1.0f, float rightwingLength = 1.0f, float leftwingWidth = 1.0f, float bodyDiameter = 1.0f, float rightwingWidth = 1.0f, float leftwingThickness = 1.0f, float rightwingThickness = 1.0f)
    {
        //left wing
        this.leftwingLength = leftwingLength;
        this.leftwingWidth = leftwingWidth;
        this.leftwingThickness = leftwingThickness;
        leftwing.animSpeed = leftwingThickness;
        leftwing.transform.localScale = new Vector3(leftwingLength, leftwingThickness, 1f);

        //right wing
        this.rightwingLength = rightwingLength;
        this.rightwingWidth = rightwingWidth;
        this.rightwingThickness = rightwingThickness;
        rightwing.animSpeed = rightwingThickness;
        rightwing.transform.localScale = new Vector3(rightwingLength, rightwingThickness, 1f);

        //body
        this.bodyDiameter = bodyDiameter;
        this.body.localScale = Vector3.one * bodyDiameter; //set Flyer body size
        this.GetComponent<Rigidbody>().mass = (Mathf.Pow(bodyDiameter/2, 3)*Mathf.PI*4.0f/3.0f + leftwingLength * leftwingWidth * leftwingThickness + rightwingLength * rightwingWidth * rightwingThickness); //calculate mass
    }

    // Update is called once per frame
    void Update()
    {
        /*
         * TODO: IMPLEMENT
         */
        leftwingflaptimer -= Time.deltaTime;
        rightwingflaptimer -= Time.deltaTime;

        if (!leftflapped && leftwingflaptimer <= leftwingThickness / 4f)
        {
            leftwing.Flap(leftwingLength * leftwingWidth); //Flap force proportional to wing area
            leftflapped = true;
        }

        if (leftwingflaptimer <= 0.0f)
        {
            leftwingflaptimer = leftwingThickness;
            leftflapped = false;
        }

        if (!rightflapped && rightwingflaptimer <= rightwingThickness / 4f)
        {
            rightwing.Flap(rightwingLength * rightwingWidth); //Flap force proportional to wing area
            rightflapped = true;
        }

        if (rightflapped && rightwingflaptimer <= 0.0f)
        {
            rightwingflaptimer = rightwingThickness;
            rightflapped = false;
        }

    }
}
