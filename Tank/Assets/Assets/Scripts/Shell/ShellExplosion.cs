using UnityEngine;

namespace Tank
{
    public class ShellExplosion : MonoBehaviour
    {
        public LayerMask m_TankMask; // ������ ������ ���͸��ϴ� �� ���Ǵ� ������
        public ParticleSystem m_ExplosionParticles; // ���߽� ����� ���� �ý���
        public float m_MaxDamage = 100f; // ��ũ �߽ɿ��� �������� �� ������
        public float m_ExplosionForce = 1000f; // ���� �߽ɿ��� ��ũ�� �߰��Ǵ� ���� ��
        public float m_MaxLifeTime = 2f; // ��ź�� ���ŵǴ� �ð�
        public float m_ExplosionRadius = 5f; // ��ũ�� ���߷κ��� ������ �޴� �Ÿ�

        private void Start()
        {
            Destroy(gameObject, m_MaxLifeTime); // MaxLifeTime���� ��ź�� �ı����� ������ �ڵ� �ı�
        }

        private float CalculateDamage(Vector3 targetPosition)
        {
            Vector3 explosionToTarget = targetPosition - transform.position; // ��ź���� Ÿ�ٱ����� ���� ����
            float explosionDistance = explosionToTarget.magnitude; // ��ź���� Ÿ�ٱ����� �Ÿ� ���
            float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius; // ��ǥ���� ������ �ִ� ���� �ݰ� �Ÿ��� ������ ���
            float damage = relativeDistance * m_MaxDamage; // �ִ� ������ ������ ���ط� ���
            damage = Mathf.Max(0f, damage); // �ּ� ������ = 0, �ִ� ������ = damage
            return damage; // damage�� ȣ��
        }

        private void OnTriggerEnter(Collider other) // �浹���� �� ȣ��Ǵ� �Լ�
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask); // ��� �浹ü�� ��ź�� ���� ��ġ���� ���� �ݰ���� ����

            for(int i = 0; i < colliders.Length; i++) // �浹ü�� ����ŭ �ݺ�
            {
                Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>(); // �浹ü���� rigidbody ����

                if(!targetRigidbody) // ���� rigidbody�� ������ ���� ���� ���
                   continue; // ����

                targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius); // ���� �� �߰�
                TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>(); // TankHealth ��ũ��Ʈ ����

                if(!targetHealth) // TankHealth ��ũ��Ʈ�� ���� ������Ʈ�� �پ� ���� �ʴٸ�
                   continue; // �����ϰ� ���� �浹ü�� �̵�

                float damage = CalculateDamage(targetRigidbody.position); // ��ź���κ��� ������ �Ÿ��� ���� ���ط� ���
                targetHealth.TakeDamage(damage); // ��ũ���� ���� �߰�
            }
            m_ExplosionParticles.transform.parent = null; // �θ� ��ü�κ��� ���� ���� �ý��� �и�
            m_ExplosionParticles.Play(); // ���� ���� �ý��� �۵�
            ParticleSystem.MainModule mainModule = m_ExplosionParticles.main; // ���ڰ� ������
            Destroy(m_ExplosionParticles.gameObject, mainModule.duration); // ���� �ı�
            Destroy(gameObject); // ��ź ����
        }
    }
}
