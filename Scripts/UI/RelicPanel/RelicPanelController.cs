using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RelicPanelController : MonoBehaviour
{
    [SerializeField] TutorialPlayer tutorialPlayer;
    [SerializeField] RelicSlot relicSlot;
    [SerializeField] Transform contentHolder;
    [SerializeField] Scrollbar scrollbar;
    [SerializeField] List<Tutorial> tutorials;
    void Start()
    {
        scrollbar.value = 1;

        for (int i = 0; i < CardDataManager.Instance.RelicSOs.Count; i++) 
        {
            Instantiate(relicSlot , contentHolder);
            relicSlot.Setup(CardDataManager.Instance.RelicSOs[i]);
        }
        tutorialPlayer.PlayTutorialsFromScratch(tutorials);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
