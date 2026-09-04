using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // ตัวแปรบอกว่ากำลังเปิดร้านค้าอยู่หรือไม่
    public static bool isShopping = false;

    [Header("Movement")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 9f;
    public float gravity = -15f;
    public float jumpHeight = 1.2f;

    [Header("Camera & Mouse Look")]
    public Transform cameraTransform;
    public float mouseSensitivity = 0.15f;
    private float xRotation = 0f;

    [Header("Weapon / Pickaxe")]
    public Transform weaponHolder;
    public float miningRange = 3f;
    private bool isSwinging = false;
    private Quaternion weaponDefaultRot;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (weaponHolder != null)
        {
            weaponDefaultRot = weaponHolder.localRotation;
        }
    }

    void Update()
    {
        // ถ้าเปิดร้านค้าอยู่ ให้หยุดการควบคุมทุกอย่าง เพื่อให้คลิกเมนูได้
        if (isShopping) return;

        HandleMouseLook();
        HandleMovement();
        HandleMining();

        // ปลดล็อคเมาส์ด้วย Esc
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // ล็อคเมาส์กลับเฉพาะตอนที่ไม่ได้เปิดร้าน
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void HandleMouseLook()
    {
        if (cameraTransform == null || Mouse.current == null || Cursor.lockState != CursorLockMode.Locked) return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue() * mouseSensitivity;
        transform.Rotate(Vector3.up * mouseDelta.x);

        xRotation -= mouseDelta.y;
        xRotation = Mathf.Clamp(xRotation, -85f, 85f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void HandleMovement()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0) velocity.y = -2f;

        float x = 0f;
        float z = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) z += 1f;
            if (Keyboard.current.sKey.isPressed) z -= 1f;
            if (Keyboard.current.aKey.isPressed) x -= 1f;
            if (Keyboard.current.dKey.isPressed) x += 1f;
        }

        bool isSprinting = Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed;
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move.normalized * currentSpeed * Time.deltaTime);

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleMining()
    {
        if (Mouse.current == null || Cursor.lockState != CursorLockMode.Locked) return;

        if (Mouse.current.leftButton.wasPressedThisFrame && !isSwinging)
        {
            StartCoroutine(SwingPickaxe());
            CheckMiningRaycast();
        }
    }

    IEnumerator SwingPickaxe()
    {
        if (weaponHolder == null) yield break;

        isSwinging = true;
        Quaternion swingRot = weaponDefaultRot * Quaternion.Euler(45f, -20f, 0f);

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 12f;
            weaponHolder.localRotation = Quaternion.Slerp(weaponDefaultRot, swingRot, t);
            yield return null;
        }

        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 8f;
            weaponHolder.localRotation = Quaternion.Slerp(swingRot, weaponDefaultRot, t);
            yield return null;
        }

        weaponHolder.localRotation = weaponDefaultRot;
        isSwinging = false;
    }

    void CheckMiningRaycast()
    {
        if (cameraTransform == null) return;

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, miningRange))
        {
            MineableRock rock = hit.collider.GetComponent<MineableRock>();
            if (rock != null)
            {
                PlayerInventory inv = GetComponent<PlayerInventory>();
                int damage = (inv != null) ? inv.pickaxePower : 1;
                rock.TakeDamage(damage);
            }
        }
    }
}