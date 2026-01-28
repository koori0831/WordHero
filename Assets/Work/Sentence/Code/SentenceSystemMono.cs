using System.Collections.Generic;
using UnityEngine;
using Work.Core.Utils.EventBus;
using Work.Input.Code;

namespace Work.Sentence.Code
{
    public sealed class SentenceSystemMono : MonoBehaviour
    {
        [Header("Databases")]
        [SerializeField] private List<SentenceTemplateSO> templates;

        [Header("Refs")]
        [SerializeField] private MonoBehaviour combatExecutorBehaviour; // ICombatExecutor 구현체
        private ICombatExecutor _executor;
        [SerializeField] private WordPaletteSO wordPaletteA, wordPaletteB;
        private int _wordPaletteIndex = 0;

        private SentenceBuilder _builder;
        private SentenceResolver _resolver;
        private SkillFactory _factory;

        private void Awake()
        {
            _executor = combatExecutorBehaviour as ICombatExecutor;
            if (_executor == null)
                Debug.LogError("combatExecutorBehaviour must implement ICombatExecutor");

            _builder = new SentenceBuilder(idleCancelSeconds: 10f, requireSubject: false, requireObject: true);
            _resolver = new SentenceResolver(templates, properBonusScore: 50);
            _factory = new SkillFactory();

            _builder.OnDirectAction += a => _executor?.ExecuteDirectAction(a);
            _builder.OnSentenceCompleted += HandleSentenceCompleted;

            InputEventSubscribe();
        }

        private void Update()
        {
            _builder.Tick(Time.deltaTime);
        }

        public void Input_WordPressed(int slotIndex, bool isReleased)
        {
            if (isReleased)
            {
                Input_WordReleased(slotIndex);
                return;
            }
            var palette = _wordPaletteIndex == 0 ? wordPaletteA : wordPaletteB;
            var word = palette.Words.Count > slotIndex ? palette.Words[slotIndex] : null;

            if (word != null)
                _builder.OnWordSlotPressed(slotIndex, word);
        }
        public void Input_WordReleased(int slotIndex) => _builder.OnWordSlotReleased(slotIndex);
        public void Input_Cancel(InputWordCancleEvent evt) => _builder.Cancel();
        public void Input_SwitchWordPalette(InputPaletteSwapEvent evt) => _wordPaletteIndex = (_wordPaletteIndex + 1) % 2;

        private void HandleSentenceCompleted(SentenceDraft draft)
        {
            var resolved = _resolver.Resolve(draft);
            if (resolved == null)
            {
                // fallback 정책(예: 약한 기본 공격 발동) - 취향껏
                Debug.LogWarning("No template matched. Fallback: BasicAttack");
                _executor?.ExecuteDirectAction(DirectActionType.BasicAttack);
                return;
            }

            var skill = _factory.Create(resolved);
            _executor?.ExecuteSkill(skill);
        }

        private void Input_SelectLeft(InputWordSellectLeftEvent evt) => Input_WordPressed(0, evt.IsRelease);
        private void Input_SelectUp(InputWordSellectUpEvent evt) => Input_WordPressed(1, evt.IsRelease);
        private void Input_SelectRight(InputWordSellectRightEvent evt) => Input_WordPressed(2, evt.IsRelease);
        private void Input_SelectDown(InputWordSellectDownEvent evt) => Input_WordPressed(3, evt.IsRelease);

        private void InputEventSubscribe()
        {
            Bus<InputWordCancleEvent>.Events += Input_Cancel;
            Bus<InputPaletteSwapEvent>.Events += Input_SwitchWordPalette;
            Bus<InputWordSellectLeftEvent>.Events += Input_SelectLeft;
            Bus<InputWordSellectUpEvent>.Events += Input_SelectUp;
            Bus<InputWordSellectRightEvent>.Events += Input_SelectRight;
            Bus<InputWordSellectDownEvent>.Events += Input_SelectDown;
        }

        private void OnDestroy()
        {
            Bus<InputWordCancleEvent>.Events -= Input_Cancel;
            Bus<InputPaletteSwapEvent>.Events -= Input_SwitchWordPalette;
            Bus<InputWordSellectLeftEvent>.Events -= Input_SelectLeft;
            Bus<InputWordSellectUpEvent>.Events -= Input_SelectUp;
            Bus<InputWordSellectRightEvent>.Events -= Input_SelectRight;
            Bus<InputWordSellectDownEvent>.Events -= Input_SelectDown;
        }
    }
}