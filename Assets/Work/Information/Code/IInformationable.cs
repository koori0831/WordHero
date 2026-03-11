using UnityEngine;

namespace Work.Information.Code
{
    public interface IInformationable
    {
        InfoDataSO InfoData { get; }
        
    }


    public interface ISelectable : IInformationable
    {
        bool IsCanShowInfo { get; }
    }
}