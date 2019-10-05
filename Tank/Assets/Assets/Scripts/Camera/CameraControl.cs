using UnityEngine;

namespace Tank
{
    public class CameraControl : MonoBehaviour
    {
        public float m_DampTime = 0.2f; // 카메라의 초점을 다시 맞추는 데 걸리는 시간
        public float m_ScreenEdgeBuffer = 4f; // 맨 위, 맨 아래 대상과 화면 가장자리 사이의 공간
        public float m_MinSize = 6.5f; // 카메라의 최소 직교 크기
        [HideInInspector] public Transform[] m_Targets; // 카메라가 포함해야 할 모든 대상
        private Camera m_Camera; // 카메라 참조
        private float m_ZoomSpeed; // 카메라의 제동을 위한 속도
        private Vector3 m_MoveVelocity; // 위치 제동을 위한 참조
        private Vector3 m_DesiredPosition; // 카메라가 이동하는 위치


        private void Awake() // 스크립트 객체가 로딩될 때 Start 함수 이전에 한 번만 호출되는 함수
        {
            m_Camera = GetComponentInChildren<Camera>(); // Camera 요소 수집
        }

        private void FindAveragePosition()
        {
            Vector3 averagePos = new Vector3();
            int numTargets = 0;

            for(int i = 0; i < m_Targets.Length; i++) //  타겟의 수만큼 반복
            {
                if(!m_Targets[i].gameObject.activeSelf) // 대상이 활성화되어 있지 않다면
                   continue; // 무시

                averagePos += m_Targets[i].position; // averagePos에 타겟의 위치 추가
                numTargets++;
            }

            if(numTargets > 0) // numTargets가 0보다 크면
               averagePos /= numTargets; // 위치의 합을 numTargets로 나누기

            averagePos.y = transform.position.y; // y값 고정
            m_DesiredPosition = averagePos; // 카메라의 위치 = averagePos
        }

        private void Move()
        {
            FindAveragePosition(); // FindAveragePosition 메서드 호출
            transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime); // 카메라 이동
        }

        private float FindRequiredSize()
        {
            Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition); // 카메라가 이동하는 위치 탐색
            float size = 0f; // 시작할 때 카메라의 크기

            for(int i = 0; i < m_Targets.Length; i++) // 타겟의 수만큼 반복
            {
                if(!m_Targets[i].gameObject.activeSelf) // 대상이 활성화되어 있지 않다면
                   continue; // 무시

                Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position); // 대상의 위치
                Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos; // 대상의 위치에서 카메라가 이동하는 위치를 뺀 값
                size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y)); // 현재 사이즈 중에서 가장 큰 것을 택하고 카메라에서 탱크의 위, 아래 거리를 선택
                size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) / m_Camera.aspect); // 카메라의 왼쪽 또는 오른쪽에 있는 탱크를 기준으로 현재 크기 중 가장 큰 크기와 계산된 크기를 선택
            }
            size += m_ScreenEdgeBuffer; // 가장자리의 공간 추가
            size = Mathf.Max(size, m_MinSize); // 카메라의 크기 설정
            return size; // size값 반환
        }

        private void Zoom()
        {
            float requiredSize = FindRequiredSize(); // 필요한 크기 탐색
            m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime); // 카메라의 직교 크기 설정
        }

        private void FixedUpdate()
        {
            Move(); // Move 메서드 호출
            Zoom(); // Zoom 메서드 호출
        }

        public void SetStartPositionAndSize()
        {
            FindAveragePosition(); // FindAveragePosition 메서드 호출
            transform.position = m_DesiredPosition; // 제동 없이 카메라의 위치 설정
            m_Camera.orthographicSize = FindRequiredSize(); // 카메라의 크기 설정
        }
    }
}
