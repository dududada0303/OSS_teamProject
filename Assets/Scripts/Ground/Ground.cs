using UnityEngine;

public class Ground : MonoBehaviour
{
    public float moveSpeed = 5.0f;

    private Ground nextGround;

    [Header("장애물 설정")]
    // 여러 장애물을 배열로 등록하세요.
    public GameObject[] obstacles;

    [Header("장애물 X 범위")]
    public float obstacleMinX = -2f;
    public float obstacleMaxX = 2f;

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

    void Move()
    {
        transform.position += Vector3.back * moveSpeed * Time.deltaTime;
    }

    bool CheckReset()
    {
        return transform.position.z < Constants.GroundResetZPosition;
    }

    void ResetGround(bool spawnObstacle)
    {
        float distance = Constants.GroundDistanceZ;

        if (nextGround != null)
        {
            transform.position = nextGround.transform.position + (Vector3.forward * distance);
        }

        // 먼저 모든 장애물을 꺼줍니다.
        foreach (GameObject obs in obstacles)
        {
            if (obs != null)
                obs.SetActive(false);
        }

        // 장애물을 생성할 차례라면, 배열에서 하나를 랜덤으로 골라 켭니다.
        if (spawnObstacle && obstacles.Length > 0)
        {
            int randomIndex = Random.Range(0, obstacles.Length);
            GameObject chosen = obstacles[randomIndex];

            chosen.SetActive(true);

            // 월드 좌표로 X 위치 랜덤 배치
            float randomX = Random.Range(obstacleMinX, obstacleMaxX);
            Vector3 worldPos = chosen.transform.position;
            worldPos.x = transform.position.x + randomX;
            chosen.transform.position = worldPos;
        }
    }

    public void SetNextGround(Ground nextGround)
    {
        this.nextGround = nextGround;
    }
}