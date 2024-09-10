using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour {
    public TMP_Dropdown packDropdown;

    public LevelsController levelsController;
    public GameController gameController;

    private int selectedPackIdx = 0;

    // Start is called before the first frame update
    void Start() {
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        for (int i = 0; i < levelsController.maps.Length; i++) {
            options.Add(new TMP_Dropdown.OptionData(levelsController.maps[i].name));
        }

        packDropdown.AddOptions(options);
    }

    public void OnClickPackDropdown(int idx) {
        selectedPackIdx = idx;
    }

    public void OnClickRandomButton() {
        int levelIdx = Random.Range(0, levelsController.PackSize(selectedPackIdx));

        LevelsController.Level level = levelsController.PrepareLevel(selectedPackIdx, levelIdx);
        gameController.OnUIStartClick(level);
    }
}
