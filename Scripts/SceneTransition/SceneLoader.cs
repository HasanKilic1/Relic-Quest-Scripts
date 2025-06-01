using MoreMountains.Feedbacks;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI loadingText;
    public static SceneName NextScene = SceneName.MainMenu;

    private void Start()
    {
        Time.timeScale = 1f;
        Debug.Log("--- NEXT SCENE " + NextScene.ToString() + " ---");
        StartCoroutine(LoadSceneAsync(NextScene));
    }

    private IEnumerator LoadSceneAsync(SceneName sceneName)
    {
        slider.value = 0f; // Reset slider
        slider.maxValue = 1f;
        slider.gameObject.SetActive(true);

        // Start loading the scene asynchronously
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName.ToString(), LoadSceneMode.Single);
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
            slider.value = Mathf.Max(progress, 0.7f); // Ensure progress is at least 0.7
            loadingText.text = "Loading... %" + (slider.value * 100).ToString("0.0");
            if (asyncOperation.progress >= 0.9f)
            {
                yield return new WaitForSeconds(2.5f);
                break;
            }
            yield return null;
        }
        asyncOperation.allowSceneActivation = true;
        slider.gameObject.SetActive(false);

        Resources.UnloadUnusedAssets(); ;
        System.GC.Collect();        
    }
}

public enum SceneName
{
    None,
    MainMenu,
    HighGarden,
    UnderGroundWorld,
    Dungeon,
    LoadingScreenScene
}