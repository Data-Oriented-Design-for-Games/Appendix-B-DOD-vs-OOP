using System.Collections;
using System.Collections.Generic;
using Survivor;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

namespace Survivor
{
    public class EnemyOOP : MonoBehaviour
    {
        public Vector2 Direction;
        public float Velocity;
        Vector2 m_boardBounds;

        public void Init(Vector2 boardBounds, Vector2 position, Vector2 direction, float velocity)
        {
            m_boardBounds = boardBounds;
            transform.localPosition = position;
            Direction = direction;
            Velocity = velocity;
        }

        // Update is called once per frame
        void Update()
        {
            Vector2 position = transform.localPosition;
            position += Direction * Velocity * Time.deltaTime;

            Logic.HandleEnemyWallCollision(m_boardBounds, ref position, ref Direction);

            transform.localPosition = position;
        }

        public void HandleCollision(EnemyOOP enemyOOP, float diameter)
        {
            float diameterSqr = diameter * diameter;
            if (Vector2.SqrMagnitude(transform.localPosition - enemyOOP.transform.localPosition) < diameterSqr)
            {
                Vector3 midPoint = (transform.localPosition + enemyOOP.transform.localPosition) * 0.5f;
                transform.localPosition = midPoint + (midPoint - transform.localPosition).normalized * diameter * 1.01f;
                enemyOOP.transform.localPosition = midPoint + (midPoint - enemyOOP.transform.localPosition).normalized * diameter * 1.01f;

                Direction = (transform.localPosition - midPoint).normalized;
                enemyOOP.Direction = (enemyOOP.transform.localPosition - midPoint).normalized;
            }
        }
    }
}