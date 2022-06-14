using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(Rigidbody))]
public class GrappleController : MonoBehaviour 
{
     private LineRenderer lineRenderer;
    private Rigidbody mybody;
    private Vector3 grapplePoint;

    [SerializeField] LayerMask whatIsGrappleable;
    //[SerializeField] Transform grappleTip, player;
    [SerializeField] GameObject swingArea;
    [SerializeField] Transform bob, webPoint;

    [SerializeField] float maxDistance = 100;
    [SerializeField] float jumpForce, swingForce;
    [SerializeField] float transitionSpeed;
    [SerializeField] bool isHit;
    [SerializeField] public bool isSwinging, jumped;

    private void Awake(){
        lineRenderer = GetComponent<LineRenderer>();
        mybody = GetComponent<Rigidbody>();
    }

    private void Update(){
        HandleInput();
        HandleSwing();
        Jump();
    }

    private void LateUpdate(){
        DrawRope();
    }

    void HandleInput(){
        if(Input.GetMouseButton(0)){
            StartGrapple();
            jumped = true;

        }
        else if (Input.GetMouseButtonUp(0)){
            StopGrapple();
            jumped = false;
        }
    }

    void StartGrapple(){
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, maxDistance, whatIsGrappleable)){
            grapplePoint = hit.point;
            isHit = true;
            if (!isSwinging){
                isSwinging = true;
                gameObject.GetComponent<Animator>().SetBool("IsSwinging", true);
                swingArea.transform.rotation = Quaternion.Euler(new Vector3(0, transform.eulerAngles.y + 90, 0));
                swingArea.SetActive(true);
                lineRenderer.enabled = true;
            }

            Debug.Log("Hit");
        }
    }
    void StopGrapple(){
        isHit = false;
        isSwinging = false;
        jumped = false;
        if(isSwinging != true){
            gameObject.GetComponent<Animator>().SetBool("Swinging", false);
            gameObject.GetComponent<Animator>().SetBool("IsSwinging", false);
        }
    }
    void Jump(){
        if(jumped){
            Vector3 newY = new Vector3(mybody.velocity.x, jumpForce, mybody.velocity.z);
            mybody.velocity = newY;
            jumped = false;
        }
    }
    void DrawRope(){
        if (!isSwinging)
            return;
        lineRenderer.SetPosition(0, webPoint.position);
        lineRenderer.SetPosition(1, grapplePoint);
    }
    void HandleSwing(){
        if(isSwinging){
           // transform.Translate(-transform.forward * swingForce * Time.deltaTime);
            gameObject.GetComponent<Animator>().SetBool("Swinging", true);
        }
        else if (!isSwinging){
            swingArea.SetActive(false);
            lineRenderer.enabled = false;
        }
    }
}
