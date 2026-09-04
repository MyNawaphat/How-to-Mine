using UnityEngine;

public class OrePickup : MonoBehaviour
{
    [Header("Magnet Settings")]
    public float magnetDistance = 3.5f;
    public float flySpeed = 12f;
    
    private Transform playerTransform;
    private PlayerInventory playerInventory;
    private Rigidbody rb;
    private Collider col;
    private bool isBeingPulled = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        // หาตัวผู้เล่นในฉาก
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerInventory = player.GetComponent<PlayerInventory>();
        }
    }

    void Update()
    {
        if (playerTransform == null || playerInventory == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        // เข้าสู่ระยะดูด
        if (distance <= magnetDistance && playerInventory.currentOre < playerInventory.maxCapacity)
        {
            if (!isBeingPulled)
            {
                isBeingPulled = true;
                // ปิดฟิสิกส์และการชน เพื่อให้ลอยทะลุเข้าตัวผู้เล่นได้
                if (rb != null) rb.isKinematic = true;
                if (col != null) col.isTrigger = true;
            }
        }

        // ลอยดูดเข้าตัวผู้เล่น
        if (isBeingPulled)
        {
            Vector3 targetPos = playerTransform.position + Vector3.up * 0.5f;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, flySpeed * Time.deltaTime);

            // ขยายระยะให้เก็บง่ายขึ้น (เมื่อเข้าใกล้ในระยะ 0.6 เมตร)
            if (Vector3.Distance(transform.position, targetPos) < 0.6f)
            {
                if (playerInventory.AddOre(1))
                {
                    Destroy(gameObject); // เม็ดแร่หายไปและนับเข้ากระเป๋า
                }
            }
        }
    }
}