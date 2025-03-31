using UnityEngine;

namespace UI
{
    public class SafeArea : MonoBehaviour
    {
        private void Awake()
        {
            if (!Application.isMobilePlatform)
            {
                Debug.LogWarning("Not on mobile platform. Disabling auto safe area adjustment.");
                return;
            }
            
            var rectTransform = GetComponent<RectTransform>();
            var safeArea = Screen.safeArea;

            var offset = safeArea.height - Screen.height;
            Debug.Log($"Offset: {offset}, from safe height: {safeArea.height} and from screen height: {Screen.height}");
            
            var offsetMax = rectTransform.offsetMax;
            offsetMax.y = offset;
            rectTransform.offsetMax = offsetMax;
        }
    }
}
