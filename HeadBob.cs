using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBob : MonoBehaviour
{
    public float bobbingSpeed = 0.01f;
    public float bobbingAmount = 0.05f;
    public float midpoint = 1.8f;

    private float timer = 0.0f;

    void Update()
    {

        if(Input.GetKey(KeyCode.LeftShift)){
            bobbingAmount = 0.3f;
            bobbingSpeed = 0.1f;
        }else{
            bobbingAmount = 0.15f;
            bobbingSpeed = 0.08f;
        }
        float waveslice = 0.0f;
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0)
        {
            timer = 0.0f;
        }
        else
        {
            waveslice = Mathf.Sin(timer);
            timer = timer + bobbingSpeed;
            if (timer > Mathf.PI * 2)
            {
                timer = timer - (Mathf.PI * 2);
            }
        }
        if (waveslice != 0)
        {
            float translateChange = waveslice * bobbingAmount;
            float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
            totalAxes = Mathf.Clamp(totalAxes, 0.0f, 1.0f);
            translateChange = totalAxes * translateChange;
            transform.localPosition = new Vector3(transform.localPosition.x, midpoint + translateChange, transform.localPosition.z);
        }
        else
        {
            transform.localPosition = new Vector3(transform.localPosition.x, midpoint, transform.localPosition.z);
        }
    }
}