using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedSpriteAnimator : MonoBehaviour
{
    [Header("Target Renderer (choose one)")]
    public SpriteRenderer spriteRenderer;
    public Image uiImage;

    [Header("Frames (ordered from min -> max speed)")]
    public List<Sprite> frames = new List<Sprite>();

    [Header("Speed range")]
    public float minSpeed = 0f;
    public float maxSpeed = 20f;

    [Header("Source (optional)")]
    public BikeDataSO bikeData; // optional: use bikeData.currentSpeed if assigned

    [Header("Behavior")]
    [Tooltip("If true, uses absolute speed (magnitude). If false, uses signed speed.")]
    public bool useAbsoluteSpeed = true;
    [Tooltip("Smoothing time in seconds (0 = immediate)")]
    public float smoothing = 0.08f;

    // Manual mode: call SetSpeed(value) if bikeData is not assigned
    private float manualSpeed = 0f;
    private float smoothNorm = 0f;

    void Update()
    {
        if (frames == null || frames.Count == 0) return;

        // determine target normalized value 0..1
        float rawSpeed = bikeData != null ? bikeData.currentSpeed : manualSpeed;
        if (useAbsoluteSpeed) rawSpeed = Mathf.Abs(rawSpeed);

        float targetNorm = Mathf.InverseLerp(minSpeed, maxSpeed, rawSpeed);
        targetNorm = Mathf.Clamp01(targetNorm);

        // smooth
        if (smoothing > 0f)
            smoothNorm = Mathf.MoveTowards(smoothNorm, targetNorm, Time.deltaTime / Mathf.Max(0.0001f, smoothing));
        else
            smoothNorm = targetNorm;

        // map to frame index
        int index = Mathf.Clamp(Mathf.RoundToInt(smoothNorm * (frames.Count - 1)), 0, frames.Count - 1);
        ApplySprite(frames[index]);
    }

    void ApplySprite(Sprite s)
    {
        if (spriteRenderer != null && spriteRenderer.sprite != s) spriteRenderer.sprite = s;
        if (uiImage != null && uiImage.sprite != s) uiImage.sprite = s;
    }

    // Call this if you don't use BikeDataSO and want to drive by code
    public void SetSpeed(float speed)
    {
        manualSpeed = speed;
    }
}