using UnityEngine;

public class SpriteAnimator: MonoBehaviour
{
    [Tooltip("Sprites for animation")]
    public Sprite[] frames;
    [Tooltip("Frames per second")]
    public float fps = 12f;

    private SpriteRenderer sr;
    private float timer;
    private int currentFrame;

    void Start() => sr = GetComponent<SpriteRenderer>();

    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= 1f/fps)
        {
            currentFrame = (currentFrame + 1) % frames.Length;
            sr.sprite = frames[currentFrame];
            timer = 0;
        }
    }
}