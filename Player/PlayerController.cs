using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rbody; //RigidBody2D型の変数
    float axisH = 0.0f;//入力
    public float speed = 3.0f; //移動速度

    public float jump = 9.0f;       //ジャンプ力
    public LayerMask groundLayer;   //着地できるレイヤー(Ground)情報を格納する変数・LayerMask型
    bool goJump = false;            //ジャンプ開始フラグ

    //アニメーション対応
    Animator animator; //アニメーターコンポーネントの情報を格納したい変数
    public string stopAnime = "PlayerStop";
    public string moveAnime = "PlayerMove";
    public string jumpAnime = "PlayerJump";
    public string goalAnime = "PlayerGoal";
    public string deadAnime = "PlayerOver";

    //アニメが切り替わったかどうかの検査用
    string nowAnime = "";
    string oldAnime = "";


    public static string gameState = "playing";    // ゲームの状態

    public int score = 0;   // スコア

    //タッチ操作対応追加
    bool isMoving = false; //タッチ操作中かどうかのフラグ

    // Start is called before the first frame update

    void Start()
    {
        //Rigidbody2Dを取ってくる
        rbody = this.GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();//Animatorを撮ってくる
        nowAnime = stopAnime;//停止から開始する
        oldAnime = stopAnime;//停止から開始する

        gameState = "playing";   // ゲーム中にする
    }

    // Update is called once per frame
    
    void Update()
    {
        // ゲーム中でなければ
       if(gameState != "playing")
        {
            return;　//　何もしたくない。ここで止める。下の処理はしない(ゲーム中じゃないので）
        }
        //移動の改造
        if (isMoving == false)
        {
            //水平方向の入力をチェック
            axisH = Input.GetAxisRaw("Horizontal");
        }
        //向きの調整
        if (axisH > 0.0f)
        {
            //右移動
            //Debug.Log("右移動");
            transform.localScale = new Vector2(1, 1);//右向き
        }
        else if (axisH < 0.0f)
        {
            //左移動
            //Debug.Log("左移動");
            transform.localScale = new Vector2(-1, 1);//左右反転させる
        }

        //キャラクターをジャンプさせる（スペースキーが押されたかどうか）
        if(Input.GetButtonDown("Jump"))
        {
            Jump();//ジャンプメソッドの発動（クラスの一番下に表記）
        }
    }
   
    private void FixedUpdate()
    {   // ゲーム中でなければ
        if (gameState != "playing")
        {
            return; //　何もしたくない。ここで止める。下の処理はしない(ゲーム中じゃないので）
        }

        //地上判定(円のセンサーを設置して、特定のレイヤーに引っかかればtrue)
        bool onGround = Physics2D.CircleCast(
                                              transform.position,//発射位置→プレイヤーの位置（足元）
                                              0.2f,             //円の半径
                                              Vector2.down,     //発射方向<new Vector2(0,-1)>→Vector2.down
                                              0.0f,            //発射距離
                                              groundLayer    //検出するレイヤー
                                              );
        //地面にいる時はVelocityがすべてに反応
        //空中にいるときはVelocityが左右にだけ反応
        if (onGround || axisH != 0)
        {
            //地面の上or速度が０でない
            //速度を更新する
            rbody.velocity = new Vector2(axisH * speed, rbody.velocity.y);
        }
        if(onGround && goJump)
        {
            //地面の上でジャンプキーが押された
            //ジャンプさせる
            Vector2 jumpPw = new Vector2(0, jump);   //ジャンプさせるベクトルを作る
            rbody.AddForce(jumpPw, ForceMode2D.Impulse);//jumpPw変数・瞬間的な力を加える
            goJump = false;//ジャンプフラグを下ろす
        }

        //アニメーション更新
        if (onGround)
        {
            //地面の上
            if (axisH == 0)
            {
                nowAnime = stopAnime; //停止中
            }
            else
            {
                nowAnime = moveAnime; //移動
            }
        }
        else
        {
            //空中
            nowAnime = jumpAnime;
        }
        if (nowAnime != oldAnime)
        {
            oldAnime = nowAnime;
            animator.Play(nowAnime);//アニメーション再生
        }
    }

    //ジャンプ(自作メソッド・ジャンプフラグを立てるメソッド）
    public void Jump()
    {
        goJump = true; //ジャンプフラグを立てる
    }
    void OnDrawGizmos()
    {
        //円を描画
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.2f);
    }

    //接触開始
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Goal")
        {
            Goal();            //  ゴール！！
        }
        else if(collision.gameObject.tag == "Dead")
        {
            GameOver();       //ゲームオーバー
        }
        else if(collision.gameObject.tag == "ScoreItem")
        {
            // 相手であるスコアアイテム
            // のItemDatasクリプトを得る
            ItemData item = collision.gameObject.GetComponent<ItemData>();
            // 相手のItemDataスクリプトの変数valueの値（スコア）を得る
            score = item.value;

            // アイテムを削除する
            Destroy(collision.gameObject);
        }
    }
    //ゴール
    public void Goal()
    {
        animator.Play(goalAnime);

        gameState = "gameclear"; //ゲーム中でなくなるplayingでない
        GameStop(); // ゲーム停止
    }
    //ゲームオーバー
    public void GameOver()
    {
        animator.Play(deadAnime);

        gameState = "gameover"; // ゲーム中でなくなるplayingでない
        GameStop(); // ゲーム停止
        // ===============
        // ゲームオーバー演出
        // ===============
        // プレイヤーあたりを消す
        GetComponent<CapsuleCollider2D>().enabled = false;
        // プレイヤーを上に少し跳ね上げる演出
        rbody.AddForce(new Vector2(0, 5), ForceMode2D.Impulse); //仮オブジェクトを作る＞new Vector2（方向、ｙ座標）
    }
    // ゲーム停止
    void GameStop()
    {
        // Rigidbody2Dを取ってくる
        //Rigidbody2D rbody = GetComponent<Rigidbody2D>();
        // 速度を０にして強制停止
        rbody.velocity = new Vector2(0, 0);
    }

    //タッチスクリーン対応追加
    //第一引数のhは水平（横）、第二引数のvは垂直（縦）を担当
    public void SetAxis(float h, float v)
    {
        //パッドの水平(横)方向の値を引数から拾う
        axisH = h;

        //もしパッドの水平(横)方向の値が0なら
        if (axisH == 0)
        {
            //パッドの水平の力が0だと、Updateメソッドにおいてキーボード操作が反応可
            isMoving = false;
        }
        else
        {
            //パッドの水平の力が入っていると、Updateメソッドにおいてキーボード操作が反応しない
            isMoving = true;
        }
    }

}
