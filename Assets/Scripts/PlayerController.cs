using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Velocity does not work since when colliding with a ceiling the player goes flying forwards because of the collision calculations.
/// I need to use the raycast based movement that I have and modify the gravity vector accordingly.
/// 
/// Update 2: I can use rigidbody, the problem is that the player rotates but does not move with the pannel. I can try to parent the player to the its target pannel that way if the pannel rotates the player will automaticly rotate with it and the gravity changes will do the rest.
/// 
/// Problem 2: The player gets out of the panels Z
/// What I can do is use the player's position and create the orthogonal projection into the pannel (maybe use the pannel's normal) and update is position in the late update once everything was calculated.
/// 
/// Update 3: (30/03)
/// The problem now is that by rotating the pannel the player loses speed. Also when rotating more than -50º in the X-axis the speed gets inverted. The tempVerticalVelocity has an incorrect Z component i.g. UP = (0, 0.6, -0.8) tempVerticalVelocity = (0, -0.6, -0.8) when -0.8 should have been inverted as well.
/// POSSIBLE SOLUTION: https://answers.unity.com/questions/193398/velocity-relative-to-local.html
/// </summary>
public class PlayerController : FreezableObject {

    //Singleton
    public static PlayerController _instance = null;
    public static PlayerController Instance {
        get { return _instance; }
    }

    #region Public & Serialized Variables
    //Inspector Component References
    [SerializeField] private Transform _targetPannel = null;
    [SerializeField] private Transform _playerTransform = null;
    [SerializeField] private BoxCollider _playerCollider = null;
    [SerializeField] private Rigidbody _playerRigid = null;
    [SerializeField] private Animator _playerAnimator = null;
    [SerializeField] private ParentingFollow _parentingScript = null;
    [SerializeField] private SpriteRenderer _spriteRenderer = null;

    //Movement
    [SerializeField] private float _movementSpeed = 1f;
    [SerializeField] private float _transitionSpeed = 1f;
    [SerializeField] private float _jumpForce = 10f;  //Force applied when 
    [SerializeField] private float _maxFallVelocity = 5f; //Maximum Y Speed value when falling.
    [SerializeField] private LayerMask _floorLayer = 0;
    [SerializeField] private Vector3 _boxCastSize = new Vector3(0.17f, 0.02f, 0.05f);
    [SerializeField] private float _maxRayDistance = 0.125f;

    //Pannel Transition Temporal
    [System.Serializable]
    public struct DoorConnection
    {
        public Transform _leftDoor, _rightDoor;
    }

    public DoorConnection _currentConnection;
    #endregion

    #region Getters & Setters
    public Transform TargetPannel { 
        get =>_targetPannel;
        set { 
            _targetPannel = value;
            _playerTransform.rotation = _targetPannel.rotation;
        }
    }

    public Transform PlayerTrans {
        get => _playerTransform;
    }

    public Rigidbody PlayerRigid {
        get => _playerRigid;
    }

    public SpriteRenderer Renderer {
        get => _spriteRenderer;
    }

    public float MovementSpeed {
        get => _movementSpeed;
    }

    public float TransitionSpeed {
        get => _transitionSpeed;
    }

    public Vector2 MovementInput { 
        get => _movementInput;
    }

    public bool IsGrounded {
        get => _grounded;
    }
    #endregion

    #region Private Variables
    //Movement
    private Vector3 _gravity = default;
    private Vector3 _cachedVelocity = Vector3.zero;
    private Vector3 _localCachedVelocity = Vector3.zero;

    private bool _jump = false;
    private Vector2 _movementInput = Vector2.zero;
    private bool _grounded = false;
    private RaycastHit _rayHit;
    private Vector3 _boxCastCenter, _boxCastHalfExtends;
    #endregion

    #region Unity Cycle
    private void Awake() {
        _instance = this;
        _gravity = new Vector3(0, -9.8f, 0);
        if (_maxFallVelocity > 0) _maxFallVelocity *= -1;

        //BoxCast parameters
        
        _boxCastHalfExtends = _boxCastSize / 2;
        _maxRayDistance = _boxCastHalfExtends.y + _playerCollider.size.y / 2;
    }

    void Start() {
        //Debug.Log(_targetPannel.up);
    }

    protected override void Update() {
        base.Update();

        ManageGravityAndOrientation();

        //Vector2 desiredMovement = ManageMovementInputs();

        //Vector3 tempVel = _playerTransform.InverseTransformVector(_playerRigid.velocity);
        //tempVel.x = desiredMovement.x * _movementSpeed;
        //if (desiredMovement.y == 1) {
        //    tempVel.y = _jumpForce;
        //}
        //if (tempVel.y < _maxFallVelocity) tempVel.y = _maxFallVelocity;  //Limit Fall Speed

        //_playerRigid.velocity = _playerTransform.TransformVector(tempVel);

        ////if (_isFrozen) {
        ////    _playerRigid.Sleep();
        ////}
    }

    private void FixedUpdate() {
        if (_isFrozen) {
            _playerRigid.Sleep();
        }
    }

    private void OnDestroy() {
        _gravity = new Vector3(0, -9.81f, 0);
    }

    //Draw the BoxCast as a gizmo to show where it currently is testing. Click the Gizmos button to see this
    void OnDrawGizmos() {
        Gizmos.color = Color.red;

        //Check if there has been a hit yet
        if (_grounded) {
            Gizmos.color = Color.green;
            //Draw a Ray forward from GameObject toward the hit
            Gizmos.DrawRay(_boxCastCenter, -_playerTransform.up * _rayHit.distance);
            //Draw a cube that extends to where the hit exists
            Gizmos.DrawWireCube(_boxCastCenter + -_playerTransform.up * _rayHit.distance, _boxCastHalfExtends * 2);
        }
        //If there hasn't been a hit yet, draw the ray at the maximum distance
        else {
            //Draw a Ray forward from GameObject toward the maximum distance
            Gizmos.DrawRay(_boxCastCenter, -_playerTransform.up * _maxRayDistance);
            //Draw a cube at the maximum distance
            Gizmos.DrawWireCube(_boxCastCenter + -_playerTransform.up * _maxRayDistance, _boxCastHalfExtends * 2);
        }
    }
    #endregion

    #region API Methods
    public void FreeMovementUpdate() {
        CheckForGrounded();

        Vector2 desiredMovement = ManageMovementInputs();

        Vector3 tempVel = _playerTransform.InverseTransformVector(_playerRigid.velocity);
        tempVel.x = desiredMovement.x * _movementSpeed;
        if (desiredMovement.y == 1) {
            tempVel.y = _jumpForce;
        }
        if (tempVel.y < _maxFallVelocity) tempVel.y = _maxFallVelocity;  //Limit Fall Speed

        _playerRigid.velocity = _playerTransform.TransformVector(tempVel);
    }

    public void LadderMovementUpdate() {
        Vector2 desiredMovement = _movementInput;
        
        Vector3 tempVel = _playerTransform.InverseTransformVector(_playerRigid.velocity);
        tempVel.x = desiredMovement.x * _movementSpeed * 0.25f;
        tempVel.y = desiredMovement.y * _movementSpeed * 0.25f;

        //Transform variation
        //Vector3 xAxis = _playerTransform.right * desiredMovement.x * (Time.deltaTime * _movementSpeed * 0.25f);
        //Vector3 yAxis = _playerTransform.up * desiredMovement.y * (Time.deltaTime * _movementSpeed * 0.25f);
        //Vector3 newPos = _playerTransform.position + xAxis + yAxis;
        //_playerTransform.position = newPos;

        _playerRigid.velocity = _playerTransform.TransformVector(tempVel);
    }

    public void TransitionMovementUpdate() {
        Vector2 desiredMovement = _movementInput;
        PlayerTransitionManager.Instance.TransitionModeMovement(desiredMovement);

        /* //OLD TEST
        Vector3 tempVel = Vector3.zero;
        if (desiredMovement.x > 0) {
            tempVel = _currentConnection._rightDoor.position - _playerTransform.position;
            //_playerRigid.MovePosition(_currentConnection._rightDoor.position);
        } else if (desiredMovement.x < 0) {
            tempVel = _currentConnection._leftDoor.position - _playerTransform.position;
            //_playerRigid.MovePosition(_currentConnection._leftDoor.position);
        }


        _playerRigid.velocity = tempVel.normalized * _movementSpeed;  //TODO: Try and use this same code but adapt for the use of a Lerp instead of a velocity. Also MovePosition works if kinematic.
        */

    }

    public override void Freeze() {
        Debug.Log("FREEZE");
        base.Freeze();

        _cachedVelocity = _playerRigid.velocity;
        SaveLocalSpeed();
        _parentingScript.ActivateParenting(_targetPannel);
    }

    public override void UnFreeze() {
        Debug.Log("UNFREEZE");
        base.UnFreeze();

        _parentingScript.StopParenting();
        _playerRigid.velocity = RestoreLocalSpeed();
        _cachedVelocity = Vector3.zero;
    }

    /// <summary>
    /// Disables the players movement collision only. In order to go through walls when moving using rigidbody.
    /// Used by ladder movement.
    /// </summary>
    public void DisableCollision() {
        _playerCollider.enabled = false;
    }

    /// <summary>
    /// Enables the players movement collision only. In order to go through walls when moving using rigidbody.
    /// Used by ladder movement.
    /// </summary>
    public void EnableCollision() {
        _playerCollider.enabled = true;
    }

    public void MovementUpdate(Vector2 input2D) {
        _movementInput = input2D;
    }

    public void JumpRequest(UnityEngine.InputSystem.InputAction.CallbackContext context) {
        if(context.performed && PlayerStateManager.Instance.GetCurrentPlayerState == PlayerStateManager.PlayerState.FreeMovement) _jump = true;
    }

    public void MovePosition(Vector3 distanceToMove) {
        //Move player to CurrentPos + distanceToMove
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Checks using a BoxCast whether or not the player is grounded each frame.
    /// </summary>
    private void CheckForGrounded() {
        _boxCastCenter = _playerCollider.center;
        _boxCastCenter = _playerTransform.TransformPoint(_boxCastCenter);
        if (Physics.BoxCast(_boxCastCenter, _boxCastHalfExtends, -_playerTransform.up, out _rayHit, _playerTransform.rotation, _maxRayDistance, _floorLayer)) {
            //Debug.Log("HIT: " + _rayHit.collider.gameObject.name);
            if (_grounded == false) {
                JustGrounded();
            }
            _grounded = true;
        } else { 
            _grounded = false;
        }
    }

    private void ManageGravityAndOrientation() {
        //TODO DANI: If NULL go ask for starting pannel/current pannel to the pannel manager.
        if (_targetPannel != null) {
            if (_playerTransform.rotation != _targetPannel.rotation) _playerTransform.rotation = _targetPannel.rotation;
            _gravity = _targetPannel.TransformVector(new Vector3(0, -9.81f, 0));
        }

        if (_gravity != Physics.gravity) {
            Physics.gravity = _gravity;
        }
    }

    /// <summary>
    /// Manages the horizontal movement as well as the jump action.
    /// </summary>
    /// <returns></returns>
    private Vector2 ManageMovementInputs() {
        Vector2 desiredMovement = Vector2.zero;

        desiredMovement.x = _movementInput.x;

        if (_jump && _grounded) {
            desiredMovement.y = 1;
            _jump = false;
        } else if (_jump && !_grounded) {
            _jump = false;
        }

        return desiredMovement;
    }

    private void SaveLocalSpeed() {
        _localCachedVelocity = _playerTransform.InverseTransformVector(_cachedVelocity);
    }

    private Vector3 RestoreLocalSpeed() {
        return _playerTransform.TransformVector(_localCachedVelocity);
    }

    private void JustGrounded() {
        _playerAnimator.SetTrigger("Landed");
    }
    #endregion
}
