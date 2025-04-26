using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

namespace Survivor
{
    public static class Logic
    {
        public static void AllocateGameData(GameData gameData, Balance balance)
        {
            gameData.EnemyPosition = new Vector2[balance.MaxEnemies];
            gameData.EnemyDirection = new Vector2[balance.MaxEnemies];

            gameData.EnemyCountGood = new int[balance.MaxEnemies];

            gameData.AverageDT = new float[10000];
        }

        public static void StartGame(GameData gameData, Balance balance, float cameraSize, float screenRatio)
        {
            gameData.MenuState = MENU_STATE.IN_GAME;

            gameData.BoardBounds.y = cameraSize;
            gameData.BoardBounds.x = gameData.BoardBounds.y * screenRatio;

            gameData.EnemyCount = gameData.SpawnRate = 1;
            gameData.EnemyCountGood[gameData.EnemyCountGoodCount++] = gameData.EnemyCount;

            for (int i = 0; i < balance.MaxEnemies; i++)
            {
                float randomX = Random.value *
                    gameData.BoardBounds.x * 2.0f -
                    gameData.BoardBounds.x;

                float randomY = Random.value *
                    gameData.BoardBounds.y * 2.0f -
                    gameData.BoardBounds.y;

                gameData.EnemyPosition[i] = new Vector2(randomX, randomY);

                gameData.EnemyDirection[i] = new Vector2(
                    Random.value - 0.5f,
                    Random.value - 0.5f
                    ).normalized;
            }

            gameData.AverageDTCount = 0;
        }

        public static void TryChangeEnemyCount(GameData gameData, Balance balance, float dt, out int oldEnemyCount)
        {
            oldEnemyCount = gameData.EnemyCount;

            gameData.AverageDT[gameData.AverageDTCount++] = dt;

            gameData.SpawnFrameCount--;
            if (gameData.SpawnFrameCount <= 0)
            {
                float avgDT = 0.0f;
                for (int i = 0; i < gameData.AverageDTCount; i++)
                    avgDT += gameData.AverageDT[i];
                avgDT /= (float)gameData.AverageDTCount;

                float avgFPS = 1.0f / avgDT;

                gameData.AverageDTCount = 0;

                if (avgFPS > 59.0f || gameData.EnemyCount == 1)
                {
                    gameData.EnemyCountGood[gameData.EnemyCountGoodCount++] = gameData.EnemyCount;

                    gameData.SpawnRate *= 2;

                    gameData.EnemyCount += gameData.SpawnRate;
                    if (gameData.EnemyCount > balance.MaxEnemies)
                        gameData.EnemyCount = balance.MaxEnemies;
                }
                else if (gameData.EnemyCountGoodCount > 1)
                {
                    gameData.SpawnRate = 1;
                    gameData.EnemyCountGoodCount--;
                    gameData.EnemyCount = gameData.EnemyCountGood[gameData.EnemyCountGoodCount];
                }

                gameData.SpawnFrameCount = balance.SpawnFrameCount;
            }
        }

        public static void TickDOD(GameData gameData, Balance balance, float dt)
        {
            for (int i = 0; i < gameData.EnemyCount; i++)
            {
                Vector2 position = gameData.EnemyPosition[i] +
                gameData.EnemyDirection[i] *
                balance.EnemyVelocity * dt;
                Vector2 direction = gameData.EnemyDirection[i];

                CheckEnemyWallCollision(gameData.BoardBounds, ref position, ref direction);

                gameData.EnemyDirection[i] = direction;
                gameData.EnemyPosition[i] = position;
            }

            CheckEnemyEnemyCollisionDOD(gameData, balance);
        }

        public static void CheckEnemyEnemyCollisionDOD(GameData gameData, Balance balance)
        {
            float diameter = balance.Diameter;
            float diameterSqr = diameter * diameter;
            for (int i = 0; i < gameData.EnemyCount; i++)
                for (int j = i + 1; j < gameData.EnemyCount; j++)
                {
                    if (Vector2.SqrMagnitude(gameData.EnemyPosition[i] - gameData.EnemyPosition[j]) < diameterSqr)
                    {
                        Vector2 midPoint = (gameData.EnemyPosition[i] + gameData.EnemyPosition[j]) * 0.5f;
                        gameData.EnemyPosition[i] = midPoint + (midPoint - gameData.EnemyPosition[i]).normalized * diameter * 1.01f;
                        gameData.EnemyPosition[j] = midPoint + (midPoint - gameData.EnemyPosition[j]).normalized * diameter * 1.01f;

                        gameData.EnemyDirection[i] = (gameData.EnemyPosition[i] - midPoint).normalized;
                        gameData.EnemyDirection[j] = (gameData.EnemyPosition[j] - midPoint).normalized;
                    }
                }

        }

        public static void CheckEnemyEnemyCollisionOOP(GameData gameData, Balance balance, List<EnemyOOP> enemyPool)
        {
            float diameter = balance.Diameter;
            for (int i = 0; i < gameData.EnemyCount; i++)
                for (int j = i + 1; j < gameData.EnemyCount; j++)
                {
                    enemyPool[i].CheckCollision(enemyPool[j], diameter);
                }
        }

        public static void CheckEnemyWallCollision(Vector2 boardBounds, ref Vector2 position, ref Vector2 direction)
        {
            if (position.x < -boardBounds.x)
            {
                position.x = -boardBounds.x;
                direction.x = -direction.x;
            }
            if (position.x > boardBounds.x)
            {
                position.x = boardBounds.x;
                direction.x = -direction.x;
            }
            if (position.y < -boardBounds.y)
            {
                position.y = -boardBounds.y;
                direction.y = -direction.y;
            }
            if (position.y > boardBounds.y)
            {
                position.y = boardBounds.y;
                direction.y = -direction.y;
            }
        }

        public static void ExitGame(GameData gameData)
        {
            gameData.MenuState = MENU_STATE.MAIN_MENU;
        }
    }
}