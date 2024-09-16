using System;
using System.Collections.Generic;

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

        public override bool Equals(Object obj) {
            if (obj == null || !(obj is Slider)) return false;

            Slider other = (Slider)obj;
            return id == other.id && orientation == other.orientation && len == other.len && x == other.x && y == other.y;
        }

        public bool IsVertical() {
            return orientation == Orientation.Vertical;
        }

        public Slider Clone() {
            return new Slider(id, orientation, len, x, y);
        }

        public bool IsObstacle() {
            return len == 1;
        }

        // Whether this is the cube that will exit.
        public bool IsSpecial() {
            return orientation == Orientation.Horizontal && y == 3 && len == 2;
        }

        public (int, int)[] GetCoords() {
            (int, int)[] coords = new (int, int)[len];

            if (IsVertical()) {
                for (int i = 0; i < len; i++) coords[i] = (5 - y - i, x);
            } else {
                for (int i = 0; i < len; i++) coords[i] = (5 - y, x + i);
            }

            return coords;
        }

        public char Hash() {
            return Puzzle.SliderHash(x, y);
        }

        public override int GetHashCode() {
            return (int)Hash();
        }

        public void ResetFromHash(char hash) {
            x = hash >> 4;
            y = hash & 0b1111;
        }
    }

    private List<Slider> sliders = new List<Slider>();
    private int[,] memory = new int[6, 6];
    private int specialSliderIdx = -1;

    public Puzzle Clone() {
        List<Slider> cloneSliders = new List<Slider>();
        foreach (Slider sl in sliders) cloneSliders.Add(sl.Clone());

        Puzzle clone = new();
        clone.sliders = cloneSliders;
        clone.memory = memory;
        clone.specialSliderIdx = specialSliderIdx;

        return clone;
    }

    public List<Slider> GetSliders() {
        return sliders;
    }

    public void AddSlider(Slider slider) {
        sliders.Add(slider);
        RecordSliderFootprintOnMemory(slider);

        if (slider.IsSpecial()) specialSliderIdx = sliders.Count - 1;
    }

    public void ReplaceSliders(List<Slider> sliders) {
        this.sliders = sliders;
        RefreshMemory();

        for (int i = 0; i < sliders.Count; i++) {
            if (sliders[i].IsSpecial()) specialSliderIdx = i;
        }
    }

    public bool IsEndPosition() {
        if (specialSliderIdx == -1) {
            Console.WriteLine("ERROR! Special slider not found.");
            return false;
        }

        for (int i = 5; i >= 2; i--) {
            if (memory[2, i] == specialSliderIdx) return true;
            if (memory[2, i] > 0) return false;
        }

        return true;
    }

    public void RefreshMemory() {
        ClearMemory();
        foreach (Slider slider in sliders) RecordSliderFootprintOnMemory(slider);
    }

    public void RecordSliderFootprintOnMemory(Slider slider) {
        foreach ((int, int) coord in slider.GetCoords()) {
            // Exit slider when in the exit slot.
            if (coord.Item2 >= 6) continue;

            if (memory[coord.Item1, coord.Item2] != -1) throw new ApplicationException("Occupied memory slot");

            memory[coord.Item1, coord.Item2] = slider.id;
        }
    }

    private void ClearMemory() {
        for (int i = 0; i < 6; i++) for (int j = 0; j < 6; j++) memory[i, j] = -1;
    }

    public char[] Hash() {
        char[] hash = new char[sliders.Count];
        for (int i = 0; i < sliders.Count; i++) hash[i] = sliders[i].Hash();

        return hash;
    }

    public void ResetFromHash(char[] hash) {
        for (int i = 0; i < hash.Length; i++) sliders[i].ResetFromHash(hash[i]);
        RefreshMemory();
    }

    public List<char[]> AllPossibleStates() {
        List<char[]> allStates = new List<char[]>();

        // For all sliders ...
        for (int i = 0; i < sliders.Count; i++) {
            // Find all the possible moves ...
            List<(int, int)> availableSliderStates = AvailableMoveForSlider(i);
            foreach (var availableSliderState in availableSliderStates) {
                char[] newState = new char[sliders.Count];
                // And merge it with the rest of the state ...
                for (int j = 0; j < sliders.Count; j++) {
                    if (j == i) {
                        newState[j] = Puzzle.SliderHash(availableSliderState.Item1, availableSliderState.Item2);
                    } else {
                        newState[j] = sliders[j].Hash();
                    }
                }
                // And add it to the output.
                allStates.Add(newState);
            }
        }

        return allStates;
    }

    public List<(int, int)> AvailableMoveForSlider(int sliderIdx) {
        List<(int, int)> coords = new List<(int, int)>();
        Slider slider = sliders[sliderIdx];

        if (slider.len == 1) return coords;

        if (slider.IsVertical()) {
            // Min-side.
            for (int i = slider.y - 1; i >= 0; i--) {
                if (memory[5 - i, slider.x] == -1) {
                    coords.Add((slider.x, i));
                } else break;
            }
            // Max-side.
            for (int i = slider.y + 1; i + slider.len <= 6; i++) {
                if (memory[5 - i - slider.len + 1, slider.x] == -1) {
                    coords.Add((slider.x, i));
                } else break;
            }
        } else {
            // Min-side.
            for (int i = slider.x - 1; i >= 0; i--) {
                if (memory[5 - slider.y, i] == -1) {
                    coords.Add((i, slider.y));
                } else break;
            }
            // Max-side.
            for (int i = slider.x + 1; i + slider.len <= 6; i++) {
                if (memory[5 - slider.y, i + slider.len - 1] == -1) {
                    coords.Add((i, slider.y));
                } else break;
            }
        }

        return coords;
    }

    public void RefreshMinMaxBoundCache() {
        RefreshMemory();

        foreach (Slider slider in sliders) {
            slider.cachedMinBound = SliderMinPossiblePos(slider);
            slider.cachedMaxBound = SliderMaxPossiblePos(slider);
        }
    }

    private int SliderMinPossiblePos(Slider slider) {
        if (slider.IsVertical()) {
            for (int i = slider.y - 1; i >= 0; i--) {
                if (memory[5 - i, slider.x] >= 0) return i + 1;
            }
        } else {
            for (int i = slider.x - 1; i >= 0; i--) {
                if (memory[5 - slider.y, i] >= 0) return i + 1;
            }
        }

        return 0;
    }

    private int SliderMaxPossiblePos(Slider slider) {
        if (slider.IsVertical()) {
            for (int i = slider.y + slider.len; i < 6; i++) {
                if (memory[5 - i, slider.x] >= 0) return i - 1;
            }
        } else {
            for (int i = slider.x + slider.len; i < 6; i++) {
                if (memory[5 - slider.y, i] >= 0) return i - 1;
            }

            // Make the main car get to the trigger zone.
            if (slider.y == 3) return 6;
        }

        return 5;
    }

    public static char SliderHash(int x, int y) {
        return (char)((char)x << 4 | (char)y);
    }
}
