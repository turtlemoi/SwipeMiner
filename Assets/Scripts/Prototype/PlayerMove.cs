using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("설정 값")]
    public float speed = 15.0f;       // 이동 속도
    public float stopDistance = 0.55f; // 벽 앞에서 얼마나 떨어져서 멈출지 (플레이어 크기 절반 + 알파)
    public LayerMask wallLayer;       // '벽'으로 인식할 레이어 (중요!)

    private Vector3 targetPosition;
    private bool isMoving = false;

    void Start()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
        // 1. 이동 중일 때 처리
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition;
                isMoving = false;
            }
        }
        // 2. 멈춰 있을 때 입력 처리 (Y축 이동으로 변경!)
        else
        {
            if (Input.GetKeyDown(KeyCode.UpArrow)) MoveInDirection(Vector2.up);
            else if (Input.GetKeyDown(KeyCode.DownArrow)) MoveInDirection(Vector2.down);
            else if (Input.GetKeyDown(KeyCode.LeftArrow)) MoveInDirection(Vector2.left);
            else if (Input.GetKeyDown(KeyCode.RightArrow)) MoveInDirection(Vector2.right);
        }
    }

    void MoveInDirection(Vector2 direction)
    {
        // ★ 핵심: 3D Raycast 대신 2D Raycast 사용!
        // 내 위치에서 방향으로 레이저를 쏩니다.
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 20.0f, wallLayer);

        if (hit.collider != null)
        {
            // 벽을 감지했으면, 벽 위치(hit.point)에서 반대 방향으로 조금 물러난 곳을 목표로 잡음
            // (direction * stopDistance)를 빼주는 이유는 벽 속에 파묻히지 않기 위함입니다.
            // 2D에서는 Vector2와 Vector3 혼용이 가능하지만 명시적으로 변환해줍니다.
            targetPosition = (Vector3)hit.point - (Vector3)(direction * stopDistance);

            Debug.Log("벽 감지됨: " + hit.collider.name); // 디버그용 로그
            isMoving = true;
        }
        else
        {
            // 벽이 없으면 그냥 이동 (테스트용)
            // 실제 게임에서는 맵 밖으로 나가면 안 되니 나중엔 막아야 합니다.
            targetPosition = transform.position + (Vector3)direction * 3.0f;
            isMoving = true;
        }
    }

    // 에디터에서 레이저를 눈으로 보여주는 기능 (디버깅용)
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.up * 2);
        Gizmos.DrawRay(transform.position, Vector3.down * 2);
        Gizmos.DrawRay(transform.position, Vector3.left * 2);
        Gizmos.DrawRay(transform.position, Vector3.right * 2);
    }
}
