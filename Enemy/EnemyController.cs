using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 3.0f; //移動速度
    public bool isToRight = false; //trueなら右向き
    public float revTime = 0; //反転するまでのインターバル　※0なら何かにぶつかるまで反転なし
    public LayerMask groundLayer; //地面レイヤー

    float time = 0; //反転までの経過時間計測用

    // Start is called before the first frame update
    void Start()
    {
        //右向きスタートにしていたら
        if (isToRight)
        {
            transform.localScale = new Vector2(-1, 1); //絵を反転（元が左向きの絵）
        }
    }

    // Update is called once per frame
    void Update()
    {
        //もしも反転時間が最初から設定されていた場合
        if (revTime > 0)
        {
            time += Time.deltaTime; //経過時間
            //経過時間が反転時間を上回ったら
            if (time > revTime)
            {
                isToRight = !isToRight; //向きのフラグを反転
                time = 0; //経過時間をリセット

                scaleSelect(); //向きフラグによって見た目の向き（スケール）もを変更

            }
        }
    }

    void FixedUpdate()
    {
        //CircleCastで地上判定
        bool onGround = Physics2D.CircleCast(
            transform.position, //敵の位置から
            0.5f, //0.5の半径の円を
            Vector2.down, //下方向に向けて
            0.5f, //0.5の距離飛ばす
            groundLayer //Groundレイヤーがあるか検知
            );

        //地上にいるならば
        if (onGround)
        {
            //Rigidbody2Dを使えるように準備
            Rigidbody2D rbody = GetComponent<Rigidbody2D>();

            //右向きの時の加速
            if (isToRight)
            {
                rbody.velocity = new Vector2(speed, rbody.velocity.y);
            }
            else //左向きの時の加速
            {
                rbody.velocity = new Vector2(-speed, rbody.velocity.y);
            }

        }
    }

    //何かと接触した時は反転
    private void OnTriggerEnter2D(Collider2D collision)
    {
        isToRight = !isToRight; //向きフラグを反転
        time = 0; //反転までの計測時間もリセット

        scaleSelect();//向きフラグによって見た目の向き（スケール）もを変更
    }

    //向きフラグに応じて見た目（スケール）を決定
    void scaleSelect()
    {
        //その時々の向きに応じて絵も反転
        if (isToRight) //右向き
        {
            transform.localScale = new Vector2(-1, 1);//絵を反転（元が左向きの絵）
        }
        else //左向き
        {
            transform.localScale = new Vector2(1, 1);//絵を元の左向き
        }
    }

    //EnemyのCircleCastの位置をギズモで視覚化
    void OnDrawGizmosSelected()
    {
        // 円を描画
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z), 0.5f);
    }
}