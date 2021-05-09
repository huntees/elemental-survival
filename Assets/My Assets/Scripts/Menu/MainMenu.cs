using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject m_mainMenuObject;
    [SerializeField] private GameObject m_controlsObject;
    [SerializeField] private GameObject m_difficultyObject;

    void Start()
    {
        if (Difficulty.instance == null)
        {
            Instantiate(m_difficultyObject);
        }
    }

    public void PlayGame()
    {
        Difficulty.instance.m_isHardMode = false;
        SceneManager.LoadScene(1);
    }

    public void PlayHardMode()
    {
        Difficulty.instance.m_isHardMode = true;
        SceneManager.LoadScene(1);
    }

    public void ShowControls()
    {
        m_mainMenuObject.SetActive(false);
        m_controlsObject.SetActive(true);
    }

    public void ShowMainMenu()
    {
        m_controlsObject.SetActive(false);
        m_mainMenuObject.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void SwitchToMainMenuScene()
    {
        SceneManager.LoadScene(0);
    }
}
