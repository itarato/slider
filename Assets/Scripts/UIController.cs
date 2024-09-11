using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
    public TMP_Dropdown packDropdown;

    public LevelsController levelsController;
    public GameController gameController;
    public Button startButton;
    public TMP_InputField exactLevelInput;

    private int selectedPackIdx = 0;
    private int exactLevel = -1;

    // Start is called before the first frame update
    void Start() {
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        for (int i = 0; i < levelsController.maps.Length; i++) {
            options.Add(new TMP_Dropdown.OptionData(levelsController.maps[i].name));
        }

        packDropdown.AddOptions(options);
    }

    public void OnClickPackDropdown(int idx) {
        if (idx == 0) return; // Top label.
        selectedPackIdx = idx - 1;
    }

    // TODO: Rename to start-button.
    public void OnClickRandomButton() {
        int levelIdx;
        int levelCount = levelsController.PackSize(selectedPackIdx);

        if (exactLevel >= 0) {
            levelIdx = exactLevel % levelCount;

            ResetForm();
        } else {
            levelIdx = UnityEngine.Random.Range(0, levelCount);
        }

        LevelsController.Level level = levelsController.PrepareLevel(selectedPackIdx, levelIdx);
        gameController.OnUIStartClick(level);
    }

    public void OnLevelNumberInputChange(string value) {
        if (value.Length > 0) {
            if (Int32.TryParse(value, out int v)) {
                exactLevel = v;
                startButton.GetComponentInChildren<TMP_Text>().text = "Go to level";
                return;
            }
        }

        ResetForm();
    }

    private void ResetForm() {
        exactLevel = -1;
        exactLevelInput.text = "";
        startButton.GetComponentInChildren<TMP_Text>().text = "Random";
    }
}
