using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class APGaugePresenter : MonoBehaviour
{
	int requiredAP = 3;
	[SerializeField] Slider gauge;
	[SerializeField] Text textValue;
	[SerializeField] Text textTimer;
	[SerializeField] Button button;

	void Start()
	{
		var ap = new AP();

		// ゲージのMAX値を設定
		this.gauge.maxValue = ap.MaxAP;
		// APの値をゲージとラベルに反映
		ap.CurrentAP.Subscribe(x =>
		{
			this.gauge.value = x;
			this.textValue.text = string.Format("{0} / {1}", x, ap.MaxAP);
		});

		// 回復時間をラベルに反映
		ap.RecoveryTimer.Subscribe(x => this.textTimer.text = x.ToString());
		// MAXの場合は回復時間を非表示
		ap.IsMax.Subscribe(x => this.textTimer.gameObject.SetActive(!x));

		// ボタンクリックでAP消費
		this.button.OnClickAsObservable()
			.Subscribe(_ =>
			{
				ap.CurrentAP.Value -= this.requiredAP;
			});
		// APが足りない場合はボタンが押せない
		ap.CurrentAP.Select(x => x >= this.requiredAP).SubscribeToInteractable(this.button);
	}
}
