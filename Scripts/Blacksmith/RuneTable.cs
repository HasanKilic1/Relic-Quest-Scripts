using MoreMountains.Feedbacks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RuneTable : MonoBehaviour , IBlacksmithComponent
{
    RuneUser runeUser;
    public Transform camTarget;
    public Vector3 camFollowOffset;
    public Vector3 camLookOffset;
    [SerializeField] List<RuneSO> runes;
    [SerializeField] Transform runeVisualSpawnPos;
    public RuneSO currentRuneSO { get; private set; }
    public BlackSmithPriceHolder currentRunePriceHolder;

    private int currentRuneID;
    private GameObject currentVisual;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI declarationText;
    [SerializeField] MMF_Player textRevealFB;
    MMF_TMPTextReveal textReveal;

    private void Start()
    {
        textReveal = textRevealFB.GetFeedbackOfType<MMF_TMPTextReveal>();
        ChangeRune(true);
        runeUser = PlayerController.Instance.GetComponent<RuneUser>();
    }
    public void ChangeRune(bool isRight)
    {
        ChangeRuneID(isRight);

        if (currentVisual != null) { Destroy(currentVisual); }
        currentVisual = Instantiate(currentRuneSO.BlackSmithVisual, runeVisualSpawnPos.position, Quaternion.identity);
        currentVisual.transform.SetParent(transform);
        if (currentVisual.TryGetComponent(out BlackSmithPriceHolder priceHolder))
        {
            currentRunePriceHolder = priceHolder;
        }
        else
        {
            Debug.LogError("Current rune visual does not have priceHolder component " + currentRuneSO.Name);
        }
        textReveal.NewText = currentRuneSO.Declaration;
        textRevealFB.PlayFeedbacks();
    }

    private void ChangeRuneID(bool isRight)
    {
        if (isRight)
        {
            currentRuneID++;
            if (currentRuneID > runes.Count - 1) { currentRuneID = 0; }
        }
        else
        {
            currentRuneID--;
            if (currentRuneID < 0)
            {
                currentRuneID = runes.Count - 1;
            }
        }
        currentRuneSO = runes[currentRuneID];
    }

    public void Select()
    {
        runeUser.SetRune(currentRuneSO);
    }
}
