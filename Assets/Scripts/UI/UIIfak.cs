using UnityEngine;
using UnityEngine.UI;

public class UIIfak : MonoBehaviour
{
    private Image _image;
    public Image fillImage;
    public Image selectedImage;

    public float normalSize;
    public float selectedSize = 1f;

    public void updateIfak(float amount, bool selected)
    {
        fillImage.fillAmount = amount;
        selectedImage.enabled = selected;
        transform.localScale = Vector3.one * (selected ? selectedSize : normalSize);

        if (amount == 0f)
        {
            _image.color = new Color32(255, 0, 0, 197);
            fillImage.color = new Color32(255, 0, 0, 72);
            selectedImage.color = new Color32(255, 0, 0, 197);
        }
        else if (amount == 0.2f)
        {
            _image.color = new Color32(255, 150, 0, 197);
            fillImage.color = new Color32(255, 150, 0, 72);
            selectedImage.color = new Color32(255, 150, 0, 197);
        }
        else if (amount <= 0.4f)
        {
            _image.color = new Color32(255, 255, 0, 197);
            fillImage.color = new Color32(255, 255, 0, 72);
            selectedImage.color = new Color32(255, 255, 0, 197);
        }
    }

    void Awake()
    {
        _image = gameObject.GetComponent<Image>();
        fillImage.fillAmount = 1;
        selectedImage.enabled = false;
        normalSize = transform.localScale.x;
        _image.color = new Color32(255, 255, 255, 197);
        fillImage.color = new Color32(255, 255, 255, 72);
        selectedImage.color = new Color32(255, 255, 255, 197);
    }
}
