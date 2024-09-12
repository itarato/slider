using System.Collections.Generic;
using UnityEngine;

public class LevelsController : MonoBehaviour {
    public class Level {
        public bool solved = false;
        public string name;
        public int levelIdx;
        public List<Puzzle.Slider> sliders = new List<Puzzle.Slider>();

        public Level(string name, int levelIdx, string michaelFoglemanFormat) {
            this.name = name;
            this.levelIdx = levelIdx;
            solved = false;

            //Debug.Log("Parse: " + michaelFoglemanFormat);

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
                        sliders.Add(new Puzzle.Slider(sliderIdx++, Puzzle.Orientation.Horizontal, 1, x, y_rev));
                    } else {
                        int len = 2;
                        Puzzle.Orientation orientation;
                        chars[i] = 'o';

                        if (x < 5 && chars[i + 1] == c) {
                            orientation = Puzzle.Orientation.Horizontal;
                            chars[i + 1] = 'o';

                            if (x < 4 && chars[i + 2] == c) {
                                len = 3;
                                chars[i + 2] = 'o';
                            }
                        } else {
                            orientation = Puzzle.Orientation.Vertical;
                            chars[i - 6] = 'o';

                            if (y > 1 && chars[i - 12] == c) {
                                len = 3;
                                chars[i - 12] = 'o';
                            }
                        }

                        sliders.Add(new Puzzle.Slider(sliderIdx++, orientation, len, x, y_rev));
                    }
                }
            }
        }
    }

    public TextAsset[] maps;
    private Dictionary<int, string[]> mapCache = new Dictionary<int, string[]>();

    public int PackSize(int packIdx) {
        //Debug.Log("Pack size: " + lines.Length.ToString());

        return Levels(packIdx).Length;
    }

    public Level PrepareLevel(int packIdx, int levelIdx) {
        //Debug.Log("Prepare level: " + packIdx.ToString() + " : " + levelIdx.ToString());

        return new Level(maps[packIdx].name, levelIdx, Levels(packIdx)[levelIdx]);
    }

    private string[] Levels(int packIdx) {
        if (!mapCache.ContainsKey(packIdx)) {
            string content = maps[packIdx].text.Trim();

            // TODO: Cache this.
            mapCache.Add(packIdx, content.Split('\n'));
        }

        return mapCache.GetValueOrDefault(packIdx);
    }
}
