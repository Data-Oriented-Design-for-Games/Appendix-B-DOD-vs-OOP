using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Survivor
{
    public class Board : MonoBehaviour
    {
        public GameObject EnemyPrefab;
        public Transform EnemyParent;
        GameObject[] m_enemyPool;
        bool[] m_enemyActive;

        public GameObject EnemyPrefabOOP;
        public Transform EnemyParentOOP;
        EnemyOOP[] m_enemyPoolOOP;

        public GameObject UI;
        public TextMeshProUGUI GameTimeText;

        // Start is called before the first frame update
        public void Init(Balance balance)
        {
            m_enemyActive = new bool[balance.MaxEnemies];
            m_enemyPool = new GameObject[balance.MaxEnemies];
            for (int i = 0; i < balance.MaxEnemies; i++)
            {
                m_enemyPool[i] = Instantiate(EnemyPrefab, EnemyParent);
                m_enemyPool[i].SetActive(false);
                m_enemyActive[i] = false;
            }

            m_enemyPoolOOP = new EnemyOOP[balance.MaxEnemies];
            for (int i = 0; i < balance.MaxEnemies; i++)
            {
                m_enemyPoolOOP[i] = Instantiate(EnemyPrefabOOP, EnemyParentOOP).GetComponent<EnemyOOP>();
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
                m_enemyPool[i].transform.localPosition = gameData.EnemyPosition[i];
                m_enemyPool[i].SetActive(i < gameData.EnemyCount);
                m_enemyActive[i] = (i < gameData.EnemyCount);
            }

            EnemyParent.gameObject.SetActive(true);
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
                m_enemyPoolOOP[i].Init(gameData.BoardBounds, gameData.EnemyPosition[i], gameData.EnemyDirection[i], gameData.EnemyVelocity[i]);
                m_enemyPoolOOP[i].gameObject.SetActive(i < gameData.EnemyCount);
            }

            EnemyParent.gameObject.SetActive(false);
            EnemyParentOOP.gameObject.SetActive(true);

            GameTimeText.text = "OOP " + gameData.EnemyCount.ToString("N0");

            UI.SetActive(true);
        }

        public void Hide(Balance balance)
        {
            for (int i = 0; i < balance.MaxEnemies; i++)
            {
                m_enemyPool[i].SetActive(false);
                m_enemyPoolOOP[i].gameObject.SetActive(false);
                m_enemyActive[i] = false;
            }

            EnemyParent.gameObject.SetActive(false);
            EnemyParentOOP.gameObject.SetActive(false);


            UI.SetActive(false);
        }

        // Update is called once per frame
        public void TickDOD(GameData gameData, Balance balance, float dt)
        {
            Logic.TickDOD(gameData, balance, dt);

            for (int i = 0; i < gameData.EnemyCount; i++)
                m_enemyPool[i].transform.localPosition = gameData.EnemyPosition[i];

            TryChangeEnemyCountDOD(gameData, balance, dt);

        }

        public void TickOOP(GameData gameData, Balance balance, float dt)
        {
            Logic.CheckEnemyEnemyCollisionOOP(gameData, balance, m_enemyPoolOOP);

            TryChangeEnemyCountOOP(gameData, balance, dt);
        }

        void TryChangeEnemyCountDOD(GameData gameData, Balance balance, float dt)
        {
            int oldAliveCount;
            Logic.TryChangeEnemyCount(gameData, balance, dt, out oldAliveCount);

            if (oldAliveCount != gameData.EnemyCount)
            {
                for (int i = 0; i < balance.MaxEnemies; i++)
                    if (m_enemyActive[i] && i >= gameData.EnemyCount)
                    {
                        m_enemyPool[i].SetActive(false);
                        m_enemyActive[i] = false;
                    }
                    else if (!m_enemyActive[i] && i < gameData.EnemyCount)
                    {
                        m_enemyPool[i].SetActive(true);
                        m_enemyActive[i] = true;
                    }


                GameTimeText.text = "DOD " + gameData.EnemyCount.ToString("N0");
            }
        }

        void TryChangeEnemyCountOOP(GameData gameData, Balance balance, float dt)
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