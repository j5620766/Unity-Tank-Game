using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Tank
{
    public class GameManager : MonoBehaviour
    {
        public int m_NumRoundsToWin = 5; // ���ӿ��� �¸��ϱ� ���� �̰ܾ� �ϴ� ���� ��
        public float m_StartDelay = 3f; // ���尡 ���۵� �������� ���� �ð�
        public float m_EndDelay = 3f; // ���尡 ���� �������� ���� �ð�
        public CameraControl m_CameraControl; // CameraControl ��ũ��Ʈ�� ���� ����
        public Text m_MessageText; // �ؽ�Ʈ ǥ�ø� ���� ����
        public GameObject m_TankPrefab; // �÷��̾ ������ �����տ� ���� ����
        public TankManager[] m_Tanks; // ��ũ�� �پ��� ������ Ȱ��ȭ, ��Ȱ��ȭ�ϱ� ���� ������ ����
        private int m_RoundNumber; // ���� ���� ���� ����
        private WaitForSeconds m_StartWait; // ���尡 ���۵� ������ �����Ǵ� �� ���Ǵ� ����
        private WaitForSeconds m_EndWait; // ���尡 ���� ������ �����Ǵ� �� ���Ǵ� ����
        private TankManager m_RoundWinner; // ���� ������ ���ڸ� �˷��ִ� ����
        private TankManager m_GameWinner; // ���ӿ��� ���� �¸��ߴ��� �˷��ִ� ����

        private void SpawnAllTanks()
        {
            for(int i = 0; i < m_Tanks.Length; i++) // ��ũ�� ����ŭ �ݺ�
            {
                m_Tanks[i].m_Instance = Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject; // ��ũ ����
                m_Tanks[i].m_PlayerNumber = i + 1; // ��ũ ��ȣ �ο�
                m_Tanks[i].Setup(); // ��ũ ����
            }
        }

        private void SetCameraTargets()
        {
            Transform[] targets = new Transform[m_Tanks.Length]; // ��ũ�� ����ŭ ��ȯ ���� ����

            for(int i = 0; i < targets.Length; i++) // ��ȯ ������ ����ŭ �ݺ�
            {
                targets[i] = m_Tanks[i].m_Instance.transform; // ��ũ ��ȯ ����
            }
            m_CameraControl.m_Targets = targets; // ī�޶� ���󰡾� �� ���
        }

        private void ResetAllTanks()
        {
            for(int i = 0; i < m_Tanks.Length; i++) // ��ũ�� ����ŭ �ݺ�
            {
                m_Tanks[i].Reset(); // ��ũ �缳��
            }
        }

        private void EnableTankControl()
        {
            for(int i = 0; i < m_Tanks.Length; i++) // ��ũ�� ����ŭ �ݺ�
            {
                m_Tanks[i].EnableControl(); // ��ũ �۵�
            }
        }

        private void DisableTankControl()
        {
            for(int i = 0; i < m_Tanks.Length; i++) // ��ũ�� ����ŭ �ݺ�
            {
                m_Tanks[i].DisableControl(); // ��ũ �۵� �Ұ���
            }
        }

        private IEnumerator RoundStarting()
        {
            ResetAllTanks(); // ResetAllTanks �޼��� ȣ��
            DisableTankControl(); // DisableTankControl �޼��� ȣ��
            m_CameraControl.SetStartPositionAndSize(); // ��ũ�� �°� ī�޶��� �ܰ� ��ġ�� �缳��
            m_RoundNumber++; // ���� +1
            m_MessageText.text = "ROUND " + m_RoundNumber; // ȭ�鿡 ���� ���� ��Ÿ���� �ؽ�Ʈ ǥ��
            yield return m_StartWait; // ������ �ð� ���� ���������� ���
        }

        private bool OneTankLeft()
        {
            int numTanksLeft = 0; // ���� ��ũ ��

            for(int i = 0; i < m_Tanks.Length; i++) // ��ũ ����ŭ �ݺ�
            {
                if(m_Tanks[i].m_Instance.activeSelf) // ��ũ�� Ȱ��ȭ�Ǿ� ������
                    numTanksLeft++; // ���� ��ũ �� �߰�
            }
            return numTanksLeft <= 1; // ���� ��ũ�� ���� 1���� �۰ų� ������ true ��ȯ, �׷��� ������ false ��ȯ
        }

        private IEnumerator RoundPlaying()
        {
            EnableTankControl(); // EnableTankControl �޼��� ȣ��
            m_MessageText.text = string.Empty; // ȭ�鿡 ȣ��� �ؽ�Ʈ ����

            while(!OneTankLeft()) // ��ũ�� �ϳ��� ������ ������ �ݺ�
            {
                yield return null;  // ���������� ������ ���
            }
        }

        private TankManager GetRoundWinner()
        {
            for(int i = 0; i < m_Tanks.Length; i++) // ��ũ�� ����ŭ �ݺ�
            {
                if(m_Tanks[i].m_Instance.activeSelf) // Ȱ���ϰ� �ִ� ��ũ�� �ִٸ�
                    return m_Tanks[i]; // ��ũ�� ��ȯ
            }
            return null; // �ƹ� ��ũ�� Ȱ���ϰ� ���� �ʴٸ� null�� ��ȯ
        }

        private TankManager GetGameWinner()
        {
            for(int i = 0; i < m_Tanks.Length; i++) // ��ũ�� ����ŭ �ݺ�
            {
                if (m_Tanks[i].m_Wins == m_NumRoundsToWin) // ��ũ�� ���� �¸� Ƚ���� ��� ������ ������ ��
                    return m_Tanks[i]; // ��ũ�� ��ȯ
            }
            return null; // ���� �¸� Ƚ���� ��� ������ ������Ű�� ���� �� null�� ��ȯ
        }

        private string EndMessage()
        {
            string message = "DRAW!"; // �⺻������ ���尡 ����Ǹ� ȣ��Ǵ� �ؽ�Ʈ

            if(m_RoundWinner != null) // ���� ���� ���ڰ� �ִٸ�
                message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!"; // message = �÷��̾� + WINS THE ROUND!

            message += "\n\n\n\n"; // �� ��ü

            for(int i = 0; i < m_Tanks.Length; i++) // ��ũ�� ����ŭ �ݺ�
            {
                message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " WINS\n"; // message += �÷��̾� : �̱� ���� Ƚ�� + WIN
            }

            if(m_GameWinner != null) // ���� ������ ���ڰ� �ִٸ�
                message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!"; // message = �÷��̾� + WINS THE GAME

            return message; // message�� ��ȯ
        }

        private IEnumerator RoundEnding()
        {
            DisableTankControl(); // DisableTankControl �޼��� ȣ��
            m_RoundWinner = null; // ���� ���� ���� �ʱ�ȭ
            m_RoundWinner = GetRoundWinner(); // ���� ���� ���� = GetRoundWinner �޼��� ȣ��

            if(m_RoundWinner != null) // ���� ���� ���ڰ� �ִٸ�
                m_RoundWinner.m_Wins++; // �¸� ���� ���ھ� �߰�

            m_GameWinner = GetGameWinner(); // ������ ���� = GetGameWinner �޼��� ȣ��
            string message = EndMessage(); // message = EndMessage �޼��� ȣ��
            m_MessageText.text = message; // messsage �ؽ�Ʈ ���
            yield return m_EndWait; // ������ �ð� ���� ���������� ���
        }

        private IEnumerator GameLoop()
        {
            yield return StartCoroutine(RoundStarting()); // RoundStarting �޼��� ���������� ����
            yield return StartCoroutine(RoundPlaying()); // RoundPlaying �޼��� ���������� ����
            yield return StartCoroutine(RoundEnding()); // RoundEnding �޼��� ���������� ����

            if(m_GameWinner != null) // ���� ������ ���ڰ� �ִٸ�
            {
                SceneManager.LoadScene(0); // Scene �ٽ� �ε�
            }
            else // �׷��� �ʴٸ�
            {
                StartCoroutine(GameLoop()); // GameLoop �޼��� ��� ȣ��
            }
        }

        private void Start() // Update �Լ��� ȣ��Ǳ� ������ �� ���� ȣ��Ǵ� �Լ�
        {
            m_StartWait = new WaitForSeconds(m_StartDelay); // ������ ���� ���� �ð� ����
            m_EndWait = new WaitForSeconds(m_EndDelay); // ���� ���� ���� �ð� ����
            SpawnAllTanks(); // SpawnAllTanks �޼��� ȣ��
            SetCameraTargets(); // SetCameraTargets �޼��� ȣ��
            StartCoroutine(GameLoop()); // GameLoop �޼��� ȣ��
        }
    }
}
