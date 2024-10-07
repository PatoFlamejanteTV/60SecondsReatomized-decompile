using System;

public static class Easing
{
	private static class Sine
	{
		public static float EaseIn(double s)
		{
			return (float)Math.Sin(s * 1.5707963705062866 - 1.5707963705062866) + 1f;
		}

		public static float EaseOut(double s)
		{
			return (float)Math.Sin(s * 1.5707963705062866);
		}

		public static float EaseInOut(double s)
		{
			return (float)(Math.Sin(s * 3.1415927410125732 - 1.5707963705062866) + 1.0) / 2f;
		}
	}

	private static class Power
	{
		public static float EaseIn(double s, int power)
		{
			return (float)Math.Pow(s, power);
		}

		public static float EaseOut(double s, int power)
		{
			int num = ((power % 2 != 0) ? 1 : (-1));
			return (float)((double)num * (Math.Pow(s - 1.0, power) + (double)num));
		}

		public static float EaseInOut(double s, int power)
		{
			s *= 2.0;
			if (s < 1.0)
			{
				return EaseIn(s, power) / 2f;
			}
			int num = ((power % 2 != 0) ? 1 : (-1));
			return (float)((double)num / 2.0 * (Math.Pow(s - 2.0, power) + (double)(num * 2)));
		}
	}

	public static float Ease(double linearStep, float acceleration, EasingType type)
	{
		float num = ((acceleration > 0f) ? EaseIn(linearStep, type) : ((acceleration < 0f) ? EaseOut(linearStep, type) : ((float)linearStep)));
		return MathHelper.Lerp(linearStep, num, Math.Abs(acceleration));
	}

	public static float EaseIn(double linearStep, EasingType type)
	{
		return type switch
		{
			EasingType.Step => (!(linearStep < 0.5)) ? 1 : 0, 
			EasingType.Linear => (float)linearStep, 
			EasingType.Sine => Sine.EaseIn(linearStep), 
			EasingType.Quadratic => Power.EaseIn(linearStep, 2), 
			EasingType.Cubic => Power.EaseIn(linearStep, 3), 
			EasingType.Quartic => Power.EaseIn(linearStep, 4), 
			EasingType.Quintic => Power.EaseIn(linearStep, 5), 
			_ => throw new NotImplementedException(), 
		};
	}

	public static float EaseOut(double linearStep, EasingType type)
	{
		return type switch
		{
			EasingType.Step => (!(linearStep < 0.5)) ? 1 : 0, 
			EasingType.Linear => (float)linearStep, 
			EasingType.Sine => Sine.EaseOut(linearStep), 
			EasingType.Quadratic => Power.EaseOut(linearStep, 2), 
			EasingType.Cubic => Power.EaseOut(linearStep, 3), 
			EasingType.Quartic => Power.EaseOut(linearStep, 4), 
			EasingType.Quintic => Power.EaseOut(linearStep, 5), 
			_ => throw new NotImplementedException(), 
		};
	}

	public static float EaseInOut(double linearStep, EasingType easeInType, EasingType easeOutType)
	{
		if (!(linearStep < 0.5))
		{
			return EaseInOut(linearStep, easeOutType);
		}
		return EaseInOut(linearStep, easeInType);
	}

	public static float EaseInOut(double linearStep, EasingType type)
	{
		return type switch
		{
			EasingType.Step => (!(linearStep < 0.5)) ? 1 : 0, 
			EasingType.Linear => (float)linearStep, 
			EasingType.Sine => Sine.EaseInOut(linearStep), 
			EasingType.Quadratic => Power.EaseInOut(linearStep, 2), 
			EasingType.Cubic => Power.EaseInOut(linearStep, 3), 
			EasingType.Quartic => Power.EaseInOut(linearStep, 4), 
			EasingType.Quintic => Power.EaseInOut(linearStep, 5), 
			_ => throw new NotImplementedException(), 
		};
	}
}
