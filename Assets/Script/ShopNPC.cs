using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ShopNPC : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject interactPrompt; 
    public GameObject shopMenuPanel;   

    [Header("Status Texts")]
    public TextMeshProUGUI infoText;   

    private bool isPlayerNearby = false;
    private bool isShopOpen = false;
    private PlayerInventory playerInventory;

    void Start()
    {
        if (interactPrompt != null) interactPrompt.SetActive(false);
        if (shopMenuPanel != null) shopMenuPanel.SetActive(false);
    }

    void Update()
    {
        if (isPlayerNearby && Keyboard.current != null)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                ToggleShop();
            }
        }

        if (isShopOpen && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CloseShop();
        }

        // แสดงผลข้อความภาษาอังกฤษเพื่อแก้บั๊กกล่องสี่เหลี่ยม □□□
        if (isShopOpen && infoText != null && playerInventory != null)
        {
            infoText.text = $"Gold: {playerInventory.money}G\nOre: {playerInventory.currentOre} / {playerInventory.maxCapacity}\nPickaxe Power: {playerInventory.pickaxePower}";
        }
    }

    public void ToggleShop()
    {
        if (isShopOpen) CloseShop();
        else OpenShop();
    }

    public void OpenShop()
    {
        isShopOpen = true;
        PlayerController.isShopping = true; // สั่งให้ตัวละครหยุดเดิน
        shopMenuPanel.SetActive(true);
        if (interactPrompt != null) interactPrompt.SetActive(false);

        // ปลดล็อคเมาส์ให้คลิกปุ่มได้เต็มที่
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseShop()
    {
        isShopOpen = false;
        PlayerController.isShopping = false; // คืนการควบคุมให้ตัวละคร
        shopMenuPanel.SetActive(false);
        if (isPlayerNearby && interactPrompt != null) interactPrompt.SetActive(true);

        // ล็อคเมาส์กลับเข้าเกม
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnClickSellAll()
    {
        if (playerInventory != null)
        {
            playerInventory.SellAllOre(pricePerOre: 15);
        }
    }

    public void OnClickUpgradeBag()
    {
        if (playerInventory != null)
        {
            playerInventory.UpgradeCapacity(extraSlot: 10, cost: 100);
        }
    }

    public void OnClickUpgradePickaxe()
    {
        if (playerInventory != null)
        {
            playerInventory.UpgradePickaxe(cost: 150);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            playerInventory = other.GetComponent<PlayerInventory>();
            if (!isShopOpen && interactPrompt != null) interactPrompt.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            CloseShop();
            if (interactPrompt != null) interactPrompt.SetActive(false);
        }
    }
}