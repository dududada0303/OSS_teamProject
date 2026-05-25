using UnityEngine;

public class TrackingPlayerCamera : MonoBehaviour
{
    public Transform Player;
    public float distanceBetween;

    private void LateUpdate()
    {
        // 플레이어를 따라가는 코드를 모두 지웠습니다.
        // 이제 카메라는 유니티 씬 뷰에 배치해 둔 그 자리에 완전히 고정됩니다.
    }
}