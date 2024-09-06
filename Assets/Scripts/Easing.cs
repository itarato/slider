public class Easing {
    public float value { get; private set; }
    private float target;

    public Easing(float initValue) {
        this.value = initValue;
        this.target = initValue;
    }

    public void setTarget(float target) { 
        this.target = target; 
    }

    public void Update() {
        float diff = target - value;
        value += diff / 3f;
    }
}
