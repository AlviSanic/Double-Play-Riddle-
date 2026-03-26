using UnityEngine;

namespace Utility {
    public class FPSDisplay : MonoBehaviour {
        private float _deltaTime;

        private void Update() {
            if (!enabled) {
                return;
            }
            _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
        }

        private void OnGUI() {
            if (!enabled) {
                return;
            }
            int w = Screen.width, h = Screen.height;

            GUIStyle style = new GUIStyle();
            Rect rect = new Rect(10, 10, w, h * 2 * 0.01f);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = 22;
            style.normal.textColor = Color.white;

            float fps = 1.0f / _deltaTime;
            string text = $"FPS: {fps:F1}";
            GUI.Label(rect, text, style);
        }
    }
}
