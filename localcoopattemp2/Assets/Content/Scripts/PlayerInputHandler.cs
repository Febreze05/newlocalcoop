using UnityEngine;
using UnityEngine.InputSystem;

namespace GDD4500.LAB01
{
    public class PlayerInputHandler : MonoBehaviour
    {
        #region FIELDS & REFERENCES
        [Header("Materials")]
        [SerializeField] private Material[] _PlayerMaterials;  // this just holds different materials for each player
        [SerializeField] private Material _GhostMaterial;      // this just optional material for ghost/placeholder players

        private InputAction _move;                            // this just reference to player's movement action
        private bool _initialized;                            // this just flag to prevent using before setup
        private PlayerMoveMechanic _moveMechanic;             // this just reference to movement script
        private PlayerInputContext _context;                  // this just stores info about this player's input/user context
        #endregion

        #region INITIALIZATION
        public void Initialize(PlayerInputContext context)
        {
            _context = context;                               // this just assign context
            this.name = $"Player {context.Index + 1}";        // this just rename GameObject for clarity

            // this just enforce binding mask to only this scheme (extra safety)
            context.Actions.bindingMask = InputBinding.MaskByGroup(context.SchemeName);

            // this just find the "Gameplay" action map inside the cloned asset
            var gameplayMap = context.Actions.FindActionMap("Gameplay", true);

            // this just store move action
            _move = gameplayMap.FindAction("Move", true);

            _initialized = true;                              // this just mark as initialized

            Debug.Log($"[Handler] Scheme = {context.SchemeName}");

            // this just setup reference to movement script
            _moveMechanic = GetComponent<PlayerMoveMechanic>();
            if (_moveMechanic != null)
                _moveMechanic.Initialize(this);

            // this just make player persist across scenes
            DontDestroyOnLoad(this.gameObject);

            // this just set material based on player index
            MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
            foreach (var meshRenderer in meshRenderers)
            {
                meshRenderer.material = _PlayerMaterials[_context.Index];
            }
        }
        #endregion

        #region UPDATE LOOP
        private void Update()
        {
            if (!_initialized) return;                        // this just ensure player is ready before reading inputs

            // this just read movement vector (from input system)
            Vector2 move = _move.ReadValue<Vector2>();

            // this just forward input to movement script if active
            if (_moveMechanic != null && move.magnitude > 0)
            {
                _moveMechanic.DoMove(move);
            }
        }
        #endregion
    }
}
