using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Tank
{
    public class GameManager : MonoBehaviour
    {
        public int m_NumRoundsToWin = 5; // 게임에서 승리하기 위해 이겨야 하는 라운드 수
        public float m_StartDelay = 3f; // 라운드가 시작될 때까지의 지연 시간
        public float m_EndDelay = 3f; // 라운드가 끝날 때까지의 지연 시간
        public CameraControl m_CameraControl; // CameraControl 스크립트에 대한 참조
        public Text m_MessageText; // 텍스트 표시를 위한 참조
        public GameObject m_TankPrefab; // 플레이어가 제어할 프리팹에 대한 참조
        public TankManager[] m_Tanks; // 탱크의 다양한 측면을 활성화, 비활성화하기 위한 관리자 모음
        private int m_RoundNumber; // 현재 진행 중인 라운드
        private WaitForSeconds m_StartWait; // 라운드가 시작될 때까지 지연되는 데 사용되는 참조
        private WaitForSeconds m_EndWait; // 라운드가 끝날 때까지 지연되는 데 사용되는 참조
        private TankManager m_RoundWinner; // 현재 라운드의 승자를 알려주는 참조
        private TankManager m_GameWinner; // 게임에서 누가 승리했는지 알려주는 참조

        private void SpawnAllTanks()
        {
            for(int i = 0; i < m_Tanks.Length; i++) // 탱크의 수만큼 반복
            {
                m_Tanks[i].m_Instance = Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject; // 탱크 설정
                m_Tanks[i].m_PlayerNumber = i + 1; // 탱크 번호 부여
                m_Tanks[i].Setup(); // 탱크 생성
            }
        }

        private void SetCameraTargets()
        {
            Transform[] targets = new Transform[m_Tanks.Length]; // 탱크의 수만큼 변환 모음 제작

            for(int i = 0; i < targets.Length; i++) // 변환 모음의 수만큼 반복
            {
                targets[i] = m_Tanks[i].m_Instance.transform; // 탱크 변환 설정
            }
            m_CameraControl.m_Targets = targets; // 카메라가 따라가야 할 대상
        }

        private void ResetAllTanks()
        {
            for(int i = 0; i < m_Tanks.Length; i++) // 탱크의 수만큼 반복
            {
                m_Tanks[i].Reset(); // 탱크 재설정
            }
        }

        private void EnableTankControl()
        {
            for(int i = 0; i < m_Tanks.Length; i++) // 탱크의 수만큼 반복
            {
                m_Tanks[i].EnableControl(); // 탱크 작동
            }
        }

        private void DisableTankControl()
        {
            for(int i = 0; i < m_Tanks.Length; i++) // 탱크의 수만큼 반복
            {
                m_Tanks[i].DisableControl(); // 탱크 작동 불가능
            }
        }

        private IEnumerator RoundStarting()
        {
            ResetAllTanks(); // ResetAllTanks 메서드 호출
            DisableTankControl(); // DisableTankControl 메서드 호출
            m_CameraControl.SetStartPositionAndSize(); // 탱크에 맞게 카메라의 줌과 위치를 재설정
            m_RoundNumber++; // 라운드 +1
            m_MessageText.text = "ROUND " + m_RoundNumber; // 화면에 라운드 수를 나타내는 텍스트 표시
            yield return m_StartWait; // 지정된 시간 동안 순차적으로 대기
        }

        private bool OneTankLeft()
        {
            int numTanksLeft = 0; // 남은 탱크 수

            for(int i = 0; i < m_Tanks.Length; i++) // 탱크 수만큼 반복
            {
                if(m_Tanks[i].m_Instance.activeSelf) // 탱크가 활성화되어 있으면
                    numTanksLeft++; // 남은 탱크 수 추가
            }
            return numTanksLeft <= 1; // 남은 탱크의 수가 1보다 작거나 같으면 true 반환, 그렇지 않으면 false 반환
        }

        private IEnumerator RoundPlaying()
        {
            EnableTankControl(); // EnableTankControl 메서드 호출
            m_MessageText.text = string.Empty; // 화면에 호출된 텍스트 제거

            while(!OneTankLeft()) // 탱크가 하나라도 없어질 때까지 반복
            {
                yield return null;  // 순차적으로 프레임 출력
            }
        }

        private TankManager GetRoundWinner()
        {
            for(int i = 0; i < m_Tanks.Length; i++) // 탱크의 수만큼 반복
            {
                if(m_Tanks[i].m_Instance.activeSelf) // 활동하고 있는 탱크가 있다면
                    return m_Tanks[i]; // 탱크값 반환
            }
            return null; // 아무 탱크도 활동하고 있지 않다면 null값 반환
        }

        private TankManager GetGameWinner()
        {
            for(int i = 0; i < m_Tanks.Length; i++) // 탱크의 수만큼 반복
            {
                if (m_Tanks[i].m_Wins == m_NumRoundsToWin) // 탱크의 라운드 승리 횟수가 우승 조건을 만족할 때
                    return m_Tanks[i]; // 탱크값 반환
            }
            return null; // 라운드 승리 횟수가 우승 조건을 만족시키지 못할 때 null값 반환
        }

        private string EndMessage()
        {
            string message = "DRAW!"; // 기본적으로 라운드가 종료되면 호출되는 텍스트

            if(m_RoundWinner != null) // 만약 라운드 승자가 있다면
                message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!"; // message = 플레이어 + WINS THE ROUND!

            message += "\n\n\n\n"; // 행 교체

            for(int i = 0; i < m_Tanks.Length; i++) // 탱크의 수만큼 반복
            {
                message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " WINS\n"; // message += 플레이어 : 이긴 라운드 횟수 + WIN
            }

            if(m_GameWinner != null) // 만약 게임의 승자가 있다면
                message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!"; // message = 플레이어 + WINS THE GAME

            return message; // message값 반환
        }

        private IEnumerator RoundEnding()
        {
            DisableTankControl(); // DisableTankControl 메서드 호출
            m_RoundWinner = null; // 이전 라운드 승자 초기화
            m_RoundWinner = GetRoundWinner(); // 현재 라운드 승자 = GetRoundWinner 메서드 호출

            if(m_RoundWinner != null) // 만약 라운드 승자가 있다면
                m_RoundWinner.m_Wins++; // 승리 라운드 스코어 추가

            m_GameWinner = GetGameWinner(); // 게임의 승자 = GetGameWinner 메서드 호출
            string message = EndMessage(); // message = EndMessage 메서드 호출
            m_MessageText.text = message; // messsage 텍스트 출력
            yield return m_EndWait; // 지정된 시간 동안 순차적으로 대기
        }

        private IEnumerator GameLoop()
        {
            yield return StartCoroutine(RoundStarting()); // RoundStarting 메서드 순차적으로 진행
            yield return StartCoroutine(RoundPlaying()); // RoundPlaying 메서드 순차적으로 진행
            yield return StartCoroutine(RoundEnding()); // RoundEnding 메서드 순차적으로 진행

            if(m_GameWinner != null) // 만약 게임의 승자가 있다면
            {
                SceneManager.LoadScene(0); // Scene 다시 로드
            }
            else // 그렇지 않다면
            {
                StartCoroutine(GameLoop()); // GameLoop 메서드 계속 호출
            }
        }

        private void Start() // Update 함수가 호출되기 직전에 한 번만 호출되는 함수
        {
            m_StartWait = new WaitForSeconds(m_StartDelay); // 시작할 때의 지연 시간 생성
            m_EndWait = new WaitForSeconds(m_EndDelay); // 끝날 때의 지연 시간 생성
            SpawnAllTanks(); // SpawnAllTanks 메서드 호출
            SetCameraTargets(); // SetCameraTargets 메서드 호출
            StartCoroutine(GameLoop()); // GameLoop 메서드 호출
        }
    }
}
