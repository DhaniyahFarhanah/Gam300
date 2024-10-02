using ArcadeVehicleController;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] bool m_HideMouseOnStart;
    [SerializeField] bool m_FindBus;
    private int m_MaxNumOfPassengers;
    private Vehicle m_Bus;

    [Header("In Game UI")]
    [SerializeField] TMP_Text m_PassengerStatusText;
    [SerializeField] TMP_Text m_CurrentNumPassengerText;

    [Header("Win Canvas")]
    [SerializeField] GameObject m_WinCanvas;

    [Header("Pause Canvas")]
    [SerializeField] GameObject m_PauseCanvas;
    public bool m_IsPaused;

    [Header("Lose Canvas")]
    [SerializeField] GameObject m_LoseCanvas;

    private AudioSource _AudioSource;
    private bool winOnce = false;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        if (m_FindBus)
            m_Bus = GameObject.FindWithTag("Player").GetComponent<Vehicle>();

        if (m_HideMouseOnStart)
        {
            Cursor.visible = false;
        }
        
        _AudioSource = gameObject.AddComponent<AudioSource>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (m_Bus == null)
            return;

        PassengerUpdate();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }

    void PassengerUpdate()
    {
        if (m_MaxNumOfPassengers == 0)
        {
            m_MaxNumOfPassengers = GameObject.FindGameObjectsWithTag("Passenger").Length;
        }

        m_PassengerStatusText.text = m_Bus.m_DeliveredPassengers.ToString() + "/" + m_MaxNumOfPassengers.ToString() + " Passengers Delivered";
        m_CurrentNumPassengerText.text = m_Bus.m_Passengers.ToString() + " Passengers On Board";

        if(m_Bus.m_DeliveredPassengers == m_MaxNumOfPassengers)
        {
            if(!winOnce)
            {
                Play(m_Bus.GetComponent<BusAudioHandler>().win);
                winOnce = true;
            }
            
            m_WinCanvas.SetActive(true);
            Time.timeScale = 0.5f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;

            if (m_HideMouseOnStart)
            {
                Cursor.visible = false;
            }

        }
    }

    public void PauseGame()
    {
        m_IsPaused = !m_IsPaused;
        m_PauseCanvas.SetActive(m_IsPaused);

        if (m_IsPaused)
        {
            Time.timeScale = 0.0f;
            Cursor.visible = true;
            
        }
        else if (!m_IsPaused)
        {
            Time.timeScale = 1.0f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            
            
        }
        else
        {
            Time.timeScale = 1.0f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            if (m_HideMouseOnStart)
            {
                Cursor.visible = false;
            }
        }
    }

    public void RestartScene()
    {
        int currentScenIndex = SceneManager.GetActiveScene().buildIndex;

        SceneManager.LoadScene(currentScenIndex);
    }

    public void CloseGame()
    {
        Application.Quit();
    }

    public void LoadNextScene()
    {
        int currentScenIndex = SceneManager.GetActiveScene().buildIndex;
    }

    public void LoadChosenSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadChosenSceneByNumber(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void Play(AudioClip clip) {
        _AudioSource.clip = clip;
        _AudioSource.Play();
    }
}
