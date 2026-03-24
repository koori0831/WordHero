using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Work.Information.Code
{
    [CreateAssetMenu(fileName = "ModelInfoDataSO", menuName = "SO/Information/ModelInfoData")]
    public class ModelInfoDataSO : InfoDataSO
    {
        [field:SerializeField] public GameObject Model {  get; private set; }

        public new ModelInfoDataSO GetInfo()
        {
            ModelInfoDataSO data = new ModelInfoDataSO();
            return data;
        }
    }
}
