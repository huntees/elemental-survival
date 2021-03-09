using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils 
{

    public static Vector3 GetCursorPosition()
    {
        Ray m_mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit m_mouseRayHit;
        if (Physics.Raycast(m_mouseRay, out m_mouseRayHit))
        {
            // y omitted other projectile goes up/down and similarly to character rotation
            return new Vector3(m_mouseRayHit.point.x, 0, m_mouseRayHit.point.z);
        }
        else
        {
            return new Vector3(0,0,0);
        }
    }
}
