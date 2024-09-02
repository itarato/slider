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
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start() {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = clonkSound;
    }

    // Update is called once per frame
    void Update() {
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

        audioSource.Play();

        gameController.OnUpdateSliderPos(this);
    }

    private void OnMouseDrag() {
        Vector3 relativeMousePos = GetMousePos3D();
        float newP;

        if (puzzleSlider.IsVertical()) {
            float diff = (relativeMousePos.y - dragOrigMousePos.y) * 10f;
            newP = dragOrigPos.z + diff;
        } else {
            float diff = (relativeMousePos.x - dragOrigMousePos.x) * 10f;
            newP = dragOrigPos.x + diff;
        }

        //Debug.LogFormat("Try slide: {0} <= {1} <= {2}", minSlide, newP, maxSlide);

        if (newP < minSlide) {
            newP = minSlide;
            //audioSource.Play();
        }
        if (newP > maxSlide) {
            newP = maxSlide;
            //audioSource.Play();
        }

        if (puzzleSlider.IsVertical()) {
            transform.position = new Vector3(transform.position.x, transform.position.y, newP);
        } else {
            transform.position = new Vector3(newP, transform.position.y, transform.position.z);
        }
    }

    private void OnTriggerEnter(Collider other) {
        gameController.signalWinning();
    }
}
