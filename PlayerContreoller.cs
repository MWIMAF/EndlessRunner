using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerContreoller : MonoBehaviour
{
    [Header("Movement")]
    public float moveAccel;
    public float maxSpeed;

    [Header("Jump")]
    public float jumpAccel;

    [Header("Ground Raycast")]
    public float groundRaycastDistance;
    public LayerMask groundLayerMask;

    [Header("Scoring")]
    public ScoreController score;
    public float scoringRatio;
    private float lastPositionX;
    private int currentScore;
    private int lastScoreHighlight;

    [Header("Game Over")]
    public GameObject gameOverScreen;
    public float fallPositionY;

    [Header("Camera")]
    public CameraController gameCamera;

    [Header("Score Highlight")]
    public int scoreHiglightRange;
    public CharacterSoundController soundHighlight;

    private Rigidbody2D rig;

    private bool isJumping;
    private bool isOnGround;
    private Animator anim;
    private CharacterSoundController sound;

    // Start is called before the first frame update
    void Start()
    {
        currentScore = 0;
        lastScoreHighlight = 0;
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        sound = GetComponent<CharacterSoundController>();
    }

    // Update is called once per frame
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(isOnGround)
            {
                isJumping = true;
                sound.PlayJump();
            }
        }

        anim.SetBool("isOnGround", isOnGround);

        int distancePassed = Mathf.FloorToInt(transform.position.x - lastPositionX);
        int scoreIncrement = Mathf.FloorToInt(distancePassed / scoringRatio);

        if(scoreIncrement > 0)
        {
            score.IncreaseCurrentScore(scoreIncrement);
            lastPositionX += distancePassed;
        }

        if(transform.position.y < fallPositionY)
        {
            GameOver();
        }
    }
    void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundRaycastDistance, groundLayerMask);
        if (hit)
        {
            if (!isOnGround && rig.velocity.y <= 0)
            {
                isOnGround = true;
            }
        }
        else
        {
            isOnGround = false;
        }

        Vector2 velocityVector = rig.velocity;

        if (isJumping)
        {
            velocityVector.y += jumpAccel;
            isJumping = false;
        }
        velocityVector.x = Mathf.Clamp(velocityVector.x + moveAccel * Time.deltaTime, 0f, maxSpeed);

        rig.velocity = velocityVector;
    }

    private void GameOver()
    {
        score.FinishScoring();

        gameCamera.enabled = false;

        gameOverScreen.SetActive(true);

        this.enabled = false;
    }

    public void IncreaseCurrentScore(int increment)
    {
        currentScore += increment;

        if(currentScore - lastScoreHighlight > scoreHiglightRange)
        {
            soundHighlight.PlayScoreHighlight();
            lastScoreHighlight += scoreHiglightRange;
        }
    }

    public void OnDrawGizmos()
    {
        Debug.DrawLine(transform.position, transform.position + (Vector3.down * groundRaycastDistance), Color.white);
    }
}
