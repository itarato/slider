using UnityEngine;

public class SlideFallController : MonoBehaviour {
    public AudioSource audioSource;

    private bool stateIsFalling = true;
    private float fallSpeed = 22f;
    private float targetHeight = 0.75f;

    // Update is called once per frame
    void Update()
    {
        if (stateIsFalling) {
            if (transform.position.y <= targetHeight) {
                stateIsFalling = false;
                transform.position = new Vector3(transform.position.x, targetHeight, transform.position.z);

                audioSource.Play();
            } else {
                transform.Translate(Vector3.down * Time.deltaTime * fallSpeed);
            }
        }
    }
}
