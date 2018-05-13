using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraShakeState { FadingIn, FadingOut, Sustained, Inactive }

public class CameraShakeInstance {
 

    public float Magnitude;
    public float Roughness;

    public Vector3 PositionInfluence;
    public Vector3 RotationInfluence;

    public bool DeleteOnInactive = true;


    float roughMod = 1, magnMod = 1;
    float fadeOutDuration, fadeInDuration;
    bool sustain;
    float currentFadeTime;
    float tick = 0;
    Vector3 amt;

    //Create a CameraShakeInstance that will fade over time
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

    //Update the current shake
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

    bool IsShaking { get { return currentFadeTime > 0 || sustain; } }

    bool IsFadingOut { get { return !sustain && currentFadeTime > 0; } }

    bool IsFadingIn { get { return currentFadeTime < 1 && sustain && fadeInDuration > 0; } }

    //Get the current state of the shake
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
    Vector3 startPosition;

    public Vector3 DefaultPosInfluence = new Vector3(0.0f, 0.0f, 0.0f);
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
}
