using UnityEngine;

public class Ground : MonoBehaviour
{
    public float moveSpeed = 5.0f;

    private Ground nextGround;

    [Header("Visual Settings")]
    public Material grassMaterial;

    [Header("Obstacle Settings")]
    public GameObject[] obstacles;

    [Range(0, 100)]
    public int spawnProbability = 50;

    [Header("Coin Settings")]
    public GameObject coinPrefab;
    public int maxCoinsPerGround = 3;
    public float coinSpawnRadius = 0.5f;

    private GameObject[] coins;

    void Start()
    {
        ApplyGrassMaterial();

        for (int i = 0; i < obstacles.Length; i++)
        {
            if (obstacles[i] != null)
            {
                obstacles[i].SetActive(false);
            }
        }

        if (coinPrefab != null)
        {
            coins = new GameObject[maxCoinsPerGround];
            for (int i = 0; i < maxCoinsPerGround; i++)
            {
                coins[i] = Instantiate(coinPrefab, transform);
                coins[i].SetActive(false);
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

    void Move()
    {
        transform.position += Vector3.back * moveSpeed * Time.deltaTime;
    }

    bool CheckReset()
    {
        return transform.position.z < Constants.GroundResetZPosition;
    }

    void ResetGround()
    {
        float distance = Constants.GroundDistanceZ;

        if (nextGround != null)
        {
            transform.position = nextGround.transform.position + (Vector3.forward * distance);
        }

        ApplyGrassMaterial();

        for (int i = 0; i < obstacles.Length; i++)
        {
            if (obstacles[i] != null)
            {
                bool shouldSpawn = Random.Range(1, 101) <= spawnProbability;
                obstacles[i].SetActive(shouldSpawn);
            }
        }

        if (coins != null)
        {
            foreach (GameObject coin in coins)
            {
                if (Random.Range(0, 100) < 60)
                {
                    bool foundSafePosition = false;
                    Vector3 spawnPos = Vector3.zero;

                    for (int attempts = 0; attempts < 5; attempts++)
                    {
                        float randomX = Random.Range(-3.5f, 3.5f);
                        float randomY = Random.Range(1f, 3.5f);
                        float randomZ = Random.Range(-3.5f, 3.5f);

                        Vector3 localPos = new Vector3(randomX, randomY, randomZ);
                        spawnPos = transform.position + localPos;

                        Collider[] hits = Physics.OverlapSphere(spawnPos, coinSpawnRadius);
                        bool isOverlapping = false;

                        foreach (Collider hit in hits)
                        {
                            if (hit.CompareTag("Obstacle"))
                            {
                                isOverlapping = true;
                                break;
                            }
                        }

                        if (!isOverlapping)
                        {
                            foundSafePosition = true;
                            break;
                        }
                    }

                    if (foundSafePosition)
                    {
                        coin.transform.position = spawnPos;
                        coin.SetActive(true);
                    }
                    else
                    {
                        coin.SetActive(false);
                    }
                }
                else
                {
                    coin.SetActive(false);
                }
            }
        }
    }

    void ApplyGrassMaterial()
    {
        if (grassMaterial != null)
        {
            Transform visualGround = transform.Find("VisualGround");
            if (visualGround != null)
            {
                MeshRenderer meshRenderer = visualGround.GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                {
                    meshRenderer.material = grassMaterial;
                }
            }
        }
    }

    public void SetNextGround(Ground nextGround)
    {
        this.nextGround = nextGround;
    }
}