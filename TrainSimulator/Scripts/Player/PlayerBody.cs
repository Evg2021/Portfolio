using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerBody : MonoBehaviour
{
    public Vector3 Forward
    {
        get { return transform.forward; }
        set { transform.forward = value; }
    }
    public Vector3 Right
    {
        get { return transform.right; }
        set { transform.right = value; }
    }
    public Quaternion Rotation
    {
        get { return transform.rotation; }
        set { transform.rotation = value; }
    }
    public void Rotate(Vector3 eulers) => transform.Rotate(eulers);
    public bool IsCrouch => _isCrouch;

    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Transform _topPart;
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private float _groundCheckHeight;
    [SerializeField] private float _canJumpTimer = 0.25f;

    protected IEnumerator _jumpTimer;
    private bool _canJump = true;
    private bool _isCrouch;
    private float _topPartHeight;
    private Vector3 _movementDirection = Vector3.zero;
    private Settings _settings;

    [Inject]
    private void Construct(GameplaySettings settings)
    {
        _settings = settings.Player.Body;
    }

    private void Start()
    {
        _jumpTimer = JumpTimer();
        _topPartHeight = _topPart.transform.localPosition.y;
    }

    public void SwitchCrouch()
    {
        Crouch(!_isCrouch);
    }

    public void Crouch(bool value)
    {
        if (_isCrouch == value) return;

        _isCrouch = value;
        float endValue = _topPartHeight - (_isCrouch ? _settings.CrouchDeep : 0);
        _topPart.DOLocalMoveY(endValue, _settings.CrouchingDuration);
    }

    public bool Jump()
    {
        if (!IsGrounded() || !_canJump) return false;

        Vector3 jumpForce = Vector3.up * _settings.JumpStrength;
        _rigidbody.AddForce(jumpForce, ForceMode.Impulse);
        StartCoroutine(_jumpTimer);

        return true;
    }

    private IEnumerator JumpTimer()
    {
        _canJump = false;

        WaitForSeconds wait = new(_canJumpTimer);

        yield return wait;

        _canJump = true;
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(_groundCheckPoint.position, Vector3.down, _groundCheckHeight);
    }

    public void Move(Vector3 position)
    {
        _movementDirection = position;
        _movementDirection.y = _rigidbody.velocity.y;
        _rigidbody.velocity = _movementDirection;
    }

    private void OnDisable()
    {
        if (_jumpTimer != null)
            StopCoroutine(_jumpTimer);
    }

    [Serializable]
    public class Settings : InstallerSettings
    {
        public float CrouchDeep;
        public float CrouchingDuration;
        public float JumpStrength;
    }
}