using GDD4500.LAB01;
using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
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

    // Update is called once per frame
    void Update()
    {
        Scored();
    }
}
