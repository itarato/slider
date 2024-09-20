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
    public Button previousLevelButton;
    public TMP_InputField exactLevelInput;

    // The level from the maps list. -1 means it's a random pack.
    private int selectedPackIdx = -1;
    private int exactLevel = -1;

    private LevelsController.Level previousLevel;

    // Start is called before the first frame update
    void Start() {
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        for (int i = 0; i < levelsController.maps.Length; i++) {
            options.Add(new TMP_Dropdown.OptionData(levelsController.maps[i].name));
        }

        packDropdown.AddOptions(options);
        exactLevelInput.gameObject.SetActive(false);

        previousLevelButton.gameObject.SetActive(false);
    }

    public void OnClickPackDropdown(int idx) {
        selectedPackIdx = idx - 1;
        exactLevelInput.gameObject.SetActive(selectedPackIdx >= 0);
    }

    public void OnClickStartButton() {
        int levelIdx;
        int packIdx;

        if (selectedPackIdx < 0) {
            packIdx = UnityEngine.Random.Range(0, levelsController.maps.Length);
        } else {
            packIdx = selectedPackIdx;
        }

        int levelCount = levelsController.PackSize(packIdx);

        if (exactLevel >= 0) {
            levelIdx = exactLevel % levelCount;
        } else {
            levelIdx = UnityEngine.Random.Range(0, levelCount);
        }

        ResetForm();
        LevelsController.Level level = levelsController.PrepareLevel(packIdx, levelIdx);

        previousLevel = level;
        previousLevelButton.gameObject.SetActive(true);

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

    public void OnClickPreviousLevelButton() {
        ResetForm();
        gameController.OnUIStartClick(previousLevel);
    }

    private void ResetForm() {
        exactLevel = -1;
        exactLevelInput.text = "";
        startButton.GetComponentInChildren<TMP_Text>().text = "Random";
    }
}
