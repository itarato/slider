using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderCubeController : MonoBehaviour
{
    private Vector3 dragOrigMousePos;
    private Vector3 dragOrigPos;
    public Puzzle.Slider puzzleSlider;
    public GameController gameController;
    public float maxSlide;
    public float minSlide;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {   
    }

    private Vector3 GetMousePos3D()
    {
        return Camera.main.ScreenToViewportPoint(Input.mousePosition);
    }

    private void OnMouseDown()
    {
        // Save drag-staring mouse + object pos to calculate drag move.
        dragOrigMousePos = GetMousePos3D();
        dragOrigPos = transform.position;
    }

    private void OnMouseUp()
    {
        // Snap to grid.
        if (puzzleSlider.IsVertical()) {
            transform.position = new Vector3(transform.position.x, transform.position.y, (float)System.Math.Round(transform.position.z));
        } else {
            transform.position = new Vector3((float)System.Math.Round(transform.position.x), transform.position.y, transform.position.z);
        }

        gameController.OnUpdateSliderPos(this);
    }

    private void OnMouseDrag()
    {
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

        if (newP < minSlide) newP = minSlide;
        if (newP > maxSlide) newP = maxSlide;

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
