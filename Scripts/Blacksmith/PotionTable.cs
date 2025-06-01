using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PotionTable : MonoBehaviour , IBlacksmithComponent
{
    [SerializeField] private BlackSmithController BlackSmithController;
    private PotionUser potionUser;
    [SerializeField] List<PotionSO> Potions;
    [SerializeField] Transform potionStartPos;
    [SerializeField] Transform potionMidPos;
    [SerializeField] Transform potionEndPos;
    public Transform CamPosition;
    public Vector3 camFollow;
    public Vector3 camLook;

    public PotionSO currentPotionSO;
    public PositionAnimationer currentPotionVisual;
    private int potionID = 0;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI declarationText;
    [SerializeField] MMF_Player textRevealFB;
    MMF_TMPTextReveal textReveal;

    private void Start()
    {
        textReveal = textRevealFB.GetFeedbackOfType<MMF_TMPTextReveal>();
        potionUser = PlayerHealth.Instance.GetComponent<PotionUser>();
        ShowNewPotion();             
    }
    public void ChangePotion(bool isRightButton)
    {
        if (currentPotionVisual != null)
        {
            currentPotionVisual.startPoint = potionMidPos.position;
            currentPotionVisual.endPoint = potionEndPos.position;
            currentPotionVisual.Animate();

            Destroy(currentPotionVisual.gameObject, 1f);
        }
        ChangePotionID(isRightButton);

        ShowNewPotion();
    }

    private void ChangePotionID(bool isRightButton)
    {
        if (isRightButton)
        {         
            potionID++;
            if (potionID == Potions.Count)
            {
                potionID = 0;
            }
        }
        else
        {
            potionID--;
            if (potionID < 0)
            {
                potionID = Potions.Count - 1;
            }
        }
    }

    private void ShowNewPotion()
    {
        currentPotionSO = Potions[potionID];
        if (currentPotionSO != null)
        {
            currentPotionVisual = Instantiate(currentPotionSO.BlackSmithVisual);
            currentPotionVisual.transform.SetParent(transform);
            currentPotionVisual.startPoint = potionStartPos.position;
            currentPotionVisual.endPoint = potionMidPos.position;
            currentPotionVisual.Animate();
            declarationText.color = currentPotionSO.DeclarationTextColor;
            textReveal.NewText = currentPotionSO.Declaration;
            textRevealFB.PlayFeedbacks();
        }
    }
    public void Select()
    {
        potionUser.AddPotion(currentPotionSO);
    }
}
