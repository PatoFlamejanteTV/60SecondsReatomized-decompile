using System.Collections.Generic;
using RG_GameCamera.Utils;
using UnityEngine;

namespace RG_GameCamera.Effects;

internal class EffectManager : MonoBehaviour
{
	private static EffectManager instance;

	private List<Effect> effects;

	public static EffectManager Instance
	{
		get
		{
			if (!instance)
			{
				instance = CameraInstance.CreateInstance<EffectManager>("EffectManager");
			}
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
		effects = new List<Effect>();
	}

	public void Register(Effect effect)
	{
		if (effect != null)
		{
			effects.Add(effect);
		}
	}

	public void StopAll()
	{
		foreach (Effect effect in effects)
		{
			effect.Stop();
		}
	}

	public T Create<T>() where T : Effect
	{
		T val = base.gameObject.GetComponent<T>();
		if (!val)
		{
			val = base.gameObject.AddComponent<T>();
			if ((bool)val)
			{
				Register(val);
				val.Init();
			}
		}
		return val;
	}

	public Effect Create(Type effectType)
	{
		return effectType switch
		{
			Type.Earthquake => Create<Earthquake>(), 
			Type.Explosion => Create<Explosion>(), 
			Type.No => Create<No>(), 
			Type.FireKick => Create<FireKick>(), 
			Type.Stomp => Create<Stomp>(), 
			Type.Yes => Create<Yes>(), 
			Type.SprintShake => Create<SprintShake>(), 
			_ => null, 
		};
	}

	public void Delete(Effect effect)
	{
		if (effects.Contains(effect))
		{
			effects.Remove(effect);
		}
	}

	public void PostUpdate()
	{
		foreach (Effect effect in effects)
		{
			if (effect.Playing)
			{
				effect.PostUpdate();
			}
		}
	}

	private void OnGUI()
	{
	}
}
