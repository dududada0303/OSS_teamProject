using UnityEngine;

public class Ground : MonoBehaviour
{
    public float moveSpeed = 5.0f;

    // 무한 스크롤링을 위한 다음 지면
    private Ground nextGround;

    [Header("장애물 설정")]
    // 맵 위에 올려둘 장애물이나 아이템 오브젝트를 여기에 연결해주세요.
    public GameObject obstacle;

    void Start()
    {

    }

    void Update()
    {

        Move();

        if (CheckReset())
        {
            ResetGround(Random.Range(1, 101) <= 50);
        }
    }

    // 이동 함수
    void Move()
    {
        transform.position += Vector3.back * moveSpeed * Time.deltaTime;
    }

    // 초기화 위치인지 확인합니다.
    bool CheckReset()
    {
        return transform.position.z < Constants.GroundResetZPosition;
    }

    // 위치를 초기화 시킵니다.
    void ResetGround(bool spawnObstacle)
    {
        float distance = Constants.GroundDistanceZ;

        // nextGround가 비어있지 않을 때만 위치를 이동시킵니다.
        if (nextGround != null)
        {
            transform.position = nextGround.transform.position + (Vector3.forward * distance);
        }

        // 확률에 따라 장애물을 켜거나 끕니다.
        if (obstacle != null)
        {
            obstacle.SetActive(spawnObstacle);
        }
    }

    // 다음 지면 위치를 설정합니다.
    public void SetNextGround(Ground nextGround)
    {
        this.nextGround = nextGround;
    }
}

