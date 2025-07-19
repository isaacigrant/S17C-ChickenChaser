using UnityEngine;

[CreateAssetMenu(fileName = "AbilityStats", menuName = "ChickenChaser/AbilityStats", order = 10)]
public class AbilityStats : ScriptableObject
{
    [SerializeField] private Sprite icon;

    [SerializeField] private float cooldown;
    [SerializeField] private bool canBeHeld;

    public Sprite Icon => icon;
    public float Cooldown => cooldown;
    public bool CanBeHeld => canBeHeld;
}
