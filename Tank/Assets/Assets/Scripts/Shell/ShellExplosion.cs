using UnityEngine;

namespace Tank
{
    public class ShellExplosion : MonoBehaviour
    {
        public LayerMask m_TankMask; // 폭발의 영향을 필터링하는 데 사용되는 참ㅁ조
        public ParticleSystem m_ExplosionParticles; // 폭발시 재생할 입자 시스템
        public float m_MaxDamage = 100f; // 탱크 중심에서 폭발했을 때 데미지
        public float m_ExplosionForce = 1000f; // 폭발 중심에서 탱크에 추가되는 힘의 양
        public float m_MaxLifeTime = 2f; // 포탄이 제거되는 시간
        public float m_ExplosionRadius = 5f; // 탱크가 폭발로부터 영향을 받는 거리

        private void Start()
        {
            Destroy(gameObject, m_MaxLifeTime); // MaxLifeTime까지 포탄이 파괴되지 않으면 자동 파괴
        }

        private float CalculateDamage(Vector3 targetPosition)
        {
            Vector3 explosionToTarget = targetPosition - transform.position; // 포탄부터 타겟까지의 벡터 생성
            float explosionDistance = explosionToTarget.magnitude; // 포탄부터 타겟까지의 거리 계산
            float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius; // 목표물에 떨어진 최대 폭발 반경 거리의 비율을 계산
            float damage = relativeDistance * m_MaxDamage; // 최대 피해의 비율로 피해량 계산
            damage = Mathf.Max(0f, damage); // 최소 데미지 = 0, 최대 데미지 = damage
            return damage; // damage값 호출
        }

        private void OnTriggerEnter(Collider other) // 충돌했을 때 호출되는 함수
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask); // 모든 충돌체를 포탄의 현재 위치에서 폭발 반경까지 수집

            for(int i = 0; i < colliders.Length; i++) // 충돌체의 수만큼 반복
            {
                Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>(); // 충돌체들의 rigidbody 수집

                if(!targetRigidbody) // 만약 rigidbody를 가지고 있지 않을 경우
                   continue; // 무시

                targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius); // 폭발 힘 추가
                TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>(); // TankHealth 스크립트 수집

                if(!targetHealth) // TankHealth 스크립트가 게임 오브젝트에 붙어 있지 않다면
                   continue; // 무시하고 다음 충돌체로 이동

                float damage = CalculateDamage(targetRigidbody.position); // 포탄으로부터 떨어진 거리에 따라 피해량 계산
                targetHealth.TakeDamage(damage); // 탱크에게 피해 추가
            }
            m_ExplosionParticles.transform.parent = null; // 부모 객체로부터 폭발 입자 시스템 분리
            m_ExplosionParticles.Play(); // 폭발 입자 시스템 작동
            ParticleSystem.MainModule mainModule = m_ExplosionParticles.main; // 입자가 끝나면
            Destroy(m_ExplosionParticles.gameObject, mainModule.duration); // 입자 파괴
            Destroy(gameObject); // 포탄 제거
        }
    }
}
