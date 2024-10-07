namespace RG.SecondsRemaster;

public static class RemasterMath
{
	public static int Modulo(int a, int b)
	{
		if (a < 0)
		{
			return b + a % b;
		}
		return a % b;
	}
}
