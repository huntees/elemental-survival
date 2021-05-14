using UnityEngine;

public class Difficulty : MonoBehaviour
{
    //Singleton 
    public static Difficulty instance;
    public bool m_isHardMode = false;

    void Awake()
    {
        if (instance != null)
        {
            return;
        }

        DontDestroyOnLoad(gameObject);
        instance = this;
    }
}
