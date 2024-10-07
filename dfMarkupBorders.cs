using System.Text.RegularExpressions;

public struct dfMarkupBorders
{
	public int left;

	public int top;

	public int right;

	public int bottom;

	public int horizontal => left + right;

	public int vertical => top + bottom;

	public dfMarkupBorders(int left, int right, int top, int bottom)
	{
		this.left = left;
		this.top = top;
		this.right = right;
		this.bottom = bottom;
	}

	public static dfMarkupBorders Parse(string value)
	{
		dfMarkupBorders result = default(dfMarkupBorders);
		value = Regex.Replace(value, "\\s+", " ");
		string[] array = value.Split(' ');
		if (array.Length == 1)
		{
			int num = dfMarkupStyle.ParseSize(value, 0);
			result.left = (result.right = num);
			result.top = (result.bottom = num);
		}
		else if (array.Length == 2)
		{
			int num2 = dfMarkupStyle.ParseSize(array[0], 0);
			result.top = (result.bottom = num2);
			int num3 = dfMarkupStyle.ParseSize(array[1], 0);
			result.left = (result.right = num3);
		}
		else if (array.Length == 3)
		{
			int num4 = dfMarkupStyle.ParseSize(array[0], 0);
			result.top = num4;
			int num5 = dfMarkupStyle.ParseSize(array[1], 0);
			result.left = (result.right = num5);
			int num6 = dfMarkupStyle.ParseSize(array[2], 0);
			result.bottom = num6;
		}
		else if (array.Length == 4)
		{
			int num7 = dfMarkupStyle.ParseSize(array[0], 0);
			result.top = num7;
			int num8 = dfMarkupStyle.ParseSize(array[1], 0);
			result.right = num8;
			int num9 = dfMarkupStyle.ParseSize(array[2], 0);
			result.bottom = num9;
			int num10 = dfMarkupStyle.ParseSize(array[3], 0);
			result.left = num10;
		}
		return result;
	}

	public override string ToString()
	{
		return $"[T:{top},R:{right},L:{left},B:{bottom}]";
	}
}
