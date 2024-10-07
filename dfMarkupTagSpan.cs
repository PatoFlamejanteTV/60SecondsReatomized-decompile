using System.Collections.Generic;
using System.Text;

[dfMarkupTagInfo("span")]
public class dfMarkupTagSpan : dfMarkupTag
{
	private static Queue<dfMarkupTagSpan> objectPool = new Queue<dfMarkupTagSpan>();

	public dfMarkupTagSpan()
		: base("span")
	{
	}

	public dfMarkupTagSpan(dfMarkupTag original)
		: base(original)
	{
	}

	protected override void _PerformLayoutImpl(dfMarkupBox container, dfMarkupStyle style)
	{
		style = applyTextStyleAttributes(style);
		dfMarkupBox dfMarkupBox2 = container;
		dfMarkupAttribute dfMarkupAttribute2 = findAttribute("margin");
		if (dfMarkupAttribute2 != null)
		{
			dfMarkupBox2 = new dfMarkupBox(this, dfMarkupDisplayType.inlineBlock, style);
			dfMarkupBox2.Margins = dfMarkupBorders.Parse(dfMarkupAttribute2.Value);
			dfMarkupBox2.Margins.top = 0;
			dfMarkupBox2.Margins.bottom = 0;
			container.AddChild(dfMarkupBox2);
		}
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < base.ChildNodes.Count; i++)
		{
			dfMarkupElement dfMarkupElement2 = base.ChildNodes[i];
			if (dfMarkupElement2 is dfMarkupString)
			{
				dfMarkupString dfMarkupString2 = dfMarkupElement2 as dfMarkupString;
				if (dfMarkupString2.Text == "\n")
				{
					if (style.PreserveWhitespace)
					{
						dfMarkupBox2.AddLineBreak();
					}
					continue;
				}
				if (base.Owner.ForceWordwrap && base.Owner.Font.MeasureText(dfMarkupString2.Text, base.Owner.FontSize, base.Owner.FontStyle).x >= (float)dfMarkupBox2.Width)
				{
					stringBuilder.Remove(0, stringBuilder.Length);
					stringBuilder.Append(dfMarkupString2.Text);
					InsertChildNode(new dfMarkupString(string.Empty), i + 1);
					dfMarkupString dfMarkupString3 = base.ChildNodes[i + 1] as dfMarkupString;
					bool flag = true;
					int num = -1;
					int num2 = 10;
					int num3 = num2;
					while (flag && num3 > 1)
					{
						num = (int)((float)dfMarkupString2.Text.Length * ((float)(num3 - 1) / (float)num2));
						dfMarkupString3.Text = stringBuilder.ToString(num, stringBuilder.Length - num);
						dfMarkupString2.Text = stringBuilder.ToString(0, num);
						flag = base.Owner.Font.MeasureText(dfMarkupString2.Text, base.Owner.FontSize, base.Owner.FontStyle).x >= (float)dfMarkupBox2.Width;
						if (!flag)
						{
							dfMarkupString2.Text += "\n";
						}
						num3--;
					}
				}
			}
			dfMarkupElement2.PerformLayout(dfMarkupBox2, style);
		}
	}

	internal static dfMarkupTagSpan Obtain()
	{
		if (objectPool.Count > 0)
		{
			return objectPool.Dequeue();
		}
		return new dfMarkupTagSpan();
	}

	internal override void Release()
	{
		base.Release();
		objectPool.Enqueue(this);
	}
}
