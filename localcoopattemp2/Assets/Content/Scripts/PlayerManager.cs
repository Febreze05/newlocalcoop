//This script was generated with ChatGPT 5 by Alex Johnstone

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace GDD4500.LAB01
{
    public class PlayerManager : MonoBehaviour
    {
        #region Singleton
        // this just holds the single instance of PlayerManager
        public static PlayerManager Instance;
        #endregion

        #region Events
        // this just fires when a player joins
        public Action<PlayerInputContext> OnPlayerJoined;

        // this just fires when a player is removed
        public Action<PlayerInputContext> OnPlayerRemoved;
        #endregion

        #region Inspector Settings
        [Header("Asset & Maps")]
        [SerializeField] private InputActionAsset sharedAsset; // this just stores your .inputactions
        [SerializeField] private string lobbyMapName = "Lobby";
        [SerializeField] private string gameplayMapName = "Gameplay";
        [Space]
        [SerializeField] private string joinActionName = "Join";
        [SerializeField] private string exitActionName = "Exit";

        [Header("Spawning")]
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private int maxPlayers = 4;

        [Header("Schemes")]
        [SerializeField] private string gamepadSchemeName = "Gamepad";
        [SerializeField] private string[] keyboardSchemeNames = { "Keyboard Left", "Keyboard Right" };

        [Header("Activation")]
        [SerializeField] private bool enableGameplayOnJoin = false; // this just decides if players instantly control
        #endregion

        #region Private Fields
        // this just tracks all players in the match
        private readonly List<PlayerInputContext> _players = new();

        // this just prevents two players from using the same keyboard scheme
        private readonly HashSet<string> _claimedKBMSchemes = new();

        // this just references input actions for joining/exiting
        private InputAction _joinAction;
        private InputAction _exitAction;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            // this just enforces Singleton pattern
            if (Instance != null)
            {
                Destroy(this.gameObject);
                return;
            }

            Instance = this;

            // this just keeps the object alive across scenes
            DontDestroyOnLoad(this.gameObject);
        }

        private void OnEnable()
        {
            // this just finds the lobby input map
            var lobby = sharedAsset.FindActionMap(lobbyMapName, true);

            // this just gets the Join action
            _joinAction = lobby.FindAction(joinActionName, true);
            _joinAction.performed += OnJoinPerformed;
            _joinAction.Enable();

            // this just gets the Exit action
            _exitAction = lobby.FindAction(exitActionName, true);
            _exitAction.performed += OnExitPerformed;
            _exitAction.Enable();
        }

        private void OnDisable()
        {
            // this just cleans up join action
            if (_joinAction != null)
            {
                _joinAction.performed -= OnJoinPerformed;
                _joinAction.Disable();
            }

            // this just cleans up exit action
            if (_exitAction != null)
            {
                _exitAction.performed -= OnExitPerformed;
                _exitAction.Disable();
            }
        }
        #endregion

        #region Exit Handling
        private void OnExitPerformed(InputAction.CallbackContext ctx)
        {
            var control = ctx.control;
            if (control == null) return;

            var device = control.device;

            // this just finds which player the device belongs to
            var player = _players.FirstOrDefault(p => p.User.pairedDevices.Contains(device));
            if (player == null)
            {
                Debug.Log("[Lobby] Exit pressed but no matching player found.");
                return;
            }

            RemovePlayer(player);
        }

        private void RemovePlayer(PlayerInputContext player)
        {
            // this just unpairs devices
            player.User.UnpairDevicesAndRemoveUser();

            // this just releases keyboard scheme if used
            if (IsKBMScheme(player.SchemeName))
            {
                _claimedKBMSchemes.Remove(player.SchemeName);
            }

            // this just destroys the player's action asset
            if (player.Actions != null)
            {
                Destroy(player.Actions);
            }

            // this just destroys the player's GameObject
            if (player.Handler != null && player.Handler.gameObject != null)
            {
                Destroy(player.Handler.gameObject);
            }

            // this just removes the player from list
            _players.Remove(player);

            Debug.Log($"[Lobby] Player {player.Index + 1} removed.");

            // this just reindexes all players so order stays correct
            for (int i = 0; i < _players.Count; i++)
            {
                _players[i].Index = i;
            }

            OnPlayerRemoved?.Invoke(player);
        }
        #endregion

        #region Join Handling
        private void OnJoinPerformed(InputAction.CallbackContext ctx)
        {
            if (_players.Count >= maxPlayers) return;

            var control = ctx.control;
            if (control == null) return;

            var device = control.device;

            // this just determines binding group and scheme
            string bindingGroup = ResolveBindingGroup(_joinAction, control);
            string schemeToUse = ChooseScheme(bindingGroup, device);
            if (schemeToUse == null)
            {
                Debug.Log($"[Lobby] No valid scheme for {device} (bindingGroup={bindingGroup}).");
                return;
            }

            // this just prevents duplicate gamepad use
            if (device is Gamepad && IsDevicePaired(device))
            {
                Debug.Log($"[Lobby] {device.displayName} is already taken.");
                return;
            }

            // this just prevents duplicate keyboard scheme use
            if (IsKBMScheme(schemeToUse) && _claimedKBMSchemes.Contains(schemeToUse))
            {
                Debug.Log("[Lobby] Keyboard scheme taken.");
                return;
            }

            // this just builds a list of devices to pair
            var toPair = new List<InputDevice>();
            if (device is Gamepad gp)
            {
                toPair.Add(gp);
            }
            else if (device is Keyboard || device is Mouse)
            {
                if (Keyboard.current != null) toPair.Add(Keyboard.current);
            }
            else
            {
                Debug.Log($"[Lobby] Ignoring unsupported device {device?.GetType().Name}.");
                return;
            }

            CreatePlayer(toPair, schemeToUse);
        }
        #endregion

        #region Player Creation
        private void CreatePlayer(List<InputDevice> devices, string scheme)
        {
            // this just makes a per-player copy of the input asset
            var perPlayer = Instantiate(sharedAsset);

            // this just creates a new InputUser and pairs devices
            var user = InputUser.PerformPairingWithDevice(devices[0]);
            for (int i = 1; i < devices.Count; i++)
                InputUser.PerformPairingWithDevice(devices[i], user);

            // this just associates actions with the user
            user.AssociateActionsWithUser(perPlayer);

            // this just activates the selected control scheme
            user.ActivateControlScheme(scheme);

            // this just enforces binding mask explicitly
            perPlayer.bindingMask = InputBinding.MaskByGroup(scheme);

            // this just enables gameplay map if auto-start enabled
            if (enableGameplayOnJoin)
            {
                var gm = perPlayer.FindActionMap(gameplayMapName, true);
                gm.Enable();
            }

            // this just spawns the player prefab
            var go = Instantiate(playerPrefab);
            var handler = go.GetComponent<PlayerInputHandler>();

            // this just creates context for the new player
            var ctx = new PlayerInputContext
            {
                Index = _players.Count,
                SchemeName = scheme,
                User = user,
                Actions = perPlayer,
                Handler = handler
            };
            _players.Add(ctx);

            // this just marks KBM scheme as claimed
            if (IsKBMScheme(scheme))
            {
                _claimedKBMSchemes.Add(scheme);
            }

            // this just initializes the handler if present
            if (handler != null) handler.Initialize(ctx);

            OnPlayerJoined?.Invoke(ctx);

            Debug.Log($"[Lobby] Player {ctx.Index + 1} joined with scheme '{scheme}' and devices: {string.Join(", ", devices.Select(d => d.displayName))}");
        }
        #endregion

        #region Match/Lobby Management
        public void StartMatch()
        {
            // this just enables gameplay map for all players
            foreach (var p in _players)
            {
                var map = p.Actions.FindActionMap(gameplayMapName, true);
                map.Enable();
            }
            Debug.Log("[Lobby] Match started.");
        }

        public void StartLobby()
        {
            // this just disables gameplay map for all players
            foreach (var p in _players)
            {
                var map = p.Actions.FindActionMap(lobbyMapName, true);
                map.Disable();
            }
            Debug.Log("[Lobby] Lobby started.");
        }
        #endregion

        #region Utilities
        private bool IsDevicePaired(InputDevice device)
        {
            // this just checks if the device is already paired
            foreach (var p in _players)
                if (p.User.pairedDevices.Contains(device)) return true;
            return false;
        }

        private static bool IsKBMScheme(string scheme)
            => scheme != null && scheme.StartsWith("Keyboard");

        private string ChooseScheme(string bindingGroup, InputDevice device)
        {
            // this just prefers binding group if it exists
            if (!string.IsNullOrEmpty(bindingGroup) &&
                sharedAsset.controlSchemes.Any(cs => cs.name == bindingGroup))
            {
                return bindingGroup;
            }

            // this just infers scheme by device type
            if (device is Gamepad) return gamepadSchemeName;

            if (device is Keyboard || device is Mouse)
            {
                foreach (var s in keyboardSchemeNames)
                    if (!_claimedKBMSchemes.Contains(s)) return s;
            }

            return null;
        }

        private static string ResolveBindingGroup(InputAction action, InputControl triggeringControl)
        {
            // this just finds which binding group matches the triggering control
            var bindings = action.bindings;
            for (int i = 0; i < bindings.Count; i++)
            {
                var b = bindings[i];
                if (b.isComposite || b.isPartOfComposite) continue;

                var path = b.path;
                if (string.IsNullOrEmpty(path)) continue;

                if (InputControlPath.Matches(path, triggeringControl))
                {
                    var groups = b.groups;
                    if (string.IsNullOrEmpty(groups)) return null;

                    var semi = groups.IndexOf(';');
                    return semi >= 0 ? groups.Substring(0, semi) : groups;
                }
            }
            return null;
        }

        public List<PlayerInputHandler> GetPlayers()
        {
            // this just returns the player handlers list
            return _players.Select(p => p.Handler).ToList();
        }
        #endregion
    }
}

