using UnityEngine;

public class Hammer1 : MonoBehaviour
{
    private bool isPickedUp = false;
    private Transform playerTransform; 

    public float switchInterval = 0.5f; 

    public float pickupDuration = 10f;

    public bool IsHoldingHammer
    {
        get { return isPickedUp; }
    }

    private Vector3 backPositionOffset = new Vector3(0.5f, 0f, 0f); 
    private Vector3 abovePositionOffset = new Vector3(0f, 1f, 0f);
    private float switchTimer = 0f;
    private bool isBackPosition = true; 

    void Update()
    {
        if (isPickedUp && playerTransform != null)
        {
            switchTimer += Time.deltaTime;

            if (switchTimer >= switchInterval)
            {
                SwitchHammerPosition();
                switchTimer = 0f;
            }

            Vector3 targetPosition = isBackPosition ? playerTransform.position + GetBackOffset() : playerTransform.position + abovePositionOffset;

            transform.position = targetPosition;

            if (isBackPosition)
            {
                float rotationZ = playerTransform.localScale.x > 0 ? -90f : -180f;
                transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);
            }
            else
            {
                transform.rotation = Quaternion.identity;
            }
        }
    }

    void SwitchHammerPosition()
    {
        isBackPosition = !isBackPosition;
    }

    Vector3 GetBackOffset()
    {
        float direction = Mathf.Sign(playerTransform.localScale.x);
        return new Vector3(direction * backPositionOffset.x, backPositionOffset.y, backPositionOffset.z);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isPickedUp)
        {
            PickUp(other.transform);
        }
    }

    public void PickUp(Transform playerTransform)
    {
        isPickedUp = true;
        this.playerTransform = playerTransform; 

        Destroy(gameObject, pickupDuration);
    }
}
