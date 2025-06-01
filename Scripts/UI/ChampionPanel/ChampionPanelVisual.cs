using MoreMountains.Feedbacks;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChampionPanelVisual : MonoBehaviour
{
    [SerializeField] ChampionPanelController championPanelController;
    [SerializeField] Camera mainCamera; // Assign this in the Inspector or get it in code
    [SerializeField] MMF_Player upgradeFeedbacks;
    [SerializeField] Transform spawnPosition;
    [SerializeField] float bgColorLerpDuration;
    [Header("UI Images")]
    [SerializeField] Image ChampionRoleImage;
    [SerializeField] Image PassiveSkillImage;
    [SerializeField] Image ActiveSkillImage;

    [Header("UI Texts")]
    [SerializeField] TextMeshProUGUI ChampionName;
    [SerializeField] TextMeshProUGUI Level;
    [SerializeField] TextMeshProUGUI Role;
    [SerializeField] TextMeshProUGUI PassiveSkillDeclaration;
    [SerializeField] TextMeshProUGUI ActiveSkillDeclaration;

    [Header("Role Sprites")]
    [SerializeField] Sprite ArcherSprite;
    [SerializeField] Sprite WizardSprite;
    [SerializeField] Sprite AssasinSprite;
    [SerializeField] Sprite WarriorSprite;

    private void Awake()
    {
        mainCamera = Camera.main;
    }
    public void HandleChampionSpawnVisual(ChampionSO championSO , ChampionDummy dummy)
    {
        if(championSO == null) { Debug.LogError("There is a problem with ChampionPanelController class.Check Champion spawn logic!");}
        
        dummy.transform.forward = spawnPosition.forward;
        dummy.Reveal(spawnPosition.position);

        ChangeParticleAndCameraBGColor(championSO);
        SetupUIElements(championSO);
    }

    private void SetupUIElements(ChampionSO championSO)
    {
        switch (championSO.Role)
        {
            case ChampionRole.Archer:
                ChampionRoleImage.sprite = ArcherSprite;
                break;
            case ChampionRole.Wizard:
                ChampionRoleImage.sprite = WizardSprite;
                break;
            case ChampionRole.Assasin:
                ChampionRoleImage.sprite = AssasinSprite;
                break;
            case ChampionRole.Warrior:
                ChampionRoleImage.sprite = WarriorSprite;
                break;
        }

        PassiveSkillImage.sprite = championSO.PassiveSkillIcon;
        ActiveSkillImage.sprite = championSO.ActiveSkillSkillIcon;

        ChampionName.text = championSO.Name;
        Role.text = championSO.Role.ToString();
        PassiveSkillDeclaration.text = championSO.PassiveDeclaration;
        ActiveSkillDeclaration.text = championSO.ActiveSkillDeclaration;
    }

    private void ChangeParticleAndCameraBGColor(ChampionSO championSO)
    {
        mainCamera.clearFlags = CameraClearFlags.SolidColor;
        StopAllCoroutines();
        StartCoroutine(LerpColor(championSO.BackGroundColor));
    }

    private IEnumerator LerpColor(Color newColor)
    {
        Color start = mainCamera.backgroundColor;
        float t = 0f;
        while(t < bgColorLerpDuration)
        {
            t+= Time.deltaTime;
            Color lerped = Color.Lerp(start, newColor, t / bgColorLerpDuration);
            mainCamera.backgroundColor = lerped;
            yield return null;
        }
    }
    public void PlayGlobalFeedbacks()
    {
        if(upgradeFeedbacks) upgradeFeedbacks.PlayFeedbacks();
    }
}
