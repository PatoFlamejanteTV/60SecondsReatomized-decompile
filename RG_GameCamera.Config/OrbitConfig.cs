using System.Collections.Generic;

namespace RG_GameCamera.Config;

public class OrbitConfig : Config
{
	public override void LoadDefault()
	{
		Dictionary<string, Param> value = new Dictionary<string, Param>
		{
			{
				"FOV",
				new RangeParam
				{
					value = 60f,
					min = 20f,
					max = 100f
				}
			},
			{
				"ZoomSpeed",
				new RangeParam
				{
					value = 2f,
					min = 0f,
					max = 10f
				}
			},
			{
				"RotationSpeedX",
				new RangeParam
				{
					value = 8f,
					min = 0f,
					max = 10f
				}
			},
			{
				"RotationSpeedY",
				new RangeParam
				{
					value = 5f,
					min = 0f,
					max = 10f
				}
			},
			{
				"PanSpeed",
				new RangeParam
				{
					value = 1f,
					min = 0f,
					max = 10f
				}
			},
			{
				"RotationYMax",
				new RangeParam
				{
					value = 90f,
					min = 0f,
					max = 90f
				}
			},
			{
				"RotationYMin",
				new RangeParam
				{
					value = -90f,
					min = -90f,
					max = 0f
				}
			},
			{
				"DisablePan",
				new BoolParam
				{
					value = false
				}
			},
			{
				"DisableZoom",
				new BoolParam
				{
					value = false
				}
			},
			{
				"DisableRotation",
				new BoolParam
				{
					value = false
				}
			},
			{
				"TargetInterpolation",
				new RangeParam
				{
					value = 0.5f,
					min = 0f,
					max = 1f
				}
			},
			{
				"Orthographic",
				new BoolParam
				{
					value = false
				}
			}
		};
		Params = new Dictionary<string, Dictionary<string, Param>> { { "Default", value } };
		Transitions = new Dictionary<string, float>();
		foreach (KeyValuePair<string, Dictionary<string, Param>> param in Params)
		{
			Transitions.Add(param.Key, 0.25f);
		}
		Deserialize(base.DefaultConfigPath);
		base.LoadDefault();
	}

	protected override void Awake()
	{
		base.Awake();
		LoadDefault();
	}
}
