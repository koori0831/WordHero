using UnityEngine;
using Work.Interaction.Code;

namespace Work.Chests.Code
{
    public class Chest : MonoBehaviour
    {
        [SerializeReference]
        public ICollectAction CollectAction;


    }
}