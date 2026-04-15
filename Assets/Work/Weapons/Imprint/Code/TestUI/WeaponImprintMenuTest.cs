using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Work.Players.Code;
using Work.Weapons.Code;
using Work.Weapons.Imprint.Code;

public class WeaponImprintMenuTest : MonoBehaviour
{
    [SerializeField] private CanvasGroup mainCanvas, inventoryCanvas;
    [SerializeField] private PlayerWeaponModule playerWeaponModule;
    [SerializeField] private Button statButton, effectButton, attackButton;
    [SerializeField] private Transform inventoryContainer;
    [SerializeField] private WordItem statWordPrefab, effectWordPrefab, attackWordPrefab;
    [SerializeField] private Button backButton;
    [SerializeField] private WeaponMenuTest weaponMenu;
    [SerializeField] private Image selectedWeaponIcon;
    [SerializeField] private TMP_Text primarySkillNameText;
    [SerializeField] private TMP_Text primarySkillSpText;
    [SerializeField] private TMP_Text primarySkillDescriptionText;
    [SerializeField] private TMP_Text secondarySkillNameText;
    [SerializeField] private TMP_Text secondarySkillSpText;
    [SerializeField] private TMP_Text secondarySkillDescriptionText;
    [SerializeField] private TMP_Text triggerDescriptionText;

    private const float SlotLiftY = 10f;

    private readonly Dictionary<ImprintType, Button> _slotButtons = new();
    private readonly Dictionary<ImprintType, Vector2> _slotDefaultPositions = new();
    private readonly Dictionary<ImprintType, TMP_Text> _slotTexts = new();
    private readonly List<WordItem> _spawnedWords = new();

    private Player _player;
    private BaseWeapon _selectedWeapon;
    private ImprintType? _openedSlotType;

    public bool IsOpen { get; private set; }

    private void Start()
    {
        _player = playerWeaponModule != null ? playerWeaponModule.GetComponent<Player>() : FindAnyObjectByType<Player>();
        if (weaponMenu == null)
            weaponMenu = FindAnyObjectByType<WeaponMenuTest>();

        _slotButtons[ImprintType.Stat] = statButton;
        _slotButtons[ImprintType.Effect] = effectButton;
        _slotButtons[ImprintType.Attack] = attackButton;

        CacheSlotDefaultPositions();
        CacheSlotTexts();

        if (statButton != null)
            statButton.onClick.AddListener(() => OnSlotButtonClicked(ImprintType.Stat));
        if (effectButton != null)
            effectButton.onClick.AddListener(() => OnSlotButtonClicked(ImprintType.Effect));
        if (attackButton != null)
            attackButton.onClick.AddListener(() => OnSlotButtonClicked(ImprintType.Attack));
        if (backButton != null)
            backButton.onClick.AddListener(OnBackButtonClicked);

        SetCanvas(mainCanvas, false);
        SetCanvas(inventoryCanvas, false);
        IsOpen = false;
    }

    public void OpenUI(BaseWeapon weapon)
    {
        if (weapon == null)
            return;

        _selectedWeapon = weapon;
        RefreshWeaponIcon();
        RefreshWeaponInfoTexts();
        SetCanvas(mainCanvas, true);
        CloseInventory();
        RefreshSlotTexts();
        IsOpen = true;
    }

    public void CloseUI()
    {
        CloseInventory();
        SetCanvas(mainCanvas, false);
        ClearWeaponIcon();
        ClearWeaponInfoTexts();
        _selectedWeapon = null;
        IsOpen = false;
    }

    private void CacheSlotDefaultPositions()
    {
        foreach (var kvp in _slotButtons)
        {
            if (kvp.Value == null)
                continue;

            RectTransform rect = kvp.Value.GetComponent<RectTransform>();
            if (rect != null)
                _slotDefaultPositions[kvp.Key] = rect.anchoredPosition;
        }
    }

    private void CacheSlotTexts()
    {
        foreach (var kvp in _slotButtons)
        {
            TMP_Text text = kvp.Value != null ? kvp.Value.GetComponentInChildren<TMP_Text>(true) : null;
            if (text != null)
                _slotTexts[kvp.Key] = text;
        }
    }

    private void OnBackButtonClicked()
    {
        CloseUI();
        weaponMenu?.ReopenFromImprint();
    }

    private void OnSlotButtonClicked(ImprintType type)
    {
        if (!IsOpen)
            return;

        if (_openedSlotType == type)
        {
            CloseInventory();
            return;
        }

        OpenInventory(type);
    }

    private void OpenInventory(ImprintType type)
    {
        CloseInventory();

        _openedSlotType = type;
        LiftSlot(type, true);
        PopulateInventory(type);
        SetCanvas(inventoryCanvas, true);
    }

    private void CloseInventory()
    {
        if (_openedSlotType.HasValue)
            LiftSlot(_openedSlotType.Value, false);

        _openedSlotType = null;
        ClearInventoryItems();
        SetCanvas(inventoryCanvas, false);
    }

    private void LiftSlot(ImprintType type, bool lift)
    {
        if (!_slotButtons.TryGetValue(type, out Button button))
            return;

        if (!_slotDefaultPositions.TryGetValue(type, out Vector2 defaultPosition))
            return;

        RectTransform rect = button.GetComponent<RectTransform>();
        if (rect == null)
            return;

        rect.anchoredPosition = lift
            ? new Vector2(defaultPosition.x, defaultPosition.y + SlotLiftY)
            : defaultPosition;
    }

    private void PopulateInventory(ImprintType type)
    {
        if (_player == null || _player.ImprintWordInventory == null)
            return;

        List<ImprintWordSO> words = _player.ImprintWordInventory.GetAllImprintWords();
        foreach (ImprintWordSO word in words)
        {
            if (word == null || word.Type != type)
                continue;

            int amount = _player.ImprintWordInventory.GetAmount(word);
            if (amount <= 0)
                continue;

            WordItem prefab = GetPrefabByType(type);
            if (prefab == null)
                continue;

            WordItem item = Instantiate(prefab, inventoryContainer);
            item.SetWord(word, amount);

            Button itemButton = item.button != null ? item.button : item.GetComponent<Button>();
            if (itemButton == null)
            {
                Debug.LogWarning($"WeaponImprintMenuTest: WordItem button is missing on prefab '{prefab.name}'.");
                Destroy(item.gameObject);
                continue;
            }

            itemButton.onClick.AddListener(() => OnWordClicked(word));
            _spawnedWords.Add(item);
        }
    }

    private void OnWordClicked(ImprintWordSO word)
    {
        if (_player == null || _selectedWeapon == null || word == null)
        {
            CloseInventory();
            return;
        }

        if (_player.ImprintWordInventory.TryUseImprintWord(word))
        {
            _selectedWeapon.Imprints.TryEquip(word);
            RefreshSlotTexts();
        }

        CloseInventory();
    }

    private WordItem GetPrefabByType(ImprintType type)
    {
        return type switch
        {
            ImprintType.Stat => statWordPrefab,
            ImprintType.Effect => effectWordPrefab,
            ImprintType.Attack => attackWordPrefab,
            _ => statWordPrefab
        };
    }

    private void ClearInventoryItems()
    {
        for (int i = 0; i < _spawnedWords.Count; i++)
        {
            if (_spawnedWords[i] != null)
                Destroy(_spawnedWords[i].gameObject);
        }

        _spawnedWords.Clear();
    }

    private void SetCanvas(CanvasGroup canvas, bool isVisible)
    {
        if (canvas == null)
            return;

        canvas.alpha = isVisible ? 1f : 0f;
        canvas.interactable = isVisible;
        canvas.blocksRaycasts = isVisible;
    }

    private void RefreshSlotTexts()
    {
        UpdateSlotText(ImprintType.Stat, _selectedWeapon != null ? _selectedWeapon.Imprints.Stat : null, "능력");
        UpdateSlotText(ImprintType.Effect, _selectedWeapon != null ? _selectedWeapon.Imprints.Effect : null, "효과");
        UpdateSlotText(ImprintType.Attack, _selectedWeapon != null ? _selectedWeapon.Imprints.Attack : null, "공격");
    }

    private void RefreshWeaponIcon()
    {
        if (selectedWeaponIcon == null)
            return;

        Sprite icon = _selectedWeapon != null ? _selectedWeapon.Data?.WeaponIcon : null;
        selectedWeaponIcon.sprite = icon;
        selectedWeaponIcon.gameObject.SetActive(icon != null);
    }

    private void ClearWeaponIcon()
    {
        if (selectedWeaponIcon == null)
            return;

        selectedWeaponIcon.sprite = null;
        selectedWeaponIcon.gameObject.SetActive(false);
    }

    private void RefreshWeaponInfoTexts()
    {
        WeaponDataSO weaponData = _selectedWeapon != null ? _selectedWeapon.Data : null;

        SetText(primarySkillNameText, weaponData?.PrimarySkill?.SkillName, "스킬 1: 없음");
        SetSkillSpText(primarySkillSpText, weaponData?.PrimarySkill != null ? weaponData.PrimarySkill.Cost : (int?)null);
        SetText(primarySkillDescriptionText, weaponData?.PrimarySkill?.SkillDescription, string.Empty);
        SetText(secondarySkillNameText, weaponData?.SecondarySkill?.SkillName, "스킬 2: 없음");
        SetSkillSpText(secondarySkillSpText, weaponData?.SecondarySkill != null ? weaponData.SecondarySkill.Cost : (int?)null);
        SetText(secondarySkillDescriptionText, weaponData?.SecondarySkill?.SkillDescription, string.Empty);
        SetText(triggerDescriptionText, weaponData?.TriggerDescription, "트리거 설명 없음");
    }

    private void ClearWeaponInfoTexts()
    {
        SetText(primarySkillNameText, string.Empty, string.Empty);
        SetSkillSpText(primarySkillSpText, null);
        SetText(primarySkillDescriptionText, string.Empty, string.Empty);
        SetText(secondarySkillNameText, string.Empty, string.Empty);
        SetSkillSpText(secondarySkillSpText, null);
        SetText(secondarySkillDescriptionText, string.Empty, string.Empty);
        SetText(triggerDescriptionText, string.Empty, string.Empty);
    }

    private static void SetText(TMP_Text target, string value, string fallback)
    {
        if (target == null)
            return;

        target.text = string.IsNullOrWhiteSpace(value) ? fallback : value;
    }

    private static void SetSkillSpText(TMP_Text target, int? spCost)
    {
        if (target == null)
            return;

        target.text = spCost.HasValue ? $"{spCost.Value} SP" : string.Empty;
    }

    private void UpdateSlotText(ImprintType type, ImprintWordSO word, string label)
    {
        if (!_slotTexts.TryGetValue(type, out TMP_Text text) || text == null)
            return;

        text.text = word != null
            ? $"{label}: {word.Name}"
            : $"{label}: 비어있음";
    }
}
