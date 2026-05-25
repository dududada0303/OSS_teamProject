using NUnit.Framework;
// 데이터 베이스를 사용하기 위함
using SQLite;
// 리스트나 딕셔너리같은 동적 데이터 묶음을 사용하기위해 필요하다.
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor.MemoryProfiler;
using UnityEngine;

public class GameUser
{
    // Id를 기본 키(PrimaryKey)로 설정하고, 데이터 추가될 때마다 1씩 증가(AutoIncrement)
    // get과 set은 클래스 필드 데이터를 외부에서 안전하게 보호하고 접근하게 해줄 수 있는 프로퍼티 문법 
    // 직접적인 변수의 접근을 막아 객체의 무결성을 유지한다.
    // get 또는 set 을 생략해 값의 접근 범위를 제한할 수 있다.

    // Read only : set 생략, Write only : get 생략
    // ex) PlayerLocalData.Id
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // 이름 속성 (중복 비허용)
    // indexed 속성까지 같이 사용시 충돌 발생
    [Unique]
    public string userName { get; set; }
    // 코인 개수와 거리를 가지고 순위를 계산
    public int Coin { get; set; }
    public int Distance { get; set; }

    // 오버라이드는 부모 클래스에 정의된 메서드 같은 부분을 재정의할 때 사용한다.
    // 오버라이드에 사용할 변수가 있는 클래스 내부에 직접 재정의하는 게 편리하다.
    public override string ToString()
    {
        return $"[Id : {Id}] {userName} [Coin : {Coin}] [Distance : {Distance}]\n";
    }
}

public class Database_Test : MonoBehaviour
{
    // DB 객체를 다룰 변수
    private SQLiteConnection connection;
    private void Awake()
    {
        // 저장할 경로, 파일명 설정
        string dbPath = Path.Combine(Application.persistentDataPath, "LocalDB.db");
        Debug.Log("DB 실제 경로 : " + dbPath);
        connection = new SQLiteConnection(dbPath);
        connection.CreateTable<GameUser>();

        // 데이터 삭제할 때 주석 취소 처리해서 한번 실행해주세요.
        // DeleteAllData();

        // DB 연결 및 테이블 자동 생성
        // using 블록을 지정하면 에러가 발생하더라도 블록을 빠져나갈 때자동으로 안전하게 닫는다.
        // SQLiteConnection은 지정 경로의 DB 파일과 연결할 수 있는 연결 객체를 메모리에 생성한다.
        // var 은 컴파일러가 알아서 타입을 자동으로 추론하도록 만드는 키워드
        /*using (connection = new SQLiteConnection(dbPath))
        {
            connection.CreateTable<GameUser>();

            // 데이터 삽입
            // AutoIncrement 덕에 데이터 삽입할 때, 값이 비어있으면 
            // 내부적으로 가장 큰 Id + 1을 해주기 때문에 따로 Id 할당해주지 않아도 된다.
            // 오히려 수동으로 조작하면 실수를 유발할 수 있기 때문에 시스템에 맡기는게 낫다.
            connection.Insert(new GameUser { userName = "User_1", Coin = 0, Distance = 100 });
            connection.Insert(new GameUser { userName = "User_2", Coin = 0, Distance = 200 });

            var users = connection.Table<GameUser>().ToList();
            // foreach는 배열이나 리스트 같은 데이터 묶음의 모든 요소를 
            // 하나씩 꺼내어 반복할 때 사용하는 반복문
            foreach(var user in users)
            {
                Debug.Log($"Id : {user.Id}, Name : {user.userName}, Coin : {user.Coin}, Distance : {user.Distance}");
            }
        }*/
        // => 데이터 베이스 테스트

        // 모든 플레이어의 정보를 담은 리스트 변수
        List<GameUser> allPlayers = GetAllData();
        foreach (var user in allPlayers)
            Debug.Log(user.ToString());
    }

    // 데이터를 삽입하는 메서드
    public void InsertData(string name, int coinCnt, int distance)
    {
        try
        {
            var insertData = new GameUser { userName = name, Coin = coinCnt, Distance = distance };
            connection.Insert(insertData);
            Debug.Log("데이터 삽입이 성공했습니다!");
        }
        // 데이터 베이스 연결이 실패하더라도 반환값이 없고, 예외를 던져 프로그램을 멈춘다.
        // 따라서 try-catch 구문을 이용해 예외를 잡아 실패 여부를 확인할 수 있다.
        
        // 주로 나오는 예외는 다음과 같다.
        // 1. SQLiteException : 경로를 찾을 수 없거나, 권한이 없거나, 파일이 손상됐을 때
        // 2. ArgumentException : 경로가 비어있거나, 올바르지 않은 문자가 섞여있을 때

        catch (SQLiteException ex)
        {
            Debug.LogWarning("데이터 삽입이 실패 했습니다!");
            Debug.LogWarning(ex.Message);
        }
    }

    // 모든 플레이어 데이터를 가져오는 메서드
    public List<GameUser> GetAllData()
    {
        return connection.Table<GameUser>().ToList();
    }

    // 조건 조회를 통해 특정 데이터를 가져오는 메서드 (미구현)
    // 필요할 경우 구현

    // 유니티 종료시 자동 호출하는 메서드
    private void OnApplicationQuit()
    {
        // 데이터 베이스 해제 메서드 호출
        FreeConnection();
    }

    // 데이터 베이스 연결을 종료하는 메서드
    void FreeConnection()
    {
        // 데이터 베이스 주소가 담겨 있다면?
        if(connection != null)
        {
            // close는 연결 통로나 핸들 같은 외부 자원만 차단하고 객체 자체는 메모리에 살려둔다.
            // open 메서드를 사용하면 다시 객체를 사용할 수 있다.

            // dispose는 모든 메모리와 리소스를 완전히 해제한다.
            connection.Close();
            connection.Dispose();
            connection = null;
            Debug.Log("데이터베이스 연결을 안전히 해제했습니다!");
        }
    }

    void DeleteAllData()
    {
        try
        {
            connection.DeleteAll<GameUser>();
            Debug.Log("모든 데이터를 삭제했습니다!");
        }

        catch (SQLiteException ex)
        {
            Debug.LogWarning("데이터 삭제에 실패 했습니다!");
            Debug.LogWarning(ex.Message);
        }
    }
}
