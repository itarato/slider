using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    // Cube prefabs to pick for instantiation.
    public GameObject[] sliderCubeCollection;

    // Main camera.
    public Camera mainCamera;
    private float landscapeFOV = 53f;
    private float portraitFOV = 95f;

    // Reference to the main game UI.
    public GameObject ui;

    // Main puzzle logic.
    private Puzzle puzzle = new Puzzle();

    // 3D object instances according to the puzzle pieces.
    private List<GameObject> sliderInstances = new List<GameObject>();

    // Aligning slider cube 3D objects.
    private static float verticalOffset = -3f;
    private static float horizontalXOffset = -3f;
    private static float horizontalZOffset = -2f;

    // Sounds.
    public AudioClip winningSound;
    private AudioSource audioSource;

    private bool isPhoneDevice;

    // Start is called before the first frame update
    void Start() {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = winningSound;

        isPhoneDevice = Application.platform == RuntimePlatform.Android;

        ResetGame();
    }

    public void Update() {
        AdjustCamera();
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
        audioSource.Play();
        foreach (var slider in sliderInstances) slider.GetComponent<SliderCubeController>().GameOver();

        Invoke("FinishGameAndShowUI", 3f);
    }

    private void FinishGameAndShowUI() {
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

    void AdjustCamera() {
        if (Screen.orientation == ScreenOrientation.LandscapeLeft ||
            Screen.orientation == ScreenOrientation.LandscapeRight || 
            !isPhoneDevice
        ) {
            mainCamera.fieldOfView = landscapeFOV;
        } else {
            mainCamera.fieldOfView = portraitFOV;
        }
    }
}
