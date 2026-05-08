using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 7f;
    public float jumpForce = 10f;

    private Rigidbody rb;
    private CapsuleCollider col;
    private float minX = float.MaxValue;
    private float maxX = float.MinValue;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();

        // 1. 모든 "Ground" 태그를 가진 오브젝트를 찾습니다.
        GameObject[] grounds = GameObject.FindGameObjectsWithTag("Ground");

        if (grounds.Length > 0)
        {
            foreach (GameObject g in grounds)
            {
                Collider groundCol = g.GetComponent<Collider>();
                if (groundCol != null)
                {
                    // 모든 바닥 중 가장 왼쪽(min)과 가장 오른쪽(max)을 찾음
                    if (groundCol.bounds.min.x < minX) minX = groundCol.bounds.min.x;
                    if (groundCol.bounds.max.x > maxX) maxX = groundCol.bounds.max.x;
                }
            }
        }
        else
        {
            Debug.LogError("바닥 타일들에 'Ground' 태그가 설정되어 있는지 확인하세요!");
        }

        // 회전 고정 및 Z축 이동 방지
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
    }

    void Update()
    {
        // 좌우 입력
        float moveInput = 0;
        if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed) moveInput = -1;
        else if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed) moveInput = 1;

        // 이동 적용
        rb.linearVelocity = new Vector3(moveInput * moveSpeed, rb.linearVelocity.y, 0);

        // 점프
        if (Keyboard.current.spaceKey.wasPressedThisFrame && IsGrounded())
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, 0);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void LateUpdate()
    {
        // 2. 계산된 전체 바닥 범위 밖으로 나가지 못하게 고정
        float playerHalfWidth = col.radius;
        float clampedX = Mathf.Clamp(transform.position.x, minX + playerHalfWidth, maxX - playerHalfWidth);
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, col.bounds.extents.y + 0.2f);
    }
    // 물리적인 충돌이 발생했을 때 유니티가 자동으로 실행해 주는 함수
    private void OnCollisionEnter(Collision collision)
    {
        // 부딪힌 대상의 태그가 아까 만든 "Obstacle"인지 확인
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("장애물에 충돌했습니다!");

            // 임시로 게임의 시간을 멈춰서 제대로 충돌했는지 확인하는 용도
            Time.timeScale = 0f;
        }
    }
}