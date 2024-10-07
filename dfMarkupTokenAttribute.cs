public class dfMarkupTokenAttribute : IPoolable
{
	public dfMarkupToken Key;

	public dfMarkupToken Value;

	private static dfList<dfMarkupTokenAttribute> pool = new dfList<dfMarkupTokenAttribute>();

	private dfMarkupTokenAttribute()
	{
	}

	public static dfMarkupTokenAttribute Obtain(dfMarkupToken key, dfMarkupToken value)
	{
		dfMarkupTokenAttribute obj = ((pool.Count > 0) ? pool.Pop() : new dfMarkupTokenAttribute());
		obj.Key = key;
		obj.Value = value;
		return obj;
	}

	public void Release()
	{
		if (Key != null)
		{
			Key.Release();
			Key = null;
		}
		if (Value != null)
		{
			Value.Release();
			Value = null;
		}
		if (!pool.Contains(this))
		{
			pool.Add(this);
		}
	}
}
