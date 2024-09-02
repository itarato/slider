using System;
using System.Collections.Generic;
using System.Diagnostics;

public class Puzzle {
    public enum Orientation {
        Vertical,
        Horizontal,
    }

    public class Slider {
        public int id;
        public Orientation orientation;
        public int len;
        public int x;
        public int y;
        public int cachedMinBound = 0;
        public int cachedMaxBound = 5;

        public Slider(int id, Orientation orientation, int len, int x, int y) {
            this.id = id;
            this.orientation = orientation;
            this.len = len;
            this.x = x;
            this.y = y;
        }

        public bool IsVertical() {
            return orientation == Orientation.Vertical;
        }
    }

    public List<Slider> sliders = new List<Slider>();

    public void Reset() {
        sliders.Clear();

        /*
         *  012345
         * 5#.....
         * 4#..4##
         * 32...#.
         * 2....1.
         * 1..3#..
         * 0......
         */

        sliders.Add(new Slider(0, Orientation.Vertical, 2, 4, 2));
        sliders.Add(new Slider(1, Orientation.Horizontal, 2, 0, 3));
        //sliders.Add(new Slider(1, Orientation.Vertical, 3, 0, 3));
        sliders.Add(new Slider(2, Orientation.Horizontal, 2, 2, 1));
        sliders.Add(new Slider(3, Orientation.Horizontal, 3, 3, 4));
    }

    public void RefreshMinMaxBoundCache() {
        // Bounds are 6 + 1 - so the exit car can occupy that +1 slot at the exit.
        bool[,] memory = new bool[7, 7];

        // Init.
        for (int i = 0; i < 6; i++) {
            for (int j = 0; j < 6; j++) {
                memory[i, j] = false;
            }
        }

        // Fill.
        foreach (Slider slider in sliders) {
            for (int i = 0; i < slider.len; i++) {
                if (slider.IsVertical()) {
                    memory[5 - slider.y - i, slider.x] = true;
                } else {
                    memory[5 - slider.y, slider.x + i] = true;
                }
            }
        }

        foreach (Slider slider in sliders) {
            slider.cachedMinBound = SliderMinPossiblePos(slider, memory);
            slider.cachedMaxBound = SliderMaxPossiblePos(slider, memory);
        }
    }

    private int SliderMinPossiblePos(Slider slider, bool[,] memory) {
        if (slider.IsVertical()) {
            for (int i = slider.y - 1; i >= 0; i--) {
                if (memory[5 - i, slider.x]) return i + 1;
            }
        } else {
            for (int i = slider.x - 1; i >= 0; i--) {
                if (memory[5 - slider.y, i]) return i + 1;
            }
        }

        return 0;
    }

    private int SliderMaxPossiblePos(Slider slider, bool[,] memory) {
        if (slider.IsVertical()) {
            for (int i = slider.y + slider.len; i < 6; i++) {
                if (memory[5 - i, slider.x]) return i - 1;
            }
        } else {
            for (int i = slider.x + slider.len; i < 6; i++) {
                if (memory[5 - slider.y, i]) return i - 1;
            }

            // Make the main car get to the trigger zone.
            if (slider.y == 3) return 6;
        }

        return 5;
    }
}
