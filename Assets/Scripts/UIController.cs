using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
    public Canvas difficultySelectionCanvas;
    public Canvas levelSelectionCanvas;

    public GameObject difficultyButtonPrefab;
    public GameObject levelButtonPrefab;
    public GameObject levelButtonRowPrefab;

    private LevelManager levelManager = new LevelManager();

    // Start is called before the first frame update
    void Start() {
        foreach (string difficulty in levelManager.DifficultyLevels()) {
            GameObject instance = Instantiate(difficultyButtonPrefab, difficultySelectionCanvas.transform);
            instance.GetComponentInChildren<TextMeshProUGUI>().text = difficulty;
            instance.GetComponentInChildren<Button>().onClick.AddListener(delegate { OnClickDifficultyButton(difficulty); });
        }
    }

    // Update is called once per frame
    void Update() {

    }

    private void OnClickDifficultyButton(string difficulty) {
        difficultySelectionCanvas.gameObject.SetActive(false);
        levelSelectionCanvas.gameObject.SetActive(true);

        LevelManager.Level[] levels = levelManager.Levels(difficulty);
        int i = 0;
        while (i < levels.Length) {
            GameObject row = Instantiate(levelButtonRowPrefab, levelSelectionCanvas.transform);

            for (int j = 0; j < 4 && i < levels.Length; j++, i++) {
                GameObject instance = Instantiate(levelButtonPrefab, row.transform);
                instance.GetComponentInChildren<TextMeshProUGUI>().text = i.ToString();
                instance.GetComponentInChildren<Button>().onClick.AddListener(delegate {  OnClickLevelButton(difficulty, i); });
            }
        }
    }

    private void ClearLevelSelectionCanvas() {
        for (int i = 0; i < levelSelectionCanvas.transform.childCount; i++) {
            Destroy(levelSelectionCanvas.transform.GetChild(i).gameObject);
        }
    }

    private void OnClickLevelButton(string difficulty, int levelIdx) {
        GameController.instance.OnUIStartClick();
    }
}
