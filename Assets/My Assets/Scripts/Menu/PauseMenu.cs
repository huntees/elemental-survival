using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject m_pauseMenuObject;
    [SerializeField] private GameObject m_controlsObject;
    [SerializeField] private GameObject m_gameOverObject;
    private bool m_isPaused = false;
    private bool m_isGameOver = false;

    public void TriggerGameOver()
    {
        m_isGameOver = true;

        gameObject.SetActive(true);
        m_pauseMenuObject.SetActive(false);
        m_gameOverObject.SetActive(true);

        Invoke("DelayedPause", 1.7f);
    }

    private void DelayedPause()
    {
        Time.timeScale = 0.0f;
    }

    public void TogglePauseMenu()
    {
        if(m_isGameOver)
        {
            return ;
        }


        m_isPaused = !m_isPaused;

        if(m_isPaused)
        {
            PauseGame();
        } 
        else
        {
            ContinueGame();
        }
    }

    private void PauseGame()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0.0f;
    }

    private void ContinueGame()
    {
        if(m_controlsObject.activeInHierarchy)
        {
            HideControls();
        }

        gameObject.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public void Restart()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(1);
    }

    public void ShowControls()
    {
        m_controlsObject.SetActive(true);
    }

    public void HideControls()
    {
        m_controlsObject.SetActive(false);
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(0);
    }
}
