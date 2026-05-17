using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("바닥 설정")]
    public GameObject groundPrefab; 
    public int groundCount = 7;     // 게임에 깔아둘 바닥의 총 개수

    // 생성된 바닥들을 기억해 둘 배열
    private Ground[] grounds;

    [Header("게임 상태 및 UI 설정")]
    public bool isGameover = false; // 현재 게임 오버 상태인지 확인하는 스위치
    public GameObject gameoverUI;   // 죽었을 때 화면에 띄울 패널(UI)

    void Start()
    {

        // 1. 게임이 시작될 때 게임 오버 UI가 켜져 있다면 꺼줍니다.
        if (gameoverUI != null)
        {
            gameoverUI.SetActive(false);
        }

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
    void Update()
    {
        // 2. 만약 게임 오버 상태인데 플레이어가 'R' 키(또는 원하는 키)를 누른다면?
        if (isGameover && Input.GetKeyDown(KeyCode.R))
        {
            // 게임을 처음부터 다시 시작합니다!
            RestartGame();
        }
    }

    // 3. Player 스크립트에서 캐릭터가 죽을 때 이 함수를 부르게 됩니다.
    public void EndGame()
    {
        isGameover = true; // 게임 오버 상태로 변경

        if (gameoverUI != null)
        {
            gameoverUI.SetActive(true); // 숨겨뒀던 게임 오버 UI를 화면에 짠! 하고 띄웁니다.
        }

        // 게임을 일시중지합니다.
        Time.timeScale = 0f;
    }

    // 4. 게임을 다시 시작하는 처리를 담당하는 함수입니다.
    private void RestartGame()
    {
        // 멈춰둔 시간을 다시 원래 속도(1)로 되돌려놓습니다.
        Time.timeScale = 1f;

        // 현재 우리가 플레이하고 있는 씬(맵)의 이름을 가져와서 처음부터 다시 로딩합니다.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
