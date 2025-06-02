using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using Unity.VisualScripting.Antlr3.Runtime;

public class action1 : MonoBehaviour
{
    [Header("速度相关")]
    public float playerMoveSpeed;
    public float playerJumpSpeed;
    [Header("跳跃次数")]
    public float playerjumpCount;
    [Header("倍率相关")]
    public float jumpMultiplier = 1;
    [Header("时间相关")]
    public float crouchTime;
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
        playerCrouch();
    }

    void playerMove()
    {
        float horizontalNum = Input.GetAxis("Horizontal");
        float faceNum = Input.GetAxisRaw("Horizontal");

        // 模拟地面摩擦力：无输入时直接重置水平速度
        if (isground && Mathf.Abs(horizontalNum) < 0.01f)
        {
            playerRB.velocity = new Vector2(0, playerRB.velocity.y);
            playerAnim.SetFloat("run", 0);
            return; // 直接返回，不再执行后续移动逻辑
        }

        playerRB.velocity = new Vector2(playerMoveSpeed * horizontalNum, playerRB.velocity.y);
        playerAnim.SetFloat("run", Mathf.Abs(playerMoveSpeed * horizontalNum));

        if (faceNum != 0)
        {
            transform.localScale = new Vector3(faceNum * 1.5f, transform.localScale.y, transform.localScale.z);
        }
    }

    void playerJump()
    {
        if(isground)
        {
            playerjumpCount = 2;
        }
        if (pressjump && isground)
        {
            pressjump = false;
            playerRB.velocity = new Vector2(playerRB.velocity.x, playerJumpSpeed * jumpMultiplier);
            playerjumpCount--;
        }
        else if (pressjump && playerjumpCount > 0 && !isground)
        {
            pressjump = false;
            playerRB.velocity = new Vector2(playerRB.velocity.x, playerJumpSpeed * jumpMultiplier);
            playerjumpCount--;
        }
    }

    void playerCrouch()
    {
        if (pressedCrouch && isground)
        {
            iscrouch = true;
            playerColl.size = new Vector2(playerSizeVector.x, playerSizeVector.y * 0.7f);
            playerColl.offset = new Vector2(playerOffsetVector.x, playerOffsetVector.y * 0.7f);
            playerMoveSpeed = 3;
        }
        else
        {
            iscrouch = false;
            playerColl.size = new Vector2(playerSizeVector.x, playerSizeVector.y);
            playerColl.offset = new Vector2(playerOffsetVector.x, playerOffsetVector.y);
            playerMoveSpeed = 7;
        }
        if (iscrouch)
        {
            crouchTime += Time.deltaTime;
            if (crouchTime >= 2)
            {
                crouchTime = 2;
            }
            jumpMultiplier = 1 + crouchTime * 0.25f;
        }
        else
        {
            crouchTime = 0;
            jumpMultiplier = 1;
        }
    }
    void FixedUpdateCheck()
    {
        isground = Physics2D.OverlapCircle(foot.position, 0.1f, ground);
    }

    void UpdateCheck()
    {
        if (Input.GetButton("Crouch"))
        {
            pressedCrouch = true;
        }
        else
        {
            pressedCrouch = false;
        }
        if (Input.GetButtonDown("Jump"))
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
        if (iscrouch)
        {
            playerAnim.SetBool("crouch", true);
        }
        else
        {
            playerAnim.SetBool("crouch", false);
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

