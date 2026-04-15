using LitMotion;
using LitMotion.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Work.Core.Utils.EventBus;
using Work.Input.Code;
using Work.Players.Code;
using Work.Weapons.Code;

public class WeaponMenuTest : MonoBehaviour
{
    [SerializeField] private CanvasGroup mainGroup;
    [SerializeField] private PlayerWeaponModule weaponModule;
    [SerializeField] private TMP_Text primaryWeaponName, secondaryWeaponName, selectedWeaponName, selectedWeaponDesc;
    [SerializeField] private Button primaryWeaponButton, secondaryWeaponButton, ImprintButton;
    [SerializeField] private Image weaponIcon;
    [SerializeField] private WeaponImprintMenuTest weaponImprintMenu;

    private WeaponDataSO _primaryWeapon;
    private WeaponDataSO _secondaryWeapon;
    private BaseWeapon _primaryWeaponInstance;
    private BaseWeapon _secondaryWeaponInstance;
    private BaseWeapon _selectedWeapon;
    private bool _isOpen;

    private void Start()
    {
        mainGroup.alpha = 0;
        mainGroup.interactable = false;
        mainGroup.blocksRaycasts = false;
        _isOpen = false;

        Bus<InputMenuEvent>.Events += OnMenuEvent;

        primaryWeaponButton.onClick.AddListener(OnPrimaryWeaponClicked);
        secondaryWeaponButton.onClick.AddListener(OnSecondaryWeaponClicked);
        ImprintButton.onClick.AddListener(OnImprintButtonClicked);
        ImprintButton.interactable = false;
        ImprintButton.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        Bus<InputMenuEvent>.Events -= OnMenuEvent;
    }

    private void OnMenuEvent(InputMenuEvent @event)
    {
        if (weaponImprintMenu != null && weaponImprintMenu.IsOpen)
        {
            weaponImprintMenu.CloseUI();
            Time.timeScale = 1f;
            return;
        }

        if (_isOpen)
            CloseUI();
        else
            OpenUI();
    }

    private void OpenUI()
    {
        Time.timeScale = 0;

        _primaryWeaponInstance = weaponModule.CurrentWeapon;
        _secondaryWeaponInstance = weaponModule.StandbyWeapon;
        _primaryWeapon = _primaryWeaponInstance?.Data;
        _secondaryWeapon = _secondaryWeaponInstance?.Data;
        _selectedWeapon = null;
        ImprintButton.interactable = false;
        ImprintButton.gameObject.SetActive(_primaryWeapon != null || _secondaryWeapon != null);

        // Primary Weapon Setup
        if (_primaryWeapon != null)
        {
            primaryWeaponButton.onClick.Invoke();
            primaryWeaponName.text = _primaryWeapon.Name;
            primaryWeaponButton.interactable = true;
        }
        else
        {
            primaryWeaponName.text = "없음";
            primaryWeaponButton.interactable = false;
        }
        UpdateButtonWidth(primaryWeaponButton, primaryWeaponName);

        // Secondary Weapon Setup
        if (_secondaryWeapon != null)
        {
            secondaryWeaponName.text = _secondaryWeapon.Name;
            secondaryWeaponButton.interactable = true;
        }
        else
        {
            secondaryWeaponName.text = "없음";
            secondaryWeaponButton.interactable = false;
        }
        UpdateButtonWidth(secondaryWeaponButton, secondaryWeaponName);

        // Default Selection Logic
        if (_primaryWeapon != null)
        {
            OnPrimaryWeaponClicked();
        }
        else if (_secondaryWeapon != null)
        {
            OnSecondaryWeaponClicked();
        }
        else
        {
            selectedWeaponName.text = "없음";
            selectedWeaponDesc.text = "장착된 무기가 없습니다.";
            if (weaponIcon != null) weaponIcon.gameObject.SetActive(false);
        }

        _isOpen = true;
        LMotion.Create(mainGroup.alpha, 1, 0.25f)
            .WithScheduler(MotionScheduler.UpdateIgnoreTimeScale)
            .WithEase(Ease.OutQuad)
            .BindToAlpha(mainGroup);
        mainGroup.interactable = true;
        mainGroup.blocksRaycasts = true;
    }

    private void CloseUI()
    {
        Time.timeScale = 1;
        _isOpen = false;
        LMotion.Create(mainGroup.alpha, 0, 0.25f)
            .WithScheduler(MotionScheduler.UpdateIgnoreTimeScale)
            .WithEase(Ease.OutQuad)
            .BindToAlpha(mainGroup);
        mainGroup.interactable = false;
        mainGroup.blocksRaycasts = false;
    }

    private void OnPrimaryWeaponClicked()
    {
        if (_primaryWeapon == null) return;
        _selectedWeapon = _primaryWeaponInstance;
        ImprintButton.interactable = _selectedWeapon != null;
        ImprintButton.gameObject.SetActive(_selectedWeapon != null);
        selectedWeaponName.text = _primaryWeapon.Name;
        selectedWeaponDesc.text = _primaryWeapon.Description;
        if (weaponIcon != null)
        {
            weaponIcon.gameObject.SetActive(true);
            weaponIcon.sprite = _primaryWeapon.WeaponIcon;
        }
    }

    private void OnSecondaryWeaponClicked()
    {
        if (_secondaryWeapon == null) return;
        _selectedWeapon = _secondaryWeaponInstance;
        ImprintButton.interactable = _selectedWeapon != null;
        ImprintButton.gameObject.SetActive(_selectedWeapon != null);
        selectedWeaponName.text = _secondaryWeapon.Name;
        selectedWeaponDesc.text = _secondaryWeapon.Description;
        if (weaponIcon != null)
        {
            weaponIcon.gameObject.SetActive(true);
            weaponIcon.sprite = _secondaryWeapon.WeaponIcon;
        }
    }

    private void OnImprintButtonClicked()
    {
        if (_selectedWeapon == null || weaponImprintMenu == null)
            return;

        HideForImprint();
        weaponImprintMenu.OpenUI(_selectedWeapon);
    }

    public void ReopenFromImprint()
    {
        _isOpen = true;
        mainGroup.alpha = 1f;
        mainGroup.interactable = true;
        mainGroup.blocksRaycasts = true;
    }

    private void HideForImprint()
    {
        _isOpen = false;
        mainGroup.alpha = 0f;
        mainGroup.interactable = false;
        mainGroup.blocksRaycasts = false;
    }

    private void UpdateButtonWidth(Button button, TMP_Text text)
    {
        text.ForceMeshUpdate(); 
        
        float newWidth = text.preferredWidth + 65f;
        RectTransform rt = button.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
        }
    }
}
