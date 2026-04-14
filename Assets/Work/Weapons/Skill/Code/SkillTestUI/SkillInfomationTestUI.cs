using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Work.Combat.Code;
using Work.Players.Code;
using Work.Weapons.Code;
using Work.Weapons.Skill.Code;

public class SkillInfomationTestUI : MonoBehaviour
{
    [SerializeField] private PlayerWeaponModule playerWeaponModule;
    [SerializeField] private Image currentWeaponIcon, standbyWeaponIcon;
    [SerializeField] private Sprite defaultWeaponIcon;
    [SerializeField] private Transform skillIconParent;
    [SerializeField] private SkillIcon skillIconPrefab;
    [SerializeField] private Color canUseCostColor = Color.white;
    [SerializeField] private Color cannotUseCostColor = new Color(0.4f, 0.08f, 0.08f, 1f);

    private SkillEnergyModule _skillEnergyModule;
    private BaseWeapon _cachedCurrentWeapon;
    private BaseWeapon _cachedStandbyWeapon;
    private float _cachedEnergy = -1f;

    private readonly List<SkillEntry> _skillEntries = new();

    private sealed class SkillEntry
    {
        public SkillIcon Icon;
        public Sprite Sprite;
        public int Cost;
    }

    private void Start()
    {
        TryResolveReferences();
        RefreshAll();
    }

    private void Update()
    {
        TryResolveReferences();
        RefreshIfNeeded();
    }

    private void OnDestroy()
    {
        UnsubscribeEnergyChanged();
    }

    private void TryResolveReferences()
    {
        if (playerWeaponModule == null)
            playerWeaponModule = FindAnyObjectByType<PlayerWeaponModule>();

        if (_skillEnergyModule != null)
            return;

        Player player = null;
        if (playerWeaponModule != null)
            player = playerWeaponModule.GetComponent<Player>();

        if (player == null)
            player = FindAnyObjectByType<Player>();

        if (player == null)
            return;

        _skillEnergyModule = player.GetModule<SkillEnergyModule>(true);
        SubscribeEnergyChanged();
    }

    private void RefreshIfNeeded()
    {
        BaseWeapon current = playerWeaponModule != null ? playerWeaponModule.CurrentWeapon : null;
        BaseWeapon standby = playerWeaponModule != null ? playerWeaponModule.StandbyWeapon : null;
        float energy = GetCurrentEnergy();

        bool weaponChanged = _cachedCurrentWeapon != current || _cachedStandbyWeapon != standby;
        if (weaponChanged)
        {
            RefreshAll();
            return;
        }

        if (Mathf.Approximately(_cachedEnergy, energy))
            return;

        _cachedEnergy = energy;
        RefreshSkillCostColors(energy);
    }

    private void RefreshAll()
    {
        BaseWeapon current = playerWeaponModule != null ? playerWeaponModule.CurrentWeapon : null;
        BaseWeapon standby = playerWeaponModule != null ? playerWeaponModule.StandbyWeapon : null;

        _cachedCurrentWeapon = current;
        _cachedStandbyWeapon = standby;
        _cachedEnergy = GetCurrentEnergy();

        RefreshWeaponIcon(currentWeaponIcon, current);
        RefreshWeaponIcon(standbyWeaponIcon, standby);
        RebuildSkillIcons(current, _cachedEnergy);
    }

    private void RefreshWeaponIcon(Image target, BaseWeapon weapon)
    {
        if (target == null)
            return;

        Sprite weaponIcon = weapon != null ? weapon.Data?.WeaponIcon : null;
        target.sprite = weaponIcon != null ? weaponIcon : defaultWeaponIcon;
        target.gameObject.SetActive(target.sprite != null);
    }

    private void RebuildSkillIcons(BaseWeapon weapon, float currentEnergy)
    {
        ClearSkillIcons();

        if (skillIconPrefab == null || skillIconParent == null || weapon == null || weapon.Data == null)
            return;

        CreateSkillIcon(weapon.Data.PrimarySkill, currentEnergy);
        CreateSkillIcon(weapon.Data.SecondarySkill, currentEnergy);
    }

    private void CreateSkillIcon(SkillDataSO skillData, float currentEnergy)
    {
        if (skillData == null)
            return;

        SkillIcon icon = Instantiate(skillIconPrefab, skillIconParent);
        icon.SetIcon(skillData.SkillIcon, skillData.Cost, GetCostColor(skillData.Cost, currentEnergy));

        _skillEntries.Add(new SkillEntry
        {
            Icon = icon,
            Sprite = skillData.SkillIcon,
            Cost = skillData.Cost,
        });
    }

    private void ClearSkillIcons()
    {
        for (int i = 0; i < _skillEntries.Count; i++)
        {
            if (_skillEntries[i].Icon != null)
                Destroy(_skillEntries[i].Icon.gameObject);
        }

        _skillEntries.Clear();
    }

    private void RefreshSkillCostColors(float currentEnergy)
    {
        for (int i = 0; i < _skillEntries.Count; i++)
        {
            SkillEntry entry = _skillEntries[i];
            if (entry.Icon == null)
                continue;

            entry.Icon.SetIcon(entry.Sprite, entry.Cost, GetCostColor(entry.Cost, currentEnergy));
        }
    }

    private Color GetCostColor(int skillCost, float currentEnergy)
    {
        return currentEnergy >= skillCost ? canUseCostColor : cannotUseCostColor;
    }

    private float GetCurrentEnergy()
    {
        if (_skillEnergyModule == null || _skillEnergyModule.EnergyContainer == null)
            return 0f;

        return _skillEnergyModule.EnergyContainer.Energy;
    }

    private void SubscribeEnergyChanged()
    {
        if (_skillEnergyModule == null || _skillEnergyModule.EnergyContainer == null)
            return;

        _skillEnergyModule.EnergyContainer.OnChangedEvent -= HandleEnergyChanged;
        _skillEnergyModule.EnergyContainer.OnChangedEvent += HandleEnergyChanged;
    }

    private void UnsubscribeEnergyChanged()
    {
        if (_skillEnergyModule == null || _skillEnergyModule.EnergyContainer == null)
            return;

        _skillEnergyModule.EnergyContainer.OnChangedEvent -= HandleEnergyChanged;
    }

    private void HandleEnergyChanged()
    {
        float energy = GetCurrentEnergy();
        _cachedEnergy = energy;
        RefreshSkillCostColors(energy);
    }
}
