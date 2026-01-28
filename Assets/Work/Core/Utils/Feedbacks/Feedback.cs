using UnityEngine;

namespace Work.Core.Utils.Feedbacks
{
    public abstract class Feedback : MonoBehaviour
    {
        public abstract void CreateFeedback();

        public abstract void StopFeedback();
    }
}