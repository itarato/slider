using UnityEngine;

/**
 * Controls the slider cube prefabs inner cube (the one having a 3D mesh) object.
 */
public class CubeBodyController : MonoBehaviour {
    // Material to use on the exit-slide.
    public Material specialMaterial;

    private Renderer objRenderer;

    // Whether it is the exit-slide.
    public bool isSpecial = false;

    // Whether colors (for slides) are enabled for the game.
    public bool colorsOn = false;

    // Start is called before the first frame update
    void Start() {
        objRenderer = GetComponent<Renderer>();

        if (isSpecial) {
            objRenderer.material = specialMaterial;
        } else if (colorsOn) {
            objRenderer.material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        }
    }
}
