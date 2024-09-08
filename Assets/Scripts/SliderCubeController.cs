using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderCubeController : MonoBehaviour {
    // Reference to the main puzzle logic.
    public Puzzle.Slider puzzleSlider;

    // Reference to the main (top level) game controller.
    public GameController gameController;

    // Controls the bounds of the slide (due to edge of the board or another cube).
    // Set by the game controller.
    public float maxSlide;
    public float minSlide;

    // Sounds.
    public AudioClip clonkSound;
    public AudioClip scratchSound;
    private AudioSource clonkAudioSource;
    private AudioSource scratchAudioSource;

    private Easing scratchSoundVolumeEasing = new Easing(0.1f);

    // Smoke particles - when scratching the board while moving.
    public ParticleSystem smokeFront;
    public ParticleSystem smokeBack;

    // Offset to where the mouse grabbed a cube.
    public float dragOffset = 0f;

    // Whether the block got away from the wall (to check if we can clunk).
    public bool isAwayFromWall = true;
    private float awayFromWallDistance = 0.1f;

    private bool isGameOver = false;
    private float gameOverRiseSpeed;

    // Start is called before the first frame update
    void Start() {
        SetupAudio();

        SmokeStopAll();

        SetAwayFromWallState();

        gameOverRiseSpeed = Random.Range(0.1f, 2f);
    }

    // Update is called once per frame
    void Update() {
        if (isGameOver) {
            transform.Translate(Vector3.up * Time.deltaTime * gameOverRiseSpeed);

            return;
        }

        scratchSoundVolumeEasing.Update();
    }

    private Vector3 GetMousePos3D() {
        return Camera.main.ViewportToWorldPoint(Input.mousePosition);
    }

    private void OnMouseDown() {
        if (isGameOver) return;

        SaveDragOffset();
    }

    private void OnMouseUp() {
        if (isGameOver) return;

        SnapToGrid();
        SmokeStopAll();
        StopScratchSound();
        SetAwayFromWallState();

        gameController.OnUpdateSliderPos(this);
    }

    private void OnMouseDrag() {
        if (isGameOver) return;

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 9.5f;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        float newP;
        if (puzzleSlider.IsVertical()) {
            newP = worldPos.z - dragOffset;
        } else {
            newP = worldPos.x - dragOffset;
        }

        if (newP <= minSlide) {
            newP = minSlide;

            if (isAwayFromWall) clonkAudioSource.Play();
            isAwayFromWall = false;
        }
        if (newP >= maxSlide) {
            newP = maxSlide;

            if (isAwayFromWall) clonkAudioSource.Play();
            isAwayFromWall = false;
        }

        if (minSlide + awayFromWallDistance < newP && maxSlide - awayFromWallDistance > newP) isAwayFromWall = true;

        float diffFrame;
        if (puzzleSlider.IsVertical()) {
            diffFrame = newP - transform.position.z;
            transform.position = new Vector3(transform.position.x, transform.position.y, newP);
        } else {
            diffFrame = newP - transform.position.x;
            transform.position = new Vector3(newP, transform.position.y, transform.position.z);
        }

        SmokeWhileDragging(diffFrame);
        ScratchSoundWhileDragging(diffFrame);
    }

    private void OnTriggerEnter(Collider other) {
        gameController.SignalWinning();
        //Debug.Log("YAY");
    }

    private void SetFrontSmokeActivity(bool isActive) {
        //Debug.Log("Front smoke: " + isActive.ToString());
        SetSmokeActivity(smokeFront, isActive);
    }

    private void SetBackSmokeActivity(bool isActive) {
        SetSmokeActivity(smokeBack, isActive);
    }

    private void SetSmokeActivity(ParticleSystem particleSystem, bool isActive) {
        if (isActive) {
            if (!particleSystem.isPlaying) particleSystem.Play();
        } else {
            if (particleSystem.isPlaying) particleSystem.Stop();
        }
    }

    private void PlayScratchSound(float volume) {
        scratchSoundVolumeEasing.setTarget(Mathf.Clamp01(volume));
        scratchAudioSource.volume = scratchSoundVolumeEasing.value;

        if (!scratchAudioSource.isPlaying) scratchAudioSource.Play();
    }

    private void StopScratchSound() {
        if (scratchAudioSource.isPlaying) scratchAudioSource.Stop();
    }

    private void SetupAudio() {
        AudioSource[] audioSources = GetComponents<AudioSource>();

        clonkAudioSource = audioSources[0];
        clonkAudioSource.clip = clonkSound;

        scratchAudioSource = audioSources[1];
        scratchAudioSource.clip = scratchSound;
    }

    private void SaveDragOffset() {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 9.5f;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        if (puzzleSlider.IsVertical()) {
            dragOffset = worldPos.z - transform.position.z;
        } else {
            dragOffset = worldPos.x - transform.position.x;
        }
    }

    private void SetAwayFromWallState() {
        if (puzzleSlider.IsVertical()) {
            isAwayFromWall = minSlide + awayFromWallDistance < transform.position.z && maxSlide - awayFromWallDistance > transform.position.z;
        } else {
            isAwayFromWall = minSlide + awayFromWallDistance < transform.position.x && maxSlide - awayFromWallDistance > transform.position.x;
        }
    }

    private void SnapToGrid() {
        float newP;
        if (puzzleSlider.IsVertical()) {
            newP = (float)System.Math.Round(transform.position.z);
            transform.position = new Vector3(transform.position.x, transform.position.y, newP);
        } else {
            newP = (float)System.Math.Round(transform.position.x);
            transform.position = new Vector3(newP, transform.position.y, transform.position.z);
        }
        if (isAwayFromWall && (newP == minSlide || newP == maxSlide)) clonkAudioSource.Play();
    }

    private void SmokeWhileDragging(float dist) {
        // Smoke control.
        if (dist > 0) {
            SetFrontSmokeActivity(true);
            SetBackSmokeActivity(false);
        } else if (dist < 0) {
            SetFrontSmokeActivity(false);
            SetBackSmokeActivity(true);
        } else {
            SetFrontSmokeActivity(false);
            SetBackSmokeActivity(false);
        }
    }

    private void ScratchSoundWhileDragging(float dist) {
        // Scratch sounds.
        PlayScratchSound(Mathf.Abs(dist));
        if (dist != 0) {
            PlayScratchSound(Mathf.Abs(dist));
            //Debug.Log(diffFrame);
        } else {
            StopScratchSound();
        }
    }

    private void SmokeStopAll() {
        SetFrontSmokeActivity(false);
        SetBackSmokeActivity(false);
    }

    public void GameOver() {
        isGameOver = true;
        SmokeStopAll();
        StopScratchSound();
        SnapToGrid();
    }
}
