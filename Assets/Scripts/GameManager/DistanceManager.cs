using UnityEngine;
using TMPro;

public class DistanceManager : MonoBehaviour
{
    [Header("UI 연결")]
    public TextMeshProUGUI distanceText;

    [Header("난이도(속도) 설정")]
    public float currentSpeed = 5.0f;        // 시작 속도 
    public float speedIncreaseAmount = 1.0f; // 레벨업 시 증가할 속도량
    public int levelUpDistance = 50;        // 몇 m마다 속도를 올릴지 기준 설정 

    private float currentDistance = 0f;      // 현재까지 달려온 거리
    private int nextLevelDistance;           // 다음 레벨업을 위한 목표 거리

    void Start()
    {
        // 1. 게임 시작 시, 첫 번째 목표 거리를 설정합니다.
        nextLevelDistance = levelUpDistance;

        // 2. (안전장치) 시작할 때 씬에 있는 모든 바닥의 속도를 currentSpeed로 강제 통일시킵니다!
        // 이렇게 하면 바닥 프리팹과 숫자가 달라서 꼬이는 버그를 완벽히 막을 수 있습니다.
        Ground[] grounds = FindObjectsOfType<Ground>();
        foreach (Ground g in grounds)
        {
            g.moveSpeed = currentSpeed;
        }
    }

    void Update()
    {
        if (Time.timeScale > 0f)
        {
            // 고정된 속도가 아니라, 계속 빨라지는 currentSpeed를 이용해 거리를 잽니다.
            currentDistance += currentSpeed * Time.deltaTime;
            distanceText.text = Mathf.FloorToInt(currentDistance).ToString() + "m";

            // 3. 현재 거리가 목표 거리(100, 200, 300...)에 도달했는지 확인!
            if (currentDistance >= nextLevelDistance)
            {
                LevelUp();
            }
        }
    }

    //  속도를 올려주는 레벨업 함수입니다.
    void LevelUp()
    {
        // 다음 목표 거리를 늘려줍니다. (예: 100m 달성 -> 다음 목표는 200m)
        nextLevelDistance += levelUpDistance;

        // 달리는 속도를 증가시킵니다.
        currentSpeed += speedIncreaseAmount;

        // 씬에 깔려있는 모든 '바닥(Ground)'들을 찾아서 스피드를 똑같이 올려줍니다!
        Ground[] grounds = FindObjectsOfType<Ground>();
        foreach (Ground g in grounds)
        {
            g.moveSpeed = currentSpeed;
        }

        Debug.Log($"레벨업! 현재 속도: {currentSpeed}");
    }
}