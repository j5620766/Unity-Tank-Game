using UnityEngine;

namespace Tank
{
    public class UIDirectionControl : MonoBehaviour
    {
        public bool m_UseRelativeRotation = true; // 게임 오브젝트에 회전 적용
        private Quaternion m_RelativeRotation; // 회전값

        private void Start() // Update 함수가 호출되기 직전에 한 번만 호출되는 함수
        {
            m_RelativeRotation = transform.parent.localRotation; // 회전값 = 부모 객체의 회전값
        }

        private void Update() // 매 프레임마다 호출되는 함수
        {
            if(m_UseRelativeRotation) // 만약 게임 오브젝트에 회전이 적용되어 있다면
               transform.rotation = m_RelativeRotation; // 회전값 적용
        }
    }
}
