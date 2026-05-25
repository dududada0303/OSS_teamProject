using UnityEngine;
using TMPro; // 텍스트 UI를 위해 필요합니다.

public class CoinManager : MonoBehaviour
{
    // 싱글톤(Singleton): 게임 내 어디서든 쉽게 접근할 수 있게 해주는 마법의 코드입니다.
    public static CoinManager instance;

    [Header("UI 연결")]
    public TextMeshProUGUI coinText;

    private int coinCount = 0; // 획득한 코인 개수

    void Awake()
    {
        // 씬에 매니저가 하나만 존재하도록 설정
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateCoinUI();
    }

    // 코인을 먹었을 때 실행될 함수
    public void AddCoin(int amount = 1)
    {
        coinCount += amount;
        UpdateCoinUI();
    }

    // UI 글자를 갱신하는 함수
    private void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = "코인: " + coinCount.ToString() + "개";
        }
    }
}