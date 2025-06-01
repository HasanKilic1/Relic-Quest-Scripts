using UnityEngine;
using UnityEngine.UI;

namespace TEST
{
    [RequireComponent(typeof(Button))]
    public class AspectRatioChanger : MonoBehaviour
    {
        private Button mButton;
        public enum AspectRatio
        {
            Aspect21x9,
            Aspect16x9,
            Aspect4x3,
            Aspect1x1
        }

        public AspectRatio targetAspectRatio = AspectRatio.Aspect16x9; // Default is 16:9
        public bool fullScreen = false; // Set to true for fullscreen mode
        private void Awake()
        {
            mButton = GetComponent<Button>();
        }
        private void OnEnable()
        {
            mButton.onClick.AddListener(ApplyAspectRatio);
        }

        public void ApplyAspectRatio()
        {
            float aspectRatioValue = GetAspectRatioValue(targetAspectRatio);
            SetResolutionForAspectRatio(aspectRatioValue);
        }

        private float GetAspectRatioValue(AspectRatio aspectRatio)
        {
            switch (aspectRatio)
            {
                case AspectRatio.Aspect21x9:
                    return 21f / 9f;
                case AspectRatio.Aspect4x3:
                    return 4f / 3f;
                case AspectRatio.Aspect16x9:
                    return 16f / 9f;
                case AspectRatio.Aspect1x1:
                    return 1f / 1f;
                default:
                    return 16f / 9f;
            }
        }

        private void SetResolutionForAspectRatio(float aspectRatio)
        {
            int screenWidth = Screen.width;
            int screenHeight = Screen.height;

            int targetHeight = Mathf.RoundToInt(screenWidth / aspectRatio);

            if (targetHeight > screenHeight)
            {
                int targetWidth = Mathf.RoundToInt(screenHeight * aspectRatio);
                Screen.SetResolution(targetWidth, screenHeight, FullScreenMode.MaximizedWindow);
                Debug.Log($"Resolution set to {Screen.resolutions[0]}x{Screen.resolutions[1]} for aspect ratio {aspectRatio:F2}");
            }
            else
            {
                Screen.SetResolution(screenWidth, targetHeight, fullScreen);
                Debug.Log($"Resolution set to {screenWidth}x{targetHeight} for aspect ratio {aspectRatio:F2}");
            }
        }
    }
}

