using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour {
    private enum State {
        Play,
        GameOver,
    }

    // Cube prefabs to pick for instantiation.
    public GameObject[] sliderCubeCollection;

    // Main camera.
    public Camera mainCamera;
    private float landscapeFOV = 65f;
    private float portraitFOV = 95f;

    // Reference to the various UIs.
    public GameObject startMenuUI;
    public GameObject inGameUI;
    public TextMeshProUGUI stepsTextUI;
    public TextMeshProUGUI levelTextUI;
    public TextMeshProUGUI perfectStepsTextUI;

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

    // Level state for the current play - for UI to present.
    private int steps = 0;
    private LevelsController.Level currentLevel;

    public static GameController instance;

    private State state = State.Play;
    private bool isColorsOn = false;

    private void Awake() {
        instance = this;
    }

    // Start is called before the first frame update
    void Start() {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = winningSound;

        isPhoneDevice = Application.platform == RuntimePlatform.Android;

        inGameUI.SetActive(false);

        ResetGame();
    }

    public void Update() {
        AdjustCamera();

        if (state == State.GameOver && perfectStepsTextUI.gameObject.activeInHierarchy) {
            perfectStepsTextUI.gameObject.transform.localScale = Vector3.one * (1f + Mathf.Sin(Time.frameCount / 20f) / 10f);
        }
    }

    public void OnUIStartClick(LevelsController.Level level) {
        currentLevel = level;

        StartGame(level.sliders);
    }

    private void ResetGame() {
        perfectStepsTextUI.gameObject.SetActive(false);

        foreach (var sliderInstance in sliderInstances) Destroy(sliderInstance);
        sliderInstances.Clear();
    }

    private void StartGame(List<Puzzle.Slider> sliders) {
        puzzle.sliders = sliders.Select(e => e.Clone()).ToList();

        float dropHeight = 1f;

        foreach (Puzzle.Slider slider in puzzle.sliders) {
            GameObject newSlider;
            int prefabIdx = slider.len - 1;
            if (slider.IsVertical()) {
                newSlider = Instantiate(sliderCubeCollection[prefabIdx], new Vector3(slider.x + verticalOffset, dropHeight, slider.y + verticalOffset), Quaternion.identity);
            } else {
                newSlider = Instantiate(sliderCubeCollection[prefabIdx], new Vector3(slider.x + horizontalXOffset, dropHeight, slider.y + horizontalZOffset), Quaternion.Euler(new Vector3(0f, 90f, 0f)));
            }
            sliderInstances.Add(newSlider);
            dropHeight += 1f;

            if (slider.IsSpecial()) {
                newSlider.GetComponentInChildren<CubeBodyController>().isSpecial = true;
            }

            SliderCubeController sliderCubeController = newSlider.GetComponent<SliderCubeController>();
            if (sliderCubeController != null) {
                sliderCubeController.puzzleSlider = slider;
                sliderCubeController.gameController = this;
            }

            if (isColorsOn) {
                CubeBodyController cubeCtrl = newSlider.GetComponentInChildren<CubeBodyController>();
                if (cubeCtrl != null) {
                    cubeCtrl.colorsOn = true;
                }
            }
        }

        UpdateSliderCubesBounds();

        startMenuUI.SetActive(false);
        inGameUI.SetActive(true);

        steps = 0;
        state = State.Play;
        UpdateScoreLine();
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

        steps++;
        UpdateScoreLine();
    }

    public void SignalWinning() {
        audioSource.Play();
        foreach (var slider in sliderInstances) slider.GetComponent<SliderCubeController>()?.GameOver();
        state = State.GameOver;

        if (steps <= currentLevel.MinStepsRequired()) perfectStepsTextUI.gameObject.SetActive(true);

        Invoke("FinishGameAndShowUI", 2f);
    }

    private void UpdateScoreLine() {
        stepsTextUI.text = "Steps: " + steps.ToString();
        levelTextUI.text = "Pack: " + currentLevel.name + " moves | Level #" + currentLevel.levelIdx.ToString();
    }

    private void FinishGameAndShowUI() {
        ResetGame();
        startMenuUI.SetActive(true);
        inGameUI.SetActive(false);
    }

    private void UpdateSliderCubesBounds() {
        puzzle.RefreshMinMaxBoundCache();

        foreach (var sliderInstance in sliderInstances) {
            SliderCubeController sliderCubeController = sliderInstance.GetComponent<SliderCubeController>();
            if (sliderCubeController == null) continue;

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
        // On phone the FOV makes the board too close however both desktop and phone reports prortrait mode.
        if (Screen.orientation == ScreenOrientation.LandscapeLeft ||
            Screen.orientation == ScreenOrientation.LandscapeRight ||
            !isPhoneDevice
        ) {
            mainCamera.fieldOfView = landscapeFOV;
        } else {
            mainCamera.fieldOfView = portraitFOV;
        }
    }

    public void OnClickExitLevel() {
        if (state == State.GameOver) return;

        FinishGameAndShowUI();
    }

    public void OnClickResetLevel() {
        CancelInvoke();
        ResetGame();
        StartGame(currentLevel.sliders);
    }

    public void OnToggleColors(bool isOn) {
        isColorsOn = isOn;
    }
}
