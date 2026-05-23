using UnityEngine;

public class Coin : MonoBehaviour
{
    public float rotateSpeed = 150f; // 코인이 회전하는 속도

    void Update()
    {
        // 매 프레임마다 Y축(또는 Z축)을 기준으로 빙글빙글 돕니다.
        // 코인 모델링의 방향에 따라 Vector3.up을 Vector3.forward 등으로 바꿔주세요.
        transform.Rotate(Vector3.right * rotateSpeed * Time.deltaTime);
    }

    // 플레이어가 코인에 닿았을 때 (트리거 충돌)
    private void OnTriggerEnter(Collider other)
    {
        // 부딪힌 대상이 플레이어인지 태그로 확인합니다.
        if (other.CompareTag("Player"))
        {
            // 코인 매니저에게 점수를 올려달라고 부탁합니다! (싱글톤 덕분에 쉽게 호출 가능)
            CoinManager.instance.AddCoin(1);

            // 코인을 화면에서 숨깁니다. (나중에 바닥이 재배치될 때 다시 나타날 수 있게)
            gameObject.SetActive(false);
        }
    }
}