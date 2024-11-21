using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualPad : MonoBehaviour
{
    public float MaxLength = 70; //タブが動く最大距離
    public bool is4DPad = false; //上下左右に動かすフラグ
    GameObject player; //プレイヤーオブジェクト
    Vector2 defPos; //動かす前のパッドの位置
    Vector2 downPos; //タッチした位置


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        //タブの初期座標(親オブジェクトに対しての相対位置 0，0，0)
        defPos = GetComponent<RectTransform>().localPosition;
    }

    // Update is called once per frame
    void Update()
    {

    }

    //タップした瞬間のイベント用メソッドを自作
    public void PadDown()
    {
        //タップした場所のスクリーン上の座標
        downPos = Input.mousePosition;
    }

    //ドラッグした瞬間のイベント用メソッドを自作
    public void PadDrag()
    {
        //ドラッグした指のスクリーン上の座標
        Vector2 mousePosition = Input.mousePosition;
        //指のドラッグ状況にあわせて、パッドの位置を決める
        Vector2 newTabPos = mousePosition - downPos; //最初にタップした位置(downPos)からの差分※最初の位置からどれだけずらしたか

        //もし上下左右モードがfalseならば
        if (is4DPad == false)
        {
            newTabPos.y = 0; //上下には動かさないのでY軸に指定する予定の値はあらかじめ0にしておく
        }

        //移動ベクトルを計算する
        Vector2 axis = newTabPos.normalized; //座標を正規化※1に統一

        //パッドの初期値とズラした距離の差を求める
        float len = Vector2.Distance(defPos, newTabPos);

        //どれだけ指をずらしてもパッドの動きは最大値までに差し替えられる
        //※axisは方向だけを担っている
        if (len > MaxLength)
        {
            //値の差し替え
            newTabPos.x = axis.x * MaxLength;
            newTabPos.y = axis.y * MaxLength;
        }

        //実際のパッドを位置を決める
        GetComponent<RectTransform>().localPosition = newTabPos;

        //連動してプレイヤーキャラを動かす
        PlayerController plcnt = player.GetComponent<PlayerController>();
        plcnt.SetAxis(axis.x, axis.y);

    }

    //指を話した時のイベント
    public void PadUp()
    {
        //タブの位置の初期化
        GetComponent<RectTransform>().localPosition = defPos;

        //プレイヤーキャラクターを停止させる
        PlayerController plcnt = player.GetComponent<PlayerController>();
        plcnt.SetAxis(0, 0);
    }

}