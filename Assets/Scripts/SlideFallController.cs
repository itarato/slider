using UnityEngine;

public class SlideFallController : MonoBehaviour {
    private bool stateIsFalling = true;
    private float fallSpeed = 22f;

    // Update is called once per frame
    void Update()
    {
        if (stateIsFalling) {
            if (transform.position.y <= 0.75f) {
                stateIsFalling = false;
                transform.position = new Vector3(transform.position.x, 0.75f, transform.position.z);
            } else {
                transform.Translate(Vector3.down * Time.deltaTime * fallSpeed);
            }
        }
    }
}
