/**
 * Very minimal tweening tool for easing a value into a target without a sudden change.
 */
public class Easing {
    public float value { get; private set; }
    private float target;

    public Easing(float initValue) {
        value = initValue;
        target = initValue;
    }

    public void setTarget(float target) { 
        this.target = target; 
    }

    public void Update() {
        float diff = target - value;
        value += diff / 3f;
    }
}
