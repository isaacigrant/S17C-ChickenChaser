using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-1000)]
public class HudManager : MonoBehaviour
{
    [Header("Interactables")]
    [SerializeField] private AbilityUIBind abilityA;
    [SerializeField] private AbilityUIBind abilityB;
    [SerializeField] private AbilityUIBind abilityC;

    [Header("HUD")]
    [SerializeField] private Transform trappedParent;
    [SerializeField] private Transform freedParent;
    [SerializeField] private Sprite caughtImg;
    [SerializeField] private Sprite freedImg;
    [SerializeField] private Image chickenImgPrefab;

    public static HudManager Instance { get; private set; }

    private PlayerChicken _owner;
    private Dictionary<AiChicken, Image> _hudChickens = new();

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(Instance);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        
    }

    #region Registering Chickens
    public void BindPlayer(PlayerChicken player)
    {
        _owner = player;

        abilityA.SetTargetAbility(player.GetCluckAbility());
        abilityB.SetTargetAbility(player.GetJumpAbility());
        abilityC.SetTargetAbility(player.GetDashAbility());
    }
    public void RegisterChicken(AiChicken chicken)
    {
        Image clone = Instantiate(chickenImgPrefab);
        _hudChickens.Add(chicken, clone);

        chicken.OnCaught += () => CaughtChicken(clone);
        chicken.OnFreed += () => FreeChicken(clone);

        FreeChicken(clone);
    }
    public void DeRegisterChicken(AiChicken chicken)
    {
        Destroy(_hudChickens[chicken]);
        _hudChickens.Remove(chicken);
    }
    private void CaughtChicken(Image target)
    {
        target.transform.SetParent(trappedParent, false);
        target.sprite = caughtImg;
    }
    private void FreeChicken(Image target)
    {
        target.transform.SetParent(freedParent, false);
        target.sprite = freedImg;
    }
    #endregion
}
