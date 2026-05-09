using UnityEngine;

public class TrackingPlayerCamera : MonoBehaviour
{
    // 플레이어의 transform 컴포넌트
    public Transform Player;
    // 플레이어와 카메라 사이 항상 존재해야하는 거리 값
    public float distanceBetween;

    // 플레이어가 움직인 후 카메라가 따라가야 하므로,
    // LateUpdate() 메서드 이용
    private void LateUpdate()
    {
        // 카메라의 현재 위치
        Vector3 cameraPos = transform.position;
        // 카메라의 위치(y 좌표만)를 플레이어 y 좌표에서 특정 거리만큼 떨어진 위치로 이동시키는 부분
        cameraPos.y = Player.position.y + distanceBetween;
        transform.position = cameraPos;
    }
}