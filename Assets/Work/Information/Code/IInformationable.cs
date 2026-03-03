using UnityEngine;

namespace Work.Information.Code
{
    public interface IInformationable
    {
        InfoDataSO InfoData { get; }
        bool IsCanShowInfo { get; }
    }
}