using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsController : MonoBehaviour {
    public GameObject directionalLight;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        directionalLight.transform.Rotate(Vector3.up * Time.deltaTime * 1f, Space.World);
    }
}
