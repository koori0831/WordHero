using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Work.Enemies.Code
{
    public enum BTVariables
    {
        Target,
        MoveDirection,
        CurrentState,
    }

    [CreateAssetMenu(fileName = "VariableSO" , menuName = "SO/Behavior/VariableData")]
    public class VariableSO : ScriptableObject
    {
        [field:SerializeField] public BTVariables VariableName { get; private set; }
    }
}
