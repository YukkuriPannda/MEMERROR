using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// uGUI の Image.fillAmount で HP を表現する。
/// fill   : 現在 HP（即時更新）
/// red    : ダメージ量を赤で残し、ゆっくり fill に追いつく
///
/// ヒエラルキー構成（Tools > Create HP Bar で自動生成）:
///   HPBar_Canvas   Canvas (World Space)
///   └ HPBar        ← このスクリプト
///     ├ background Image (HPBar@2x.png)
///     ├ red        Image (赤, Filled / Horizontal)
///     └ fill       Image (HPBar_fill@2x.png, Filled / Horizontal)
/// </summary>
public class HPBar : MonoBehaviour
{
    [SerializeField] HPController hpController;
    [SerializeField] Image fillImage;
    [SerializeField] Image redImage;
    [SerializeField] float redLerpSpeed = 2f;

    void Awake()
    {
        if (hpController == null)
            hpController = GetComponentInParent<HPController>();
    }

    void OnEnable()
    {
        if (hpController != null)
            hpController.OnHPChanged += Refresh;
    }

    void OnDisable()
    {
        if (hpController != null)
            hpController.OnHPChanged -= Refresh;
    }

    void Start() => Refresh();

    void Update()
    {
        // 赤バーをゆっくり fill に追いつかせる
        if (redImage.fillAmount > fillImage.fillAmount)
            redImage.fillAmount = Mathf.Lerp(redImage.fillAmount, fillImage.fillAmount, redLerpSpeed * Time.deltaTime);
    }

    void Refresh()
    {
        fillImage.fillAmount = hpController.CurrentHP / hpController.MaxHP;
        // 回復時は赤バーも即時追従（赤が fill より小さくなるのは不自然なため）
        if (redImage.fillAmount < fillImage.fillAmount)
            redImage.fillAmount = fillImage.fillAmount;
    }
}
