using System.Collections.Generic;

public abstract class dfMarkupElement
{
	public dfMarkupElement Parent { get; protected set; }

	protected List<dfMarkupElement> ChildNodes { get; private set; }

	public dfMarkupElement()
	{
		ChildNodes = new List<dfMarkupElement>();
	}

	public void InsertChildNode(dfMarkupElement node, int index)
	{
		node.Parent = this;
		if (index >= 0)
		{
			if (ChildNodes.Count < index)
			{
				ChildNodes.Insert(index, node);
			}
			else
			{
				ChildNodes.Add(node);
			}
		}
	}

	public void AddChildNode(dfMarkupElement node)
	{
		node.Parent = this;
		ChildNodes.Add(node);
	}

	public void PerformLayout(dfMarkupBox container, dfMarkupStyle style)
	{
		_PerformLayoutImpl(container, style);
	}

	internal virtual void Release()
	{
		Parent = null;
		ChildNodes.Clear();
	}

	protected abstract void _PerformLayoutImpl(dfMarkupBox container, dfMarkupStyle style);
}
