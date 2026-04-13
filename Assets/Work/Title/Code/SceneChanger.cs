using UnityEngine;

namespace Work.Title.Code
{
    public class SceneChanger : MonoBehaviour
    {
        [SerializeField] private string sceneName;

        public void ChangeScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
    }
}