using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeRotation : MonoBehaviour
{
    private Quaternion m_rotation;

    void Awake()
    {
        m_rotation = transform.rotation;
    }

    void FixedUpdate()
    {
        transform.rotation = m_rotation;
    }
}
