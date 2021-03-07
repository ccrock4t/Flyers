using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wing : MonoBehaviour
{
    public Rigidbody parentRB;
    public float animSpeed = 0.0f, animtimer = 0.0f;
    Vector3 startpos, endpos;
    Quaternion startrot, endrot;

    // Start is called before the first frame update
    void Start()
    {
        parentRB = this.transform.parent.GetComponent<Rigidbody>();
        startrot = Quaternion.Euler(0, this.transform.localRotation.eulerAngles.y, 30f); endrot = Quaternion.Euler(0, this.transform.localRotation.eulerAngles.y, -30f);
    }

    // Update is called once per frame
    void Update()
    {
        animtimer += Time.deltaTime;
        if (animtimer >= animSpeed)
        {
            animtimer = 0.0f;
        }
        else if(animtimer >= animSpeed/2f)
        {
            transform.localRotation = Quaternion.Lerp(startrot, endrot, (animtimer % (animSpeed/2f) / (animSpeed / 2f)) * 1.0f);
        }
        else
        {
            transform.localRotation = Quaternion.Lerp(endrot, startrot, (animtimer % (animSpeed / 2f) / (animSpeed / 2f)) * 1.0f);
        }

    }

    public void Flap(float force)
    {
        this.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, force*200.0f));
    }
}
