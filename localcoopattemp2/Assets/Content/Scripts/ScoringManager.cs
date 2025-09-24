using GDD4500.LAB01;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class ScoringManager : MonoBehaviour
{
    #region Variables
    // this just keeps track of each player's score
    int[] setupScoring = { 0, 0 };

    // this just stores colliders for each player's goal zone
    [SerializeField] private Collider[] playerGoals;

    // this just references the ball prefab in the game
    [SerializeField] private GameObject ballPrefab;

    // this just holds the ball’s rigidbody for physics
    private Rigidbody ballRb;

    // this just defines where the ball starts/reset point
    private Vector3 ballStart = Vector3.zero;

    // this just stores the starting positions for each player
    [SerializeField] private GameObject[] playerStarts;

    // this just keeps a list of all players currently in the game
    private List<PlayerInputHandler> existingPlayers;

    // this just references the UI panel for displaying scores
    public GameObject ScorePanel;

    // this just stores references to the score labels for each player
    private Label[] PlayerScores;

    // this just holds the UI element names for each player's score label
    private string[] PlayerTitlenames = { "Player2Score", "Player1Score" };
    #endregion

    #region Game Logic
    private void ResetGame()
    {
        // this just resets the ball’s velocity and rotation
        ballRb.linearVelocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;

        // this just moves the ball back to its start position
        ballPrefab.transform.position = ballStart;

        // this just refreshes the player list
        existingPlayers = PlayerManager.Instance.GetPlayers();

        // this just resets players to their starting positions
        int playersInLevel = 0;
        foreach (var player in existingPlayers)
        {
            player.transform.position = playerStarts[playersInLevel].transform.position;
            playersInLevel++;
        }
    }

    private void Scored()
    {
        // this just checks if the ball has entered a player’s goal
        for (int i = 0; i < playerGoals.Length; i++)
        {
            if (playerGoals[i].bounds.Contains(ballPrefab.transform.position))
            {
                // this just resets the game state
                ResetGame();

                // this just increments the player’s score
                setupScoring[i]++;
                Debug.Log(setupScoring[i]);

                // this just updates the score label in the UI
                PlayerScores[i].text = setupScoring[i].ToString();

                // this just checks if the player has reached the winning score
                if (setupScoring[i] >= 7)
                {
                    var root = GetComponent<UIDocument>().rootVisualElement;
                    var basicmenu = root.Q<VisualElement>(className: "scoreboard");

                    if (basicmenu != null)
                    {
                        // this just ensures player list is up-to-date
                        existingPlayers = PlayerManager.Instance.GetPlayers();

                        // this just fades out and ends the match
                        StartCoroutine(FadeOutAndDisable(basicmenu, 2.0f));
                    }
                }
            }
        }
    }
    #endregion

    #region Unity Methods
    private void Start()
    {
        // this just sets up score labels
        PlayerScores = new Label[playerGoals.Length];

        // this just saves the ball start position
        ballStart = ballPrefab.transform.position;

        // this just gets the rigidbody for ball physics
        ballRb = ballPrefab.GetComponent<Rigidbody>();

        // this just resets the game state at the start
        ResetGame();

        // this just gets the UIDocument component
        var uiDocument = GetComponent<UIDocument>();

        // this just gets the root UI element
        var root = uiDocument.rootVisualElement;

        // this just loads a UI stylesheet for animations
        var styleSheet = Resources.Load<StyleSheet>("StyleSheets/PanelAnimation");
        if (styleSheet != null && !root.styleSheets.Contains(styleSheet))
            root.styleSheets.Add(styleSheet);

        // this just finds and configures the scoreboard panel
        var basicMenu = root.Q<VisualElement>(className: "scoreboard");
        if (basicMenu != null)
        {
            basicMenu.SetEnabled(false); // this just starts disabled
            basicMenu.AddToClassList("scoreboard"); // this just ensures styling is applied
            StartCoroutine(EnableWithDelay(basicMenu, 0.05f)); // this just fades it in
        }

        // this just links score labels in the UI to each player
        for (int i = 0; i < playerGoals.Length; i++)
        {
            Debug.Log(PlayerTitlenames[i]);
            PlayerScores[i] = root.Q<Label>(PlayerTitlenames[i]);
            if (PlayerScores[i] == null)
            {
                Debug.LogWarning($"Label with name '{PlayerTitlenames[i]}' not found in UI.");
            }
        }
    }

    private void Update()
    {
        // this just constantly checks if a score has occurred
        Scored();
    }
    #endregion

    #region Coroutines
    private IEnumerator EnableWithDelay(VisualElement element, float delay)
    {
        // this just waits for a small delay before enabling the UI
        yield return new WaitForSeconds(delay);
        element.SetEnabled(true);
    }

    private IEnumerator FadeOutAndDisable(VisualElement element, float duration = 0.3f)
    {
        // this just disables the element (hides scoreboard)
        element.SetEnabled(false);

        // this just waits for a fade duration
        yield return new WaitForSeconds(duration);

        Debug.Log("Settings");

        // this just loads the LobbyRoom scene after fade out
        SceneManager.LoadScene("LobbyRoom");
    }
    #endregion
}
