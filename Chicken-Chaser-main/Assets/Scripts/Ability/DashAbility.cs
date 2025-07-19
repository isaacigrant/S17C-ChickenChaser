using System.Collections;
using UnityEngine;
using Utilities;
using static UnityEngine.UI.GridLayoutGroup;

[RequireComponent(typeof(Rigidbody))]
public class DashAbility : AbstractAbility
{
    [Header("Dash")]
    [SerializeField] private float dashDistance;
    [SerializeField] private float dashDuration;

    [Header("Effects")]
    [SerializeField] private ParticleSystem feathers;

    private Rigidbody _rigidbody;
    private bool _canDash = true;
    private float _radius;

    private const int NUM_SAMPLES = 10;
    private static readonly Vector3 maxVertical = new Vector3(0, 1, 0);

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        SphereCollider c = GetComponentInChildren<SphereCollider>();
        _radius = c.radius * c.transform.lossyScale.x;
    }

    private IEnumerator ActivateAbility(Vector3 direction)
    {
        direction = new Vector3(direction.x, 0, direction.z) * dashDistance;
        _canDash = false;

        Vector3 origin = transform.position;
        float yMax = 0;
        Vector3 start = origin + maxVertical;

        for (int i = 0; i < NUM_SAMPLES; i++)
        {
            float percent = (float)i / NUM_SAMPLES;

            if (Physics.Raycast(start + direction * percent, Vector3.down, out RaycastHit targetHit, dashDistance, StaticUtilities.VisibilityLayer))
            {
                Debug.DrawRay(start + direction * percent, Vector3.down * dashDistance, (targetHit.point.y > yMax)?Color.green:Color.red, 3);

                if (targetHit.point.y > yMax) yMax = targetHit.point.y;
            }
            else
            {
                Debug.DrawRay(start + direction * percent, Vector3.down * dashDistance, Color.red, 3);
            }
        }

        float d = yMax - origin.y + _radius * 2;

        if (d > 0) direction.y = d;

        direction = direction.normalized;

        feathers.transform.forward = -direction;
        feathers.Play();

        Vector3 endPoint = Physics.SphereCast(origin, _radius, direction, out RaycastHit hit, dashDistance, StaticUtilities.VisibilityLayer) ? hit.point + hit.normal * (_radius * 2) : direction * dashDistance + transform.position;

        Debug.DrawLine(origin, endPoint, Color.magenta, 5);

        float curTime = 0;

        _rigidbody.isKinematic = true;

        while (curTime < dashDuration)
        {
            curTime += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, endPoint, curTime / dashDuration);
            yield return null;
        }

        transform.position = Vector3.Lerp(transform.position, endPoint, 1);

        _rigidbody.isKinematic = false;
        feathers.Stop();
        _canDash = true;
    }

    public override bool CanActivate()
    {
        return _canDash && base.CanActivate();
    }

    protected override void Activate()
    {
        StartCoroutine(ActivateAbility(owner.GetLookDirection()));
    }


    private void OnDrawGizmosSelected()
    {
        SphereCollider c = GetComponentInChildren<SphereCollider>();
        _radius = c.radius * c.transform.lossyScale.x;

        Gizmos.color = Color.yellow;
        GizmosExtras.DrawWireSphereCast(transform.position, transform.forward, dashDistance, _radius);
    }

    public override void ForceCancelAbility()
    {
        base.ForceCancelAbility();
        feathers.Stop();
    }

    protected override int AbilityTriggerID()
    {
        return StaticUtilities.DashAnimID;
    }
}
