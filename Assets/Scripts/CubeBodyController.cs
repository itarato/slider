using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeBodyController : MonoBehaviour
{
    private Renderer objRenderer;

    // Start is called before the first frame update
    void Start()
    {
        objRenderer = GetComponent<Renderer>();
        objRenderer.material.color = new Color(Random.Range(0.6f, 1.0f), Random.Range(0.6f, 1.0f), Random.Range(0.6f, 1.0f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
