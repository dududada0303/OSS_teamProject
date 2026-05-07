using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("이동 및 점프 설정")]
    public float moveSpeed = 5f;      // 이동 속도
    public float jumpForce = 7f;      // 점프 힘

    private Rigidbody rb;
    private bool isGrounded;          // 바닥 체크

    void Start()
    {
        // 3D용 리지드바디 컴포넌트 연결
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 1. 좌우 이동 (방향키 왼쪽, 오른쪽 / A, D)
        float moveInput = Input.GetAxisRaw("Horizontal");

        // 3D 공간에서 X축은 좌우, Y축은 높이입니다.
        rb.linearVelocity = new Vector3(moveInput * moveSpeed, rb.linearVelocity.y, 0);

        // 2. 점프 (스페이스바)
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            // 위쪽 방향으로 순간적인 힘(Impulse)을 가함
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false; // 점프 중에는 바닥 상태를 false로
        }
    }

    // 바닥과 충돌했을 때 호출되는 함수
    private void OnCollisionEnter(Collision collision)
    {
        // 바닥 오브젝트의 태그(Tag)가 "Ground"인 경우에만 다시 점프 가능
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}