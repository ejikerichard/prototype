using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour
{
    [SerializeField] Transform target;

    [SerializeField] float smoothTime;
    [SerializeField] float deltaTime;
    [SerializeField] float speed;
    [SerializeField] public float offsetZ, offsetY, offsetX;



    public float Turnspeed = 2.0f;
    public Quaternion offsetRotation;

    private Vector3 VelocityF;

    void Start(){
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void FixedUpdate(){
        FollowTarget();
    }
    void FollowTarget(){
        Vector3 targetNewPos = new Vector3(target.position.x + offsetX, target.position.y + offsetY, target.position.z + offsetZ);
        transform.position = Vector3.SmoothDamp(transform.position, targetNewPos, ref VelocityF, smoothTime, speed, deltaTime);
    }
}
