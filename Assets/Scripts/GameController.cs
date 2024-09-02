using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public GameObject[] sliderCubeCollection;

    private Puzzle puzzle = new Puzzle();
    private List<SliderCubeController> sliderCubeControllers = new List<SliderCubeController>();

    private static float verticalOffset = -3f;
    private static float horizontalXOffset = -3f;
    private static float horizontalZOffset = -2f;

    // Start is called before the first frame update
    void Start() {
        puzzle.Reset();
        sliderCubeControllers.Clear();

        foreach (Puzzle.Slider slider in puzzle.sliders) {
            GameObject newSlider;
            int prefabIdx = slider.len - 2;
            if (slider.IsVertical()) {
                newSlider = Instantiate(sliderCubeCollection[prefabIdx], new Vector3(slider.x + verticalOffset, 0.75f, slider.y + verticalOffset), Quaternion.identity);
            } else {
                newSlider = Instantiate(sliderCubeCollection[prefabIdx], new Vector3(slider.x + horizontalXOffset, 0.75f, slider.y + horizontalZOffset), Quaternion.Euler(new Vector3(0f, 90f, 0f)));
            }

            SliderCubeController sliderCubeController = newSlider.GetComponent<SliderCubeController>();
            sliderCubeControllers.Add(sliderCubeController);

            sliderCubeController.puzzleSlider = slider;
            sliderCubeController.gameController = this;

        }

        UpdateSliderCubesBounds();
    }

    // Update is called once per frame
    void Update() {
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

    public void signalWinning() {
        // TODO
    }

    private void UpdateSliderCubesBounds() {
        puzzle.RefreshMinMaxBoundCache();

        foreach (SliderCubeController sliderCubeController in sliderCubeControllers) {
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
