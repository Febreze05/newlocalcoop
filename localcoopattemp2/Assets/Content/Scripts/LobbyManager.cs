using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

namespace GDD4500.LAB01
{
    public class LobbyManager : MonoBehaviour
    {

        #region Fields & References
        private List<PlayerInputHandler> _existingPlayers;        // this just stores currently connected players

        [Header("Lobby Settings")]
        [SerializeField] private string _GameplaySceneName = "Game";   // this just name of gameplay scene to load
        [SerializeField] private Collider _startTrigger;               // this just trigger that players must enter to start
        [SerializeField] private TextMeshPro _playerCountText;         // this just UI element for player count

        private bool _matchStarted = false;                           // this just prevents match from starting multiple times
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            _existingPlayers = PlayerManager.Instance.GetPlayers();    // this just get list of active players from PlayerManager

            // this just subscribe to events for join/remove
            PlayerManager.Instance.OnPlayerJoined += OnPlayerJoined;
            PlayerManager.Instance.OnPlayerRemoved += OnPlayerRemoved;
        }

        private void OnDestroy()
        {
            // this just unsubscribe from events to prevent memory leaks
            PlayerManager.Instance.OnPlayerJoined -= OnPlayerJoined;
            PlayerManager.Instance.OnPlayerRemoved -= OnPlayerRemoved;
        }
        #endregion

        #region Player Event Handlers
        private void OnPlayerJoined(PlayerInputContext ctx)
        {
            _existingPlayers.Add(ctx.Handler);    // this just add new player to list
            Debug.Log($"Player {ctx.Index + 1} joined lobby");
        }

        private void OnPlayerRemoved(PlayerInputContext ctx)
        {
            _existingPlayers.Remove(ctx.Handler); // this just remove player from list
            Debug.Log($"Player {ctx.Index + 1} left lobby");
        }
        #endregion

        #region Update Loop
        private void Update()
        {
            if (_matchStarted) return;   // this just block input if match already started

            // this just count players currently inside trigger zone
            int playersInTrigger = 0;
            foreach (var player in _existingPlayers)
            {
                if (_startTrigger.bounds.Contains(player.transform.position))
                {
                    playersInTrigger++;
                }
            }

            // this just check if all players are in the start trigger
            if (_existingPlayers.Count > 0 && playersInTrigger == _existingPlayers.Count)
            {
                StartCoroutine(StartMatch());
            }

            UpdatePlayerCountUI(playersInTrigger);
        }
        #endregion

        #region UI Updates
        private void UpdatePlayerCountUI(int playersInTrigger)
        {
            // this just update text to show players inside trigger
            _playerCountText.text = $"Players in Start: {playersInTrigger}/{_existingPlayers.Count}";
        }
        #endregion

        #region Match Flow
        private IEnumerator StartMatch()
        {
            PlayerManager.Instance.StartMatch();   // this just start player input maps
            Debug.Log("Match started");
            _matchStarted = true;

            yield return new WaitForSeconds(1f);   // this just delay before scene load

            SceneManager.LoadScene(_GameplaySceneName);  // this just load gameplay scene
        }
        #endregion
    }
}
