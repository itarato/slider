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
    public GameObject backButtonPrefab;

    private LevelManager levelManager = new LevelManager();

    // Start is called before the first frame update
    void Start() {
        foreach (string difficulty in levelManager.DifficultyLevels()) {
            GameObject instance = Instantiate(difficultyButtonPrefab, difficultySelectionCanvas.transform);
            instance.GetComponentInChildren<TextMeshProUGUI>().text = difficulty;
            instance.GetComponentInChildren<Button>().onClick.AddListener(delegate { OnClickDifficultyButton(difficulty); });
        }
    }

    private void OnClickDifficultyButton(string difficulty) {
        difficultySelectionCanvas.gameObject.SetActive(false);
        levelSelectionCanvas.gameObject.SetActive(true);

        ClearLevelSelectionCanvas();

        List<LevelManager.Level> levels = levelManager.Levels(difficulty);
        int i = 0;
        while (i < levels.Count) {
            GameObject row = Instantiate(levelButtonRowPrefab, levelSelectionCanvas.transform);

            for (int j = 0; j < 4 && i < levels.Count; j++, i++) {
                GameObject instance = Instantiate(levelButtonPrefab, row.transform);
                instance.GetComponentInChildren<TextMeshProUGUI>().text = i.ToString();
                int levelIdx = i;
                instance.GetComponentInChildren<Button>().onClick.AddListener(delegate { OnClickLevelButton(difficulty, levelIdx); });
            }
        }

        GameObject lastRow = Instantiate(levelButtonRowPrefab, levelSelectionCanvas.transform);
        GameObject backButton = Instantiate(backButtonPrefab, lastRow.transform);

        backButton.GetComponentInChildren<Button>().onClick.AddListener(delegate {
            levelSelectionCanvas.gameObject.SetActive(false);
            difficultySelectionCanvas.gameObject.SetActive(true);
        });
    }

    private void ClearLevelSelectionCanvas() {
        for (int i = 0; i < levelSelectionCanvas.transform.childCount; i++) {
            Destroy(levelSelectionCanvas.transform.GetChild(i).gameObject);
        }
    }

    private void OnClickLevelButton(string difficulty, int levelIdx) {
        Debug.Log(difficulty + " " + levelIdx.ToString());

        levelSelectionCanvas.gameObject.SetActive(false);
        difficultySelectionCanvas.gameObject.SetActive(true);

        GameController.instance.OnUIStartClick(levelManager.Levels(difficulty)[levelIdx].sliders);
    }
}
