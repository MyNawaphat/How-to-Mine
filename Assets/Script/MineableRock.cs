using UnityEngine;

public class MineableRock : MonoBehaviour
{
    [Header("Rock Settings")]
    public string oreName = "Common Stone"; // ชื่อแร่
    public int maxHealth = 3;               // จำนวนครั้งที่ต้องขุด
    private int currentHealth;

    [Header("Drop Settings")]
    public GameObject orePrefab;            // เม็ดแร่ที่จะดรอป
    public int minDropCount = 4;            // จำนวนดรอปขั้นต่ำ (เช่น หาง่าย 4, หายาก 2)
    public int maxDropCount = 8;            // จำนวนดรอปสูงสุด (เช่น หาง่าย 8, หายาก 4)
    public float explosionForce = 3f;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{oreName} โดนขุด! เลือดเหลือ: {currentHealth}");

        transform.localScale *= 0.95f;

        if (currentHealth <= 0)
        {
            BreakRock();
        }
    }

    void BreakRock()
    {
        // สุ่มจำนวนเม็ดแร่ระหว่าง min ถึง max
        int randomDrop = Random.Range(minDropCount, maxDropCount + 1);
        Debug.Log($"💥 {oreName} แตกแล้ว! สุ่มดรอปแร่ออกมาได้: {randomDrop} ก้อน");

        if (orePrefab != null)
        {
            for (int i = 0; i < randomDrop; i++)
            {
                Vector3 spawnPos = transform.position + Vector3.up * 0.5f + Random.insideUnitSphere * 0.3f;
                GameObject ore = Instantiate(orePrefab, spawnPos, Random.rotation);

                Rigidbody rb = ore.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 forceDir = (Vector3.up * 1.5f + Random.insideUnitSphere).normalized;
                    rb.AddForce(forceDir * explosionForce, ForceMode.Impulse);
                }
            }
        }

        Destroy(gameObject);
    }
}