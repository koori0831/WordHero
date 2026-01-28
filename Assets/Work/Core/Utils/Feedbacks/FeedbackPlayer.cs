using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Work.Core.Utils.Feedbacks
{
    public class FeedbackPlayer : MonoBehaviour
    {
        private Feedback[] feedbacks;

        public void Awake()
        {
            feedbacks = GetComponents<Feedback>();
        }

        public void PlayFeedbacks()
        {
            foreach (var feedback in feedbacks)
            {
                feedback.CreateFeedback();
            }
        }

        public void StopFeedbacks()
        {
            foreach (var feedback in feedbacks)
            {
                feedback.StopFeedback();
            }
        }
    }
}
