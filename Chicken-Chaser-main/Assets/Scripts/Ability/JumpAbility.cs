using UnityEngine;
using Utilities;

[RequireComponent(typeof(Rigidbody))]
public class JumpAbility : AbstractAbility
{
    [Header("Jump")]
    [SerializeField] private float jumpForce;

    private Rigidbody _physicsBody;

    private void Awake()
    {
        _physicsBody = GetComponent<Rigidbody>();
    }

    public override bool CanActivate()
    {
        return owner.GetIsGrounded() && base.CanActivate();
    }

    protected override void Activate()
    {
        _physicsBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
    protected override int AbilityTriggerID()
    {
        return StaticUtilities.JumpAnimID;
    }
}
