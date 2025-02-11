using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Survivor
{
    [Serializable]
    public class Balance
    {
        public int MaxEnemies;
        public float EnemyVelocityMin;
        public float EnemyVelocityMax;
        public float PlayerVelocity;
        public float MinCollisionDistance;

        public float Diameter;
        public int SpawnFrameCount;
    }
}