using UnityEngine;

namespace Levels
{
    public class Level
    {
        public string Name { get; set; }
        public int PlayersOctobearsCollected { get; set; }
        public int PlayersManaCollected { get; set; }
        public int PlayersDeathCount { get; set; }
        public int PlayersStars { get; set; }
        public int PlayersBestTime { get; set; }
        public bool PlayerAlreadyCompleted { get; set; }

        public Level(string name, int playersOctobearsCollected, int playersManaCollected, int playersDeathCount, int playersStars, int playersBestTime, bool playerAlreadyCompleted)
        {
            this.Name = name;
            this.PlayersOctobearsCollected = playersOctobearsCollected;
            this.PlayersManaCollected = playersManaCollected;
            this.PlayersDeathCount = playersDeathCount;
            this.PlayersStars = playersStars;
            this.PlayersBestTime = playersBestTime;
            this.PlayerAlreadyCompleted = playerAlreadyCompleted;
        }
    }
}

