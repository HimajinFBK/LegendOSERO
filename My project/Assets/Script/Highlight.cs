
using UnityEngine;

public class Highlight : MonoBehaviour
{

    // 通常時のオブジェクトの色
    [SerializeField]
    private Color normalColor;

    // マウスオーバー時のオブジェクトの色
    [SerializeField]
    private Color mouseOverColor;

    // オブジェクトのマテリアル情報を保持
    private Material material;

    // スクリプトが初めて実行されたときに呼ばれるメソッド
    private void Start()
    {
        // このオブジェクトのMeshRendererからマテリアルを取得し、保持
        material = GetComponent<MeshRenderer>().material;

        // オブジェクトの色を通常時の色に設定
        material.color = normalColor;
    }

    // マウスがオブジェクト上に乗ったときに呼ばれるメソッド
    private void OnMouseEnter()
    {
        // マウスオーバー時の色に変更
        material.color = mouseOverColor;
    }

    // マウスがオブジェクト上から離れたときに呼ばれるメソッド
    private void OnMouseExit()
    {
        // 通常時の色に戻す
        material.color = normalColor;
    }

    // オブジェクトが破棄されるときに呼ばれるメソッド
    private void OnDestroy()
    {
        // マテリアルを破棄し、メモリを解放（インスタンスごとのマテリアルを使用している場合）
        Destroy(material);
    }

}
