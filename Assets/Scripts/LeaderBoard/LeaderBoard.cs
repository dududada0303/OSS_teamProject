using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class LeaderBoard : MonoBehaviour
{
    public TextMeshProUGUI ranking;
    public TMP_InputField myInputField;
    Database_Test db;
    List<GameUser> allPlayers;

    // 메모리 절약을 위한 StringBuilder
    StringBuilder sb;

    public string userName = null;
    public int coinCnt;
    public int Distance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        db = FindObjectOfType<Database_Test>();
        sb = new StringBuilder();

        // 반복 횟수를 조절해 플레이어 출력 횟수를 결정한다.
        int repeatCnt = 0;

        // 모든 플레이어의 정보를 담은 리스트 변수

        if (db != null)
        {
            allPlayers = db.GetAllData();
            foreach (var user in allPlayers)
            {
                // 리더보드에는 4명까지만 출력
                if (repeatCnt > 4)
                    break;
                sb.AppendLine(user.ToString());
                repeatCnt++;
            }
            ranking.SetText(sb);
        }
        else
            Debug.Log("DataBase 컴포넌트를 찾을 수 없습니다.\n");

        // 닉네임을 입력했을 때 실행할 함수 지정
        myInputField.onEndEdit.AddListener(StoreName);
    }

    // 따로 변수에 저장하는 메서드
    void StoreName(string inputText)
    {
        userName = inputText;
    }
}