using UnityEngine;

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

    #region Public Variables
    //Inspector Component References
    public Transform _targetPannel = null;
    public Transform _playersTrans = null;
    public Rigidbody _playerRigid = null;
    public ParentingFollow _parentingScript = null;

    //
    public float _transVelocity = 1f;
    public float _jumpForce = 10f;  //Force applied when 
    public float _maxFallVelocity = 5f; //Maximum Y Speed value when falling.
    #endregion

    #region Private Variables
    //Movement
    private Vector3 _gravity = default;
    private Vector3 _cachedVelocity = Vector3.zero;
    private Vector3 _localCachedVelocity = Vector3.zero;

    private bool _jump = true;
    private Vector2 _movementInput = Vector2.zero;
    #endregion

    #region Unity Cycle
    private void Awake() {
        _gravity = new Vector3(0, -9.8f, 0);
        if (_maxFallVelocity > 0) _maxFallVelocity *= -1;
    }

    void Start() {
        //Debug.Log(_targetPannel.up);
    }

    protected override void Update() {
        base.Update();

        ManageGravityAndOrientation();

        Vector2 desiredMovement = ManageMovementInputs();

        Vector3 tempVel = _playersTrans.InverseTransformVector(_playerRigid.velocity);
        tempVel.x = desiredMovement.x * _transVelocity;
        if (desiredMovement.y == 1) {
            tempVel.y = _jumpForce;
        }
        if (tempVel.y < _maxFallVelocity) tempVel.y = _maxFallVelocity;  //Limit Fall Speed

        _playerRigid.velocity = _playersTrans.TransformVector(tempVel);

        if (_isFrozen) {
            _playerRigid.Sleep();
        }
    }

    private void OnDestroy() {
        _gravity = new Vector3(0, -9.81f, 0);
    }
    #endregion

    #region API Methods
    public override void Freeze() {
        base.Freeze();

        _cachedVelocity = _playerRigid.velocity;
        SaveLocalSpeed();
        _parentingScript.ActivateParenting(_targetPannel);
    }

    public override void UnFreeze() {
        base.UnFreeze();

        _parentingScript.StopParenting();
        _playerRigid.velocity = RestoreLocalSpeed();
        _cachedVelocity = Vector3.zero;
    }

    public void MovementUpdate(Vector2 input2D) {
        _movementInput = input2D;
    }

    public void JumpRequest() {
        _jump = true;
    }

    public void MovePosition(Vector3 distanceToMove) {
        //Move player to CurrentPos + distanceToMove
    }
    #endregion

    #region Private Methods
    private void ManageGravityAndOrientation() {
        if (_targetPannel != null) {
            if (_playersTrans.rotation != _targetPannel.rotation) _playersTrans.rotation = _targetPannel.rotation;
            _gravity = _targetPannel.TransformVector(new Vector3(0, -9.81f, 0));
        }

        if (_gravity != Physics.gravity) {
            Physics.gravity = _gravity;
        }
    }

    /// <summary>
    /// Manages the horizontal movement as well as the jump action.
    /// TODO: USE new INPUT SYSTEM
    /// </summary>
    /// <returns></returns>
    private Vector2 ManageMovementInputs() {
        Vector2 desiredMovement = Vector2.zero;

        desiredMovement.x = _movementInput.x;

        if (_jump) {
            desiredMovement.y = 1;
            _jump = false;
        }

        return desiredMovement;
    }

    private void SaveLocalSpeed() {
        _localCachedVelocity = _playersTrans.InverseTransformVector(_cachedVelocity);
    }

    private Vector3 RestoreLocalSpeed() {
        return _playersTrans.TransformVector(_localCachedVelocity);
    }
    #endregion

}
