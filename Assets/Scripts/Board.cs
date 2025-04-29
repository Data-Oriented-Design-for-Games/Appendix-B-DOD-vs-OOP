using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Unity.VisualScripting;

namespace Survivor
{
    public class Board : MonoBehaviour
    {
        public GameObject EnemyPrefabDOD;
        public Transform EnemyParentDOD;
        GameObject[] m_enemyPoolDOD;
        bool[] m_enemyActiveDOD;

        public GameObject EnemyPrefabOOP;
        public Transform EnemyParentOOP;
        List<EnemyOOP> m_enemyPoolOOP;

        public GameObject UI;
        public TextMeshProUGUI GameTimeText;

        // Start is called before the first frame update
        public void Init(Balance balance)
        {
            m_enemyActiveDOD = new bool[balance.MaxEnemies];
            m_enemyPoolDOD = new GameObject[balance.MaxEnemies];
            for (int i = 0; i < balance.MaxEnemies; i++)
            {
                m_enemyPoolDOD[i] = Instantiate(EnemyPrefabDOD, EnemyParentDOD);
                m_enemyPoolDOD[i].SetActive(false);
                m_enemyActiveDOD[i] = false;
            }

            m_enemyPoolOOP = new List<EnemyOOP>();
            for (int i = 0; i < balance.MaxEnemies; i++)
            {
                m_enemyPoolOOP.Add(Instantiate(EnemyPrefabOOP, EnemyParentOOP).GetComponent<EnemyOOP>());
                m_enemyPoolOOP[i].gameObject.SetActive(false);
            }

            UI.SetActive(false);
        }

        public void ShowDOD(
            GameData gameData,
            Balance balance,
            Camera mainCamera,
            float screenRatio)
        {
            Logic.StartGame(gameData, balance, mainCamera.orthographicSize, screenRatio);

            for (int i = 0; i < balance.MaxEnemies; i++)
            {
                m_enemyPoolDOD[i].transform.localPosition = gameData.EnemyPosition[i];
                m_enemyPoolDOD[i].SetActive(i < gameData.EnemyCount);
                m_enemyActiveDOD[i] = (i < gameData.EnemyCount);
            }

            EnemyParentDOD.gameObject.SetActive(true);
            EnemyParentOOP.gameObject.SetActive(false);

            GameTimeText.text = "DOD " + gameData.EnemyCount.ToString("N0");

            UI.SetActive(true);
        }

        public void ShowOOP(
            GameData gameData,
            Balance balance,
            Camera mainCamera,
            float screenRatio)
        {
            Logic.StartGame(gameData, balance, mainCamera.orthographicSize, screenRatio);

            for (int i = 0; i < balance.MaxEnemies; i++)
            {
                m_enemyPoolOOP[i].Init(gameData.BoardBounds, gameData.EnemyPosition[i], gameData.EnemyDirection[i], balance.EnemyVelocity);
                m_enemyPoolOOP[i].gameObject.SetActive(i < gameData.EnemyCount);
            }

            EnemyParentDOD.gameObject.SetActive(false);
            EnemyParentOOP.gameObject.SetActive(true);

            GameTimeText.text = "OOP " + gameData.EnemyCount.ToString("N0");

            UI.SetActive(true);
        }

        public void Hide(Balance balance)
        {
            for (int i = 0; i < balance.MaxEnemies; i++)
            {
                m_enemyPoolDOD[i].SetActive(false);
                m_enemyPoolOOP[i].gameObject.SetActive(false);
                m_enemyActiveDOD[i] = false;
            }

            EnemyParentDOD.gameObject.SetActive(false);
            EnemyParentOOP.gameObject.SetActive(false);


            UI.SetActive(false);
        }

        // Update is called once per frame
        public void TickDOD(GameData gameData, Balance balance, float dt)
        {
            Logic.TickDOD(gameData, balance, dt);

            for (int i = 0; i < gameData.EnemyCount; i++)
                m_enemyPoolDOD[i].transform.localPosition = gameData.EnemyPosition[i];

            tryChangeEnemyCountDOD(gameData, balance, dt);

        }

        public void TickOOP(GameData gameData, Balance balance, float dt)
        {
            Logic.HandleEnemyToEnemyCollisionOOP(gameData, balance, m_enemyPoolOOP);

            tryChangeEnemyCountOOP(gameData, balance, dt);
        }

        void tryChangeEnemyCountDOD(GameData gameData, Balance balance, float dt)
        {
            int oldAliveCount;
            Logic.TryChangeEnemyCount(gameData, balance, dt, out oldAliveCount);

            if (oldAliveCount != gameData.EnemyCount)
            {
                for (int i = 0; i < balance.MaxEnemies; i++)
                    if (m_enemyActiveDOD[i] && i >= gameData.EnemyCount)
                    {
                        m_enemyPoolDOD[i].SetActive(false);
                        m_enemyActiveDOD[i] = false;
                    }
                    else if (!m_enemyActiveDOD[i] && i < gameData.EnemyCount)
                    {
                        m_enemyPoolDOD[i].SetActive(true);
                        m_enemyActiveDOD[i] = true;
                    }


                GameTimeText.text = "DOD " + gameData.EnemyCount.ToString("N0");
            }
        }

        void tryChangeEnemyCountOOP(GameData gameData, Balance balance, float dt)
        {
            int oldAliveCount;
            Logic.TryChangeEnemyCount(gameData, balance, dt, out oldAliveCount);

            if (oldAliveCount != gameData.EnemyCount)
            {
                for (int i = 0; i < balance.MaxEnemies; i++)
                    if (m_enemyPoolOOP[i].isActiveAndEnabled && i >= gameData.EnemyCount)
                        m_enemyPoolOOP[i].gameObject.SetActive(false);
                    else if (!m_enemyPoolOOP[i].isActiveAndEnabled && i < gameData.EnemyCount)
                        m_enemyPoolOOP[i].gameObject.SetActive(true);

                GameTimeText.text = "OOP " + gameData.EnemyCount.ToString("N0");
            }
        }
    }
}