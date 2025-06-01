using UnityEngine;
using UnityEngine.UI;

namespace TEST
{
    [RequireComponent(typeof(Button))]
    public class ScreenResolutionChanger : MonoBehaviour
    {
        public enum ScreenResolutionButtonType
        {
            R_1920_1080,
            R_2560_1440,
            R_3840_2160,
            R_3440_1440
        }
        [SerializeField] FullScreenMode mode;
        [SerializeField] ScreenResolutionButtonType resolutionButtonType;
        private Resolution originalResolution;
        private FullScreenMode originalFullScreenMode;

        private Button selectButton;

        private void Awake()
        {
            selectButton = GetComponent<Button>();
        }
        void Start()
        {
            // Save the current settings so they can be restored later if needed
            originalResolution = Screen.currentResolution;
            originalFullScreenMode = Screen.fullScreenMode;

            selectButton.onClick.AddListener(ApplyResolution);
        }

        public void SetResolution(int width, int height, FullScreenMode fullScreenMode)
        {
            Screen.SetResolution(width, height, fullScreenMode);
            Debug.Log($"Screen resolution set to {width}x{height}, FullScreenMode: {fullScreenMode}");
        }

        public void ResetResolution()
        {
            SetResolution(originalResolution.width, originalResolution.height, originalFullScreenMode);
        }

        public string GetCurrentResolution()
        {
            return $"{Screen.width}x{Screen.height} @ {Screen.currentResolution.refreshRateRatio}Hz, FullScreenMode: {Screen.fullScreenMode}";
        }

        public void SetResolution1080p(FullScreenMode fullScreenMode)
        {
            SetResolution(1920, 1080, fullScreenMode);
        }

        public void SetResolution1440p(FullScreenMode fullScreenMode)
        {
            SetResolution(2560, 1440, fullScreenMode);
        }

        public void SetResolution4K(FullScreenMode fullScreenMode)
        {
            SetResolution(3840, 2160, fullScreenMode);
        }

        public void SetResolutionUltraWide(FullScreenMode fullScreenMode)
        {
            SetResolution(3440, 1440, fullScreenMode);
        }

        public void ToggleFullScreen()
        {
            FullScreenMode newMode = Screen.fullScreenMode == FullScreenMode.Windowed ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
            SetResolution(Screen.width, Screen.height, newMode);
            Debug.Log($"Fullscreen mode set to: {Screen.fullScreenMode}");
        }

        private void ApplyResolution()
        {
            switch (resolutionButtonType)
            {
                case ScreenResolutionButtonType.R_1920_1080:
                    SetResolution1080p(mode);
                    break;
                case ScreenResolutionButtonType.R_2560_1440:
                    SetResolution1440p(mode);
                    break;
                case ScreenResolutionButtonType.R_3840_2160:
                    SetResolution4K(mode);
                    break;
                case ScreenResolutionButtonType.R_3440_1440:
                    SetResolutionUltraWide(mode);
                    break;
            }
        }
    }
}