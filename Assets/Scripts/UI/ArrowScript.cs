using UnityEngine;
using UnityEngine.EventSystems;

public class ArrowScript : MonoBehaviour
{

    public RectTransform rightArrow;
    public RectTransform leftArrow;
    private RectTransform currentTarget;

    public float distancex = 200f;

    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GameObject selected = EventSystem.current.currentSelectedGameObject;

        if (selected == null) return;
        
            RectTransform button = selected.GetComponent<RectTransform>();

        if (button == null) return;
            
            Vector3 targetPos = button.position;
            leftArrow.position = targetPos + Vector3.left * distancex;
            rightArrow.position = targetPos + Vector3.right * distancex;
        
    }
}
