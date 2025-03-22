using System;
using System.Collections.Generic;
using UnityEngine;

namespace Survivor
{
public enum MENU_STATE { NONE, IN_GAME, GAME_OVER };

[Serializable]
public class GameData
{
        public int EnemyCount;
        public int EnemyCountGood;
        public int SpawnRate;
        public int SpawnFrameCount;

        public float[] AverageDT;
        public int AverageDTCount;

        public Vector2[] EnemyPosition;
        public Vector2[] EnemyDirection;

        public Vector2 BoardBounds;

        public MENU_STATE GameState = MENU_STATE.NONE;
}
}