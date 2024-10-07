using RG.Parsecs.Survival;
using RG.SecondsRemaster.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Core;

[CreateAssetMenu(menuName = "60 Seconds Remaster!/Crafting/New Remedium", fileName = "New Remedium")]
public class SecondsRemedium : Remedium
{
	[SerializeField]
	private SecondsRemediumStaticData _secondsRemediumStaticData;

	[SerializeField]
	private SecondsRemediumRuntimeData _secondsRemediumRuntimeData;

	[SerializeField]
	private IconSizeDefinition _iconSizeDefinition;

	public SecondsRemediumStaticData SecondsRemediumStaticData => _secondsRemediumStaticData;

	public SecondsRemediumRuntimeData SecondsRemediumRuntimeData => _secondsRemediumRuntimeData;

	public IconSizeDefinition IconSizeDefinition => _iconSizeDefinition;

	public override void CreateNewRuntimeData()
	{
		base.CreateNewRuntimeData();
		_secondsRemediumRuntimeData = new SecondsRemediumRuntimeData();
	}

	public override bool IsUpgradable()
	{
		return false;
	}

	public override void Upgrade()
	{
		throw new UnityException("Cannot use Upgrade method in SecondsRemedium object");
	}

	public override void Downgrade()
	{
		throw new UnityException("Cannot use Downgrade method in SecondsRemedium");
	}

	public override void InitializeWithDefaultData()
	{
		base.InitializeWithDefaultData();
		_secondsRemediumRuntimeData.IsDamaged = _secondsRemediumStaticData.IsDamaged;
	}

	public override void Add()
	{
		if (!BaseRuntimeData.IsAvailable || _secondsRemediumRuntimeData.IsDamaged)
		{
			InitializeWithDefaultData();
		}
		BaseRuntimeData.IsAvailable = true;
	}

	public virtual void SetDamage()
	{
		_secondsRemediumRuntimeData.IsDamaged = true;
	}

	public virtual void Repair()
	{
		_secondsRemediumRuntimeData.IsDamaged = false;
	}

	public override void Use()
	{
		if (!SecondsRemediumRuntimeData.IsDamaged)
		{
			_secondsRemediumRuntimeData.IsDamaged = true;
		}
	}

	public override bool IsDamaged()
	{
		return SecondsRemediumRuntimeData.IsDamaged;
	}

	public override bool IsLockable()
	{
		if (base.IsLockable())
		{
			return !IsDamaged();
		}
		return false;
	}

	protected override string InnerSerialize()
	{
		return JsonUtility.ToJson(new SecondsItemWrapper
		{
			IsAvailable = RuntimeData.IsAvailable,
			IsDamage = _secondsRemediumRuntimeData.IsDamaged,
			Level = RuntimeData.Level,
			Lock = RuntimeData.Lock,
			OnExpedition = RuntimeData.IsOnExpedition,
			IsEnable = RuntimeData.IsEnabled
		});
	}

	protected override void InnerDeserialize(string jsonData)
	{
		SecondsItemWrapper secondsItemWrapper = JsonUtility.FromJson<SecondsItemWrapper>(jsonData);
		RuntimeData.IsAvailable = secondsItemWrapper.IsAvailable;
		RuntimeData.Level = secondsItemWrapper.Level;
		RuntimeData.Lock = secondsItemWrapper.Lock;
		RuntimeData.IsOnExpedition = secondsItemWrapper.OnExpedition;
		_secondsRemediumRuntimeData.IsDamaged = secondsItemWrapper.IsDamage;
		RuntimeData.IsEnabled = secondsItemWrapper.IsEnable;
	}
}
