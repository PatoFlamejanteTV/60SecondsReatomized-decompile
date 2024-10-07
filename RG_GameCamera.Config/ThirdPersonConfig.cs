using System.Collections.Generic;
using UnityEngine;

namespace RG_GameCamera.Config;

public class ThirdPersonConfig : Config
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
				"Distance",
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
				"RotationYMax",
				new RangeParam
				{
					value = 80f,
					min = 0f,
					max = 80f
				}
			},
			{
				"RotationYMin",
				new RangeParam
				{
					value = -80f,
					min = -80f,
					max = 0f
				}
			},
			{
				"TargetOffset",
				new Vector3Param
				{
					value = Vector3.zero
				}
			},
			{
				"FollowCoef",
				new RangeParam
				{
					value = 1f,
					min = 0f,
					max = 10f
				}
			},
			{
				"Spring",
				new RangeParam
				{
					value = 0f,
					min = 0f,
					max = 1f
				}
			},
			{
				"DefaultYRotation",
				new RangeParam
				{
					value = 0f,
					min = -80f,
					max = 80f
				}
			},
			{
				"DefaultXRotation",
				new RangeParam
				{
					value = 0f,
					min = -180f,
					max = 180f
				}
			},
			{
				"AutoYRotation",
				new RangeParam
				{
					value = 0f,
					min = 0f,
					max = 1f
				}
			},
			{
				"DeadZone",
				new Vector2Param
				{
					value = Vector2.zero
				}
			},
			{
				"Orthographic",
				new BoolParam
				{
					value = false
				}
			},
			{
				"FreeRotate",
				new BoolParam
				{
					value = false
				}
			}
		};
		Dictionary<string, Param> value2 = new Dictionary<string, Param>
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
				"Distance",
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
				"RotationYMax",
				new RangeParam
				{
					value = 80f,
					min = 0f,
					max = 80f
				}
			},
			{
				"RotationYMin",
				new RangeParam
				{
					value = -80f,
					min = -80f,
					max = 0f
				}
			},
			{
				"TargetOffset",
				new Vector3Param
				{
					value = Vector3.zero
				}
			},
			{
				"FollowCoef",
				new RangeParam
				{
					value = 1f,
					min = 0f,
					max = 10f
				}
			},
			{
				"Spring",
				new RangeParam
				{
					value = 0f,
					min = 0f,
					max = 1f
				}
			},
			{
				"DefaultYRotation",
				new RangeParam
				{
					value = 0f,
					min = -80f,
					max = 80f
				}
			},
			{
				"AutoYRotation",
				new RangeParam
				{
					value = 0f,
					min = 0f,
					max = 1f
				}
			},
			{
				"DeadZone",
				new Vector2Param
				{
					value = Vector2.zero
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
		Dictionary<string, Param> value3 = new Dictionary<string, Param>
		{
			{
				"FOV",
				new RangeParam
				{
					value = 40f,
					min = 20f,
					max = 100f
				}
			},
			{
				"Distance",
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
				"RotationYMax",
				new RangeParam
				{
					value = 80f,
					min = 0f,
					max = 80f
				}
			},
			{
				"RotationYMin",
				new RangeParam
				{
					value = -80f,
					min = -80f,
					max = 0f
				}
			},
			{
				"TargetOffset",
				new Vector3Param
				{
					value = Vector3.zero
				}
			},
			{
				"FollowCoef",
				new RangeParam
				{
					value = 0f,
					min = 0f,
					max = 10f
				}
			},
			{
				"Spring",
				new RangeParam
				{
					value = 0f,
					min = 0f,
					max = 1f
				}
			},
			{
				"DefaultYRotation",
				new RangeParam
				{
					value = 0f,
					min = -80f,
					max = 80f
				}
			},
			{
				"AutoYRotation",
				new RangeParam
				{
					value = 0f,
					min = 0f,
					max = 1f
				}
			},
			{
				"DeadZone",
				new Vector2Param
				{
					value = Vector2.zero
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
		Dictionary<string, Param> value4 = new Dictionary<string, Param>
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
				"Distance",
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
				"RotationYMax",
				new RangeParam
				{
					value = 80f,
					min = 0f,
					max = 80f
				}
			},
			{
				"RotationYMin",
				new RangeParam
				{
					value = -80f,
					min = -80f,
					max = 0f
				}
			},
			{
				"TargetOffset",
				new Vector3Param
				{
					value = Vector3.zero
				}
			},
			{
				"FollowCoef",
				new RangeParam
				{
					value = 1f,
					min = 0f,
					max = 10f
				}
			},
			{
				"Spring",
				new RangeParam
				{
					value = 0f,
					min = 0f,
					max = 1f
				}
			},
			{
				"DefaultYRotation",
				new RangeParam
				{
					value = 0f,
					min = -80f,
					max = 80f
				}
			},
			{
				"AutoYRotation",
				new RangeParam
				{
					value = 0f,
					min = 0f,
					max = 1f
				}
			},
			{
				"DeadZone",
				new Vector2Param
				{
					value = Vector2.zero
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
		Params = new Dictionary<string, Dictionary<string, Param>>
		{
			{ "Default", value },
			{ "Crouch", value2 },
			{ "Aim", value3 },
			{ "Sprint", value4 }
		};
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
