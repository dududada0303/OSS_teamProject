using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float moveSpeed = 7f;
    public float jumpForce = 10f;

    private Rigidbody rb;
    private CapsuleCollider col;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
    }

    void Update()
    {
        // 1. 좌우 이동
        float moveInput = 0;
        if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed) moveInput = -1;
        else if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed) moveInput = 1;

        rb.linearVelocity = new Vector3(moveInput * moveSpeed, rb.linearVelocity.y, 0);

        // 2. 점프 (레이캐스트로 바닥 체크)
        if (Keyboard.current.spaceKey.wasPressedThisFrame && IsGrounded())
        {
            // 점프할 때 기존 Y축 속도를 초기화해주면 훨씬 일정하게 뜁니다.
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, 0);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    // 발바닥 아래로 짧은 선을 쏴서 땅이 있는지 확인하는 함수
    bool IsGrounded()
    {
        // 캡슐 콜라이더의 아래쪽 끝 지점에서 0.2만큼 아래로 레이를 쏩니다.
        return Physics.Raycast(transform.position, Vector3.down, col.bounds.extents.y + 0.2f);
    }
}