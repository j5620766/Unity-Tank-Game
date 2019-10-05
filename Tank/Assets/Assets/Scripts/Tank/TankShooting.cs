using UnityEngine;
using UnityEngine.UI;

namespace Tank
{
    public class TankShooting : MonoBehaviour
    {
        public int m_PlayerNumber = 1; // 각각의 탱크가 어느 플레이어에 속하는지 식별하기 위한 변수
        public Rigidbody m_Shell; // 포탄 프리팹 설정
        public Transform m_FireTransform; // 포탄이 발사되는 위치
        public Slider m_AimSlider; // 현재 발사력을 표시
        public float m_MinLaunchForce = 15f; // 발사 버튼을 누르지 않으면 포탄에 가해지는 힘
        public float m_MaxLaunchForce = 30f; // 발사 버튼을 최대로 눌렀을 때 포탄에 가해지는 힘
        public float m_MaxChargeTime = 0.75f; // 포탄이 최대 힘으로 발사될 때 최대로 충전하는 시간 
        private string m_FireButton; // 포탄을 발사하는 데 이용되는 입력축 이름
        private float m_CurrentLaunchForce; // 발사 버튼에서 손을 떼면 포탄에 가해지는 힘
        private float m_ChargeSpeed; // 최대 충전 시간에 근거해 발사력이 증가하는 속도
        private bool m_Fired; // 버튼을 눌러 포탄을 발사했는지 여부

        private void OnEnable() // 스크립트가 켜질 때 Awake 함수 바로 다음에 호출되는 함수
        {
            m_CurrentLaunchForce = m_MinLaunchForce; // 탱크가 나타나면 발사력을 재설정
            m_AimSlider.value = m_MinLaunchForce; // 탱크가 나타나면 UI를 재설정
        }

        private void Start() // Update 함수가 호출되기 직전에 한 번만 호출되는 함수
        {
            m_FireButton = "Fire" + m_PlayerNumber; // 플레이어 번호에 따른 발사축 이름 저장
            m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime; // 발사력이 증가하는 속도는 최대 충전 시간에 의한 가능한 힘의 범위
        }

        private void Fire()
        {
            m_Fired = true; // 발사
            Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody; // 포탄의 인스턴스 생성 및 rigidbody 에 대한 참조 저장
            shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward; // 발사 위치의 전방에서 포탄 속도 = 현재 발사력
            m_CurrentLaunchForce = m_MinLaunchForce; // 발사력을 재설정
        }

        private void Update() // 매 프레임마다 호출되는 함수
        {
            m_AimSlider.value = m_MinLaunchForce; // 슬라이더에 표시되는 최소 발사력의 기본값

            if(m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired) // 최대 힘이 초과되고 포탄이 아직 발사되지 않은 경우
            {
                m_CurrentLaunchForce = m_MaxLaunchForce; // 현재 발사력 = 최대 발사력
                Fire(); // Fire 메서드 호출
            }
            else if(Input.GetButtonDown(m_FireButton)) // 버튼이 눌렸을 때
            {
                m_Fired = false; // 발사 중지
                m_CurrentLaunchForce = m_MinLaunchForce; // 현재 발사력 = 최소 발사력
            }
            else if(Input.GetButton(m_FireButton) && !m_Fired) // 버튼을 누르고 있지만, 포탄이 아직 발사되지 않은 경우
            {
                m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime; // 시간에 따라 발사력 증가
                m_AimSlider.value = m_CurrentLaunchForce; // 슬라이더 값 업데이트
            }
            else if(Input.GetButtonUp(m_FireButton) && !m_Fired) // 버튼이 해제되고 포탄이 아직 발사되지 않은 경우
            {
                Fire(); // Fire 메서드 호출
            }
        }
    }
}
