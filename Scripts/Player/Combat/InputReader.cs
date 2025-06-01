using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour
{
    [SerializeField] Joystick movementJoystick;
    public static InputReader Instance{get; private set;}
    private ActionMap actionMap;
    private PlayerStateMachine playerStateMachine;

    public static event Action RollAction;
    public static event Action AbilityAction;
    public static event Action PotionAction;
    public static event Action RunePressedAction;
    public static event Action RuneReleasedAction;
    public static event Action UIRightAction;
    public static event Action UILeftAction;
    public static event Action SouthButtonPressed;
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        actionMap = new ActionMap();
        actionMap.Player.Enable();

        SubscribeToInputActions();
    }

    private void Start()
    {
        playerStateMachine = PlayerController.Instance.GetComponent<PlayerStateMachine>();
    }

    private void OnEnable()
    {
        actionMap.Player.Enable();
        SubscribeToInputActions();
    }

    private void OnDisable()
    {
        UnsubscribeFromInputActions();           
        actionMap.Player.Disable();
    }

    private void SubscribeToInputActions()
    {
        actionMap.Player.Roll.performed += OnRollPerformed;
        actionMap.Player.Ability.performed += OnAbilityPerformed;
        actionMap.Player.Potion.performed += OnPotionPerformed;

        actionMap.Player.Rune.performed += OnRunePerformed;
        actionMap.Player.Rune.canceled += OnRunePerformed;

        actionMap.Player.UIRightTransition.performed += OnUIRightPerformed;
        actionMap.Player.UILeftTransition.performed += OnUILeftPerformed;
        actionMap.Player.SouthButton.performed += OnSouthButtonPerformed;
    }

    private void UnsubscribeFromInputActions()
    {
        actionMap.Player.Roll.performed -= OnRollPerformed;
        actionMap.Player.Ability.performed -= OnAbilityPerformed;
        actionMap.Player.Potion.performed -= OnPotionPerformed;

        actionMap.Player.Rune.performed -= OnRunePerformed;
        actionMap.Player.Rune.canceled -= OnRunePerformed;

        actionMap.Player.UIRightTransition.performed -= OnUIRightPerformed;
        actionMap.Player.UILeftTransition.performed -= OnUILeftPerformed;
        actionMap.Player.SouthButton.performed -= OnSouthButtonPerformed;
    }

    #region Mobile
    public void CallRoll() => RollAction?.Invoke();
    public void CallAbility() => AbilityAction?.Invoke();
    public void CallPotion() => PotionAction?.Invoke();
    public void CallRunePressed() => RunePressedAction?.Invoke();
    public void CallRuneReleased() => RuneReleasedAction?.Invoke();
    #endregion

    #region PC
    public Vector2 GetMovementVector()
    {
        Vector2 movementVector = actionMap.Player.Movement.ReadValue<Vector2>();
        if (GameStateManager.Instance.GamePlatform == GamePlatform.Mobile)
        {
            movementVector = new Vector2(movementJoystick.Horizontal, movementJoystick.Vertical);
        }
        return movementVector.normalized;
    }

    private void OnRollPerformed(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            RollAction?.Invoke();
        }
    }

    private void OnAbilityPerformed(InputAction.CallbackContext context)
    {
        if(playerStateMachine.inputClosed) { return; }
        if(context.performed)
        {
            AbilityAction?.Invoke();
        }
    }

    public void OnPotionPerformed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PotionAction?.Invoke();
        }
    }

    private void OnRunePerformed(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            RunePressedAction?.Invoke();
        }
        if (context.phase == InputActionPhase.Canceled)
        {
            RuneReleasedAction?.Invoke();
        }
    }

    private void OnUILeftPerformed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            UILeftAction?.Invoke();
        }
    }

    private void OnUIRightPerformed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            UIRightAction?.Invoke();
        }
    }
    private void OnSouthButtonPerformed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SouthButtonPressed?.Invoke();
        }
    }
    #endregion
}
