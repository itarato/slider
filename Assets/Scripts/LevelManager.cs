public class LevelManager {
    public class Level {
        public bool solved = false;
    }

    public string[] DifficultyLevels() {
        return new string[] { "Easy", "Medium", "Hard" };
    }

    public Level[] Levels(string difficultyLevel) {
        return new Level[] { new Level(), new Level(), new Level() };
    }
}
