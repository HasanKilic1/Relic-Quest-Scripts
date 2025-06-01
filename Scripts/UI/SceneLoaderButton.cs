using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SceneLoaderButton : MonoBehaviour
{
    [SerializeField] SceneName sceneToLoad;
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(SwitchLoadingScreen);
    }

    public void SwitchLoadingScreen()
    {
        SceneLoader.NextScene = sceneToLoad;
        SceneManager.LoadScene(SceneName.LoadingScreenScene.ToString());
    }
}
