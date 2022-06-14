using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class Controller : MonoBehaviour{

    [SerializeField] Rigidbody mybody;
    [SerializeField] Animator animator;
    [SerializeField] CapsuleCollider capsuleCollider;
    [SerializeField] Transform groundCheck;
    [SerializeField] Camera camera;


    [SerializeField] WayPoints wayPoints;
    [SerializeField] Transform currentWaypoint;
    [SerializeField] GameObject brickPrefab;

    [SerializeField] float moveSpeed;
    [SerializeField] float gravitySpeed;
    [SerializeField] float distanceThreshold =0.1f;

    [SerializeField] int wayPointIndex;

    private Vector3 oldPos;

    void Start(){
        mybody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        camera = GameObject.Find("Main Camera").GetComponent<Camera>();

        currentWaypoint = wayPoints.GetNextWaypoint(currentWaypoint);
        transform.position = currentWaypoint.position;

        currentWaypoint = wayPoints.GetNextWaypoint(currentWaypoint);
    }
    void FixedUpdate(){
        Move(moveSpeed);
        UpdateAnimator();
    }

    void LateUpdate(){
        oldPos = transform.position;
    }
    public bool isGrounded(){
        float radius = capsuleCollider.radius * 0.9f;
        Vector3 pos = groundCheck.transform.position;
        LayerMask groundLayer = LayerMask.GetMask("ground");

        return Physics.CheckSphere(pos, radius, groundLayer);
    }
    void Move(float speed){
        if(!isGrounded()){
            Physics.gravity = new Vector3(0, -gravitySpeed, 0);
        }

        if(currentWaypoint != null){
            Vector3 DirMovement = new Vector3(currentWaypoint.position.x, currentWaypoint.position.y, currentWaypoint.position.z);
            transform.position = Vector3.MoveTowards(transform.position, DirMovement, speed * Time.deltaTime);

            if(Vector3.Distance(transform.position, currentWaypoint.position) < distanceThreshold){
                currentWaypoint = wayPoints.GetNextWaypoint(currentWaypoint);
            }


            Vector3 dir = new Vector3(transform.position.x, 0, transform.position.z);
            if(dir == Vector3.zero){
                return;
            }

            Vector3 direction = (transform.position - oldPos).normalized;
            transform.rotation = Quaternion.LookRotation(direction);
        }
        else{
            Debug.Log(currentWaypoint + "is null");
        }
    }

    void UpdateAnimator(){
        animator.SetBool("IsGrounded", isGrounded());
        animator.SetFloat("VerticalVelocity", mybody.velocity.y);
    }


    private void OnTriggerEnter(Collider other){
        if(other.gameObject.tag == "Transition"){
            Quaternion newRot = Quaternion.Euler(23.043f, -130.792f, 0);
            camera.transform.rotation = Quaternion.Slerp(camera.transform.rotation, newRot, moveSpeed);

            camera.gameObject.GetComponent<CameraRig>().offsetZ = 6.61f;
            camera.gameObject.GetComponent<CameraRig>().offsetY = 14.5f;
            camera.gameObject.GetComponent<CameraRig>().offsetX = 10.01f;


            Debug.Log("Enter");
        }
        else if(other.gameObject.tag == "TransitionTwo"){
            Quaternion newRot = Quaternion.Euler(23.043f, -86.0170f, 0);
            camera.transform.rotation = Quaternion.Slerp(camera.transform.rotation, newRot, moveSpeed);

            camera.gameObject.GetComponent<CameraRig>().offsetZ = -1.3f;
            camera.gameObject.GetComponent<CameraRig>().offsetY = 14.9f;
            camera.gameObject.GetComponent<CameraRig>().offsetX = 11.31f;
        }
        if(other.gameObject.tag == "Bricks"){
            GameObject Instance = (GameObject)Instantiate(brickPrefab, other.transform.position, Quaternion.identity) as GameObject;
            Destroy(other.gameObject);
            animator.enabled = false;
            gameObject.GetComponent<Controller>().enabled = false;
            gameObject.GetComponent<GrappleController>().enabled = false;

            StartCoroutine(RestartTimer());

        }
    }

    IEnumerator RestartTimer(){
        yield return new WaitForSeconds(0.9f);
        SceneManager.LoadScene(0);
    }
}
