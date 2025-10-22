using UnityEngine;
using System.Collections;

public class CameraModelCameo : MonoBehaviour
{
    [Header("Refs")]
    public Transform model;       
    private Animator animator;
    private AudioSource audioSource;
    public GameManager gameManager;
    private bool stop;

    [Header("Timing")]
    public float triggerInterval = 30f; // Repeat every 30s
    private float nextTriggerTime;

    [Header("Positions (local to camera)")]
    public Vector3 behindPos = new Vector3(0f, -0.25f, -0.6f); // hidden behind camera
    public Vector3 enterPos = new Vector3(-0.6f, -0.25f, 0.7f);
    public Vector3 centerPos = new Vector3(0.0f, -0.25f, 0.7f);
    public Vector3 exitPos = new Vector3(0.6f, -0.25f, 0.7f);

    [Header("Animation Timings")]
    public float slideInTime = 0.6f;
    public float holdCenterTime = 1.0f;      // how long to stay in center (mambo plays here)
    public float slideOutTime = 0.6f;
    public float returnBehindTime = 0.35f;

    [Header("Rotation (local Y)")]
    public float leftRotationY = 180f;
    public float centerRotationY = 0f;

    [Header("Scale")]
    public float cameoScale = 0.25f;         // make the full model small for HUD-like pop

    float elapsed;
    bool fired;

    void Start()
    {
        audioSource =model? model.GetComponent<AudioSource>(): null;

        animator = model ? model.GetComponent<Animator>() : null;

        if (gameManager) gameManager.OnGameOver += () => stop = true;

        if (!model)
        {
            Debug.LogWarning("CameraModelCameo: assign 'model' (child of camera).");
            enabled = false; return;
        }

        // ensure it¡¦s a child of the camera so local positions make sense
        if (model.parent != Camera.main.transform)
            model.SetParent(Camera.main.transform, false);

        // start hidden behind the camera
        model.localScale = Vector3.one * cameoScale;
        model.localPosition = behindPos;
        model.localRotation = Quaternion.Euler(0f, leftRotationY, 0f);

        nextTriggerTime = triggerInterval;
    }

    void Update()
    {
        if (stop) return;

        if (Time.timeScale == 0f) return;

        if (Time.time >= nextTriggerTime)
        {
            nextTriggerTime += triggerInterval;
            StartCoroutine(CameoRoutine());
        }
    }

    IEnumerator CameoRoutine()
    {
        // Behind ¡÷ Left ¡÷ Center (rotate Y 180¡÷0)
        yield return MoveAndRotateLocal(model, behindPos, enterPos, leftRotationY, leftRotationY, slideInTime * 0.6f);
        yield return MoveAndRotateLocal(model, enterPos, centerPos, leftRotationY, centerRotationY, slideInTime * 0.4f);

        // Play mambo animation
        if (animator) animator.SetTrigger("mambo");
        if (audioSource) audioSource.Play();
        yield return new WaitForSeconds(holdCenterTime);

        // Center ¡÷ Right (rotate Y 0¡÷180)
        yield return MoveAndRotateLocal(model, centerPos, exitPos, centerRotationY, leftRotationY, slideOutTime);

        // Right ¡÷ Behind (keep rotation 180)
        yield return MoveAndRotateLocal(model, exitPos, behindPos, leftRotationY, leftRotationY, returnBehindTime);
    }

    static IEnumerator MoveAndRotateLocal(Transform t, Vector3 fromPos, Vector3 toPos, float fromY, float toY, float dur)
    {
        if (dur <= 0f) { t.localPosition = toPos; t.localRotation = Quaternion.Euler(0, toY, 0); yield break; }

        float a = 0f;
        while (a < 1f)
        {
            a += Time.deltaTime / dur;
            t.localPosition = Vector3.Lerp(fromPos, toPos, Mathf.SmoothStep(0f, 1f, a));
            float yRot = Mathf.Lerp(fromY, toY, Mathf.SmoothStep(0f, 1f, a));
            t.localRotation = Quaternion.Euler(0f, yRot, 0f);
            yield return null;
        }

        t.localPosition = toPos;
        t.localRotation = Quaternion.Euler(0f, toY, 0f);
    }
}
