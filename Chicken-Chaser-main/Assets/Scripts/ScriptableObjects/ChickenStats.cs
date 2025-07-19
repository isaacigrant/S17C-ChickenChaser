using UnityEngine;

[CreateAssetMenu(fileName = "ChickenStats", menuName = "ChickenChaser/ChickenStats", order = 100)]
public class ChickenStats : ScriptableObject
{
    [Header("Movement")]
    [SerializeField] protected float speed;
    [SerializeField] protected float maxSpeed;

    [Header("Foot Management")]
    [SerializeField] protected float footRadius;
    [SerializeField] protected float footDistance;

    public float Speed => speed;
    public float MaxSpeed => maxSpeed;
    public float FootRadius => footRadius;
    public float FootDistance => footDistance;
}
