using System;
using UniRx;

public class AP
{
	int maxAP = 10;
	int recoveryInterval = 5;
	public int MaxAP { get { return this.maxAP; } }
	public ReactiveProperty<int> CurrentAP { get; private set; }
	public ReactiveProperty<int> RecoveryTimer { get; private set; }
	public ReadOnlyReactiveProperty<bool> IsMax { get; private set; }

	public AP()
	{
		// 値の初期化
		this.CurrentAP = new ReactiveProperty<int>(this.maxAP);
		this.RecoveryTimer = new ReactiveProperty<int>(this.recoveryInterval);
		this.IsMax = this.CurrentAP
			.Select(x => x >= this.maxAP)
			.DistinctUntilChanged()
			.ToReadOnlyReactiveProperty();
		// APが満タンじゃなくなったら回復処理を開始
		this.IsMax
			.Where(x => !x)
			.Subscribe(_ => this.startRecovery());
	}

	void startRecovery()
	{
		Observable.Timer(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1))
			.Select(x => (int) (this.recoveryInterval - x))
			.TakeWhile(x => x > 0)
			.Subscribe(
				x => this.RecoveryTimer.Value = x,
				() =>
				{
					this.CurrentAP.Value++;
					if (!this.IsMax.Value) this.startRecovery();
				});
	}
}
