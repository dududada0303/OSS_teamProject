using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 7f;
    public float jumpForce = 10f;

    [Header("슬라이딩 설정")]
    public float slideDuration = 0.8f;      // 슬라이딩 유지 시간 (초)
    public float slideCooldown = 0.3f;       // 슬라이딩 후 재사용 대기 시간 (초)
    [Range(0.1f, 0.9f)]
    public float slideHeightRatio = 0.4f;    // 슬라이딩 시 높이 비율 (원래의 40%)

    [Header("비주얼 연결")]
    // Inspector에서 PlayerVisual 오브젝트를 여기에 드래그 앤 드롭 해주세요!
    public Transform playerVisual;

    private Rigidbody rb;
    private CapsuleCollider col;
    private float minX = float.MaxValue;
    private float maxX = float.MinValue;

    // 슬라이딩 관련 내부 변수
    private bool isSliding = false;
    private float slideTimer = 0f;
    private float cooldownTimer = 0f;
    private float originalHeight;            // 콜라이더 원래 높이
    private Vector3 originalCenter;          // 콜라이더 원래 중심점
    private Vector3 originalVisualScale;     // 비주얼 원래 크기
    private Vector3 originalVisualLocalPos;  // 비주얼 원래 위치

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();

        // 원래 값들을 저장해 둡니다 (슬라이딩 끝나면 되돌리기 위해)
        originalHeight = col.height;
        originalCenter = col.center;

        // playerVisual이 연결되어 있으면 원래 크기와 위치도 저장
        if (playerVisual != null)
        {
            originalVisualScale = playerVisual.localScale;
            originalVisualLocalPos = playerVisual.localPosition;
        }
        else
        {
            Debug.LogWarning("PlayerVisual이 연결되지 않았습니다! Inspector에서 연결해 주세요.");
        }

        // 바닥 범위 계산 (이동 제한용)
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
        // ── 쿨다운 타이머 ──
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        // ── 좌우 이동 ──
        float moveInput = 0;
        if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed) moveInput = -1;
        else if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed) moveInput = 1;

        rb.linearVelocity = new Vector3(moveInput * moveSpeed, rb.linearVelocity.y, 0);

        // ── 점프 (슬라이딩 중에는 점프 불가) ──
        if (!isSliding && Keyboard.current.spaceKey.wasPressedThisFrame && IsGrounded())
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, 0);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // ── 슬라이딩 시작 (아래 방향키 또는 S키) ──
        bool slideKeyPressed = Keyboard.current.downArrowKey.wasPressedThisFrame
                            || Keyboard.current.sKey.wasPressedThisFrame;

        if (slideKeyPressed && !isSliding && cooldownTimer <= 0f && IsGrounded())
        {
            StartSlide();
        }

        // ── 슬라이딩 타이머 ──
        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0f)
            {
                EndSlide();
            }
        }
    }

    // ========== 슬라이딩 시작 ==========
    void StartSlide()
    {
        isSliding = true;
        slideTimer = slideDuration;

        // ─── 1) 히트박스(콜라이더) 줄이기 ───
        float newHeight = originalHeight * slideHeightRatio;
        col.height = newHeight;

        float heightDifference = originalHeight - newHeight;
        col.center = originalCenter - new Vector3(0, heightDifference / 2f, 0);

        // ─── 2) 눈에 보이는 모양(비주얼) 줄이기 ───
        if (playerVisual != null)
        {
            // Y축 스케일을 줄여서 캡슐을 납작하게 만듭니다
            Vector3 slideScale = originalVisualScale;
            slideScale.y = originalVisualScale.y * slideHeightRatio;
            playerVisual.localScale = slideScale;

            // 비주얼도 아래로 내려서 바닥에 붙게 합니다
            Vector3 slidePos = originalVisualLocalPos;
            slidePos.y = originalVisualLocalPos.y - (heightDifference / 2f);
            playerVisual.localPosition = slidePos;
        }

        Debug.Log("슬라이딩 시작!");
    }

    // ========== 슬라이딩 종료 ==========
    void EndSlide()
    {
        isSliding = false;
        cooldownTimer = slideCooldown;

        // ─── 1) 히트박스 원래대로 ───
        col.height = originalHeight;
        col.center = originalCenter;

        // ─── 2) 비주얼 원래대로 ───
        if (playerVisual != null)
        {
            playerVisual.localScale = originalVisualScale;
            playerVisual.localPosition = originalVisualLocalPos;
        }

        Debug.Log("슬라이딩 종료!");
    }

    // 외부에서 슬라이딩 상태 확인용
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