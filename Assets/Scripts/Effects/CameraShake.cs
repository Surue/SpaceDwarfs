using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraShakeState { FadingIn, FadingOut, Sustained, Inactive }

public class CameraShakeInstance {
    /// <summary>
    /// The intensity of the shake. It is recommended that you use ScaleMagnitude to alter the magnitude of a shake.
    /// </summary>
    public float Magnitude;

    /// <summary>
    /// Roughness of the shake. It is recommended that you use ScaleRoughness to alter the roughness of a shake.
    /// </summary>
    public float Roughness;

    /// <summary>
    /// How much influence this shake has over the local position axes of the camera.
    /// </summary>
    public Vector3 PositionInfluence;

    /// <summary>
    /// How much influence this shake has over the local rotation axes of the camera.
    /// </summary>
    public Vector3 RotationInfluence;

    /// <summary>
    /// Should this shake be removed from the CameraShakeInstance list when not active?
    /// </summary>
    public bool DeleteOnInactive = true;


    float roughMod = 1, magnMod = 1;
    float fadeOutDuration, fadeInDuration;
    bool sustain;
    float currentFadeTime;
    float tick = 0;
    Vector3 amt;

    /// <summary>
    /// Will create a new instance that will shake once and fade over the given number of seconds.
    /// </summary>
    /// <param name="magnitude">The intensity of the shake.</param>
    /// <param name="fadeOutTime">How long, in seconds, to fade out the shake.</param>
    /// <param name="roughness">Roughness of the shake. Lower values are smoother, higher values are more jarring.</param>
    public CameraShakeInstance(float magnitude, float roughness, float fadeInTime, float fadeOutTime) {
        this.Magnitude = magnitude;
        fadeOutDuration = fadeOutTime;
        fadeInDuration = fadeInTime;
        this.Roughness = roughness;
        if(fadeInTime > 0) {
            sustain = true;
            currentFadeTime = 0;
        } else {
            sustain = false;
            currentFadeTime = 1;
        }

        tick = Random.Range(-100, 100);
    }

    /// <summary>
    /// Will create a new instance that will start a sustained shake.
    /// </summary>
    /// <param name="magnitude">The intensity of the shake.</param>
    /// <param name="roughness">Roughness of the shake. Lower values are smoother, higher values are more jarring.</param>
    public CameraShakeInstance(float magnitude, float roughness) {
        this.Magnitude = magnitude;
        this.Roughness = roughness;
        sustain = true;

        tick = Random.Range(-100, 100);
    }

    public Vector3 UpdateShake() {
        amt.x = Mathf.PerlinNoise(tick, 0) - 0.5f;
        amt.y = Mathf.PerlinNoise(0, tick) - 0.5f;
        amt.z = Mathf.PerlinNoise(tick, tick) - 0.5f;

        if(fadeInDuration > 0 && sustain) {
            if(currentFadeTime < 1)
                currentFadeTime += Time.deltaTime / fadeInDuration;
            else if(fadeOutDuration > 0)
                sustain = false;
        }

        if(!sustain)
            currentFadeTime -= Time.deltaTime / fadeOutDuration;

        if(sustain)
            tick += Time.deltaTime * Roughness * roughMod;
        else
            tick += Time.deltaTime * Roughness * roughMod * currentFadeTime;

        return amt * Magnitude * magnMod * currentFadeTime;
    }

    /// <summary>
    /// Starts a fade out over the given number of seconds.
    /// </summary>
    /// <param name="fadeOutTime">The duration, in seconds, of the fade out.</param>
    public void StartFadeOut(float fadeOutTime) {
        if(fadeOutTime == 0)
            currentFadeTime = 0;

        fadeOutDuration = fadeOutTime;
        fadeInDuration = 0;
        sustain = false;
    }

    /// <summary>
    /// Starts a fade in over the given number of seconds.
    /// </summary>
    /// <param name="fadeInTime">The duration, in seconds, of the fade in.</param>
    public void StartFadeIn(float fadeInTime) {
        if(fadeInTime == 0)
            currentFadeTime = 1;

        fadeInDuration = fadeInTime;
        fadeOutDuration = 0;
        sustain = true;
    }

    /// <summary>
    /// Scales this shake's roughness while preserving the initial Roughness.
    /// </summary>
    public float ScaleRoughness {
        get { return roughMod; }
        set { roughMod = value; }
    }

    /// <summary>
    /// Scales this shake's magnitude while preserving the initial Magnitude.
    /// </summary>
    public float ScaleMagnitude {
        get { return magnMod; }
        set { magnMod = value; }
    }

    /// <summary>
    /// A normalized value (about 0 to about 1) that represents the current level of intensity.
    /// </summary>
    public float NormalizedFadeTime { get { return currentFadeTime; } }

    bool IsShaking { get { return currentFadeTime > 0 || sustain; } }

    bool IsFadingOut { get { return !sustain && currentFadeTime > 0; } }

    bool IsFadingIn { get { return currentFadeTime < 1 && sustain && fadeInDuration > 0; } }

    /// <summary>
    /// Gets the current state of the shake.
    /// </summary>
    public CameraShakeState CurrentState {
        get {
            if(IsFadingIn)
                return CameraShakeState.FadingIn;
            else if(IsFadingOut)
                return CameraShakeState.FadingOut;
            else if(IsShaking)
                return CameraShakeState.Sustained;
            else
                return CameraShakeState.Inactive;
        }
    }
}

public class CameraShake : MonoBehaviour {

    public float power = 0.7f;
    public float duration = 1.0f;

    public float slowDownAmount = 1.0f;
    public bool shouldShake = false;

    Vector3 startPosition;
    float initialDuration;

    public Vector3 DefaultPosInfluence = new Vector3(0.15f, 0.15f, 0.15f);
    public Vector3 DefaultRotInfluence = new Vector3(1, 1, 1);

    static CameraShake instance;
    public static CameraShake Instance {
        get {
            return instance;
        }
    }

    private void Awake() {
        if(instance == null) {
            instance = this;
        } else if(instance != this) {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start () {
        startPosition = transform.localPosition;
	}

    List<CameraShakeInstance> cameraShakeInstances = new List<CameraShakeInstance>();

    Vector3 posAddShake, rotAddShake;

    // Update is called once per frame
    void Update () {
        posAddShake = startPosition;
        rotAddShake = Vector3.zero;

        for(int i = 0;i < cameraShakeInstances.Count;i++) {
            if(i >= cameraShakeInstances.Count)
                break;

            CameraShakeInstance c = cameraShakeInstances[i];

            if(c.CurrentState == CameraShakeState.Inactive && c.DeleteOnInactive) {
                cameraShakeInstances.RemoveAt(i);
                i--;
            } else if(c.CurrentState != CameraShakeState.Inactive) {
                posAddShake += MultiplyVectors(c.UpdateShake(), c.PositionInfluence);
                rotAddShake += MultiplyVectors(c.UpdateShake(), c.RotationInfluence);
            }
        }

        transform.localPosition = posAddShake;
        transform.localEulerAngles = rotAddShake;
    }

    public static Vector3 MultiplyVectors(Vector3 v, Vector3 w) {
        v.x *= w.x;
        v.y *= w.y;
        v.z *= w.z;

        return v;
    }

    public CameraShakeInstance ShakeOnce(float magnitude, float roughness, float fadeInTime, float fadeOutTime) {
        CameraShakeInstance shake = new CameraShakeInstance(magnitude, roughness, fadeInTime, fadeOutTime);
        shake.PositionInfluence = DefaultPosInfluence;
        shake.RotationInfluence = DefaultRotInfluence;
        cameraShakeInstances.Add(shake);

        return shake;
    }

    public void AddShakeEffect(float power = 0.05f, float duration = 0.005f, float slowDownAmount = 1) {
        shouldShake = true;
        this.power = power;
        this.duration = duration;
        this.slowDownAmount = slowDownAmount;
    }
}
