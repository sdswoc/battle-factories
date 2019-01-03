using UnityEngine.UI;
using UnityEngine;

namespace UI
{
    public class FPS : MonoBehaviour
    {
        public int avgFrameRate;
        public Text display_Text;

        private void Awake()
        {
            Application.targetFrameRate = 60;
            if (!Application.isEditor)
            {
                gameObject.SetActive(false);
            }
        }
        public void Update()
        {
            float current = 0;
            current = (int)(1f / Time.unscaledDeltaTime);
            avgFrameRate = (int)current;
            display_Text.text = avgFrameRate.ToString() + " FPS";
        }
    }
}