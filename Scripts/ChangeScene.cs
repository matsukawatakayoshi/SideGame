using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; //シーンの切り替えに必要SceneManagerクラスをしている名前空間
public class ChangeScene : MonoBehaviour
{

    public string sceneName; // 読み込むシーン名：目的となるシーン名を入れる変数

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
           
    }
    // シーンを読み込む
    public void Load()
    {
        SceneManager.LoadScene(sceneName);
    }
}
