using AI;
using Interfaces;
using Managers;
using ScriptableObjects;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Utilities;

public class AiChicken : Chicken, IDetector
{
    [SerializeField] private HearStats activeHearing;

    public Action OnCaught;
    public Action OnFreed;

    private static int _numActiveChickens;

    private FaceTarget _faceTarget;
    private AudioDetection _audioDetection;
    private NavMeshAgent _agent;

    private void OnEnable()
    {
        _faceTarget.enabled = false;
        _agent.enabled = true;

        _audioDetection.SetStats(activeHearing);

        bodyColider.enabled = true;

        animatorController.SetBool(StaticUtilities.CluckAnimID, true);
        animatorController.enabled = true;

        PlayerChicken.OnPlayerCaught += MoveTo;
        PlayerChicken.OnPlayerEscaped += MoveTo;

        _numActiveChickens += 1;
        ScoreManager.Instance.UpdateScore();
    }

    protected override void Awake()
    {
        base.Awake();

        _faceTarget = GetComponent<FaceTarget>();
        _audioDetection = GetComponent<AudioDetection>();
        _agent = GetComponent<NavMeshAgent>();

        _agent.speed = stats.MaxSpeed;
        _agent.acceleration = stats.Speed;
        
        HudManager.Instance.RegisterChicken(this);
        GameManager.RegisterAIChicken();
    }

    private void OnDisable()
    {
        PlayerChicken.OnPlayerCaught -= MoveTo;
        PlayerChicken.OnPlayerEscaped -= MoveTo;

        animatorController.SetBool(StaticUtilities.CluckAnimID, false);
        animatorController.enabled = false;

        _agent.ResetPath();
        _agent.enabled = false;

        bodyColider.enabled = false;

        _faceTarget.enabled = true;

        _numActiveChickens -= 1;
        ScoreManager.Instance.UpdateScore();
    }

    private void OnDestroy()
    {
        HudManager.Instance.DeRegisterChicken(this);
    }

    public void AddDetection(Vector3 location, float detection, EDetectionType type)
    {
        if (!enabled && detection < 1) return;

        print("I'm moving towards: " + location);

        _agent.SetDestination(location);
        animatorController.SetBool(StaticUtilities.CluckAnimID, false);
    }

    public override void OnCaptured()
    {
        animatorController.SetFloat(StaticUtilities.MoveSpeedAnimID, 0);
        OnCaught.Invoke();
    }

    public override void OnFreedFromCage()
    {
        enabled = true;
        OnFreed.Invoke();
    }

    public override void OnEscaped(Vector3 position)
    {
        print($"I'm trying to escape {gameObject}");

        MoveTo(position);

        StartCoroutine(CheckForEscaped());

        visibility = 0;
    }

    public static int NumActiveAIChickens()
    {
        return _numActiveChickens;
    }

    protected override void HandleMovement()
    {
        currentSpeed = Mathf.Max(0, _agent.remainingDistance - _agent.stoppingDistance + 0.2f);
        animatorController.SetFloat(StaticUtilities.MoveSpeedAnimID, currentSpeed);
    }

    private void MoveTo(Vector3 location)
    {
        _agent.SetDestination(location);
    }

    private IEnumerator CheckForEscaped()
    {
        WaitUntil target = new WaitUntil(() => _agent.hasPath && _agent.remainingDistance <= _agent.stoppingDistance);

        yield return target;

        print("I'm trying to escape");

        Destroy(gameObject);
    }
}
