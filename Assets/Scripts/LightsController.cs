using UnityEngine;

public class LightsController : MonoBehaviour {
    public GameObject directionalLight;

    private float rotSpeed = 1f;

    // Update is called once per frame
    void Update() {
        directionalLight.transform.Rotate(Vector3.up * Time.deltaTime * rotSpeed, Space.World);
    }
}
