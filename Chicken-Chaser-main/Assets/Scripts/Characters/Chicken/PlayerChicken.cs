using System;
using UnityEngine;
using UnityEngine.AI;
using Utilities;
using Managers;

public class PlayerChicken : Chicken
{
    private Vector3 _moveDirection;
    private Vector2 _lookDirection;

    [Header("Looking")]
    [SerializeField, Range(0, 90)] private float pitchLimit = 30;
    [SerializeField, Range(0, 180)] private float yawLimit = 180;
    [SerializeField] private float lookSpeed = 5;

    [Header("Abilities")]
    [SerializeField] private AbstractAbility jumpAbility;
    [SerializeField] private AbstractAbility cluckAbility;
    [SerializeField] private AbstractAbility dashAbility;
    [Header("Effects")]
    [SerializeField] private GameObject lossCam;



    public static Action<Vector3> OnPlayerCaught;
    public static Action<Vector3> OnPlayerEscaped;
    public static Action OnPlayerRescued;

    private void OnEnable()
    {
        physicsBody.isKinematic = false;
        bodyColider.enabled = true;
        SettingsManager.SaveFile.onLookSenseChanged += OnLookSenseChanged;
        lookSpeed = SettingsManager.currentSettings.LookSensitivity;
    }

    protected override void Awake()
    {
        base.Awake();

        HudManager.Instance.BindPlayer(this);
        PlayerControls.Initialize(this);
        PlayerControls.UseGameControls();
    }
    private void OnDisable()
    {
        physicsBody.isKinematic = true;
        bodyColider.enabled = false;
        PlayerControls.DisablePlayer();
        jumpAbility.ForceCancelAbility();
        cluckAbility.ForceCancelAbility();
        dashAbility.ForceCancelAbility();
        SettingsManager.SaveFile.onLookSenseChanged -= OnLookSenseChanged;
    }

    public void SetDashState(bool state)
    {
        if (state) dashAbility.StartUsingAbility();
        else dashAbility.StopUsingAbility();
    }
    public void SetCluckState(bool state)
    {
        if (state) cluckAbility.StartUsingAbility();
        else cluckAbility.StopUsingAbility();
    }
    public void SetJumpState(bool state)
    {
        if (state) jumpAbility.StartUsingAbility();
        else jumpAbility.StopUsingAbility();
    }

    public void SetMoveDirection(Vector2 direction)
    {
        _moveDirection = new Vector3(direction.x, 0, direction.y);
    }
    public void SetLookDirection(Vector2 direction)
    {
        _lookDirection = direction;
    }

    protected override void HandleMovement()
    {
        Vector3 direction = _moveDirection;

        if (isGrounded)
        {
            direction = Vector3.ProjectOnPlane(_moveDirection, slopeNormal);
        }

        physicsBody.AddForce(transform.rotation * direction * stats.Speed, ForceMode.Acceleration);

        Vector2 horizontalVelocity = new Vector2(physicsBody.linearVelocity.x, physicsBody.linearVelocity.z);
        currentSpeed = horizontalVelocity.magnitude;

        if (currentSpeed > stats.MaxSpeed)
        {
            horizontalVelocity = horizontalVelocity.normalized * stats.MaxSpeed;
            physicsBody.linearVelocity = new Vector3(horizontalVelocity.x, physicsBody.linearVelocity.y, horizontalVelocity.y);
            currentSpeed = stats.MaxSpeed;
        }

        HandleLooking();
    }

    public override void OnFreedFromCage()
    {
        enabled = true;
        PlayerControls.UseGameControls();
        OnPlayerRescued?.Invoke();

        cluckAbility.StopUsingAbility();
        lossCam.SetActive(false);
        GameManager.PlayUISound(stats.FreedSound);
    }

    public override void OnEscaped(Vector3 position)
    {
        print("Player won the game!");

        OnPlayerEscaped?.Invoke(transform.position);

        NavMeshAgent agent = gameObject.AddComponent<NavMeshAgent>();
        agent.enabled = true;
        agent.baseOffset = 0.16f;
        agent.height = 0.32f;
        agent.radius = 0.2f;
        agent.agentTypeID = 0;
        agent.SetDestination(position);

        animatorController.SetFloat(StaticUtilities.MoveSpeedAnimID, stats.MaxSpeed);

        enabled = false;
    }

    public override void OnCaptured()
    {
        print("Player has been captured");

        animatorController.SetFloat(StaticUtilities.MoveSpeedAnimID, 0);
        cluckAbility.StartUsingAbility();

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        visibility = 0;

        OnPlayerCaught?.Invoke(transform.position);
        lossCam.SetActive(true);
        GameManager.PlayUISound(stats.CaughtSound);
    }

    private void HandleLooking()
    {
        float timeShift = Time.deltaTime;
        float pitchChange = head.localEulerAngles.x - lookSpeed * _lookDirection.y * timeShift;
        float yawChange = transform.localEulerAngles.y + lookSpeed * _lookDirection.x * timeShift;

        if (pitchChange > pitchLimit && pitchChange < 180) pitchChange = pitchLimit;
        else if (pitchChange < 360 - pitchLimit && pitchChange > 180) pitchChange = -pitchLimit;
        if (yawChange > yawLimit && yawChange < 180) yawChange = yawLimit;
        else if (yawChange < 360 - yawLimit && yawChange > 180) yawChange = -yawLimit;

        transform.localEulerAngles = new Vector3(0, yawChange, 0);
        head.localEulerAngles = new Vector3(pitchChange, 0, 0);
    }

    public AbstractAbility GetCluckAbility()
    {
        return cluckAbility;
    }
    public AbstractAbility GetJumpAbility()
    {
        return jumpAbility;
    }
    public AbstractAbility GetDashAbility()
    {
        return dashAbility;
    }
    private void OnLookSenseChanged(float val)
    { 
        lookSpeed = val;
    }
}
