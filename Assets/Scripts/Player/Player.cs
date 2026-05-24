using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 7f;
    public float jumpForce = 10f;

    [Header("슬라이딩 설정")]
    [Range(0.1f, 0.9f)]
    public float slideHeightRatio = 0.4f;    // 슬라이딩 시 높이 비율 (원래의 40%)

    [Header("비주얼 연결")]
    public Transform playerVisual;

    private Rigidbody rb;
    private CapsuleCollider col;
    private float minX = float.MaxValue;
    private float maxX = float.MinValue;

    // 슬라이딩 관련 내부 변수
    private bool isSliding = false;
    private float originalHeight;
    private Vector3 originalCenter;
    private Vector3 originalVisualScale;
    private Vector3 originalVisualLocalPos;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();

        originalHeight = col.height;
        originalCenter = col.center;

        if (playerVisual != null)
        {
            originalVisualScale = playerVisual.localScale;
            originalVisualLocalPos = playerVisual.localPosition;
        }
        else
        {
            Debug.LogWarning("PlayerVisual이 연결되지 않았습니다! Inspector에서 연결해 주세요.");
        }

        GameObject[] grounds = GameObject.FindGameObjectsWithTag("Ground");
        if (grounds.Length > 0)
        {
            foreach (GameObject g in grounds)
            {
                Collider groundCol = g.GetComponent<Collider>();
                if (groundCol != null)
                {
                    if (groundCol.bounds.min.x < minX) minX = groundCol.bounds.min.x;
                    if (groundCol.bounds.max.x > maxX) maxX = groundCol.bounds.max.x;
                }
            }
        }
        else
        {
            Debug.LogError("바닥 타일들에 'Ground' 태그가 설정되어 있는지 확인하세요!");
        }

        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
    }

    void Update()
    {
        // ── 좌우 이동 ──
        float moveInput = 0;
        if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed) moveInput = -1;
        else if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed) moveInput = 1;

        rb.linearVelocity = new Vector3(moveInput * moveSpeed, rb.linearVelocity.y, 0);

        // ── 슬라이딩 (누르고 있는 동안 유지) ──
        bool slideKeyHeld = Keyboard.current.downArrowKey.isPressed
                         || Keyboard.current.sKey.isPressed;

        if (slideKeyHeld && !isSliding && IsGrounded())
        {
            // 키를 누르기 시작 → 슬라이딩 시작
            StartSlide();
        }
        else if (!slideKeyHeld && isSliding)
        {
            // 키를 뗌 → 슬라이딩 종료
            EndSlide();
        }

        // ── 점프 (슬라이딩 중에는 점프 불가) ──
        if (!isSliding && Keyboard.current.spaceKey.wasPressedThisFrame && IsGrounded())
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, 0);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void StartSlide()
    {
        isSliding = true;

        float newHeight = originalHeight * slideHeightRatio;
        col.height = newHeight;

        float heightDifference = originalHeight - newHeight;
        col.center = originalCenter - new Vector3(0, heightDifference / 2f, 0);

        if (playerVisual != null)
        {
            Vector3 slideScale = originalVisualScale;
            slideScale.y = originalVisualScale.y * slideHeightRatio;
            playerVisual.localScale = slideScale;

            Vector3 slidePos = originalVisualLocalPos;
            slidePos.y = originalVisualLocalPos.y - (heightDifference / 2f);
            playerVisual.localPosition = slidePos;
        }
    }

    void EndSlide()
    {
        isSliding = false;

        col.height = originalHeight;
        col.center = originalCenter;

        if (playerVisual != null)
        {
            playerVisual.localScale = originalVisualScale;
            playerVisual.localPosition = originalVisualLocalPos;
        }
    }

    public bool IsSliding => isSliding;

    void LateUpdate()
    {
        float playerHalfWidth = col.radius;
        float clampedX = Mathf.Clamp(transform.position.x, minX + playerHalfWidth, maxX - playerHalfWidth);
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, col.bounds.extents.y + 0.2f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("장애물에 충돌했습니다!");
            Die();
        }
    }

    public void Die()
    {
        if (isSliding) EndSlide();

        gameObject.SetActive(false);

        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.EndGame();
        }
        else
        {
            Time.timeScale = 0f;
        }
    }
}