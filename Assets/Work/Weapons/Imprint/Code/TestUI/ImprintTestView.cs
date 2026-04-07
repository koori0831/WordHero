using System;
using UnityEngine;
using Work.Players.Code;
using Work.Weapons.Code;

namespace Work.Weapons.Imprint.Code.TestUI
{
    public class ImprintTestView : MonoBehaviour
    {
        public Action<ImprintWordSO> OnEquipToCurrentWeapon;
        public Action<ImprintWordSO> OnEquipToStandbyWeapon;

        private ImprintTestModel _model;
        private ImprintTestPresenter _presenter;

        private ImprintWordSO _selectedWord = null;
        private Rect _windowRect = new Rect(10, 10, 350, 450);

        private void Start()
        {
            Player player = FindAnyObjectByType<Player>();
            if (player != null)
            {
                _model = new ImprintTestModel(player);
                _presenter = new ImprintTestPresenter(_model, this);
            }
            else
            {
                Debug.LogWarning("ImprintTestView: Player not found in scene.");
            }
        }

        private void OnGUI()
        {
            if (_model == null) return;
            _windowRect = GUILayout.Window(10101, _windowRect, DrawWindow, "Imprint System Test");
        }

        private void DrawWindow(int windowID)
        {
            GUILayout.BeginVertical();

            DrawWeaponState("Current Weapon", _model.CurrentWeapon);
            GUILayout.Space(10);
            DrawWeaponState("Standby Weapon", _model.StandbyWeapon);
            
            GUILayout.Space(20);
            GUILayout.Label("=== Inventory ===");
            DrawInventory();

            GUILayout.Space(20);
            GUILayout.Label("=== Actions ===");
            DrawActions();

            GUILayout.EndVertical();
            GUI.DragWindow(_windowRect);
        }

        private void DrawWeaponState(string label, BaseWeapon weapon)
        {
            GUILayout.Label($"[{label}]: {(weapon != null ? weapon.name : "None")}");
            if (weapon != null)
            {
                GUILayout.Label($"- Attack: {GetWordName(weapon.Imprints.Attack)}");
                GUILayout.Label($"- Effect: {GetWordName(weapon.Imprints.Effect)}");
                GUILayout.Label($"- Stat: {GetWordName(weapon.Imprints.Stat)}");
            }
        }

        private string GetWordName(ImprintWordSO word)
        {
            return word != null ? word.DisplayName : "Empty";
        }

        private void DrawInventory()
        {
            var words = _model.GetInventoryWords();
            if (words == null || words.Count == 0)
            {
                GUILayout.Label("No Imprint Words in inventory.");
                return;
            }

            foreach (var word in words)
            {
                int amount = _model.GetWordAmount(word);
                bool isSelected = (_selectedWord == word);
                
                GUI.color = isSelected ? Color.green : Color.white;
                if (GUILayout.Button($"{word.DisplayName} ({word.Type}) - x{amount}"))
                {
                    _selectedWord = word;
                }
                GUI.color = Color.white;
            }
        }

        private void DrawActions()
        {
            if (_selectedWord == null)
            {
                GUILayout.Label("Select a word from inventory to equip.");
                return;
            }

            GUILayout.Label($"Selected: {_selectedWord.DisplayName}");

            GUILayout.BeginHorizontal();
            
            GUI.enabled = _model.CurrentWeapon != null;
            if (GUILayout.Button("Equip to Current"))
            {
                OnEquipToCurrentWeapon?.Invoke(_selectedWord);
                _selectedWord = null; 
            }

            GUI.enabled = _model.StandbyWeapon != null;
            if (GUILayout.Button("Equip to Standby"))
            {
                OnEquipToStandbyWeapon?.Invoke(_selectedWord);
                _selectedWord = null;
            }
            GUI.enabled = true;

            GUILayout.EndHorizontal();
        }
    }
}
