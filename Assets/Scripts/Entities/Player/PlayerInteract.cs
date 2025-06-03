using EPOOutline;
using StarterAssets;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteract : MonoBehaviour
{
    public string interactableLayer = "Interactable";
    public string targetTag = "Target"; // Tag que activará el prefab
    public GameObject markerPrefab; // Prefab que instanciamos
    public Transform rayOrigin;
    public float rayDistance = 3f;

    private GameObject currentInteractable;
    private GameObject currentMarker; // Instancia del prefab

    public Vector3 collisionPoint;
    public Vector3 hitNormal;

    private PlayerInventory _inventory;

    void Start()
    {
        _inventory = GetComponent<PlayerInventory>();

        if (rayOrigin == null)
        {
            rayOrigin = transform;
        }
    }

    void Update()
    {
        DetectInteractable();

        // Si hay marcador, sigue actualizando su posición frente al jugador
        if (currentMarker != null && currentInteractable != null)
        {
            currentMarker.transform.position = collisionPoint;
            currentMarker.transform.rotation = Quaternion.LookRotation(hitNormal);
        }
    }

    void DetectInteractable()
    {
        Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);
        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.red);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            GameObject hitObj = hit.collider.gameObject;

            // Verifica si el PRIMER objeto golpeado está en la layer deseada
            if (hitObj.layer == LayerMask.NameToLayer(interactableLayer))
            {
                if (currentInteractable != hitObj)
                {
                    if (currentInteractable != null)
                    {
                        var prevOutline = currentInteractable.GetComponent<Outlinable>();
                        if (prevOutline != null) prevOutline.enabled = false;

                        if (currentMarker != null)
                        {
                            Destroy(currentMarker);
                            currentMarker = null;
                        }
                    }

                    currentInteractable = hitObj;
                    collisionPoint = hit.point;
                    hitNormal = hit.normal;

                    var outline = currentInteractable.GetComponent<Outlinable>();
                    if (outline != null)
                    {
                        if (!(currentInteractable.CompareTag(_inventory.ComputerTag) && _inventory.hasComputer) && 
                            !(currentInteractable.CompareTag(_inventory.ComputerTag) && !_inventory.hasPendrive) && 
                            !(currentInteractable.CompareTag(_inventory.TentTag) && _inventory.hasWaited))
                        {
                            outline.enabled = true;
                        }
                    }

                    if (currentInteractable.CompareTag(targetTag) && markerPrefab != null && _inventory.hasBomb && _inventory.bombsAmount > 0)
                    {
                        currentMarker = Instantiate(markerPrefab, collisionPoint, Quaternion.LookRotation(hit.normal));
                    }
                }
                else
                {
                    if (currentInteractable.CompareTag(_inventory.ComputerTag) && _inventory.hasComputer)
                    {
                        currentInteractable.GetComponent<Outlinable>().enabled = false;
                    }

                    if (currentInteractable.CompareTag(_inventory.TentTag) && _inventory.hasWaited)
                    {
                        currentInteractable.GetComponent<Outlinable>().enabled = false;
                    }

                    collisionPoint = hit.point;
                    hitNormal = hit.normal;
                }

                return;
            }
        }

        // Nada válido detectado
        if (currentInteractable != null)
        {
            var outline = currentInteractable.GetComponent<Outlinable>();
            if (outline != null) outline.enabled = false;

            if (currentMarker != null)
            {
                Destroy(currentMarker);
                currentMarker = null;
            }

            currentInteractable = null;
        }
    }



    public GameObject GetCurrentInteractable()
    {
        return currentInteractable;
    }
}
