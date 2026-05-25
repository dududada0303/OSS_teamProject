using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using System.Collections;
using System.Collections.Generic; // 런타임 레인 관리를 위해 추가

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

    // 🏃‍♂️ 게임에서 사용할 기본 3개 레인의 X 좌표 기준값
    private float[] lanes = { -3.0f, 0.0f, 3.0f };

    void Start()
    {
        leaderboard = FindObjectOfType<LeaderBoard>();
        if (leaderboard != null)
        {
            leaderboard.userName = null;
        }

        // 게임이 시작될 때, 일단 이 바닥에 있는 모든 장애물을 안 보이게 만듭니다.(초반 안전지대 형성)
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

        // -----------------------------------------------------------------
        // [수정된 장애물 랜덤 생성 파트] 
        // -----------------------------------------------------------------
        // 사용할 수 있는 레인 목록을 담은 복사 리스트를 만듭니다.
        List<float> availableLanes = new List<float>(lanes);

        for (int i = 0; i < obstacles.Length; i++)
        {
            if (obstacles[i] != null)
            {
                // 주사위 확률을 체크하고, 동시에 남은 레인 자리가 있는지도 확인합니다.
                bool shouldSpawn = (Random.Range(1, 101) <= spawnProbability) && (availableLanes.Count > 0);

                // 주사위 결과에 따라 켜거나 끕니다.
                obstacles[i].SetActive(shouldSpawn);

                if (shouldSpawn)
                {
                    // 남은 레인 리스트 중에서 무작위로 인덱스를 하나 선택합니다.
                    int randomIndex = Random.Range(0, availableLanes.Count);
                    float chosenX = availableLanes[randomIndex];

                    // 다른 장애물이 이 레인을 또 쓰지 못하도록 리스트에서 제거합니다. (중복 방지 핵심!)
                    availableLanes.RemoveAt(randomIndex);

                    // 장애물의 로컬 좌표를 받아와 X축만 선택된 레인 값으로 교체합니다.
                    Vector3 currentPos = obstacles[i].transform.localPosition;
                    obstacles[i].transform.localPosition = new Vector3(chosenX, currentPos.y, currentPos.z);
                }
            }
        }
        // -----------------------------------------------------------------

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