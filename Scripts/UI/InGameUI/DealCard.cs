using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class DealCard : MonoBehaviour
{
    [SerializeField] Image resourceIcon;
    [SerializeField] TextMeshProUGUI resourceDeclaration;
    [SerializeField] Image[] paintableFrames;    
    [SerializeField] Color TakeColor; // green
    [SerializeField] Color GiveColor; // red
    [SerializeField] Transform arrow;
    [SerializeField] MMF_Player initFeedbacks;
    MMF_TMPTextReveal textReveal;

    public void SetupDealCard(TransactionData transactionData)
    {
        SetupUIProperties(transactionData);        
    }
    private void SetupUIProperties(TransactionData transactionData)
    {
        resourceIcon.sprite = transactionData.Icon;
        Color paint = transactionData.DealType == DealType.GiveToPlayer ? TakeColor : GiveColor;
        resourceDeclaration.color = paint;
        resourceDeclaration.text = transactionData.UI_Declaration;
        arrow.transform.localRotation = transactionData.DealType == DealType.GiveToPlayer ? Quaternion.Euler(0, 90, 0) : Quaternion.Euler(0, -90, 0);

        foreach (var frame in paintableFrames)
        {
            frame.color = paint;                        
        }

        textReveal = initFeedbacks.GetFeedbackOfType<MMF_TMPTextReveal>();
        textReveal.NewText = resourceDeclaration.text;
        initFeedbacks.PlayFeedbacks();
    }
   
}
