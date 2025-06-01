using UnityEngine;
using UnityEngine.UI;

public class MapPanel : MonoBehaviour
{
    [SerializeField] Button exitButton;

    private void Start()
    {
        exitButton.onClick.AddListener(() => { Destroy(gameObject); });
    }
}
