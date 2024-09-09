using System.Collections.Generic;
using System.Linq;

public class LevelManager {
    public class Level {
        public bool solved = false;
        public List<Puzzle.Slider> sliders = new List<Puzzle.Slider>();

        public Level(int[,] levelDataList) {
            solved = false;

            for (int i = 0; i < levelDataList.Length / 4; i++) {
                sliders.Add(new Puzzle.Slider(
                    i,
                    (levelDataList[i, 0] == 0) ? Puzzle.Orientation.Horizontal : Puzzle.Orientation.Vertical,
                    levelDataList[i, 1],
                    levelDataList[i, 2],
                    levelDataList[i, 3]
                ));
            }
        }
    }

    private Dictionary<string, List<Level>> levels = new Dictionary<string, List<Level>>();

    public LevelManager() {
        levels.Add("Easy", new List<Level> {
            new Level(new int[,]{ // 1
                {0, 2, 2, 3},
                {1, 2, 2, 4},
                {0, 3, 3, 4},
                {1, 2, 4, 2},
                {1, 2, 4, 0},
            }),
            new Level(new int[,]{ // 2
                {0, 2, 2, 3},
                {1, 3, 4, 3},
                {1, 2, 2, 0},
                {0, 3, 3, 1},
            }),
            new Level(new int[,]{ // 3
                {0, 2, 0, 3},
                {1, 3, 5, 3},
                {1, 3, 0, 0},
                {0, 2, 1, 2},
                {0, 3, 3, 2},
            }),
            new Level(new int[,]{ // 4
                {0, 2, 0, 3},
                {0, 3, 0, 5},
                {1, 2, 5, 4},
                {1, 2, 2, 3},
                {1, 3, 2, 0},
                {1, 3, 5, 1},
                {0, 2, 4, 0},
            }),
            new Level(new int[,]{ // 5
                {0, 2, 3, 3},
                {1, 2, 3, 4},
                {1, 3, 5, 3},
                {1, 2, 3, 1},
                {0, 2, 4, 1},
            }),
            new Level(new int[,]{ // 6
                {0, 2, 3, 3},
                {1, 2, 5, 4},
                {1, 2, 5, 2},
                {1, 3, 3, 0},
                {0, 2, 4, 1},
            }),
            new Level(new int[,]{ // 7
                {0, 2, 0, 3},
                {1, 3, 2, 3},
                {0, 3, 0, 2},
                {0, 3, 0, 1},
                {1, 3, 5, 1},
                {0, 2, 4, 0},
            }),
            new Level(new int[,]{ // 8
                {0, 2, 0, 3},
                {1, 2, 2, 4},
                {1, 2, 2, 2},
                {0, 3, 2, 1},
                {1, 3, 5, 0},
            }),
            new Level(new int[,]{ // 9
                {0, 2, 3, 3},
                {1, 3, 5, 1},
                {0, 3, 0, 2},
                {1, 2, 4, 1},
                {1, 2, 2, 0},
                {0, 3, 3, 0},
            }),
            new Level(new int[,]{ // 10
                {0, 2, 1, 3},
                {0, 2, 2, 5},
                {1, 3, 4, 3},
                {1, 3, 5, 3},
                {1, 2, 0, 3},
                {1, 3, 3, 1},
                {1, 2, 2, 0},
                {0, 2, 4, 1},
                {0, 3, 3, 0},
            }),
        });
        levels.Add("Medium", new List<Level> {
            new Level(new int[,]{ // 11
                {0, 2, 0, 3},
                {0, 3, 1, 5},
                {0, 2, 4, 5},
                {0, 2, 4, 4},
                {1, 2, 2, 2},
                {1, 2, 5, 2},
                {0, 3, 0, 1},
                {1, 2, 5, 0},
            }),
            new Level(new int[,]{ // 12
                {0, 2, 3, 3},
                {1, 3, 2, 3},
                {1, 2, 4, 4},
                {1, 3, 5, 2},
                {1, 3, 3, 0},
                {0, 2, 4, 1},
            }),
            new Level(new int[,]{ // 13
                {0, 2, 0, 3},
                {1, 3, 2, 3},
                {1, 3, 0, 1},
                {1, 3, 5, 0},
            }),
            new Level(new int[,]{ // 14
                {0, 2, 0, 3},
                {1, 2, 1, 4},
                {0, 2, 2, 5},
                {0, 2, 2, 4},
                {1, 3, 4, 3},
                {1, 2, 5, 3},
                {1, 3, 2, 1},
                {0, 2, 3, 2},
            }),
            new Level(new int[,]{ // 15
                {0, 2, 0, 3},
                {1, 2, 3, 4},
                {1, 2, 3, 2},
                {1, 2, 1, 0},
                {0, 2, 2, 1},
                {1, 3, 4, 0},
            }),
            new Level(new int[,]{ // 16
                {0, 2, 2, 3},
                {1, 3, 1, 3},
                {0, 2, 2, 5},
                {1, 3, 4, 3},
                {1, 2, 2, 0},
                {0, 3, 3, 1},
            }),
            new Level(new int[,]{ // 17
                {0, 2, 1, 3},
                {1, 3, 5, 3},
                {1, 2, 0, 2},
                {1, 3, 3, 1},
                {1, 2, 4, 2},
                {1, 2, 0, 0},
                {1, 2, 2, 0},
                {0, 3, 3, 0},
                {0, 2, 4, 1},
            }),
            new Level(new int[,]{ // 18
                {0, 2, 0, 3},
                {0, 3, 0, 5},
                {1, 3, 3, 3},
                {1, 2, 1, 1},
                {0, 2, 2, 1},
                {1, 3, 5, 0},
            }),
            new Level(new int[,]{ // 19
                {0, 2, 3, 3},
                {1, 3, 2, 3},
                {1, 3, 5, 3},
                {0, 3, 0, 0},
                {1, 2, 3, 0},
                {0, 2, 4, 1},
            }),
            new Level(new int[,]{ // 20
                {0, 2, 0, 3},
                {0, 3, 0, 5},
                {1, 2, 3, 4},
                {1, 2, 5, 4},
                {1, 2, 2, 2},
                {1, 3, 0, 0},
                {0, 3, 1, 1},
                {0, 2, 4, 1},
            }),
        });
        levels.Add("Hard", new List<Level>{
            new Level(new int[,]{ // 21
                {0, 2, 1, 3},
                {1, 3, 3, 3},
                {1, 3, 4, 1},
                {1, 2, 0, 0},
                {1, 2, 1, 0},
                {0, 2, 2, 1},
                {0, 3, 2, 0},
            }),
        });
        levels.Add("Brutal", new List<Level>{
            new Level(new int[,]{ // 31
                {0, 2, 0, 3},
                {1, 2, 0, 4},
                {0, 2, 1, 5},
                {0, 2, 1, 4},
                {1, 3, 3, 3},
                {1, 2, 2, 2},
                {0, 2, 3, 2},
                {1, 3, 5, 0},
                {0, 2, 0, 1},
                {0, 2, 0, 0},
                {1, 2, 2, 0},
                {1, 2, 4, 0},
            }),
        });
    }

    public string[] DifficultyLevels() {
        return levels.Keys.ToArray();
    }

    public List<Level> Levels(string difficultyLevel) {
        return levels.GetValueOrDefault(difficultyLevel);
    }
}
