using UnityEngine;

public class GroundManager : MonoBehaviour
{
    [Header("바닥 설정")]
    public GameObject groundPrefab; 
    public int groundCount = 7;     // 게임에 깔아둘 바닥의 총 개수

    // 생성된 바닥들을 기억해 둘 배열
    private Ground[] grounds;

    void Start()
    {
        // 설정한 개수만큼 바닥을 담을 공간을 만듭니다.
        grounds = new Ground[groundCount];

        // 바닥을 하나씩 생성하고 위치를 잡아줍니다.
        for (int i = 0; i < groundCount; i++)
        {
            // Constants.GroundDistanceZ 간격만큼 띄워서 차례대로 배치합니다.
            float zPos = i * Constants.GroundDistanceZ;
            Vector3 spawnPosition = new Vector3(0, 0, zPos);

            // 프리팹을 생성하고 배열에 저장합니다.
            GameObject go = Instantiate(groundPrefab, spawnPosition, Quaternion.identity);
            grounds[i] = go.GetComponent<Ground>();
        }

        // 지면이 무한 생성되도록 꼬리를 물도록 하는 반복문입니다.
        for (int i = 0; i < groundCount; i++)
        {
            if (i == 0)
            {
                grounds[i].SetNextGround(grounds[groundCount - 1]);
            }
            else
            {
                grounds[i].SetNextGround(grounds[i - 1]);
            }
        }
    }
}