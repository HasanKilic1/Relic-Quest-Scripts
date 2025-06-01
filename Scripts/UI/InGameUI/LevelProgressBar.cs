using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LevelProgressBar : MonoBehaviour
{
    [SerializeField] LevelProgressPoint[] levelPoints;
    [SerializeField] Slider progressSlider;
    [SerializeField] Slider bossSlider;

    [SerializeField] Sprite normalIcon;
    [SerializeField] Sprite bossIcon;
    [SerializeField] Sprite relicCardsIcon;
    [SerializeField] Sprite blacksmithIcon;

    private void OnEnable()
    {
        GameStateManager.OnLevelStart += UpdateLevelProgressBar;
        EnemyHealth.OnBossHealthChanged += UpdateBossHealthBar;
    }

    private void OnDisable()
    {
        GameStateManager.OnLevelStart -= UpdateLevelProgressBar;
        EnemyHealth.OnBossHealthChanged -= UpdateBossHealthBar;
    }

    private void UpdateBossHealthBar(float currentHealth, float maxHealth)
    {
        if(progressSlider.gameObject.activeInHierarchy) progressSlider.gameObject.SetActive(false);
        if(!bossSlider.gameObject.activeInHierarchy) bossSlider.gameObject.SetActive(true);

        bossSlider.maxValue = maxHealth;
        bossSlider.minValue = 0f;
        bossSlider.value = currentHealth;
    }

    private void UpdateLevelProgressBar(int newLevel)
    {
        if (!progressSlider.gameObject.activeInHierarchy) progressSlider.gameObject.SetActive(true);
        if (bossSlider.gameObject.activeInHierarchy) bossSlider.gameObject.SetActive(false);

        progressSlider.value = ((GameStateManager.Instance.CurrentLevel - 1) % 5) * 0.25f;
        Debug.Log("new progress bar value :" +  progressSlider.value);
        if (newLevel % 5 == 1)
        {
            SetupPoints();
        }
    }

    private void SetupPoints()
    {
        for (int i = 0; i < levelPoints.Length; i++)
        {
            int level = GameStateManager.Instance.CurrentLevel + i;
            levelPoints[i].SetupUI(GetSpriteOnLevel(level), level);
        }
    }

    public Sprite GetSpriteOnLevel(int level)
    {
        if (GameStateManager.Instance.CardLevels.Contains(level)) { return relicCardsIcon; }
        else if (GameStateManager.Instance.BlackSmithLevels.Contains(level)) { return blacksmithIcon; }
        return normalIcon;
    }
}
