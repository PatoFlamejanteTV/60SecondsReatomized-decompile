[dfMarkupTagInfo("a")]
public class dfMarkupTagAnchor : dfMarkupTag
{
	public string HRef
	{
		get
		{
			dfMarkupAttribute dfMarkupAttribute2 = findAttribute("href");
			if (dfMarkupAttribute2 == null)
			{
				return "";
			}
			return dfMarkupAttribute2.Value;
		}
	}

	public dfMarkupTagAnchor()
		: base("a")
	{
	}

	public dfMarkupTagAnchor(dfMarkupTag original)
		: base(original)
	{
	}

	protected override void _PerformLayoutImpl(dfMarkupBox container, dfMarkupStyle style)
	{
		style.TextDecoration = dfMarkupTextDecoration.Underline;
		style = applyTextStyleAttributes(style);
		for (int i = 0; i < base.ChildNodes.Count; i++)
		{
			dfMarkupElement dfMarkupElement2 = base.ChildNodes[i];
			if (dfMarkupElement2 is dfMarkupString && (dfMarkupElement2 as dfMarkupString).Text == "\n")
			{
				if (style.PreserveWhitespace)
				{
					container.AddLineBreak();
				}
			}
			else
			{
				dfMarkupElement2.PerformLayout(container, style);
			}
		}
	}
}
