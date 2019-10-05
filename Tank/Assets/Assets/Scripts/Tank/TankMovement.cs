using UnityEngine;

namespace Tank
{
    public class TankMovement : MonoBehaviour
    {
        public int m_PlayerNumber = 1; // 각각의 탱크가 어느 플레이어에 속하는지 식별하기 위한 변수
        public float m_Speed = 12f; // 탱크의 속도
        public float m_TurnSpeed = 180f; // 탱크의 회전 속도
        private string m_MovementAxisName; // 전진 및 후진을 위한 입력축의 이름
        private string m_TurnAxisName; // 회전을 위한 입력축의 이름
        private Rigidbody m_Rigidbody; // 탱크 이동을 위한 물리 현상을 추가시키는 요소
        private float m_MovementInputValue; // 이동 입력의 현재 값
        private float m_TurnInputValue; // 회전 입력의 현재 값
        private ParticleSystem[] m_particleSystems; // 탱크가 사용하는 모든 입자 시스템

        private void Awake() // Start 함수가 호출되기 이전에 스크립트 객체가 로딩될 때 호출되는 함수
        {
            m_Rigidbody = GetComponent<Rigidbody>(); // 게임 오브젝트에 물리 현상을 추가할 수 있는 요소를 추가
        }

        private void OnEnable() // 스크립트가 켜질 때 Awake 함수 바로 다음에 호출되는 함수
        {
            m_Rigidbody.isKinematic = false; //외력이 작용할 수 있게 함
            m_MovementInputValue = 0f; // 움직임 입력값 초기화
            m_TurnInputValue = 0f; // 회전 입력값 초기화
            m_particleSystems = GetComponentsInChildren<ParticleSystem>(); // 탱크를 움직이기 위한 입자 시스템

            for(int i = 0; i < m_particleSystems.Length; ++i)
            {
                m_particleSystems[i].Play(); // 탱크의 모든 입자 시스템 실행
            }
        }

        private void Start() // Update 함수가 호출되기 직전에 한 번만 호출되는 함수
        {
            m_MovementAxisName = "Vertical" + m_PlayerNumber; // 플레이어 번호에 따른 움직임축 이름 저장
            m_TurnAxisName = "Horizontal" + m_PlayerNumber; // 플레이어 번호에 따른 회전축 이름 저장
        }

        private void Update() // 매 프레임마다 호출되는 함수
        {
            m_MovementInputValue = Input.GetAxis(m_MovementAxisName); // 사용자의 입력값을 추출
            m_TurnInputValue = Input.GetAxis(m_TurnAxisName); // 사용자의 입력값을 추출
        }

        private void Move() // 탱크의 움직임
        {
            Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime; // 입력값에 따라 벡터를 형성
            m_Rigidbody.MovePosition(m_Rigidbody.position + movement); // 벡터를 Rigidbody에 적용
        }

        private void Turn() //탱크의 회전
        {
            float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime; // 입력값에 따라 변수값을 설정
            Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f); // 유니티의 회전 단위인 Quaternian으로 변환
            m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation); // 회전을 rigidbody에 적용
        }

        private void FixedUpdate() // 매 렌더링 프레임 사이에 여러 번 호출되는 함수
        {
            Move(); // 탱크의 움직임 조정
            Turn(); // 탱크의 회전 조정
        }

        private void OnDisable() // 스크립트가 꺼질 때 또는, 스크립트가 붙은 게임 오브젝트가 제거될 때 호출되는 함수
        {
            m_Rigidbody.isKinematic = true; // 외력이 작용하지 않게 하여 더이상 움직이지 못하게 함

            for(int i = 0; i < m_particleSystems.Length; ++i)
            {
                m_particleSystems[i].Stop(); // 탱크의 모든 입자 시스템을 중지
            }
        }
    }
}
