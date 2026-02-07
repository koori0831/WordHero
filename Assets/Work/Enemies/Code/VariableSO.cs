#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
namespace Work.Enemies.Code
{
    public enum BTVariables
    {
        Target,
        CurrentState,
        CurrentAnimation,
        NextAnimation,
        ChangeAnimationEvent,
        ChangeStateEvent,
        DetectRange,
        TargetLayerNumber,
        AttackRange,
        ChaseRange,
        PatrolRange,
        RunSpeed,
        WalkSpeed,
        PatrolPointCount,
    }

    [CreateAssetMenu(fileName = "VariableSO", menuName = "SO/Behavior/VariableData")]
    public class VariableSO : ScriptableObject
    {
        [field: SerializeField] public BTVariables VariableName { get; private set; }

#if UNITY_EDITOR
        private void OnValidate()
        {
            try
            {
                string path = AssetDatabase.GetAssetPath(this);
                if (string.IsNullOrEmpty(path)) return;

                string newName = VariableName.ToString();
                AssetDatabase.RenameAsset(path, newName);
                AssetDatabase.SaveAssets();
            }
            catch { }
        }
#endif
    }
}
