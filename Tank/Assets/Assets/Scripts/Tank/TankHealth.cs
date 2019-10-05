using UnityEngine;
using UnityEngine.UI;

namespace Tank
{
    public class TankHealth : MonoBehaviour
    {
        public float m_StartingHealth = 100f; // 시작할 때 탱크의 체력
        public Slider m_Slider; // 현재 탱크의 체력량을 나타내는 슬라이더
        public Image m_FillImage; // 슬라이더의 이미지
        public Color m_FullHealthColor = Color.green; // 체력이 최대일 때 색깔 = 녹색
        public Color m_ZeroHealthColor = Color.red; // 체력이 최대가 아닐 때 색깔 = 빨간색
        public GameObject m_ExplosionPrefab; // Awake 함수에서 인스턴스화되고 탱크가 죽을 때마다 사용되는 프리팹
        private ParticleSystem m_ExplosionParticles; // 탱크가 파괴될 때 입자 시스템 작동
        private float m_CurrentHealth; // 현재 탱크의 체력
        private bool m_Dead; // 탱크의 체력이 0이 되었는지 아닌지

        private void Awake()
        {
            m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>(); // 폭발 프리팹을 인스턴스화하고 입자 시스템애 대한 참조 저장
            m_ExplosionParticles.gameObject.SetActive(false); // 필요할 때 활성화 할 수 있도록 비활성화
        }

        private void SetHealthUI()
        {
            m_Slider.value = m_CurrentHealth; // 슬라이더 값 = 현재 탱크의 체력
            m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth); // 체력량을 백분율로 계산해 체력이 0일 때와 100일 때 사이의 색깔을 구현
        }

        private void OnEnable() // 스크립트가 켜질 때 Awake 함수 바로 다음에 호출되는 함수
        {
            m_CurrentHealth = m_StartingHealth; // 현재 탱크의 체력 = 시작할 때 탱크의 체력
            m_Dead = false; // 탱크 생존
            SetHealthUI(); // SetHealthUI 메서드 호출
        }

        private void OnDeath()
        {
            m_Dead = true; // 탱크 사망
            m_ExplosionParticles.transform.position = transform.position; // 인스턴스화한 폭발 프리팹을 탱크의 위치로 이동
            m_ExplosionParticles.gameObject.SetActive(true); // 활성화
            m_ExplosionParticles.Play(); // 탱크 폭발 입자 시스템 작동
            gameObject.SetActive(false); //탱크 비활성화
        }

        public void TakeDamage(float amount)
        {
            m_CurrentHealth -= amount; // 피해량에 따라 현재 체력량을 감소
            SetHealthUI(); // SetHealthUI 메서드 호출

            if(m_CurrentHealth <= 0f && !m_Dead) // 만약 현재 탱크의 체력이 0이하가 된다면
            {
                OnDeath(); // OnDeath 메서드 호출
            }
        }
    }
}
