using System.Collections;
using UnityEngine;
using Work.Weapons.Code;

namespace Work.Shops.Code
{
    public class WordModel : MonoBehaviour
    {
        [field:SerializeField] public ImprintType WordType { get; private set; }
    }
}