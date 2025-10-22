using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraModelCameo : MonoBehaviour
{
    [Header("Refs")]
    public Transform model;                 // child of Main Camera
    private Animator animator;
    private AudioSource audioSource;

    [Header("Timing")]
    public float triggerInterval = 30f;     // repeat every 30s
    private float nextTriggerTime;

    [Header("Positions (local to camera)")]
    public Vector3 behindPos = new(0f, -0.25f, -0.6f);
    public Vector3 enterPos = new (-0.6f, -0.25f, 1f);
    public Vector3 centerPos = new (0.0f, -0.25f, 1f);
    public Vector3 exitPos = new (0.6f, -0.25f, 1f);

    [Header("Rotation (local Y)")]
    public float leftRotationY = 180f;
    public float centerRotationY = 0f;

    [Header("Durations")]
    public float slideInTime = 0.6f;
    public float holdCenterTime = 1.0f;
    public float slideOutTime = 0.6f;
    public float returnBehindTime = 0.35f;

    [Header("Visuals")]
    public float cameoScale = 0.25f;

    [Header("Actions (Animator Triggers)")]
    public string[] actionTriggers = new string[] { "mambo", "fornight","left_right","waving","streching","floating" };

    [Header("Randomization")]
    public bool enableSeed = true;
    public int seed = 12345;
    public bool avoidImmediateRepeat = true;

    // seeded RNG + shuffle-bag state
    private System.Random rng;
    private readonly List<int> bag = new();
    private int lastIndex = -1;

    void Start()
    {
        audioSource = model ? model.GetComponent<AudioSource>() : null;
        animator = model ? model.GetComponent<Animator>() : null;

        if (!model)
        {
            Debug.LogWarning("CameraModelCameo: assign 'model' (child of camera).");
            enabled = false; return;
        }

        if (model.parent != Camera.main.transform)
            model.SetParent(Camera.main.transform, false);

        model.localScale = Vector3.one * cameoScale;
        model.localPosition = behindPos;
        model.localRotation = Quaternion.Euler(0f, leftRotationY, 0f);

        rng = enableSeed ? new System.Random(seed) : new System.Random();
        RefillBag();

        nextTriggerTime = triggerInterval;
    }


    void Update()
    {
        if (Time.timeScale == 0f) return;

        if (Time.time >= nextTriggerTime)
        {
            nextTriggerTime += triggerInterval;
            StartCoroutine(CameoRoutine());
        }
    }

    IEnumerator CameoRoutine()
    {
        if (animator) { ResetAllAnimatorTriggers(animator);}

        // Behind ¡÷ Left ¡÷ Center (rotate 180¡÷0)
        yield return MoveAndRotateLocal(model, behindPos, enterPos, leftRotationY, leftRotationY, slideInTime * 0.6f);
        yield return MoveAndRotateLocal(model, enterPos, centerPos, leftRotationY, centerRotationY, slideInTime * 0.4f);

        string chosen = NextActionTrigger();
        if (animator && !string.IsNullOrEmpty(chosen))
            animator.SetTrigger(chosen);

        if (audioSource) audioSource.Play();

        yield return new WaitForSeconds(holdCenterTime);

        // Center ¡÷ Right (rotate 0¡÷180) ¡÷ Behind
        yield return MoveAndRotateLocal(model, centerPos, exitPos, centerRotationY, leftRotationY, slideOutTime);
        yield return MoveAndRotateLocal(model, exitPos, behindPos, leftRotationY, leftRotationY, returnBehindTime);
    }

    static IEnumerator MoveAndRotateLocal(Transform t, Vector3 fromPos, Vector3 toPos, float fromY, float toY, float dur)
    {
        if (dur <= 0f) { t.localPosition = toPos; t.localRotation = Quaternion.Euler(0, toY, 0); yield break; }

        float a = 0f;
        while (a < 1f)
        {
            a += Time.deltaTime / dur;
            float s = Mathf.SmoothStep(0f, 1f, a);
            t.localPosition = Vector3.Lerp(fromPos, toPos, s);
            t.localRotation = Quaternion.Euler(0f, Mathf.Lerp(fromY, toY, s), 0f);
            yield return null;
        }
        t.localPosition = toPos;
        t.localRotation = Quaternion.Euler(0f, toY, 0f);
    }

    // ------- Shuffle-bag randomization (seeded) -------

    // Fill bag with 0..N-1, then Fisher¡VYates shuffle using seeded RNG
    void RefillBag()
    {
        bag.Clear();
        if (actionTriggers == null || actionTriggers.Length == 0) return;

        for (int i = 0; i < actionTriggers.Length; i++) bag.Add(i);

        // Fisher¡VYates with System.Random
        for (int i = bag.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (bag[i], bag[j]) = (bag[j], bag[i]);
        }

        // Optional: avoid starting new bag with the same as lastIndex
        if (avoidImmediateRepeat && lastIndex >= 0 && bag.Count > 1 && bag[0] == lastIndex)
        {
            // swap first with any different index (choose 1..end)
            int swapWith = 1 + rng.Next(bag.Count - 1);
            (bag[0], bag[swapWith]) = (bag[swapWith], bag[0]);
        }
    }

    string NextActionTrigger()
    {
        if (actionTriggers == null || actionTriggers.Length == 0) return null;

        if (bag.Count == 0) RefillBag();

        int idx = bag[0];
        bag.RemoveAt(0);

        // extra guard against immediate repeats when bag size was 1
        if (avoidImmediateRepeat && actionTriggers.Length > 1 && idx == lastIndex)
        {
            // pick a different index directly
            idx = (idx + 1) % actionTriggers.Length;
        }

        lastIndex = idx;
        return actionTriggers[idx];
    }

    void ResetAllAnimatorTriggers(Animator anim)
    {
        if (!anim) return;
        for (int i = 0; i < anim.parameterCount; i++)
        {
            var p = anim.GetParameter(i);
            if (p.type == AnimatorControllerParameterType.Trigger)
                anim.ResetTrigger(p.name);
        }
    }
}
