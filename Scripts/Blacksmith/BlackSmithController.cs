using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BlackSmithController : MonoBehaviour
{
    [SerializeField] Transform mainPanelCamLook;
    [SerializeField] Transform playerPosition;
    [SerializeField] Vector3 mainFollowOffset;
    [SerializeField] Vector3 mainLookOffset;
    BlackSmithCamera blackSmithCamera;
    [SerializeField] StatChanger[] statChangers;
    [SerializeField] RuneTable runeTable;
    [SerializeField] PotionTable potionTable;
    [Header("Panel Buttons")]
    [SerializeField] Button StatsButton;
    [SerializeField] Button PotionsButton;
    [SerializeField] Button RunesButton;
    [Header("Transition Buttons")]
    public Button RightTransitionButton;
    public Button LeftTransitionButton;
    [Header("Selection Button")]
    [SerializeField] Button SelectButton;
    [SerializeField] TextMeshProUGUI priceText;
    [SerializeField] GameObject gemVisual;
    [SerializeField] GameObject coinVisual;
    [SerializeField] GameObject adVisual;
    [Header("Exit Button")]
    [SerializeField] Button exitButton;

    [Header("Info")]
    [SerializeField] GameObject[] gamePadInfoObjects;
    int selectedID = 0;

    private BlackSmithPanel Panel;
    private IBlacksmithComponent currentComponent;
    private BlackSmithPriceHolder priceHolder;

    private List<GameObject> closeOnMainPanel;
    private bool transitionValid = true;

    void Start()
    {
        blackSmithCamera = CameraController.Instance.blackSmithVCam.GetComponent<BlackSmithCamera>();
        CameraController.Instance.OpenBlacksmithCamera();
        SetupButtons();
        AssignClosedObjectsOnMainPanel();
        SetPanel(BlackSmithPanel.Main);

        HideOrShowInfosPlatformRelatively();

        PlayerHealth.Instance.GetComponent<PlayerStateMachine>().inputClosed = true;
        
        Invoke(nameof(LateInitialize), 1f);
    }
    void Update()
    {
        if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            RightTransition();
        }
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            LeftTransition();
        }

        if (HasGamePad)
        {
            if (Gamepad.current.rightShoulder.wasPressedThisFrame)
            {
                RightTransition();
            }
            if (Gamepad.current.leftShoulder.wasPressedThisFrame)
            {
                LeftTransition();
            }
        }

        RePositionPlayer();
    }
    public void Initialize()
    {
        CameraController.Instance.OpenBlacksmithCamera();
        InGameUI.Instance.HideOnBlacksmithEnter();
    }

    private void LateInitialize()
    {
        EventSystem.current.SetSelectedGameObject(RunesButton.gameObject);
    }

    private void RePositionPlayer()
    {
        PlayerHealth.Instance.transform.position = playerPosition.position;
    }

    private void SetupButtons()
    {
        SelectButton.onClick.AddListener(TrySelectHighlighted);
        RightTransitionButton.onClick.AddListener(() => MakeTransitionBetweenComponents(true));
        LeftTransitionButton.onClick.AddListener(() => MakeTransitionBetweenComponents(false));
        StatsButton.onClick.AddListener(() => SetPanel(BlackSmithPanel.Stats));
        RunesButton.onClick.AddListener(() => SetPanel(BlackSmithPanel.Runes));
        PotionsButton.onClick.AddListener(() => SetPanel(BlackSmithPanel.Potions));
        exitButton.onClick.AddListener(Exit);
    }

    private void HideOrShowInfosPlatformRelatively()
    {
        foreach (var obj in gamePadInfoObjects)
        {
            obj.SetActive(false);
        }

        if (GameStateManager.Instance.GamePlatform == GamePlatform.PC) // Make sure current platform is PC
        {
            if (Gamepad.current != null) // Make sure there is binded gamePad
            {
                foreach (var obj in gamePadInfoObjects)
                {
                    obj.SetActive(true);
                }
                Debug.Log("Gamepad binded and infos will be shown");
            }
        }
        else
        {
            foreach (var obj in gamePadInfoObjects) // Close when platform is mobile
            {
                obj.SetActive(false);
            }
            Debug.Log("Current platform is mobile");
        }
    }

    

    //These objects will be disabled at main panel
    private void AssignClosedObjectsOnMainPanel()
    {
        closeOnMainPanel = new List<GameObject>{ SelectButton.gameObject,
                                                 RightTransitionButton.gameObject,  
                                                 LeftTransitionButton.gameObject};
    }

    private void RightTransition()
    {
        MakeTransitionBetweenComponents(isRight: true);
    }

    private void LeftTransition()
    {
        MakeTransitionBetweenComponents(isRight: false);
    }   

    public void SetPanel(BlackSmithPanel Panel)
    {
        this.Panel = Panel;
        switch (Panel)
        {
            case BlackSmithPanel.Main:
                closeOnMainPanel.ForEach(obj => obj.SetActive(false));
                blackSmithCamera.AssignTarget(mainPanelCamLook, mainFollowOffset, mainLookOffset);
                break;
            case BlackSmithPanel.Stats:
                closeOnMainPanel.ForEach(obj => obj.SetActive(true));
                blackSmithCamera.AssignTarget(statChangers[0].followTransform, statChangers[0].CamFollowOffset, statChangers[0].CamLookOffset);
                currentComponent = statChangers[0].GetComponent<IBlacksmithComponent>();
                priceHolder = statChangers[0].GetComponent<BlackSmithPriceHolder>();

                break;
            case BlackSmithPanel.Potions:
                closeOnMainPanel.ForEach(obj => obj.SetActive(true));

                blackSmithCamera.AssignTarget(potionTable.CamPosition, potionTable.camFollow, potionTable.camLook);
                currentComponent = potionTable.GetComponent<IBlacksmithComponent>();
                priceHolder = potionTable.currentPotionVisual.GetComponent<BlackSmithPriceHolder>();
                break;
            case BlackSmithPanel.Runes:
                closeOnMainPanel.ForEach(obj => obj.SetActive(true));

                blackSmithCamera.AssignTarget(runeTable.camTarget, runeTable.camFollowOffset, runeTable.camLookOffset);
                currentComponent = runeTable.GetComponent<IBlacksmithComponent>();
                priceHolder = runeTable.currentRunePriceHolder.GetComponent<BlackSmithPriceHolder>();
                break;
        }
        RefreshPriceButton();
    }

    private void MakeTransitionBetweenComponents(bool isRight)
    {
        if(!transitionValid) return;
        switch (Panel)
        {
            case BlackSmithPanel.Main:
                break;
            case BlackSmithPanel.Stats:

                if (isRight)
                {
                    selectedID++;
                    if (selectedID == statChangers.Length) { selectedID = 0; }
                }
                else
                {
                    selectedID--;
                    if (selectedID < 0) { selectedID = statChangers.Length - 1; }
                }

                blackSmithCamera.AssignTarget(statChangers[selectedID].followTransform, statChangers[selectedID].CamFollowOffset , statChangers[selectedID].CamLookOffset);
                currentComponent = statChangers[selectedID].GetComponent<IBlacksmithComponent>();
                priceHolder = statChangers[selectedID].GetComponent<BlackSmithPriceHolder>();

                RefreshPriceButton();
                //No need to enter cooldown on stats panel
                break;

            case BlackSmithPanel.Potions:
                potionTable.ChangePotion(isRight);
                priceHolder = potionTable.currentPotionVisual.GetComponent<BlackSmithPriceHolder>();
                StartCoroutine(TransitionCooldownRoutine()); 
                break;

            case BlackSmithPanel.Runes:
                runeTable.ChangeRune(isRight);
                priceHolder = runeTable.currentRunePriceHolder.GetComponent<BlackSmithPriceHolder>();
                StartCoroutine(TransitionCooldownRoutine());
                break;
        }
        RefreshPriceButton();
        
    }

    private IEnumerator TransitionCooldownRoutine()
    {
        transitionValid = false; RightTransitionButton.interactable = false; LeftTransitionButton.interactable = false;
        yield return new WaitForSeconds(1.5f);
        transitionValid = true; RightTransitionButton.interactable = true; LeftTransitionButton.interactable = true;
    }

    private void TrySelectHighlighted()
    {
        if (priceHolder.CanDemand())
        {
            priceHolder.Spend();
            currentComponent.Select();
            RefreshPriceButton();
        }
    }

    private void RefreshPriceButton()
    {
        if (priceHolder == null || !SelectButton.gameObject.activeInHierarchy) return;

        SelectButton.interactable = priceHolder.CanDemand();
        priceText.text = priceHolder.GetPrice().ToString();

        switch (priceHolder.priceType)
        {
            case PriceType.Coin:
                gemVisual.gameObject.SetActive(false);
                adVisual.gameObject.SetActive(false);

                coinVisual.gameObject.SetActive(true);
                break;
            case PriceType.Gem:

                coinVisual.gameObject.SetActive(false);
                adVisual.gameObject.SetActive(false);

                gemVisual.gameObject.SetActive(true);
                break;
            case PriceType.Ads:

                coinVisual.gameObject.SetActive(false);
                gemVisual.gameObject.SetActive(false);

                adVisual.gameObject.SetActive(true);
                break;
        }
    }

    private void Exit()
    {
        PlayerHealth.Instance.GetComponent<PlayerStateMachine>().inputClosed = false;

        blackSmithCamera.ReturnStartSettings();
        CameraController.Instance.OpenMainCamera();
        InGameUI.Instance.ShowOnBlackSmithExit();
        GameStateManager.Instance.StartNewLevel();
        GiftManager.Instance.CloseBlacksmith();
        Destroy(gameObject);
    }
    private bool HasGamePad => Gamepad.current != null;
    public enum BlackSmithPanel
    {
        Main,
        Stats,
        Potions,
        Runes
    }
}