using UnityEngine;
using UnityEngine.UI;

public class ImageGradient : MonoBehaviour
{
    public Gradient Gradient;
    public Image Image;

    private void Update()
    {
        Evaluate();
    }

    private void Evaluate()
    {
        Image.color = Gradient.Evaluate(Image.fillAmount);
    }
}
