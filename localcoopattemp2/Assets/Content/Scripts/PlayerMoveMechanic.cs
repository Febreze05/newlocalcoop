using UnityEngine;

namespace GDD4500.LAB01
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMoveMechanic : MonoBehaviour
    {
        #region Settings
        [Header("Settings")]

        // this just controls how fast the player accelerates
        [SerializeField] public float _Acceleration = 10;

        // this just controls the maximum speed the player can reach
        [SerializeField] public float _MaxSpeed = 20;

        // this just controls how quickly the player slows down when not moving
        [SerializeField] private float _Deceleration = 0.85f;
        #endregion

        #region Components
        // this just stores a reference to the Rigidbody for physics movement
        private Rigidbody _rigidbody;

        // this just stores the input handler for this player
        private PlayerInputHandler _inputHandler;
        #endregion

        #region Movement Variables
        // this just represents the current movement vector of the player
        private Vector3 _moveVector;
        #endregion

        #region Initialization
        public void Initialize(PlayerInputHandler handler)
        {
            // this just assigns the input handler for this player
            _inputHandler = handler;

            // this just gets the Rigidbody component on this object
            _rigidbody = GetComponent<Rigidbody>();
        }
        #endregion

        #region Movement Methods
        public void DoMove(Vector2 value)
        {
            // this just adds input values to the movement vector
            _moveVector.x += value.x;
            _moveVector.z += value.y;

            // this just clamps the movement vector so speed never exceeds MaxSpeed
            _moveVector = Vector3.ClampMagnitude(_moveVector, _MaxSpeed);

            // this just rotates the player to face the movement direction
            this.transform.forward = _moveVector;
        }
        #endregion

        #region Unity Methods
        private void Update()
        {
            // this just applies deceleration over time to reduce movement
            _moveVector *= _Deceleration;
        }

        private void FixedUpdate()
        {
            // this just ensures the Rigidbody exists before applying physics
            if (_rigidbody == null) return;

            // this just applies a force based on the movement vector and acceleration
            _rigidbody.AddForce(_moveVector * _Acceleration);
        }
        #endregion
    }
}
