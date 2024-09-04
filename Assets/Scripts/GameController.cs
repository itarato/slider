using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public GameObject[] sliderCubeCollection;
    public GameObject ui;

    private Puzzle puzzle = new Puzzle();
    private List<GameObject> sliderInstances = new List<GameObject>();

    private static float verticalOffset = -3f;
    private static float horizontalXOffset = -3f;
    private static float horizontalZOffset = -2f;

    // Start is called before the first frame update
    void Start() {
        ResetGame();
    }

    public void OnUIStartClick() {
        StartGame();
    }

    private void ResetGame() {
        puzzle.Reset();

        foreach (var sliderInstance in sliderInstances) Destroy(sliderInstance);
        sliderInstances.Clear();
    }

    private void StartGame() {
        foreach (Puzzle.Slider slider in puzzle.sliders) {
            GameObject newSlider;
            int prefabIdx = slider.len - 2;
            if (slider.IsVertical()) {
                newSlider = Instantiate(sliderCubeCollection[prefabIdx], new Vector3(slider.x + verticalOffset, 0.75f, slider.y + verticalOffset), Quaternion.identity);
            } else {
                newSlider = Instantiate(sliderCubeCollection[prefabIdx], new Vector3(slider.x + horizontalXOffset, 0.75f, slider.y + horizontalZOffset), Quaternion.Euler(new Vector3(0f, 90f, 0f)));
            }

            SliderCubeController sliderCubeController = newSlider.GetComponent<SliderCubeController>();
            sliderInstances.Add(newSlider);

            sliderCubeController.puzzleSlider = slider;
            sliderCubeController.gameController = this;
        }

        UpdateSliderCubesBounds();

        ui.SetActive(false);
    }

    public void OnUpdateSliderPos(SliderCubeController sliderCubeController) {
        //Debug.Log("OnUpdateSliderPos");
        Puzzle.Slider slider = sliderCubeController.puzzleSlider;

        // Update current position.
        if (slider.IsVertical()) {
            slider.x = (int)System.Math.Round(sliderCubeController.transform.position.x - verticalOffset);
            slider.y = (int)System.Math.Round(sliderCubeController.transform.position.z - verticalOffset);
        } else {
            slider.x = (int)System.Math.Round(sliderCubeController.transform.position.x - horizontalXOffset);
            slider.y = (int)System.Math.Round(sliderCubeController.transform.position.z - horizontalZOffset);
        }
        //Debug.LogFormat("New pos: {0}:{1}", slider.x, slider.y);

        // Set new min/max.
        UpdateSliderCubesBounds();
    }

    public void SignalWinning() {
        ResetGame();
        ui.SetActive(true);
    }

    private void UpdateSliderCubesBounds() {
        puzzle.RefreshMinMaxBoundCache();

        foreach (var sliderInstance in sliderInstances) {
            SliderCubeController sliderCubeController = sliderInstance.GetComponent<SliderCubeController>();
            Puzzle.Slider slider = sliderCubeController.puzzleSlider;

            int min = slider.cachedMinBound;
            int max = slider.cachedMaxBound;

            if (slider.IsVertical()) {
                sliderCubeController.minSlide = verticalOffset + (float)min;
                sliderCubeController.maxSlide = verticalOffset + (float)(max - slider.len + 1);
            } else {
                sliderCubeController.minSlide = verticalOffset + (float)min;
                sliderCubeController.maxSlide = verticalOffset + (float)(max - slider.len + 1);
            }

            //Debug.LogFormat("ID={4} New min: {0}={1} | New max: {2}={3}", min, sliderCubeController.minSlide, max, sliderCubeController.maxSlide, slider.id);
        }
    }
}
