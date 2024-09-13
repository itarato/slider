using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeBodyController : MonoBehaviour
{
    private Renderer objRenderer;
    public Material specialMaterial;
    public bool isSpecial = false;

    // Start is called before the first frame update
    void Start() {
        objRenderer = GetComponent<Renderer>();

        if (isSpecial) {
            objRenderer.material = specialMaterial;
        } else {
            //objRenderer.material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        }
    }
}
