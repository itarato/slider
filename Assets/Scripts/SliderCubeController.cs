using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SliderCubeController : MonoBehaviour {
    // Offset helpers for dragging.
    private Vector3 dragOrigMousePos;
    private Vector3 dragOrigPos;

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

    // Start is called before the first frame update
    void Start() {
        AudioSource[] audioSources = GetComponents<AudioSource>();

        clonkAudioSource = audioSources[0];
        clonkAudioSource.clip = clonkSound;

        scratchAudioSource = audioSources[1];
        scratchAudioSource.clip = scratchSound;

        SetFrontSmokeActivity(false);
        SetBackSmokeActivity(false);
    }

    // Update is called once per frame
    void Update() {
        scratchSoundVolumeEasing.Update();
    }

    private Vector3 GetMousePos3D() {
        return Camera.main.ScreenToViewportPoint(Input.mousePosition);
    }

    private void OnMouseDown() {
        // Save drag-staring mouse + object pos to calculate drag move.
        dragOrigMousePos = GetMousePos3D();
        dragOrigPos = transform.position;
    }

    private void OnMouseUp() {
        // Snap to grid.
        if (puzzleSlider.IsVertical()) {
            transform.position = new Vector3(transform.position.x, transform.position.y, (float)System.Math.Round(transform.position.z));
        } else {
            transform.position = new Vector3((float)System.Math.Round(transform.position.x), transform.position.y, transform.position.z);
        }

        SetFrontSmokeActivity(false);
        SetBackSmokeActivity(false);

        clonkAudioSource.Play();
        StopScratchSound();

        gameController.OnUpdateSliderPos(this);
    }

    private void OnMouseDrag() {
        Vector3 relativeMousePos = GetMousePos3D();
        float newP;
        float diffOrig;

        if (puzzleSlider.IsVertical()) {
            diffOrig = (relativeMousePos.y - dragOrigMousePos.y) * 10f;
            newP = dragOrigPos.z + diffOrig;
        } else {
            diffOrig = (relativeMousePos.x - dragOrigMousePos.x) * 10f;
            newP = dragOrigPos.x + diffOrig;
        }

        //Debug.LogFormat("Try slide: {0} <= {1} <= {2}", minSlide, newP, maxSlide);

        if (newP < minSlide) newP = minSlide;
        if (newP > maxSlide) newP = maxSlide;


        float diffFrame;
        if (puzzleSlider.IsVertical()) {
            diffFrame = newP - transform.position.z;
            transform.position = new Vector3(transform.position.x, transform.position.y, newP);
        } else {
            diffFrame = newP - transform.position.x;
            transform.position = new Vector3(newP, transform.position.y, transform.position.z);
        }

        if (diffFrame > 0) {
            SetFrontSmokeActivity(true);
            SetBackSmokeActivity(false);
        } else if (diffFrame < 0) {
            SetFrontSmokeActivity(false);
            SetBackSmokeActivity(true);
        } else {
            SetFrontSmokeActivity(false);
            SetBackSmokeActivity(false);
        }

        PlayScratchSound(Mathf.Abs(diffFrame));
        //if (diffFrame != 0) {
        //    PlayScratchSound(Mathf.Abs(diffFrame));
        //    //Debug.Log(diffFrame);
        //} else {
        //    StopScratchSound();
        //}
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
}
