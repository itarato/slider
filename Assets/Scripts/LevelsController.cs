using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelsController : MonoBehaviour {
    public class Level {
        public string name;
        public int levelIdx;
        public List<Common.Puzzle.Slider> sliders = new List<Common.Puzzle.Slider>();

        public Level(string name, int levelIdx, string michaelFoglemanFormat) {
            this.name = name;
            this.levelIdx = levelIdx;

            char[] chars = michaelFoglemanFormat.ToCharArray();
            int sliderIdx = 0;
            for (int y = 5; y >= 0; y--) {
                for (int x = 0; x < 6; x++) {
                    int y_rev = 5 - y;
                    int i = y * 6 + x;
                    int c = chars[i];

                    if (c == 'o') {
                        // Noop.
                    } else if (c == 'x') {
                        sliders.Add(new Common.Puzzle.Slider(sliderIdx++, Common.Puzzle.Orientation.Horizontal, 1, x, y_rev));
                    } else {
                        int len = 2;
                        Common.Puzzle.Orientation orientation;
                        chars[i] = 'o';

                        if (x < 5 && chars[i + 1] == c) {
                            orientation = Common.Puzzle.Orientation.Horizontal;
                            chars[i + 1] = 'o';

                            if (x < 4 && chars[i + 2] == c) {
                                len = 3;
                                chars[i + 2] = 'o';
                            }
                        } else {
                            orientation = Common.Puzzle.Orientation.Vertical;
                            chars[i - 6] = 'o';

                            if (y > 1 && chars[i - 12] == c) {
                                len = 3;
                                chars[i - 12] = 'o';
                            }
                        }

                        sliders.Add(new Common.Puzzle.Slider(sliderIdx++, orientation, len, x, y_rev));
                    }
                }
            }
        }

        public int MinStepsRequired() {
            return Int32.Parse(name);
        }
    }

    // Collection of map pack file assets.
    public TextAsset[] maps;
    private Dictionary<int, string[]> mapCache = new Dictionary<int, string[]>();

    public int PackSize(int packIdx) {
        return Levels(packIdx).Length;
    }

    public Level PrepareLevel(int packIdx, int levelIdx) {
        return new Level(maps[packIdx].name, levelIdx, Levels(packIdx)[levelIdx]);
    }

    /**
     * Caching the extracted raw level data.
     */
    private string[] Levels(int packIdx) {
        if (!mapCache.ContainsKey(packIdx)) {
            string content = maps[packIdx].text.Trim();
            mapCache.Add(packIdx, content.Split('\n'));
        }

        return mapCache.GetValueOrDefault(packIdx);
    }
}
