using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class action2 : MonoBehaviour
{
    [Header("速度相关")]
    public float playerMoveSpeed = 4;
    public float playerJumpSpeed = 11;
    [Header("跳跃次数")]
    public float playerjumpCount;
    [Header("判断")]
    public bool isground;
    public bool pressjump;
    public bool iscrouch;
    public bool pressedCrouch;
    [Header("其他组件")]
    public Transform foot;
    public LayerMask ground;
    public Rigidbody2D playerRB;
    public Animator playerAnim;
    [Header("碰撞体相关")]
    public CapsuleCollider2D playerColl;
    public Vector2 playerOffsetVector;
    public Vector2 playerSizeVector;

    public AudioClip switchSound;  // 用于关联要播放的音效文件
    private AudioSource audioSource;  // 音频源组件，用于播放音效

    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        playerColl = GetComponent<CapsuleCollider2D>();
        playerAnim = GetComponent<Animator>();
        playerOffsetVector = new Vector2(playerColl.offset.x, playerColl.offset.y);
        playerSizeVector = new Vector2(playerColl.size.x, playerColl.size.y);
        audioSource = gameObject.AddComponent<AudioSource>();
        playerAnim.SetBool("hurt", false);
        audioSource.clip = switchSound;
    }

    void Update()
    {
        UpdateCheck();
        AnimSwitch();
    }

    private void FixedUpdate()
    {
        FixedUpdateCheck();
        playerMove();
        playerJump();
    }

    void playerMove()
    {
        float horizontalNum = Input.GetAxis("Horizontal2");
        float faceNum = Input.GetAxisRaw("Horizontal2");

        // 模拟地面摩擦力：无输入时直接重置水平速度
        if (isground && Mathf.Abs(horizontalNum) < 0.01f)
        {
            playerRB.velocity = new Vector2(
                Mathf.Lerp(playerRB.velocity.x, 0, Time.fixedDeltaTime * 10), // 10为摩擦系数（可调整）
                playerRB.velocity.y
            );
            playerAnim.SetFloat("run", 0);
            return; // 直接返回，不再执行后续移动逻辑
        }

        playerRB.velocity = new Vector2(playerMoveSpeed * horizontalNum, playerRB.velocity.y);
        playerAnim.SetFloat("run", Mathf.Abs(playerMoveSpeed * horizontalNum));

        if (faceNum != 0)
        {
            transform.localScale = new Vector3(faceNum * 1.3f, transform.localScale.y, transform.localScale.z);
        }
    }

    void playerJump()
    {
        if (isground)
        {
            playerjumpCount = 2;
        }
        if (pressjump && isground)
        {
            pressjump = false;
            playerRB.velocity = new Vector2(playerRB.velocity.x, playerJumpSpeed * 1);
            playerjumpCount--;
        }
        else if (pressjump && playerjumpCount > 0 && !isground)
        {
            pressjump = false;
            playerRB.velocity = new Vector2(playerRB.velocity.x, playerJumpSpeed * 1);
            playerjumpCount--;
        }
    }

    void FixedUpdateCheck()
    {
        isground = Physics2D.OverlapCircle(foot.position, 0.1f, ground);
    }

    void UpdateCheck()
    {
        if (Input.GetButtonDown("Jump2"))
        {
            pressjump = true;
        }
    }

    void AnimSwitch()
    {
        if (isground)
        {
            playerAnim.SetBool("jump", false);
        }
        if (!isground && playerRB.velocity.y != 0)
        {
            playerAnim.SetBool("jump", true);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "enemy")
        {
            playerAnim.SetTrigger("hurt");
            audioSource.Play();
            StartCoroutine(LoadSceneAfterSound("Boss"));
        }
        else if (collision.gameObject.tag == "shadowWM")
        {
            playerAnim.SetTrigger("hurt");
            audioSource.Play();
            StartCoroutine(LoadSceneAfterSound("Boss"));
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "enemy")
        {
            playerAnim.SetTrigger("hurt");
            audioSource.Play();
            StartCoroutine(LoadSceneAfterSound("Boss"));
        }
    }

    private System.Collections.IEnumerator LoadSceneAfterSound(string sceneName)
    {
        float soundLength = switchSound.length;
        yield return new WaitForSeconds(soundLength);
        SceneManager.LoadScene("Boss");
    }
}
