using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class dfMarkupBoxText : dfMarkupBox
{
	private static int[] TRIANGLE_INDICES = new int[6] { 0, 1, 2, 0, 2, 3 };

	private static Queue<dfMarkupBoxText> objectPool = new Queue<dfMarkupBoxText>();

	private static Regex whitespacePattern = new Regex("\\s+");

	private dfRenderData renderData = new dfRenderData();

	private bool isWhitespace;

	public string Text { get; private set; }

	public bool IsWhitespace => isWhitespace;

	public dfMarkupBoxText(dfMarkupElement element, dfMarkupDisplayType display, dfMarkupStyle style)
		: base(element, display, style)
	{
	}

	public static dfMarkupBoxText Obtain(dfMarkupElement element, dfMarkupDisplayType display, dfMarkupStyle style)
	{
		if (objectPool.Count > 0)
		{
			dfMarkupBoxText obj = objectPool.Dequeue();
			obj.Element = element;
			obj.Display = display;
			obj.Style = style;
			obj.Position = Vector2.zero;
			obj.Size = Vector2.zero;
			obj.Baseline = (int)((float)style.FontSize * 1.1f);
			obj.Margins = default(dfMarkupBorders);
			obj.Padding = default(dfMarkupBorders);
			return obj;
		}
		return new dfMarkupBoxText(element, display, style);
	}

	public override void Release()
	{
		base.Release();
		Text = "";
		renderData.Clear();
		objectPool.Enqueue(this);
	}

	internal void SetText(string text)
	{
		Text = text;
		if (Style.Font == null)
		{
			return;
		}
		isWhitespace = whitespacePattern.IsMatch(Text);
		string text2 = ((Style.PreserveWhitespace || !isWhitespace) ? Text : " ");
		int fontSize = Style.FontSize;
		Vector2 size = new Vector2(0f, Style.LineHeight);
		Style.Font.RequestCharacters(text2, Style.FontSize, Style.FontStyle);
		CharacterInfo info = default(CharacterInfo);
		for (int i = 0; i < text2.Length; i++)
		{
			if (Style.Font.BaseFont.GetCharacterInfo(text2[i], out info, fontSize, Style.FontStyle))
			{
				float num = info.vert.x + info.vert.width;
				if (text2[i] == ' ')
				{
					num = Mathf.Max(num, (float)fontSize * 0.33f);
				}
				else if (text2[i] == '\t')
				{
					num += (float)(fontSize * 3);
				}
				size.x += num;
			}
		}
		Size = size;
		dfDynamicFont font = Style.Font;
		float num2 = (float)fontSize / (float)font.FontSize;
		Baseline = Mathf.CeilToInt((float)font.Baseline * num2);
	}

	protected override dfRenderData OnRebuildRenderData()
	{
		renderData.Clear();
		if (Style.Font == null)
		{
			return null;
		}
		if (Style.TextDecoration == dfMarkupTextDecoration.Underline)
		{
			renderUnderline();
		}
		renderText(Text);
		return renderData;
	}

	private void renderUnderline()
	{
	}

	private void renderText(string text)
	{
		dfDynamicFont font = Style.Font;
		int fontSize = Style.FontSize;
		FontStyle fontStyle = Style.FontStyle;
		CharacterInfo info = default(CharacterInfo);
		dfList<Vector3> vertices = renderData.Vertices;
		dfList<int> triangles = renderData.Triangles;
		dfList<Vector2> uV = renderData.UV;
		dfList<Color32> colors = renderData.Colors;
		float num = (float)fontSize / (float)font.FontSize;
		float num2 = (float)font.Descent * num;
		float num3 = 0f;
		font.RequestCharacters(text, fontSize, fontStyle);
		renderData.Material = font.Material;
		for (int i = 0; i < text.Length; i++)
		{
			if (font.BaseFont.GetCharacterInfo(text[i], out info, fontSize, fontStyle))
			{
				addTriangleIndices(vertices, triangles);
				float num4 = (float)font.FontSize + info.vert.y - (float)fontSize + num2;
				float num5 = num3 + info.vert.x;
				float num6 = num4;
				float x = num5 + info.vert.width;
				float y = num6 + info.vert.height;
				Vector3 item = new Vector3(num5, num6);
				Vector3 item2 = new Vector3(x, num6);
				Vector3 item3 = new Vector3(x, y);
				Vector3 item4 = new Vector3(num5, y);
				vertices.Add(item);
				vertices.Add(item2);
				vertices.Add(item3);
				vertices.Add(item4);
				Color color = Style.Color;
				colors.Add(color);
				colors.Add(color);
				colors.Add(color);
				colors.Add(color);
				Rect uv = info.uv;
				float x2 = uv.x;
				float y2 = uv.y + uv.height;
				float x3 = x2 + uv.width;
				float y3 = uv.y;
				if (info.flipped)
				{
					uV.Add(new Vector2(x3, y3));
					uV.Add(new Vector2(x3, y2));
					uV.Add(new Vector2(x2, y2));
					uV.Add(new Vector2(x2, y3));
				}
				else
				{
					uV.Add(new Vector2(x2, y2));
					uV.Add(new Vector2(x3, y2));
					uV.Add(new Vector2(x3, y3));
					uV.Add(new Vector2(x2, y3));
				}
				num3 += (float)Mathf.CeilToInt(info.vert.x + info.vert.width);
			}
		}
	}

	private static void addTriangleIndices(dfList<Vector3> verts, dfList<int> triangles)
	{
		int count = verts.Count;
		int[] tRIANGLE_INDICES = TRIANGLE_INDICES;
		for (int i = 0; i < tRIANGLE_INDICES.Length; i++)
		{
			triangles.Add(count + tRIANGLE_INDICES[i]);
		}
	}
}
