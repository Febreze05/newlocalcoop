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
    int [] setupScoring = {0,0};
    [SerializeField] private Collider[] playerGoals;
    [SerializeField] private GameObject ballPrefab;
    private Rigidbody ballRb;
    Vector3 ballStart = Vector3.zero;
    [SerializeField] private GameObject[] playerStarts;
    private List<PlayerInputHandler> existingPlayers;
    public GameObject ScorePanel;
    private Label[] PlayerScores;
    private string[] PlayerTitlenames = {"Player2Score", "Player1Score" };




    private void ResetGame()
    {
        ballRb.linearVelocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;
        ballPrefab.transform.position = ballStart;
        existingPlayers = PlayerManager.Instance.GetPlayers();
        int playersInLevel = 0;
        foreach (var player in existingPlayers)
        {
            player.transform.position = playerStarts[playersInLevel].transform.position;
            playersInLevel++;
        }
    }

    void Scored()
    {
        for (int i = 0; i < playerGoals.Length; i++)
        {
            if (playerGoals[i].bounds.Contains(ballPrefab.transform.position))
            {
                ResetGame();
                setupScoring[i]++;
                Debug.Log(setupScoring[i]);
                PlayerScores[i].text = setupScoring[i].ToString();
                if (setupScoring[i] >= 7)
                {
                    var root = GetComponent<UIDocument>().rootVisualElement;
                    var basicmenu = root.Q<VisualElement>(className: "scoreboard");

                    if (basicmenu != null)
                    {
                        existingPlayers = PlayerManager.Instance.GetPlayers();
                    
                        StartCoroutine(FadeOutAndDisable(basicmenu, 2.0f));
                    }
                    
                }
            }
        }
        
    }
 




// Start is called once before the first execution of Update after the MonoBehaviour is created
void Start()
    {
        PlayerScores = new Label[playerGoals.Length];
        ballStart = ballPrefab.transform.position;
        ballRb = ballPrefab.GetComponent<Rigidbody>();
        ResetGame();
        // this is just getting the UIDocument component
        var uiDocument = GetComponent<UIDocument>();

        // this is just getting the root UI element
        var root = uiDocument.rootVisualElement;

        var styleSheet = Resources.Load<StyleSheet>("StyleSheets/PanelAnimation");
        if (styleSheet != null && !root.styleSheets.Contains(styleSheet))
            root.styleSheets.Add(styleSheet);

        // Animate any VisualElement with the "Basicbook" class
        var basicMenu = root.Q<VisualElement>(className: "scoreboard");
        if (basicMenu != null)
        {
            basicMenu.SetEnabled(false); // Start disabled (opacity 0)
            basicMenu.AddToClassList("scoreboard"); // Ensure class is applied
            StartCoroutine(EnableWithDelay(basicMenu, 0.05f)); // Fade in
        }

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

    IEnumerator EnableWithDelay(VisualElement element, float delay)
    {
        yield return new WaitForSeconds(delay);
        element.SetEnabled(true);
    }

    IEnumerator FadeOutAndDisable(VisualElement element, float duration = 0.3f)
    {
        element.SetEnabled(false);
        yield return new WaitForSeconds(duration);
        Debug.Log("Settings");
        SceneManager.LoadScene("LobbyRoom");

    }





    // Update is called once per frame
    void Update()
    {
        Scored();
    }








}
