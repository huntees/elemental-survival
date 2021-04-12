using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;

    [SerializeField] float smoothSpeed = 10f;
    [SerializeField] Vector3 offset = new Vector3(0, 6, -8);

    private Transform myTransform;

    // Start is called before the first frame update
    void Start()
    {
        myTransform = transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        myTransform.position = Vector3.Lerp(myTransform.position, target.position + offset, smoothSpeed * Time.deltaTime);

        //myTransform.LookAt(target);
    }
}
