using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
    public Canvas difficultyMenuCanvas;
    public GameObject difficultyButtonPrefab;

    private LevelManager levelManager = new LevelManager();

    // Start is called before the first frame update
    void Start() {
        foreach (string difficulty in levelManager.DifficultyLevels()) {
            GameObject instance = Instantiate(difficultyButtonPrefab, difficultyMenuCanvas.transform);
            instance.GetComponentInChildren<TextMeshProUGUI>().text = difficulty;
        }
    }

    // Update is called once per frame
    void Update() {

    }
}
