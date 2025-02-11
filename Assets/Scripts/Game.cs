using UnityEngine;
using CommonTools;
using System.Collections.Generic;
using TMPro;

namespace Survivor
{
    public class Game : Singleton<Game>
    {
        public Board Board;
        public Camera MainCamera;

        public GameObject UIMainMenu;
        public GameObject UIGameOver;

        public TextMeshProUGUI TestResults;

        GameData m_gameData = new GameData();
        public Balance m_balance;

        int m_screenShotIdx = 0;

        public bool DOD = true;

        // Start is called before the first frame update
        void Start()
        {
            Application.targetFrameRate = 120;

            Logic.AllocateGameData(m_gameData, m_balance);
            Board.Init(m_balance);

            UIMainMenu.SetActive(true);
            UIGameOver.SetActive(false);

        }

        public void StartGameDOD()
        {
            DOD = true;
            UIMainMenu.SetActive(false);
            UIGameOver.SetActive(false);
            Board.ShowDOD(m_gameData, m_balance, MainCamera, (float)Screen.width / (float)Screen.height);
        }

        public void StartGameOOP()
        {
            DOD = false;
            UIMainMenu.SetActive(false);
            UIGameOver.SetActive(false);
            Board.ShowOOP(m_gameData, m_balance, MainCamera, (float)Screen.width / (float)Screen.height);
        }

        public void ExitGame()
        {
            Board.Hide(m_balance);
            UIMainMenu.SetActive(true);
        }

        // Update is called once per frame
        void Update()
        {
            if (m_gameData.GameState == MENU_STATE.IN_GAME)
                if (DOD)
                    Board.TickDOD(m_gameData, m_balance, Time.deltaTime);
                else
                    Board.TickOOP(m_gameData, m_balance, Time.deltaTime);

            if (Input.GetKeyUp("s"))
                captureScreenshot();
        }
        void captureScreenshot()
        {
            ScreenCapture.CaptureScreenshot("screenshot" + (m_screenShotIdx++) + ".png");
        }
    }
}