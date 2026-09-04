using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Currency & Ore")]
    public int currentOre = 0;       // แร่ในกระเป๋า
    public int maxCapacity = 20;     // ความจุสูงสุด
    public int money = 0;            // เงินสะสม (Gold)

    [Header("Tools Level")]
    public int pickaxePower = 1;     // ดาเมจจอบ (เลเวล 1 สับลด 1)

    // เก็บแร่
    public bool AddOre(int amount)
    {
        if (currentOre >= maxCapacity)
        {
            Debug.Log("⚠️ กระเป๋าเต็มแล้ว!");
            return false;
        }

        currentOre += amount;
        Debug.Log($"💎 แร่: {currentOre} / {maxCapacity}");
        return true;
    }

    // ขายแร่ทั้งหมด (ก้อนละ 15G)
    public void SellAllOre(int pricePerOre = 15)
    {
        if (currentOre <= 0)
        {
            Debug.Log("ไม่มีแร่ในกระเป๋าให้ขาย!");
            return;
        }

        int earned = currentOre * pricePerOre;
        money += earned;
        currentOre = 0; // ล้างกระเป๋า
        Debug.Log($"💰 ขายแร่หมดแล้ว! ได้เงินมา +{earned}G (เงินรวม: {money}G)");
    }

    // ซื้อเพิ่มความจุกระเป๋า
    public bool UpgradeCapacity(int extraSlot, int cost)
    {
        if (money >= cost)
        {
            money -= cost;
            maxCapacity += extraSlot;
            Debug.Log($"🎒 ขยายกระเป๋าสำเร็จ! ความจุใหม่: {maxCapacity} (เสียเงิน {cost}G)");
            return true;
        }
        Debug.Log("เงินไม่พอขยายกระเป๋า!");
        return false;
    }

    // ซื้ออัปเกรดจอบขุดแรงขึ้น
    public bool UpgradePickaxe(int cost)
    {
        if (money >= cost)
        {
            money -= cost;
            pickaxePower += 1;
            Debug.Log($"⛏️ อัปเกรดจอบสำเร็จ! ความแรงขุด: {pickaxePower} (เสียเงิน {cost}G)");
            return true;
        }
        Debug.Log("เงินไม่พออัปเกรดจอบ!");
        return false;
    }
}