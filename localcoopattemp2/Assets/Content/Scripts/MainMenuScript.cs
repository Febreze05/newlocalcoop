using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuScript : MonoBehaviour
{
    #region Variables
    private Button startButton;
    private Button exitButton;
    public GameObject pausePanel;
    #endregion

    #region Unity Methods
    void OnEnable()
    {
        // this is just getting the UIDocument component
        var uiDocument = gameObject.GetComponent<UIDocument>();

        // this is just getting the root UI element
        var root = uiDocument.rootVisualElement;

        // this is just getting the "Start" button by name
        startButton = root.Q<Button>("Start");

        // this is just adding click event listener
        startButton.clicked += OnStartButtonClick;

        // this is just getting the "exit" button by name
        exitButton = root.Q<Button>("Exit");

        // this is just adding click event listener
        exitButton.clicked += OnExitButtonClick;
    }

    void OnDisable()
    {
        // this is just unsubscribing event to prevent memory leaks
        startButton.clicked -= OnStartButtonClick;
        exitButton.clicked -= OnExitButtonClick;
    }
    #endregion

    #region Button Logic
    void OnStartButtonClick()
    {
        // this is just loading the game scene
        SceneManager.LoadScene("LobbyRoom");
    }

    // this is just exiting the the application
    void OnExitButtonClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stops play mode in the editor
#else
            Application.Quit();
#endif
    }
    #endregion

}
