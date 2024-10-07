using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[ExecuteInEditMode]
[dfCategory("Basic Controls")]
[dfTooltip("Displays text information, optionally allowing the use of markup to specify colors and embedded sprites")]
[dfHelp("http://www.daikonforge.com/docs/df-gui/classdf_label.html")]
[AddComponentMenu("Daikon Forge/User Interface/Label")]
public class dfLabel : dfControl, IDFMultiRender, IRendersText
{
	[SerializeField]
	protected dfAtlas atlas;

	[SerializeField]
	protected dfFontBase font;

	[SerializeField]
	protected string backgroundSprite;

	[SerializeField]
	protected Color32 backgroundColor = UnityEngine.Color.white;

	[SerializeField]
	protected bool autoSize;

	[SerializeField]
	protected bool autoHeight;

	[SerializeField]
	protected bool wordWrap;

	[SerializeField]
	protected string text = "Label";

	[SerializeField]
	protected Color32 bottomColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	[SerializeField]
	protected TextAlignment align;

	[SerializeField]
	protected dfVerticalAlignment vertAlign;

	[SerializeField]
	protected float textScale = 1f;

	[SerializeField]
	protected dfTextScaleMode textScaleMode;

	[SerializeField]
	protected int charSpacing;

	[SerializeField]
	protected bool colorizeSymbols;

	[SerializeField]
	protected bool processMarkup;

	[SerializeField]
	protected bool outline;

	[SerializeField]
	protected int outlineWidth = 1;

	[SerializeField]
	protected bool enableGradient;

	[SerializeField]
	protected Color32 outlineColor = UnityEngine.Color.black;

	[SerializeField]
	protected bool shadow;

	[SerializeField]
	protected Color32 shadowColor = UnityEngine.Color.black;

	[SerializeField]
	protected Vector2 shadowOffset = new Vector2(1f, -1f);

	[SerializeField]
	protected RectOffset padding = new RectOffset();

	[SerializeField]
	protected int tabSize = 48;

	[SerializeField]
	protected List<int> tabStops = new List<int>();

	private Vector2 startSize = Vector2.zero;

	private bool isFontCallbackAssigned;

	private dfRenderData textRenderData;

	private dfList<dfRenderData> renderDataBuffers = dfList<dfRenderData>.Obtain();

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

	public dfFontBase Font
	{
		get
		{
			if (font == null)
			{
				dfGUIManager manager = GetManager();
				if (manager != null)
				{
					font = manager.DefaultFont;
				}
			}
			return font;
		}
		set
		{
			if (value != font)
			{
				unbindTextureRebuildCallback();
				font = value;
				bindTextureRebuildCallback();
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

	public float TextScale
	{
		get
		{
			return textScale;
		}
		set
		{
			value = Mathf.Max(0.1f, value);
			if (!Mathf.Approximately(textScale, value))
			{
				dfFontManager.Invalidate(Font);
				textScale = value;
				Invalidate();
			}
		}
	}

	public dfTextScaleMode TextScaleMode
	{
		get
		{
			return textScaleMode;
		}
		set
		{
			textScaleMode = value;
			Invalidate();
		}
	}

	public int CharacterSpacing
	{
		get
		{
			return charSpacing;
		}
		set
		{
			value = Mathf.Max(0, value);
			if (value != charSpacing)
			{
				charSpacing = value;
				Invalidate();
			}
		}
	}

	public bool ColorizeSymbols
	{
		get
		{
			return colorizeSymbols;
		}
		set
		{
			if (value != colorizeSymbols)
			{
				colorizeSymbols = value;
				Invalidate();
			}
		}
	}

	public bool ProcessMarkup
	{
		get
		{
			return processMarkup;
		}
		set
		{
			if (value != processMarkup)
			{
				processMarkup = value;
				Invalidate();
			}
		}
	}

	public bool ShowGradient
	{
		get
		{
			return enableGradient;
		}
		set
		{
			if (value != enableGradient)
			{
				enableGradient = value;
				Invalidate();
			}
		}
	}

	public Color32 BottomColor
	{
		get
		{
			return bottomColor;
		}
		set
		{
			if (!bottomColor.Equals(value))
			{
				bottomColor = value;
				OnColorChanged();
			}
		}
	}

	public string Text
	{
		get
		{
			return text;
		}
		set
		{
			value = ((value != null) ? value.Replace("\\t", "\t").Replace("\\n", "\n") : string.Empty);
			if (!string.Equals(value, text))
			{
				dfFontManager.Invalidate(Font);
				localizationKey = value;
				text = getLocalizedValue(value);
				OnTextChanged();
			}
		}
	}

	public bool AutoSize
	{
		get
		{
			return autoSize;
		}
		set
		{
			if (value != autoSize)
			{
				if (value)
				{
					autoHeight = false;
				}
				autoSize = value;
				Invalidate();
			}
		}
	}

	public bool AutoHeight
	{
		get
		{
			if (autoHeight)
			{
				return !autoSize;
			}
			return false;
		}
		set
		{
			if (value != autoHeight)
			{
				if (value)
				{
					autoSize = false;
				}
				autoHeight = value;
				Invalidate();
			}
		}
	}

	public bool WordWrap
	{
		get
		{
			return wordWrap;
		}
		set
		{
			if (value != wordWrap)
			{
				wordWrap = value;
				Invalidate();
			}
		}
	}

	public TextAlignment TextAlignment
	{
		get
		{
			return align;
		}
		set
		{
			if (value != align)
			{
				align = value;
				Invalidate();
			}
		}
	}

	public dfVerticalAlignment VerticalAlignment
	{
		get
		{
			return vertAlign;
		}
		set
		{
			if (value != vertAlign)
			{
				vertAlign = value;
				Invalidate();
			}
		}
	}

	public bool Outline
	{
		get
		{
			return outline;
		}
		set
		{
			if (value != outline)
			{
				outline = value;
				Invalidate();
			}
		}
	}

	public int OutlineSize
	{
		get
		{
			return outlineWidth;
		}
		set
		{
			value = Mathf.Max(0, value);
			if (value != outlineWidth)
			{
				outlineWidth = value;
				Invalidate();
			}
		}
	}

	public Color32 OutlineColor
	{
		get
		{
			return outlineColor;
		}
		set
		{
			if (!value.Equals(outlineColor))
			{
				outlineColor = value;
				Invalidate();
			}
		}
	}

	public bool Shadow
	{
		get
		{
			return shadow;
		}
		set
		{
			if (value != shadow)
			{
				shadow = value;
				Invalidate();
			}
		}
	}

	public Color32 ShadowColor
	{
		get
		{
			return shadowColor;
		}
		set
		{
			if (!value.Equals(shadowColor))
			{
				shadowColor = value;
				Invalidate();
			}
		}
	}

	public Vector2 ShadowOffset
	{
		get
		{
			return shadowOffset;
		}
		set
		{
			if (value != shadowOffset)
			{
				shadowOffset = value;
				Invalidate();
			}
		}
	}

	public RectOffset Padding
	{
		get
		{
			if (padding == null)
			{
				padding = new RectOffset();
			}
			return padding;
		}
		set
		{
			value = value.ConstrainPadding();
			if (!object.Equals(value, padding))
			{
				padding = value;
				Invalidate();
			}
		}
	}

	public int TabSize
	{
		get
		{
			return tabSize;
		}
		set
		{
			value = Mathf.Max(0, value);
			if (value != tabSize)
			{
				tabSize = value;
				Invalidate();
			}
		}
	}

	public List<int> TabStops => tabStops;

	public event PropertyChangedEventHandler<string> TextChanged;

	protected internal override void OnLocalize()
	{
		base.OnLocalize();
		Text = getLocalizedValue(localizationKey ?? text);
	}

	protected internal void OnTextChanged()
	{
		if (base.CustomWordWrapAllowed && Settings.Data != null && Settings.Data.DoesCurrentLanguageWordwrap())
		{
			WordWrapper instance = WordWrapper.GetInstance();
			int num = Mathf.CeilToInt((float)Font.FontSize * TextScale);
			text = instance.WrapText(text, (int)(base.Width / (float)num), WordWrapper.EWrapAlgorithm.Dynamic);
		}
		Invalidate();
		Signal("OnTextChanged", this, text);
		if (this.TextChanged != null)
		{
			this.TextChanged(this, text);
		}
	}

	public override void Start()
	{
		base.Start();
		localizationKey = Text;
	}

	public override void OnEnable()
	{
		base.OnEnable();
		bool flag = Font != null && Font.IsValid;
		if (Application.isPlaying && !flag)
		{
			Font = GetManager().DefaultFont;
		}
		bindTextureRebuildCallback();
		if (size.sqrMagnitude <= float.Epsilon)
		{
			base.Size = new Vector2(150f, 25f);
		}
	}

	public override void OnDisable()
	{
		base.OnDisable();
		unbindTextureRebuildCallback();
	}

	public override void Update()
	{
		if (autoSize)
		{
			autoHeight = false;
		}
		if (Font == null)
		{
			Font = GetManager().DefaultFont;
		}
		base.Update();
	}

	public override void Awake()
	{
		base.Awake();
		startSize = (Application.isPlaying ? base.Size : Vector2.zero);
	}

	public override Vector2 CalculateMinimumSize()
	{
		if (Font != null)
		{
			float num = (float)Font.FontSize * TextScale * 0.75f;
			return Vector2.Max(base.CalculateMinimumSize(), new Vector2(num, num));
		}
		return base.CalculateMinimumSize();
	}

	[HideInInspector]
	public override void Invalidate()
	{
		base.Invalidate();
		if (Font == null || !Font.IsValid || GetManager() == null)
		{
			return;
		}
		bool flag = size.sqrMagnitude <= float.Epsilon;
		if (!autoSize && !autoHeight && !flag)
		{
			return;
		}
		if (string.IsNullOrEmpty(Text))
		{
			Vector2 vector;
			Vector2 vector2 = (vector = size);
			if (flag)
			{
				vector = new Vector2(150f, 24f);
			}
			if (AutoSize || AutoHeight)
			{
				vector.y = Mathf.CeilToInt((float)Font.LineHeight * TextScale);
			}
			if (vector2 != vector)
			{
				SuspendLayout();
				base.Size = vector;
				ResumeLayout();
			}
			return;
		}
		Vector2 vector3 = size;
		using (dfFontRendererBase dfFontRendererBase2 = obtainRenderer())
		{
			Vector2 vector4 = dfFontRendererBase2.MeasureString(text).RoundToInt();
			if (AutoSize || flag)
			{
				size = vector4 + new Vector2(padding.horizontal, padding.vertical);
			}
			else if (AutoHeight)
			{
				size = new Vector2(size.x, vector4.y + (float)padding.vertical);
			}
		}
		if ((size - vector3).sqrMagnitude >= 1f)
		{
			raiseSizeChangedEvent();
		}
	}

	private dfFontRendererBase obtainRenderer()
	{
		bool flag = base.Size.sqrMagnitude <= float.Epsilon;
		Vector2 vector = base.Size - new Vector2(padding.horizontal, padding.vertical);
		Vector2 vector2 = ((autoSize || flag) ? getAutoSizeDefault() : vector);
		if (autoHeight)
		{
			vector2 = new Vector2(vector.x, 2.1474836E+09f);
		}
		float num = PixelsToUnits();
		Vector3 vector3 = (pivot.TransformToUpperLeft(base.Size) + new Vector3(padding.left, -padding.top)) * num;
		float num2 = TextScale * getTextScaleMultiplier();
		dfFontRendererBase dfFontRendererBase2 = Font.ObtainRenderer();
		dfFontRendererBase2.WordWrap = WordWrap;
		dfFontRendererBase2.MaxSize = vector2;
		dfFontRendererBase2.PixelRatio = num;
		dfFontRendererBase2.TextScale = num2;
		dfFontRendererBase2.CharacterSpacing = CharacterSpacing;
		dfFontRendererBase2.VectorOffset = vector3.Quantize(num);
		dfFontRendererBase2.MultiLine = true;
		dfFontRendererBase2.TabSize = TabSize;
		dfFontRendererBase2.TabStops = TabStops;
		dfFontRendererBase2.TextAlign = ((!autoSize) ? TextAlignment : TextAlignment.Left);
		dfFontRendererBase2.ColorizeSymbols = ColorizeSymbols;
		dfFontRendererBase2.ProcessMarkup = ProcessMarkup;
		dfFontRendererBase2.DefaultColor = (base.IsEnabled ? base.Color : base.DisabledColor);
		dfFontRendererBase2.BottomColor = (enableGradient ? new Color32?(BottomColor) : null);
		dfFontRendererBase2.OverrideMarkupColors = !base.IsEnabled;
		dfFontRendererBase2.Opacity = CalculateOpacity();
		dfFontRendererBase2.Outline = Outline;
		dfFontRendererBase2.OutlineSize = OutlineSize;
		dfFontRendererBase2.OutlineColor = OutlineColor;
		dfFontRendererBase2.Shadow = Shadow;
		dfFontRendererBase2.ShadowColor = ShadowColor;
		dfFontRendererBase2.ShadowOffset = ShadowOffset;
		if (dfFontRendererBase2 is dfDynamicFont.DynamicFontRenderer dynamicFontRenderer)
		{
			dynamicFontRenderer.SpriteAtlas = Atlas;
			dynamicFontRenderer.SpriteBuffer = renderData;
		}
		if (vertAlign != 0)
		{
			dfFontRendererBase2.VectorOffset = getVertAlignOffset(dfFontRendererBase2);
		}
		return dfFontRendererBase2;
	}

	private float getTextScaleMultiplier()
	{
		if (textScaleMode == dfTextScaleMode.None || !Application.isPlaying)
		{
			return 1f;
		}
		if (textScaleMode == dfTextScaleMode.ScreenResolution)
		{
			return (float)Screen.height / (float)GetManager().FixedHeight;
		}
		if (autoSize)
		{
			return 1f;
		}
		return base.Size.y / startSize.y;
	}

	private Vector2 getAutoSizeDefault()
	{
		float x = ((maxSize.x > float.Epsilon) ? maxSize.x : 2.1474836E+09f);
		float y = ((maxSize.y > float.Epsilon) ? maxSize.y : 2.1474836E+09f);
		return new Vector2(x, y);
	}

	private Vector3 getVertAlignOffset(dfFontRendererBase textRenderer)
	{
		float num = PixelsToUnits();
		Vector2 vector = textRenderer.MeasureString(text) * num;
		Vector3 vectorOffset = textRenderer.VectorOffset;
		float num2 = (base.Height - (float)padding.vertical) * num;
		if (vector.y >= num2)
		{
			return vectorOffset;
		}
		switch (vertAlign)
		{
		case dfVerticalAlignment.Middle:
			vectorOffset.y -= (num2 - vector.y) * 0.5f;
			break;
		case dfVerticalAlignment.Bottom:
			vectorOffset.y -= num2 - vector.y;
			break;
		}
		return vectorOffset;
	}

	protected internal virtual void renderBackground()
	{
		if (Atlas == null)
		{
			return;
		}
		dfAtlas.ItemInfo itemInfo = Atlas[backgroundSprite];
		if (!(itemInfo == null))
		{
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

	public dfList<dfRenderData> RenderMultiple()
	{
		try
		{
			if (!isControlInvalidated && (textRenderData != null || renderData != null))
			{
				Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
				for (int i = 0; i < renderDataBuffers.Count; i++)
				{
					renderDataBuffers[i].Transform = localToWorldMatrix;
				}
				return renderDataBuffers;
			}
			if (Atlas == null || Font == null || !isVisible)
			{
				return null;
			}
			if (renderData == null)
			{
				renderData = dfRenderData.Obtain();
				textRenderData = dfRenderData.Obtain();
				isControlInvalidated = true;
			}
			resetRenderBuffers();
			renderBackground();
			if (string.IsNullOrEmpty(Text))
			{
				if (AutoSize || AutoHeight)
				{
					base.Height = Mathf.CeilToInt((float)Font.LineHeight * TextScale);
				}
				return renderDataBuffers;
			}
			bool flag = size.sqrMagnitude <= float.Epsilon;
			using (dfFontRendererBase dfFontRendererBase2 = obtainRenderer())
			{
				dfFontRendererBase2.Render(text, textRenderData);
				if (AutoSize || flag)
				{
					base.Size = (dfFontRendererBase2.RenderedSize + new Vector2(padding.horizontal, padding.vertical)).CeilToInt();
				}
				else if (AutoHeight)
				{
					base.Size = new Vector2(size.x, dfFontRendererBase2.RenderedSize.y + (float)padding.vertical).CeilToInt();
				}
			}
			updateCollider();
			return renderDataBuffers;
		}
		finally
		{
			isControlInvalidated = false;
		}
	}

	private void resetRenderBuffers()
	{
		renderDataBuffers.Clear();
		Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
		if (renderData != null)
		{
			renderData.Clear();
			renderData.Material = Atlas.Material;
			renderData.Transform = localToWorldMatrix;
			renderDataBuffers.Add(renderData);
		}
		if (textRenderData != null)
		{
			textRenderData.Clear();
			textRenderData.Material = Atlas.Material;
			textRenderData.Transform = localToWorldMatrix;
			renderDataBuffers.Add(textRenderData);
		}
	}

	private void bindTextureRebuildCallback()
	{
		if (!isFontCallbackAssigned && !(Font == null) && Font is dfDynamicFont)
		{
			Font baseFont = (Font as dfDynamicFont).BaseFont;
			baseFont.textureRebuildCallback = (Font.FontTextureRebuildCallback)Delegate.Combine(baseFont.textureRebuildCallback, new Font.FontTextureRebuildCallback(onFontTextureRebuilt));
			isFontCallbackAssigned = true;
		}
	}

	private void unbindTextureRebuildCallback()
	{
		if (isFontCallbackAssigned && !(Font == null))
		{
			if (Font is dfDynamicFont)
			{
				Font baseFont = (Font as dfDynamicFont).BaseFont;
				baseFont.textureRebuildCallback = (Font.FontTextureRebuildCallback)Delegate.Remove(baseFont.textureRebuildCallback, new Font.FontTextureRebuildCallback(onFontTextureRebuilt));
			}
			isFontCallbackAssigned = false;
		}
	}

	private void requestCharacterInfo()
	{
		dfDynamicFont dfDynamicFont2 = Font as dfDynamicFont;
		if (!(dfDynamicFont2 == null) && dfFontManager.IsDirty(Font) && !string.IsNullOrEmpty(text))
		{
			float num = TextScale * getTextScaleMultiplier();
			int fontSize = Mathf.CeilToInt((float)font.FontSize * num);
			dfDynamicFont2.AddCharacterRequest(text, fontSize, FontStyle.Normal);
		}
	}

	private void onFontTextureRebuilt()
	{
		requestCharacterInfo();
		Invalidate();
	}

	public void UpdateFontInfo()
	{
		requestCharacterInfo();
	}
}
