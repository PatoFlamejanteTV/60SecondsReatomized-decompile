using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
[dfCategory("Basic Controls")]
[dfTooltip("Implements a scrollable control container")]
[dfHelp("http://www.daikonforge.com/docs/df-gui/classdf_scroll_panel.html")]
[ExecuteInEditMode]
[AddComponentMenu("Daikon Forge/User Interface/Containers/Scrollable Panel")]
public class dfScrollPanel : dfControl
{
	public enum LayoutDirection
	{
		Horizontal,
		Vertical
	}

	[SerializeField]
	protected dfAtlas atlas;

	[SerializeField]
	protected string backgroundSprite;

	[SerializeField]
	protected Color32 backgroundColor = UnityEngine.Color.white;

	[SerializeField]
	protected bool autoReset = true;

	[SerializeField]
	protected bool autoLayout;

	[SerializeField]
	protected RectOffset scrollPadding = new RectOffset();

	[SerializeField]
	protected RectOffset flowPadding = new RectOffset();

	[SerializeField]
	protected LayoutDirection flowDirection;

	[SerializeField]
	protected bool wrapLayout;

	[SerializeField]
	protected Vector2 scrollPosition = Vector2.zero;

	[SerializeField]
	protected int scrollWheelAmount = 10;

	[SerializeField]
	protected dfScrollbar horzScroll;

	[SerializeField]
	protected dfScrollbar vertScroll;

	[SerializeField]
	protected dfControlOrientation wheelDirection;

	[SerializeField]
	protected bool scrollWithArrowKeys;

	[SerializeField]
	protected bool useScrollMomentum;

	[SerializeField]
	protected bool useVirtualScrolling;

	[SerializeField]
	protected bool autoFitVirtualTiles = true;

	[SerializeField]
	protected dfControl virtualScrollingTile;

	private bool initialized;

	private bool resetNeeded;

	private bool scrolling;

	private bool isMouseDown;

	private Vector2 touchStartPosition = Vector2.zero;

	private Vector2 scrollMomentum = Vector2.zero;

	private object virtualScrollData;

	public bool UseScrollMomentum
	{
		get
		{
			return useScrollMomentum;
		}
		set
		{
			useScrollMomentum = value;
			scrollMomentum = Vector2.zero;
		}
	}

	public bool ScrollWithArrowKeys
	{
		get
		{
			return scrollWithArrowKeys;
		}
		set
		{
			scrollWithArrowKeys = value;
		}
	}

	public dfAtlas Atlas
	{
		get
		{
			if (atlas == null)
			{
				dfGUIManager manager = GetManager();
				if (manager != null)
				{
					return atlas = manager.DefaultAtlas;
				}
			}
			return atlas;
		}
		set
		{
			if (!dfAtlas.Equals(value, atlas))
			{
				atlas = value;
				Invalidate();
			}
		}
	}

	public string BackgroundSprite
	{
		get
		{
			return backgroundSprite;
		}
		set
		{
			if (value != backgroundSprite)
			{
				backgroundSprite = value;
				Invalidate();
			}
		}
	}

	public Color32 BackgroundColor
	{
		get
		{
			return backgroundColor;
		}
		set
		{
			if (!object.Equals(value, backgroundColor))
			{
				backgroundColor = value;
				Invalidate();
			}
		}
	}

	public bool AutoReset
	{
		get
		{
			return autoReset;
		}
		set
		{
			if (value != autoReset)
			{
				autoReset = value;
				Reset();
			}
		}
	}

	public RectOffset ScrollPadding
	{
		get
		{
			if (scrollPadding == null)
			{
				scrollPadding = new RectOffset();
			}
			return scrollPadding;
		}
		set
		{
			value = value.ConstrainPadding();
			if (!object.Equals(value, scrollPadding))
			{
				scrollPadding = value;
				if (AutoReset || AutoLayout)
				{
					Reset();
				}
			}
		}
	}

	public bool AutoLayout
	{
		get
		{
			return autoLayout;
		}
		set
		{
			if (value != autoLayout)
			{
				autoLayout = value;
				if (AutoReset || AutoLayout)
				{
					Reset();
				}
			}
		}
	}

	public bool WrapLayout
	{
		get
		{
			return wrapLayout;
		}
		set
		{
			if (value != wrapLayout)
			{
				wrapLayout = value;
				Reset();
			}
		}
	}

	public LayoutDirection FlowDirection
	{
		get
		{
			return flowDirection;
		}
		set
		{
			if (value != flowDirection)
			{
				flowDirection = value;
				Reset();
			}
		}
	}

	public RectOffset FlowPadding
	{
		get
		{
			if (flowPadding == null)
			{
				flowPadding = new RectOffset();
			}
			return flowPadding;
		}
		set
		{
			value = value.ConstrainPadding();
			if (!object.Equals(value, flowPadding))
			{
				flowPadding = value;
				Reset();
			}
		}
	}

	public Vector2 ScrollPosition
	{
		get
		{
			return scrollPosition;
		}
		set
		{
			Vector2 vector = calculateViewSize();
			Vector2 vector2 = new Vector2(size.x - (float)scrollPadding.horizontal, size.y - (float)scrollPadding.vertical);
			value = Vector2.Min(vector - vector2, value);
			value = Vector2.Max(Vector2.zero, value);
			value = value.RoundToInt();
			if ((value - scrollPosition).sqrMagnitude > float.Epsilon)
			{
				Vector2 vector3 = value - scrollPosition;
				scrollPosition = value;
				scrollChildControls(vector3);
				updateScrollbars();
			}
			OnScrollPositionChanged();
		}
	}

	public int ScrollWheelAmount
	{
		get
		{
			return scrollWheelAmount;
		}
		set
		{
			scrollWheelAmount = value;
		}
	}

	public dfScrollbar HorzScrollbar
	{
		get
		{
			return horzScroll;
		}
		set
		{
			horzScroll = value;
			updateScrollbars();
		}
	}

	public dfScrollbar VertScrollbar
	{
		get
		{
			return vertScroll;
		}
		set
		{
			vertScroll = value;
			updateScrollbars();
		}
	}

	public dfControlOrientation WheelScrollDirection
	{
		get
		{
			return wheelDirection;
		}
		set
		{
			wheelDirection = value;
		}
	}

	public bool UseVirtualScrolling
	{
		get
		{
			return useVirtualScrolling;
		}
		set
		{
			useVirtualScrolling = value;
			if (!value)
			{
				VirtualScrollingTile = null;
			}
		}
	}

	public bool AutoFitVirtualTiles
	{
		get
		{
			return autoFitVirtualTiles;
		}
		set
		{
			autoFitVirtualTiles = value;
		}
	}

	public dfControl VirtualScrollingTile
	{
		get
		{
			if (!useVirtualScrolling)
			{
				return null;
			}
			return virtualScrollingTile;
		}
		set
		{
			virtualScrollingTile = (useVirtualScrolling ? value : null);
		}
	}

	public override bool CanFocus
	{
		get
		{
			if (base.IsEnabled && base.IsVisible)
			{
				return true;
			}
			return base.CanFocus;
		}
	}

	public event PropertyChangedEventHandler<Vector2> ScrollPositionChanged;

	protected internal override RectOffset GetClipPadding()
	{
		return scrollPadding ?? dfRectOffsetExtensions.Empty;
	}

	protected internal override Plane[] GetClippingPlanes()
	{
		if (!base.ClipChildren)
		{
			return null;
		}
		Vector3[] corners = GetCorners();
		Vector3 vector = base.transform.TransformDirection(Vector3.right);
		Vector3 vector2 = base.transform.TransformDirection(Vector3.left);
		Vector3 vector3 = base.transform.TransformDirection(Vector3.up);
		Vector3 vector4 = base.transform.TransformDirection(Vector3.down);
		float num = PixelsToUnits();
		RectOffset rectOffset = ScrollPadding;
		corners[0] += vector * rectOffset.left * num + vector4 * rectOffset.top * num;
		corners[1] += vector2 * rectOffset.right * num + vector4 * rectOffset.top * num;
		corners[2] += vector * rectOffset.left * num + vector3 * rectOffset.bottom * num;
		return new Plane[4]
		{
			new Plane(vector, corners[0]),
			new Plane(vector2, corners[1]),
			new Plane(vector3, corners[2]),
			new Plane(vector4, corners[0])
		};
	}

	public override void OnDestroy()
	{
		if (horzScroll != null)
		{
			horzScroll.ValueChanged -= horzScroll_ValueChanged;
		}
		if (vertScroll != null)
		{
			vertScroll.ValueChanged -= vertScroll_ValueChanged;
		}
		horzScroll = null;
		vertScroll = null;
	}

	public override void Update()
	{
		base.Update();
		if (useScrollMomentum && !isMouseDown && scrollMomentum.magnitude > 0.25f)
		{
			ScrollPosition += scrollMomentum;
			scrollMomentum *= 0.95f - Time.deltaTime;
		}
		if (isControlInvalidated && autoLayout && base.IsVisible)
		{
			AutoArrange();
			updateScrollbars();
		}
	}

	public override void LateUpdate()
	{
		base.LateUpdate();
		initialize();
		if (resetNeeded && base.IsVisible)
		{
			resetNeeded = false;
			if (autoReset || autoLayout)
			{
				Reset();
			}
		}
	}

	public override void OnEnable()
	{
		base.OnEnable();
		if (size == Vector2.zero)
		{
			SuspendLayout();
			Camera camera = GetCamera();
			base.Size = new Vector3(camera.pixelWidth / 2, camera.pixelHeight / 2);
			ResumeLayout();
		}
		if (autoLayout)
		{
			AutoArrange();
		}
		updateScrollbars();
	}

	protected internal override void OnIsVisibleChanged()
	{
		base.OnIsVisibleChanged();
		if (base.IsVisible && (autoReset || autoLayout))
		{
			Reset();
			updateScrollbars();
		}
	}

	protected internal override void OnSizeChanged()
	{
		base.OnSizeChanged();
		if (autoReset || autoLayout)
		{
			Reset();
			return;
		}
		Vector2 lhs = calculateMinChildPosition();
		if (lhs.x > (float)scrollPadding.left || lhs.y > (float)scrollPadding.top)
		{
			lhs -= new Vector2(scrollPadding.left, scrollPadding.top);
			lhs = Vector2.Max(lhs, Vector2.zero);
			scrollChildControls(lhs);
		}
		updateScrollbars();
	}

	protected internal override void OnResolutionChanged(Vector2 previousResolution, Vector2 currentResolution)
	{
		base.OnResolutionChanged(previousResolution, currentResolution);
		resetNeeded = AutoLayout || AutoReset;
	}

	protected internal override void OnGotFocus(dfFocusEventArgs args)
	{
		if (args.Source != this)
		{
			ScrollIntoView(args.Source);
		}
		base.OnGotFocus(args);
	}

	protected internal override void OnKeyDown(dfKeyEventArgs args)
	{
		if (!scrollWithArrowKeys || args.Used)
		{
			base.OnKeyDown(args);
			return;
		}
		float num = ((horzScroll != null) ? horzScroll.IncrementAmount : 1f);
		float num2 = ((vertScroll != null) ? vertScroll.IncrementAmount : 1f);
		if (args.KeyCode == KeyCode.LeftArrow)
		{
			ScrollPosition += new Vector2(0f - num, 0f);
			args.Use();
		}
		else if (args.KeyCode == KeyCode.RightArrow)
		{
			ScrollPosition += new Vector2(num, 0f);
			args.Use();
		}
		else if (args.KeyCode == KeyCode.UpArrow)
		{
			ScrollPosition += new Vector2(0f, 0f - num2);
			args.Use();
		}
		else if (args.KeyCode == KeyCode.DownArrow)
		{
			ScrollPosition += new Vector2(0f, num2);
			args.Use();
		}
		base.OnKeyDown(args);
	}

	protected internal override void OnMouseEnter(dfMouseEventArgs args)
	{
		base.OnMouseEnter(args);
		touchStartPosition = args.Position;
	}

	protected internal override void OnMouseDown(dfMouseEventArgs args)
	{
		base.OnMouseDown(args);
		scrollMomentum = Vector2.zero;
		touchStartPosition = args.Position;
		isMouseDown = IsInteractive;
	}

	internal override void OnDragStart(dfDragEventArgs args)
	{
		base.OnDragStart(args);
		scrollMomentum = Vector2.zero;
		if (args.Used)
		{
			isMouseDown = false;
		}
	}

	protected internal override void OnMouseUp(dfMouseEventArgs args)
	{
		base.OnMouseUp(args);
		isMouseDown = false;
	}

	protected internal override void OnMouseMove(dfMouseEventArgs args)
	{
		if ((args is dfTouchEventArgs || isMouseDown) && !args.Used && (args.Position - touchStartPosition).magnitude > 5f)
		{
			Vector2 vector = args.MoveDelta.Scale(-1f, 1f);
			dfGUIManager manager = GetManager();
			Vector2 screenSize = manager.GetScreenSize();
			Camera renderCamera = manager.RenderCamera;
			vector.x = screenSize.x * (vector.x / (float)renderCamera.pixelWidth);
			vector.y = screenSize.y * (vector.y / (float)renderCamera.pixelHeight);
			ScrollPosition += vector;
			scrollMomentum = (scrollMomentum + vector) * 0.5f;
			args.Use();
		}
		base.OnMouseMove(args);
	}

	protected internal override void OnMouseWheel(dfMouseEventArgs args)
	{
		try
		{
			if (!args.Used)
			{
				float num = ((wheelDirection != 0) ? ((vertScroll != null) ? vertScroll.IncrementAmount : ((float)scrollWheelAmount)) : ((horzScroll != null) ? horzScroll.IncrementAmount : ((float)scrollWheelAmount)));
				if (wheelDirection == dfControlOrientation.Horizontal)
				{
					ScrollPosition = new Vector2(scrollPosition.x - num * args.WheelDelta, scrollPosition.y);
					scrollMomentum = new Vector2((0f - num) * args.WheelDelta, 0f);
				}
				else
				{
					ScrollPosition = new Vector2(scrollPosition.x, scrollPosition.y - num * args.WheelDelta);
					scrollMomentum = new Vector2(0f, (0f - num) * args.WheelDelta);
				}
				args.Use();
				Signal("OnMouseWheel", this, args);
			}
		}
		finally
		{
			base.OnMouseWheel(args);
		}
	}

	protected internal override void OnControlAdded(dfControl child)
	{
		base.OnControlAdded(child);
		attachEvents(child);
		if (autoLayout)
		{
			AutoArrange();
		}
	}

	protected internal override void OnControlRemoved(dfControl child)
	{
		base.OnControlRemoved(child);
		if (child != null)
		{
			detachEvents(child);
		}
		if (autoLayout)
		{
			AutoArrange();
		}
		else
		{
			updateScrollbars();
		}
	}

	protected override void OnRebuildRenderData()
	{
		if (Atlas == null || string.IsNullOrEmpty(backgroundSprite))
		{
			return;
		}
		dfAtlas.ItemInfo itemInfo = Atlas[backgroundSprite];
		if (!(itemInfo == null))
		{
			renderData.Material = Atlas.Material;
			Color32 color = ApplyOpacity(BackgroundColor);
			dfSprite.RenderOptions renderOptions = default(dfSprite.RenderOptions);
			renderOptions.atlas = atlas;
			renderOptions.color = color;
			renderOptions.fillAmount = 1f;
			renderOptions.flip = dfSpriteFlip.None;
			renderOptions.offset = pivot.TransformToUpperLeft(base.Size);
			renderOptions.pixelsToUnits = PixelsToUnits();
			renderOptions.size = base.Size;
			renderOptions.spriteInfo = itemInfo;
			dfSprite.RenderOptions options = renderOptions;
			if (itemInfo.border.horizontal == 0 && itemInfo.border.vertical == 0)
			{
				dfSprite.renderSprite(renderData, options);
			}
			else
			{
				dfSlicedSprite.renderSprite(renderData, options);
			}
		}
	}

	protected internal void OnScrollPositionChanged()
	{
		Invalidate();
		SignalHierarchy("OnScrollPositionChanged", this, ScrollPosition);
		if (this.ScrollPositionChanged != null)
		{
			this.ScrollPositionChanged(this, ScrollPosition);
		}
	}

	public void FitToContents()
	{
		if (controls.Count != 0)
		{
			Vector2 vector = Vector2.zero;
			for (int i = 0; i < controls.Count; i++)
			{
				dfControl dfControl2 = controls[i];
				Vector2 rhs = (Vector2)dfControl2.RelativePosition + dfControl2.Size;
				vector = Vector2.Max(vector, rhs);
			}
			base.Size = vector + new Vector2(scrollPadding.right, scrollPadding.bottom);
		}
	}

	public void CenterChildControls()
	{
		if (controls.Count != 0)
		{
			Vector2 vector = Vector2.one * float.MaxValue;
			Vector2 vector2 = Vector2.one * float.MinValue;
			for (int i = 0; i < controls.Count; i++)
			{
				dfControl dfControl2 = controls[i];
				Vector2 vector3 = dfControl2.RelativePosition;
				Vector2 rhs = vector3 + dfControl2.Size;
				vector = Vector2.Min(vector, vector3);
				vector2 = Vector2.Max(vector2, rhs);
			}
			Vector2 vector4 = vector2 - vector;
			Vector2 vector5 = (base.Size - vector4) * 0.5f;
			for (int j = 0; j < controls.Count; j++)
			{
				dfControl obj = controls[j];
				obj.RelativePosition = (Vector2)obj.RelativePosition - vector + vector5;
			}
		}
	}

	public void ScrollToTop()
	{
		scrollMomentum = Vector2.zero;
		ScrollPosition = new Vector2(scrollPosition.x, 0f);
	}

	public void ScrollToBottom()
	{
		scrollMomentum = Vector2.zero;
		ScrollPosition = new Vector2(scrollPosition.x, 2.1474836E+09f);
	}

	public void ScrollToLeft()
	{
		scrollMomentum = Vector2.zero;
		ScrollPosition = new Vector2(0f, scrollPosition.y);
	}

	public void ScrollToRight()
	{
		scrollMomentum = Vector2.zero;
		ScrollPosition = new Vector2(2.1474836E+09f, scrollPosition.y);
	}

	public void ScrollIntoView(dfControl control)
	{
		scrollMomentum = Vector2.zero;
		Rect rect = new Rect(scrollPosition.x + (float)scrollPadding.left, scrollPosition.y + (float)scrollPadding.top, size.x - (float)scrollPadding.horizontal, size.y - (float)scrollPadding.vertical).RoundToInt();
		Vector3 relativePosition = control.RelativePosition;
		Vector2 vector = control.Size;
		while (!controls.Contains(control))
		{
			control = control.Parent;
			relativePosition += control.RelativePosition;
		}
		Rect other = new Rect(scrollPosition.x + relativePosition.x, scrollPosition.y + relativePosition.y, vector.x, vector.y).RoundToInt();
		if (!rect.Contains(other))
		{
			Vector2 vector2 = scrollPosition;
			if (other.xMin < rect.xMin)
			{
				vector2.x = other.xMin - (float)scrollPadding.left;
			}
			else if (other.xMax > rect.xMax)
			{
				vector2.x = other.xMax - Mathf.Max(size.x, vector.x) + (float)scrollPadding.horizontal;
			}
			if (other.y < rect.y)
			{
				vector2.y = other.yMin - (float)scrollPadding.top;
			}
			else if (other.yMax > rect.yMax)
			{
				vector2.y = other.yMax - Mathf.Max(size.y, vector.y) + (float)scrollPadding.vertical;
			}
			ScrollPosition = vector2;
			scrollMomentum = Vector2.zero;
		}
	}

	public void Reset()
	{
		try
		{
			SuspendLayout();
			if (autoLayout)
			{
				Vector2 vector = ScrollPosition;
				ScrollPosition = Vector2.zero;
				AutoArrange();
				ScrollPosition = vector;
			}
			else
			{
				scrollPadding = ScrollPadding.ConstrainPadding();
				Vector3 vector2 = calculateMinChildPosition();
				vector2 -= new Vector3(scrollPadding.left, scrollPadding.top);
				for (int i = 0; i < controls.Count; i++)
				{
					controls[i].RelativePosition -= vector2;
				}
				scrollPosition = Vector2.zero;
			}
			Invalidate();
			updateScrollbars();
		}
		finally
		{
			ResumeLayout();
		}
	}

	private void Virtualize<T>(IList<T> backingList, int startIndex)
	{
		if (!useVirtualScrolling)
		{
			Debug.LogError("Virtual scrolling not enabled for this dfScrollPanel: " + base.name);
			return;
		}
		if (virtualScrollingTile == null)
		{
			Debug.LogError("Virtual scrolling cannot be started without assigning VirtualScrollingTile: " + base.name);
			return;
		}
		_ = backingList.Count;
		dfVirtualScrollData<T> dfVirtualScrollData2 = GetVirtualScrollData<T>() ?? initVirtualScrollData(backingList);
		bool flag = isVerticalFlow();
		RectOffset rectOffset = (dfVirtualScrollData2.ItemPadding = new RectOffset(FlowPadding.left, FlowPadding.right, FlowPadding.top, FlowPadding.bottom));
		int num = (flag ? rectOffset.vertical : rectOffset.horizontal);
		int num2 = (flag ? rectOffset.top : rectOffset.left);
		float num3 = (flag ? base.Height : base.Width);
		AutoLayout = false;
		AutoReset = false;
		IDFVirtualScrollingTile iDFVirtualScrollingTile = dfVirtualScrollData2.DummyTop ?? (dfVirtualScrollData2.DummyTop = initTile(rectOffset));
		dfPanel dfPanel2 = iDFVirtualScrollingTile.GetDfPanel();
		float num4 = (flag ? iDFVirtualScrollingTile.GetDfPanel().Height : iDFVirtualScrollingTile.GetDfPanel().Width);
		dfPanel2.IsEnabled = false;
		dfPanel2.Opacity = 0f;
		dfPanel2.gameObject.hideFlags = HideFlags.HideInHierarchy;
		dfScrollbar dfScrollbar2;
		if ((bool)(dfScrollbar2 = VertScrollbar) || (bool)(dfScrollbar2 = HorzScrollbar))
		{
			dfPanel obj = (dfVirtualScrollData2.DummyBottom ?? (dfVirtualScrollData2.DummyBottom = initTile(rectOffset))).GetDfPanel();
			float num5 = (flag ? dfPanel2.RelativePosition.y : dfPanel2.RelativePosition.x) + ((float)(backingList.Count - 1) * (num4 + (float)num) + (float)num2);
			obj.RelativePosition = (flag ? new Vector3(dfPanel2.RelativePosition.x, num5) : new Vector3(num5, dfPanel2.RelativePosition.y));
			obj.IsEnabled = dfPanel2.IsEnabled;
			obj.gameObject.hideFlags = dfPanel2.hideFlags;
			obj.Opacity = dfPanel2.Opacity;
			if (startIndex == 0 && dfScrollbar2.MaxValue != 0f)
			{
				startIndex = Mathf.RoundToInt(dfScrollbar2.Value / dfScrollbar2.MaxValue * (float)(backingList.Count - 1));
			}
			dfScrollbar2.Value = (float)startIndex * (num4 + (float)num);
		}
		float num6 = num3 / (num4 + (float)num);
		int num7 = Mathf.RoundToInt(num6);
		int num8 = (((float)num7 > num6) ? (num7 + 1) : (num7 + 2));
		float num9 = num2;
		float num10 = startIndex;
		for (int i = 0; i < num8 && i < backingList.Count; i++)
		{
			if (startIndex > backingList.Count)
			{
				break;
			}
			try
			{
				IDFVirtualScrollingTile iDFVirtualScrollingTile2 = ((dfVirtualScrollData2.IsInitialized && dfVirtualScrollData2.Tiles.Count >= i + 1) ? dfVirtualScrollData2.Tiles[i] : initTile(rectOffset));
				dfPanel obj2 = iDFVirtualScrollingTile2.GetDfPanel();
				float num11 = num9;
				obj2.RelativePosition = (flag ? new Vector2(rectOffset.left, num11) : new Vector2(num11, rectOffset.top));
				num9 = num11 + num4 + (float)num;
				if (!dfVirtualScrollData2.Tiles.Contains(iDFVirtualScrollingTile2))
				{
					dfVirtualScrollData2.Tiles.Add(iDFVirtualScrollingTile2);
				}
				iDFVirtualScrollingTile2.VirtualScrollItemIndex = startIndex;
				iDFVirtualScrollingTile2.OnScrollPanelItemVirtualize(backingList[startIndex]);
				startIndex++;
			}
			catch
			{
				foreach (IDFVirtualScrollingTile tile in dfVirtualScrollData2.Tiles)
				{
					tile.OnScrollPanelItemVirtualize(backingList[--tile.VirtualScrollItemIndex]);
				}
			}
		}
		if (num10 != 0f && this.ScrollPositionChanged != null)
		{
			ScrollPositionChanged -= virtualScrollPositionChanged<T>;
		}
		dfVirtualScrollData2.IsInitialized = true;
		ScrollPositionChanged += virtualScrollPositionChanged<T>;
	}

	public void Virtualize<T>(IList<T> backingList, dfPanel tile)
	{
		if (!tile.GetComponents<MonoBehaviour>().FirstOrDefault((MonoBehaviour t) => t is IDFVirtualScrollingTile))
		{
			Debug.LogError("The tile you've chosen does not implement IDFVirtualScrollingTile!");
			return;
		}
		UseVirtualScrolling = true;
		VirtualScrollingTile = tile;
		Virtualize(backingList, 0);
	}

	public void Virtualize<T>(IList<T> backingList)
	{
		Virtualize(backingList, 0);
	}

	public void ResetVirtualScrollingData()
	{
		virtualScrollData = null;
		dfControl[] array = controls.ToArray();
		foreach (dfControl dfControl2 in array)
		{
			RemoveControl(dfControl2);
			UnityEngine.Object.Destroy(dfControl2.gameObject);
		}
		ScrollPosition = Vector2.zero;
	}

	public dfVirtualScrollData<T> GetVirtualScrollData<T>()
	{
		return (dfVirtualScrollData<T>)virtualScrollData;
	}

	[HideInInspector]
	private void AutoArrange()
	{
		SuspendLayout();
		try
		{
			scrollPadding = ScrollPadding.ConstrainPadding();
			flowPadding = FlowPadding.ConstrainPadding();
			float num = (float)scrollPadding.left + (float)flowPadding.left - scrollPosition.x;
			float num2 = (float)scrollPadding.top + (float)flowPadding.top - scrollPosition.y;
			float num3 = 0f;
			float num4 = 0f;
			for (int i = 0; i < controls.Count; i++)
			{
				dfControl dfControl2 = controls[i];
				if (!dfControl2.GetIsVisibleRaw() || !dfControl2.enabled || !dfControl2.gameObject.activeSelf || dfControl2 == horzScroll || dfControl2 == vertScroll)
				{
					continue;
				}
				if (wrapLayout)
				{
					if (flowDirection == LayoutDirection.Horizontal)
					{
						if (num + dfControl2.Width >= size.x - (float)scrollPadding.right)
						{
							num = (float)scrollPadding.left + (float)flowPadding.left;
							num2 += num4;
							num4 = 0f;
						}
					}
					else if (num2 + dfControl2.Height + (float)flowPadding.vertical >= size.y - (float)scrollPadding.bottom)
					{
						num2 = (float)scrollPadding.top + (float)flowPadding.top;
						num += num3;
						num3 = 0f;
					}
				}
				Vector2 vector = new Vector2(num, num2);
				dfControl2.RelativePosition = vector;
				float num5 = dfControl2.Width + (float)flowPadding.horizontal;
				float num6 = dfControl2.Height + (float)flowPadding.vertical;
				num3 = Mathf.Max(num5, num3);
				num4 = Mathf.Max(num6, num4);
				if (flowDirection == LayoutDirection.Horizontal)
				{
					num += num5;
				}
				else
				{
					num2 += num6;
				}
			}
			updateScrollbars();
		}
		finally
		{
			ResumeLayout();
		}
	}

	[HideInInspector]
	private void initialize()
	{
		if (initialized)
		{
			return;
		}
		initialized = true;
		if (Application.isPlaying)
		{
			if (horzScroll != null)
			{
				horzScroll.ValueChanged += horzScroll_ValueChanged;
			}
			if (vertScroll != null)
			{
				vertScroll.ValueChanged += vertScroll_ValueChanged;
			}
		}
		if (resetNeeded || autoLayout || autoReset)
		{
			Reset();
		}
		Invalidate();
		ScrollPosition = Vector2.zero;
		updateScrollbars();
	}

	private void vertScroll_ValueChanged(dfControl control, float value)
	{
		ScrollPosition = new Vector2(scrollPosition.x, value);
	}

	private void horzScroll_ValueChanged(dfControl control, float value)
	{
		ScrollPosition = new Vector2(value, ScrollPosition.y);
	}

	private void scrollChildControls(Vector3 delta)
	{
		try
		{
			scrolling = true;
			delta = delta.Scale(1f, -1f, 1f);
			for (int i = 0; i < controls.Count; i++)
			{
				dfControl obj = controls[i];
				obj.Position = (obj.Position - delta).RoundToInt();
			}
		}
		finally
		{
			scrolling = false;
		}
	}

	private Vector2 calculateMinChildPosition()
	{
		float num = float.MaxValue;
		float num2 = float.MaxValue;
		for (int i = 0; i < controls.Count; i++)
		{
			dfControl dfControl2 = controls[i];
			if (dfControl2.enabled && dfControl2.gameObject.activeSelf)
			{
				Vector3 vector = dfControl2.RelativePosition.FloorToInt();
				num = Mathf.Min(num, vector.x);
				num2 = Mathf.Min(num2, vector.y);
			}
		}
		return new Vector2(num, num2);
	}

	private Vector2 calculateViewSize()
	{
		Vector2 vector = new Vector2(scrollPadding.horizontal, scrollPadding.vertical).RoundToInt();
		Vector2 vector2 = base.Size.RoundToInt() - vector;
		if (!base.IsVisible || controls.Count == 0)
		{
			return vector2;
		}
		Vector2 vector3 = Vector2.one * float.MaxValue;
		Vector2 vector4 = Vector2.one * float.MinValue;
		for (int i = 0; i < controls.Count; i++)
		{
			dfControl dfControl2 = controls[i];
			if (!Application.isPlaying || dfControl2.IsVisible)
			{
				Vector2 vector5 = dfControl2.RelativePosition.CeilToInt();
				Vector2 lhs = vector5 + dfControl2.Size.CeilToInt();
				vector3 = Vector2.Min(vector5, vector3);
				vector4 = Vector2.Max(lhs, vector4);
			}
		}
		Vector2 vector6 = Vector2.Max(Vector2.zero, vector3 - new Vector2(scrollPadding.left, scrollPadding.top));
		vector4 = Vector2.Max(vector4 + vector6, vector2);
		return vector4 - vector3 + vector6;
	}

	[HideInInspector]
	private void updateScrollbars()
	{
		Vector2 vector = calculateViewSize();
		Vector2 vector2 = base.Size - new Vector2(scrollPadding.horizontal, scrollPadding.vertical);
		if (horzScroll != null)
		{
			horzScroll.MinValue = 0f;
			horzScroll.MaxValue = vector.x;
			horzScroll.ScrollSize = vector2.x;
			horzScroll.Value = Mathf.Max(0f, scrollPosition.x);
		}
		if (vertScroll != null)
		{
			vertScroll.MinValue = 0f;
			vertScroll.MaxValue = vector.y;
			vertScroll.ScrollSize = vector2.y;
			vertScroll.Value = Mathf.Max(0f, scrollPosition.y);
		}
	}

	private void virtualScrollPositionChanged<T>(dfControl control, Vector2 value)
	{
		dfVirtualScrollData<T> dfVirtualScrollData2 = GetVirtualScrollData<T>();
		if (dfVirtualScrollData2 == null)
		{
			return;
		}
		IList<T> backingList = dfVirtualScrollData2.BackingList;
		RectOffset itemPadding = dfVirtualScrollData2.ItemPadding;
		List<IDFVirtualScrollingTile> tiles = dfVirtualScrollData2.Tiles;
		bool flag = isVerticalFlow();
		float num = (flag ? (value.y - dfVirtualScrollData2.LastScrollPosition.y) : (value.x - dfVirtualScrollData2.LastScrollPosition.x));
		dfVirtualScrollData2.LastScrollPosition = value;
		if (Mathf.Abs(num) > base.Height && ((bool)VertScrollbar || (bool)HorzScrollbar))
		{
			int startIndex = Mathf.RoundToInt((flag ? (value.y / VertScrollbar.MaxValue) : (value.x / HorzScrollbar.MaxValue)) * (float)(backingList.Count - 1));
			Virtualize(backingList, startIndex);
			return;
		}
		foreach (IDFVirtualScrollingTile item in tiles)
		{
			int index = 0;
			float newY = 0f;
			bool flag2 = false;
			dfPanel dfPanel2 = item.GetDfPanel();
			float num2 = (flag ? dfPanel2.RelativePosition.y : dfPanel2.RelativePosition.x);
			float num3 = (flag ? dfPanel2.Height : dfPanel2.Width);
			float num4 = (flag ? base.Height : base.Width);
			if (num > 0f)
			{
				if (!(num2 + num3 <= 0f))
				{
					continue;
				}
				dfVirtualScrollData2.GetNewLimits(flag, getMaxes: true, out index, out newY);
				if (index >= backingList.Count)
				{
					continue;
				}
				flag2 = true;
				dfPanel2.RelativePosition = (flag ? new Vector3(dfPanel2.RelativePosition.x, newY + num3 + (float)itemPadding.vertical) : new Vector3(newY + num3 + (float)itemPadding.horizontal, dfPanel2.RelativePosition.y));
			}
			else if (num < 0f)
			{
				if (!(num2 >= num4))
				{
					continue;
				}
				dfVirtualScrollData2.GetNewLimits(flag, getMaxes: false, out index, out newY);
				if (index < 0)
				{
					continue;
				}
				flag2 = true;
				dfPanel2.RelativePosition = (flag ? new Vector3(dfPanel2.RelativePosition.x, newY - (num3 + (float)itemPadding.vertical)) : new Vector3(newY - (num3 + (float)itemPadding.horizontal), dfPanel2.RelativePosition.y));
			}
			if (flag2)
			{
				item.VirtualScrollItemIndex = index;
				item.OnScrollPanelItemVirtualize(backingList[index]);
			}
		}
	}

	private dfVirtualScrollData<T> initVirtualScrollData<T>(IList<T> backingList)
	{
		return (dfVirtualScrollData<T>)(virtualScrollData = new dfVirtualScrollData<T>
		{
			BackingList = backingList
		});
	}

	private IDFVirtualScrollingTile initTile(RectOffset padding)
	{
		IDFVirtualScrollingTile obj = (IDFVirtualScrollingTile)UnityEngine.Object.Instantiate(virtualScrollingTile.GetComponents<MonoBehaviour>().FirstOrDefault((MonoBehaviour p) => p is IDFVirtualScrollingTile));
		dfPanel dfPanel2 = obj.GetDfPanel();
		bool flag = isVerticalFlow();
		AddControl(dfPanel2);
		if (AutoFitVirtualTiles)
		{
			if (flag)
			{
				dfPanel2.Width = base.Width - (float)padding.horizontal;
			}
			else
			{
				dfPanel2.Height = base.Height - (float)padding.vertical;
			}
		}
		dfPanel2.RelativePosition = new Vector3(padding.left, padding.top);
		return obj;
	}

	private bool isVerticalFlow()
	{
		return FlowDirection == LayoutDirection.Vertical;
	}

	private void attachEvents(dfControl control)
	{
		control.IsVisibleChanged += childIsVisibleChanged;
		control.PositionChanged += childControlInvalidated;
		control.SizeChanged += childControlInvalidated;
		control.ZOrderChanged += childOrderChanged;
	}

	private void detachEvents(dfControl control)
	{
		control.IsVisibleChanged -= childIsVisibleChanged;
		control.PositionChanged -= childControlInvalidated;
		control.SizeChanged -= childControlInvalidated;
		control.ZOrderChanged -= childOrderChanged;
	}

	private void childOrderChanged(dfControl control, int value)
	{
		onChildControlInvalidatedLayout();
	}

	private void childIsVisibleChanged(dfControl control, bool value)
	{
		onChildControlInvalidatedLayout();
	}

	private void childControlInvalidated(dfControl control, Vector2 value)
	{
		onChildControlInvalidatedLayout();
	}

	[HideInInspector]
	private void onChildControlInvalidatedLayout()
	{
		if (!scrolling && !base.IsLayoutSuspended)
		{
			if (autoLayout)
			{
				AutoArrange();
			}
			updateScrollbars();
			Invalidate();
		}
	}
}
