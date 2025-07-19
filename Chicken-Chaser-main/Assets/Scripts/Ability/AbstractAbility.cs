using System.Collections;
using UnityEngine;

public abstract class AbstractAbility : MonoBehaviour
{
    [SerializeField] private AbilityStats stats;

    protected Chicken owner;
    protected Animator animatorController;

    private bool _isReady = true;
    private bool _isBeingHeld;
    private float _currentCooldownTime;

    private void Start()
    {
        owner = GetComponentInParent<Chicken>();
        animatorController = GetComponentInChildren<Animator>();
    }

    public void StartUsingAbility()
    {
        _isBeingHeld = true;

        if (_isReady) StartCoroutine(BeginCooldown());
        if (IsBooleanAnimation()) animatorController.SetBool(AbilityBoolID(), true);
    }
    public void StopUsingAbility()
    {
        _isBeingHeld = false;

        if (IsBooleanAnimation()) animatorController.SetBool(AbilityBoolID(), false);
    }

    public Sprite GetIcon()
    {
        return stats.Icon;
    }

    public float GetCooldownPercent()
    {
        return _currentCooldownTime / stats.Cooldown;
    }

    private IEnumerator BeginCooldown()
    {
        do
        {
            yield return new WaitUntil(CanActivate);

            if (!_isBeingHeld) yield break;

            Activate();

            if (IsTriggerAnimation()) animatorController.SetTrigger(AbilityTriggerID());

            _currentCooldownTime = 0;
            _isReady = true;

            while (_currentCooldownTime < stats.Cooldown)
            {
                _currentCooldownTime += Time.deltaTime;
                yield return null;
            }

            _currentCooldownTime = stats.Cooldown;
            _isReady = true;
        } while (_isBeingHeld && stats.CanBeHeld);

        StopUsingAbility();
    }

    private bool IsTriggerAnimation()
    {
        return AbilityTriggerID() != 0;
    }
    private bool IsBooleanAnimation()
    {
        return AbilityBoolID() != 0;
    }

    public virtual bool CanActivate()
    {
        return _isReady;
    }
    public virtual void ForceCancelAbility()
    {
        _currentCooldownTime = stats.Cooldown;
        _isReady = true;
        StopAllCoroutines();
        StopUsingAbility();
    }

    protected virtual int AbilityBoolID()
    {
        return 0;
    }
    protected virtual int AbilityTriggerID()
    {
        return 0;
    }

    protected abstract void Activate();
}
