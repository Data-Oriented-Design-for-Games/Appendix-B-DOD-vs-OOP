using System;
using System.Collections.Generic;
using UnityEngine;

namespace Survivor
{
public enum MENU_STATE { MAIN_MENU, IN_GAME };

[Serializable]
public class GameData
{
        public int EnemyCount;
        public int[] EnemyCountGood;
        public int EnemyCountGoodCount;
        public int SpawnRate;
        public int FPSFrameCount;

        public float[] DeltaTime;
        public int DeltaTimeCount;

        public Vector2[] EnemyPosition;
        public Vector2[] EnemyDirection;

        public Vector2 BoardBounds;

        public MENU_STATE MenuState = MENU_STATE.MAIN_MENU;
}
}