# WordHero

## Sentence ↔ StatSystem 테스트용 빠른 셋업 가이드

아래 순서대로 SO를 만들어 SampleScene에서 바로 테스트할 수 있습니다.

### 1) StatSO 준비
1. `Assets/Work/StatSystem/SO` 폴더에서 `StatSO` 생성  
   - 예: `AttackPower` (StatName: `AttackPower`, BaseValue: 10, Min: 0, Max: 999)
2. 플레이어/테스트 엔티티에 `EntityStatCompo`가 있어야 하고, 해당 컴포넌트의 `statOverrides`에 위 `StatSO`를 추가합니다.

### 2) WordDefinitionSO 준비 (문장 구성용)
다음 SO들을 `Assets/Work/Sentence/SO/Word` 등에 생성합니다.

- **Object**: `object_attackpower`  
  - PartOfSpeech: Object  
  - Linked Stat: `AttackPower` (위에서 만든 StatSO 연결)
- **Verb**: `verb_buff`  
  - PartOfSpeech: Verb  
  - HasNumericValue: On  
  - NumericKind: Percent  
  - NumericValue: 20  (=> +20%)
- **Option**: `option_seconds`  
  - PartOfSpeech: Option  
  - HasNumericValue: On  
  - NumericKind: DurationSeconds  
  - NumericValue: 5  (=> 5초간)

> 필요하다면 Percent 대신 Flat(정수 증가)을 쓰면 `+n` 형태로 적용됩니다.

### 3) EffectGraphSO + SentenceTemplateSO 준비
1. `EffectGraphSO` 생성  
   - Kind: Buff  
   - BaseDuration: 2 (기본값, 옵션으로 덮어쓰면 무시됨)  
   - **ApplyStatModifier: On**  
   - TargetStat: 비워도 됨 (Object의 LinkedStat가 우선 적용됨)  
   - StatOp/StatAmount: 기본값 유지해도 됨 (Verb/Option 숫자 토큰이 있으면 그 값 사용)
2. `SentenceTemplateSO` 생성  
   - AllowOption: On  
   - ObjectTags/VerbTags/OptionTags는 원하는대로 (테스트는 비워둬도 됨)  
   - Effect에 위 `EffectGraphSO` 연결

### 4) SentenceSystem에 템플릿 등록
SampleScene의 `SentenceSystem` 오브젝트에서 `templates` 리스트에 위 `SentenceTemplateSO`를 추가합니다.

### 5) 테스트 방법
1. 게임 실행 후 문장을 다음처럼 구성합니다.  
   - **Object**: attackpower  
   - **Verb**: buff(20%)  
   - **Option**: 5초  
2. 스킬 발동 시 `PlayerCombatExecutor`가 `StatApplyModifierEvent`를 올리고,  
   `EntityStatCompo`가 `AttackPower`에 **5초간 +20%** 버프를 적용합니다.

> 실시간 값 변화 확인은 `EntityStatCompo`/`StatSO` 로그나 UI 바인딩으로 확인 가능합니다.
