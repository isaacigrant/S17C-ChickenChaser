using Characters;
using Interfaces;
using System;
using UnityEngine;
using Utilities;

public abstract class Chicken : MonoBehaviour, IVisualDetectable, ITrappable
{
    [SerializeField] protected ChickenStats stats;

    [Header("Objects")]
    [SerializeField] protected Transform head;
    [SerializeField] protected Transform foot;

    [SerializeField]private ParticleSystem landEffect;

    protected AudioSource audio;
    protected Rigidbody physicsBody;
    protected Animator animatorController;
    protected Collider bodyColider;
    protected bool isGrounded;

    protected float currentSpeed;
    protected float currentFallTime;
    protected Vector3 slopeNormal;

    [Header("Detection")]
    [SerializeField] protected float visibility = 1;

    protected virtual void Awake()
    {
        physicsBody = GetComponent<Rigidbody>();
        animatorController = GetComponentInChildren<Animator>();
        bodyColider = GetComponentInChildren<Collider>();
        audio = GetComponentInChildren<AudioSource>();
        ChickenAnimatorReceiver car = transform.GetChild(0).GetComponent<ChickenAnimatorReceiver>();
        car.OnLandEffect += HandleLanding;

    }

    private void FixedUpdate()
    {
        HandleGroundState();
        HandleMovement();
        HandleAnims();
    }

    public bool GetIsGrounded()
    {
        return isGrounded;
    }
    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }
    public Vector3 GetLookDirection()
    {
        return head.forward;
    }

    private void HandleGroundState()
    {
        bool newGroundState = Physics.SphereCast(foot.position, stats.FootRadius, Vector3.down, out RaycastHit slope, stats.FootDistance, StaticUtilities.GroundLayers);

        if (newGroundState != isGrounded)
        {
            isGrounded = newGroundState;
            animatorController.SetBool(StaticUtilities.IsGroundedAnimID, isGrounded);

            if (currentFallTime >= 0)
            {
                HandleLanding(MathF.Max(currentFallTime/2, 3));
                currentFallTime = 0;
            }
        }

        if (!isGrounded) currentFallTime += Time.deltaTime;
        else slopeNormal = slope.normal;
    }

    protected virtual void HandleLanding(float force)
    {
        landEffect.emission.SetBurst(0, new ParticleSystem.Burst(0, UnityEngine.Random.Range(10, 20)* force));
        landEffect.Play();
    }
    protected virtual void HandleAnims()
    {
        animatorController.SetFloat(StaticUtilities.MoveSpeedAnimID, currentSpeed);
    }

    protected abstract void HandleMovement();

    public abstract void OnFreedFromCage();
    public abstract void OnEscaped(Vector3 position);
    public abstract void OnCaptured();

    public void AddVisibility(float visibility)
    {
        this.visibility += visibility;
    }

    public void RemoveVisibility(float visibility)
    {
        this.visibility -= Mathf.Max(0, visibility);
    }

    public float GetVisibility()
    {
        return visibility;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public bool CanBeTrapped()
    {
        return isActiveAndEnabled;
    }

    public void OnPreCapture()
    {
        enabled = false;
    }
}
