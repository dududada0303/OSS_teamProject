using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using System.Collections;

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

    [Header("코인 설정")]
    public GameObject coinPrefab;     // 코인 프리팹
    public int maxCoinsPerGround = 3; // 바닥 타일 하나당 생성할 최대 코인 개수
    public float coinSpawnRadius = 0.5f; // 장애물과 겹치는지 판단할 기준 반경 (코인 크기 정도)

    private GameObject[] coins; // 만들어둔 코인들을 보관할 서랍장
    public LeaderBoard leaderboard;

    void Start()
    {
        leaderboard = FindObjectOfType<LeaderBoard>();
        leaderboard.userName = null;

        //  게임이 시작될 때, 일단 이 바닥에 있는 모든 장애물을 안 보이게 만듭니다.(초반 안전지대 형성)
        for (int i = 0; i < obstacles.Length; i++)
        {
            if (obstacles[i] != null)
            {
                obstacles[i].SetActive(false);
            }
        }

        // 바닥이 처음 만들어질 때 코인도 미리 최대치만큼 만들어둡니다.
        if (coinPrefab != null)
        {
            coins = new GameObject[maxCoinsPerGround];
            for (int i = 0; i < maxCoinsPerGround; i++)
            {
                // 코인을 현재 바닥의 자식 오브젝트로 생성합니다.
                coins[i] = Instantiate(coinPrefab, transform);
                coins[i].SetActive(false); // 일단은 숨겨둡니다.
            }
        }
    }

    void Update()
    {
        if (leaderboard == null || string.IsNullOrEmpty(leaderboard.userName))
            return;
        else
        {
            leaderboard.gameObject.SetActive(false);
        }  

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

        // 바닥이 초기화될 때마다 코인 위치도 새로 랜덤하게 잡아줍니다.
        if (coins != null)
        {
            foreach (GameObject coin in coins)
            {
                // 코인이 나올 확률 설정 (예: 60% 확률로 코인 등장)
                if (Random.Range(0, 100) < 60)
                {
                    bool foundSafePosition = false;
                    Vector3 spawnPos = Vector3.zero;

                    // 장애물과 겹치지 않는 자리를 찾기 위해 최대 5번 시도합니다.
                    for (int attempts = 0; attempts < 5; attempts++)
                    {
                        // 현재 바닥 기준으로 랜덤한 로컬 좌표를 뽑습니다.
                        // (은규님 맵 크기에 맞춰 범위 X, Y, Z 숫자를 조절해 주세요!)
                        float randomX = Random.Range(-3.5f, 3.5f);
                        float randomY = Random.Range(1f, 3.5f);
                        float randomZ = Random.Range(-3.5f, 3.5f);

                        Vector3 localPos = new Vector3(randomX, randomY, randomZ);
                        spawnPos = transform.position + localPos;

                        // 해당 위치에 장애물(Obstacle)이 겹쳐 있는지 '보이지 않는 원'을 그려서 검사합니다.
                        Collider[] hits = Physics.OverlapSphere(spawnPos, coinSpawnRadius);
                        bool isOverlapping = false;

                        foreach (Collider hit in hits)
                        {
                            if (hit.CompareTag("Obstacle"))
                            {
                                isOverlapping = true;
                                break; // 장애물 발견! 이번 위치는 실패
                            }
                        }

                        // 겹치는 장애물이 없다면? 완벽한 자리입니다!
                        if (!isOverlapping)
                        {
                            foundSafePosition = true;
                            break; // 자리 찾기 반복문 탈출
                        }
                    }

                    // 안전한 자리를 찾았다면 코인을 배치하고 켭니다.
                    if (foundSafePosition)
                    {
                        coin.transform.position = spawnPos;
                        coin.SetActive(true);
                    }
                    else
                    {
                        coin.SetActive(false); // 끝내 빈자리를 못 찾았다면 켜지 않습니다.
                    }
                }
                else
                {
                    coin.SetActive(false); // 확률에 당첨되지 않으면 켜지 않습니다.
                }
            }
        }
    }


    // 다음 지면 위치를 설정합니다.
    public void SetNextGround(Ground nextGround)
    {
        this.nextGround = nextGround;
    }
}

