using UnityEngine;
using UnityEngine.InputSystem;

namespace GDD4500.LAB01
{
    public class PlayerInputHandler : MonoBehaviour
    {
        [SerializeField] private Material[] _PlayerMaterials;
        [SerializeField] private Material _GhostMaterial;
        private InputAction _move;

        private bool _initialized;

        private PlayerMoveMechanic _moveMechanic;
        private PlayerInputContext _context;


        public void Initialize(PlayerInputContext context)
        {
            _context = context;
            this.name = $"Player {context.Index + 1}";
            
            // Enforce listening only to this scheme (extra safety; the manager already did ActivateControlScheme).
            context.Actions.bindingMask = InputBinding.MaskByGroup(context.SchemeName);
            var gameplayMap = context.Actions.FindActionMap("Gameplay", true);

            //Store the actions 
            _move = gameplayMap.FindAction("Move", true);


            _initialized = true;

            // Enable if not already enabled by the manager.
            // gameplay.Enable();

            Debug.Log($"[Handler] Scheme = {context.SchemeName}");

            // Grab and init move
            _moveMechanic = GetComponent<PlayerMoveMechanic>();
            if (_moveMechanic != null) _moveMechanic.Initialize(this);


            // Make the player persist between scenes
            DontDestroyOnLoad(this.gameObject);

            MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
            foreach (var meshRenderer in meshRenderers)
            {
                meshRenderer.material = _PlayerMaterials[_context.Index];
            }
        }

        private void Update()
        {
            if (!_initialized) return;

            // Reads the move value (of Vector 2 type)
            Vector2 move = _move.ReadValue<Vector2>();

            // Reading the magnitude is relatively expensive so this logic should be optimized
            if (_moveMechanic != null && move.magnitude > 0)
            {
                _moveMechanic.DoMove(move);
            }
        }
    }
}
