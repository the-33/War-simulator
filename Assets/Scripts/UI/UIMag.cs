using UnityEngine;
using UnityEngine.UI;

public class UIMag : MonoBehaviour
{
    private Image _image;
    public Image fillImage;
    public Image selectedImage;

    public float normalSize;
    public float selectedSize = 0.9f;

    public void updateMag(float amount, bool selected)
    {
        fillImage.fillAmount = amount;
        selectedImage.enabled = selected;
        transform.localScale = Vector3.one * (selected ? selectedSize : normalSize);

        if(amount == 0f)
        {
            _image.color = new Color32(255, 0, 0, 197);
            fillImage.color = new Color32(255, 0, 0, 148);
            selectedImage.color = new Color32(255, 0, 0, 197);
        }
        else if(amount <= 0.25f)
        {
            _image.color = new Color32(255, 255, 0, 197);
            fillImage.color = new Color32(255, 255, 0, 148);
            selectedImage.color = new Color32(255, 255, 0, 197);
        }
        else
        {
            _image.color = new Color32(255, 255, 255, 197);
            fillImage.color = new Color32(255, 255, 255, 148);
            selectedImage.color = new Color32(255, 255, 255, 197);
        }
    }   

    void Awake()
    {
        _image = gameObject.GetComponent<Image>();
        fillImage.fillAmount = 1;
        selectedImage.enabled = false;
        normalSize = transform.localScale.x;
        _image.color = new Color32(255, 255, 255, 197);
        fillImage.color = new Color32(255, 255, 255, 148);
        selectedImage.color = new Color32(255, 255, 255, 197);
    }
}
