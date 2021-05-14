using UnityEngine;

//Only uses for enemy health bar
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
