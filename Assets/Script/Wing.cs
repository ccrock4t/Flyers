using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wing : MonoBehaviour
{
    public Rigidbody parentRB;
    public Flyer parentFlyer;
    float baseFlapPeriod = 0.2f;
    public float wingThickness = 0.0f, animtimer = 0.0f;
    Vector3 startpos, endpos;
    Quaternion startrot, endrot;

    // Start is called before the first frame update
    void Start()
    {
        parentRB = this.transform.parent.GetComponent<Rigidbody>();
        parentFlyer = this.transform.parent.GetComponent<Flyer>();
        startrot = Quaternion.Euler(0, this.transform.localRotation.eulerAngles.y, 30f); endrot = Quaternion.Euler(0, this.transform.localRotation.eulerAngles.y, -30f);
    }

    // Update is called once per frame
    void Update()
    {
        if (parentFlyer.dead)
        {
            return;
        }
        animtimer += Time.deltaTime;
        if (animtimer >= this.GetFlapPeriod())
        {
            //reset
            animtimer = 0.0f;
        }
        else if(animtimer >= this.GetFlapPeriod() / 2f)
        {
            //flap down animation
            transform.localRotation = Quaternion.Lerp(startrot, endrot, (animtimer % (this.GetFlapPeriod() / 2f) / (this.GetFlapPeriod() / 2f)) * 1.0f);
        }
        else
        {
            //flap up animation
            transform.localRotation = Quaternion.Lerp(endrot, startrot, (animtimer % (this.GetFlapPeriod() / 2f) / (this.GetFlapPeriod() / 2f)) * 1.0f);
        }

    }

    public float GetFlapPeriod()
    {
        return (baseFlapPeriod / (wingThickness/5)) + 0.1f;
    }

    public void Flap(float force)
    {
        float totalforce = force*8f + 50f;
        parentRB.AddRelativeForce(new Vector3(0,totalforce));
    }
}
