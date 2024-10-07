using System;
using UnityEngine;

public class dfMarkupToken : IPoolable
{
	private static dfList<dfMarkupToken> pool = new dfList<dfMarkupToken>();

	private bool inUse;

	private string value;

	private dfList<dfMarkupTokenAttribute> attributes = new dfList<dfMarkupTokenAttribute>();

	public int AttributeCount => attributes.Count;

	public dfMarkupTokenType TokenType { get; private set; }

	public string Source { get; private set; }

	public int StartOffset { get; private set; }

	public int EndOffset { get; private set; }

	public int Width { get; internal set; }

	public int Height { get; set; }

	public int Length => EndOffset - StartOffset + 1;

	public string Value
	{
		get
		{
			if (value == null)
			{
				int length = Mathf.Min(EndOffset - StartOffset + 1, Source.Length - StartOffset);
				value = Source.Substring(StartOffset, length);
			}
			return value;
		}
	}

	public char this[int index]
	{
		get
		{
			if (index < 0 || index >= Length)
			{
				throw new IndexOutOfRangeException(string.Format("Index {0} is out of range ({2}:{1})", index, Length, Value));
			}
			return Source[StartOffset + index];
		}
	}

	protected dfMarkupToken()
	{
	}

	public static dfMarkupToken Obtain(string source, dfMarkupTokenType type, int startIndex, int endIndex)
	{
		dfMarkupToken obj = ((pool.Count > 0) ? pool.Pop() : new dfMarkupToken());
		obj.inUse = true;
		obj.Source = source;
		obj.TokenType = type;
		obj.StartOffset = startIndex;
		obj.EndOffset = Mathf.Min(source.Length - 1, endIndex);
		return obj;
	}

	public void Release()
	{
		if (inUse)
		{
			inUse = false;
			value = null;
			Source = null;
			TokenType = dfMarkupTokenType.Invalid;
			int width = (Height = 0);
			Width = width;
			width = (EndOffset = 0);
			StartOffset = width;
			attributes.ReleaseItems();
			pool.Add(this);
		}
	}

	internal bool Matches(dfMarkupToken other)
	{
		int length = Length;
		if (length != other.Length)
		{
			return false;
		}
		for (int i = 0; i < length; i++)
		{
			if (char.ToLower(Source[StartOffset + i]) != char.ToLower(other.Source[other.StartOffset + i]))
			{
				return false;
			}
		}
		return true;
	}

	internal bool Matches(string value)
	{
		int length = Length;
		if (length != value.Length)
		{
			return false;
		}
		for (int i = 0; i < length; i++)
		{
			if (char.ToLower(Source[StartOffset + i]) != char.ToLower(value[i]))
			{
				return false;
			}
		}
		return true;
	}

	internal void AddAttribute(dfMarkupToken key, dfMarkupToken value)
	{
		attributes.Add(dfMarkupTokenAttribute.Obtain(key, value));
	}

	public dfMarkupTokenAttribute GetAttribute(int index)
	{
		if (index < 0 || index >= attributes.Count)
		{
			throw new IndexOutOfRangeException("Invalid attribute index: " + index);
		}
		return attributes[index];
	}
}
