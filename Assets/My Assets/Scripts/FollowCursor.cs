using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCursor : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //do a raycast from the camera through the screen
        //at the mouse cursor position into the 3d world
        RaycastHit hit;
        Vector3 mousePos = Input.mousePosition;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            transform.position = new Vector3(hit.point.x, 0.5f, hit.point.z);
        }

        //must have hit something
        //set the transform position to the point in 3d space where
        //our raycast intersected something
        
    }
}
