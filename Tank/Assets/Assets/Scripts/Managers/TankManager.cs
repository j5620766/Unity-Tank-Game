using System;
using UnityEngine;

namespace Tank
{
    [Serializable]
    public class TankManager
    {
        public Color m_PlayerColor; // 탱크의 색깔
        public Transform m_SpawnPoint; // 탱크가 생성될 때 위치와 방향
        [HideInInspector] public int m_PlayerNumber; // 탱크의 번호
        [HideInInspector] public string m_ColoredPlayerText; // 번호와 함께 탱크를 나타내는 문자열
        [HideInInspector] public GameObject m_Instance; // 탱크가 생성될 때 인스턴스에 대한 참조
        [HideInInspector] public int m_Wins; // 플레이어가 이긴 횟수
        private TankMovement m_Movement; // TankMovement에 대한 참조
        private TankShooting m_Shooting; // TankShooting에 대한 참조
        private GameObject m_CanvasGameObject; // 각 라운드의 시작 및 종료 단계에서 UI 비활성화하는 데 사용되는 변수

        public void Setup()
        {
            m_Movement = m_Instance.GetComponent<TankMovement>(); // TankMovement에 대한 참조를 불러옴
            m_Shooting = m_Instance.GetComponent<TankShooting>(); // TankShooting에 대한 참조를 불러옴
            m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas>().gameObject; // Canvas에 대한 참조를 불러옴
            m_Movement.m_PlayerNumber = m_PlayerNumber; // 탱크의 번호를 일정하게 설정
            m_Shooting.m_PlayerNumber = m_PlayerNumber; // 탱크의 번호를 일정하게 설정
            m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerNumber + "</color>"; // 탱크의 색상과 번호에 따라 올바른 색상을 사용해 문자열 생성
            MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer>(); //탱크의 모든 렌더러를 불러옴

            for(int i = 0; i < renderers.Length; i++) // 렌더러의 길이만큼 반복
            {
                renderers[i].material.color = m_PlayerColor; // 재료의 색상을 탱크의 색상으로 설정
            }
        }

        public void DisableControl() // 플레이어가 탱크를 제어할 수 없어야 하는 단계
        {
            m_Movement.enabled = false; // Movement = false
            m_Shooting.enabled = false; // Shooting = false
            m_CanvasGameObject.SetActive(false); // m_CanvasGameObject = false
        }

        public void EnableControl() // 플레이어가 탱크를 제어할 수 있어야 하는 단계
        {
            m_Movement.enabled = true; // Movement = true
            m_Shooting.enabled = true; // Shooting = true
            m_CanvasGameObject.SetActive(true); // m_CanvasGameObject = true
        }

        public void Reset() // 각 라운드가 시작될 때
        {
            m_Instance.transform.position = m_SpawnPoint.position; // 탱크의 위치 초기화
            m_Instance.transform.rotation = m_SpawnPoint.rotation; // 탱크의 방향 초기화
            m_Instance.SetActive(false); // 인스턴스 초기화
            m_Instance.SetActive(true); // 인스턴스 재실행
        }
    }
}
