using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CooldownUI))]
[RequireComponent(typeof(Button))]
public class InputUI : MonoBehaviour , IPointerUpHandler
{
    Button button;
    public enum InputType
    {
        Roll,
        Ability,
        Rune,
        Potion
    }
    public InputType type;

    private void Awake()
    {
        button = GetComponent<Button>();
    }
    void Start()
    {
        button.onClick.AddListener(CallPressed);
    }

    private void CallPressed()
    {
        switch (type)
        {
            case InputType.Roll:
                InputReader.Instance.CallRoll();
                break;
            case InputType.Ability:
                InputReader.Instance.CallAbility();
                break;
            case InputType.Rune:
                InputReader.Instance.CallRunePressed();
                break;
            case InputType.Potion:
                InputReader.Instance.CallPotion();
                break;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(type == InputType.Rune) // rune release action
        {
            InputReader.Instance.CallRuneReleased();
        }
    }
}
