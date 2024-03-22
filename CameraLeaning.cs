using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lean : MonoBehaviour
{
    //Attach this script to your player object

    //The Camera Parent's Animator variable
    public Animator cameraAnim;

    //The LayerMask variable which determines what payers the raycast can hit
    public LayerMask layers;

    //The RaycastHit variable collects information from obejcts it hits
    RaycastHit hit;

    void Update()
    {

        if (Input.GetKey(KeyCode.Q) && !Physics.Raycast(transform.position, -transform.right, out hit, 1f, layers))
        {
            //The camera's lean left animation will play
            cameraAnim.ResetTrigger("idle");
            cameraAnim.ResetTrigger("right");
            cameraAnim.SetTrigger("left");
        }

        else if (Input.GetKey(KeyCode.E) && !Physics.Raycast(transform.position, transform.right, out hit, 1f, layers))
        {
            //The camera's lean right animation will play
            cameraAnim.ResetTrigger("idle");
            cameraAnim.ResetTrigger("left");
            cameraAnim.SetTrigger("right");
        }
 
        else
        {
            //The camera's idle animation will play
            cameraAnim.ResetTrigger("right");
            cameraAnim.ResetTrigger("left");
            cameraAnim.SetTrigger("idle");
        }
    }
}