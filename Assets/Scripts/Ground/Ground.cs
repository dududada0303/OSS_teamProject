using UnityEngine;

public class Ground : MonoBehaviour
{
    public float moveSpeed = 5.0f;

    // 무한 스크롤링을 위한 다음 지면
    private Ground nextGround;

    [Header("장애물 설정")]
    // 맵 위에 올려둘 장애물이나 아이템 오브젝트를 여기에 연결해주세요.
    public GameObject[] obstacles;

    // 장애물 생성 확률 슬라이더
    [Range(0, 100)]
    public int spawnProbability = 50; // 기본 확률은 50%로 설정

    void Start()
    {
        //  게임이 시작될 때, 일단 이 바닥에 있는 모든 장애물을 안 보이게 만듭니다.(초반 안전지대 형성)
        for (int i = 0; i < obstacles.Length; i++)
        {
            if (obstacles[i] != null)
            {
                obstacles[i].SetActive(false);
            }
        }
    }

    void Update()
    {

        Move();

        if (CheckReset())
        {
            ResetGround();
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
    void ResetGround()
    {
        float distance = Constants.GroundDistanceZ;

        // nextGround가 비어있지 않을 때만 위치를 이동시킵니다.
        if (nextGround != null)
        {
            transform.position = nextGround.transform.position + (Vector3.forward * distance);
        }

        // 반복문(for)을 사용해 서랍장(배열)에 들어있는 장애물들을 하나씩 검사합니다.
        for (int i = 0; i < obstacles.Length; i++)
        {
            // 혹시 실수로 빈칸을 만들어두었을 경우를 대비한 안전장치입니다.
            if (obstacles[i] != null)
            {
                // 각 장애물 자리마다 우리가 설정한 확률(예: 50%)로 주사위를 굴립니다!
                bool shouldSpawn = Random.Range(1, 101) <= spawnProbability;

                // 주사위 결과에 따라 켜거나 끕니다.
                obstacles[i].SetActive(shouldSpawn);
            }
        }
    }

    // 다음 지면 위치를 설정합니다.
    public void SetNextGround(Ground nextGround)
    {
        this.nextGround = nextGround;
    }
}

