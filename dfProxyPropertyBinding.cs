using System;
using UnityEngine;

[Serializable]
[AddComponentMenu("Daikon Forge/Data Binding/Proxy Property Binding")]
public class dfProxyPropertyBinding : MonoBehaviour, IDataBindingComponent
{
	public dfComponentMemberInfo DataSource;

	public dfComponentMemberInfo DataTarget;

	public bool TwoWay;

	private dfObservableProperty sourceProperty;

	private dfObservableProperty targetProperty;

	private bool isBound;

	private bool eventsAttached;

	public bool IsBound => isBound;

	public void Awake()
	{
	}

	public void OnEnable()
	{
		if (!isBound && IsDataSourceValid() && DataTarget.IsValid)
		{
			Bind();
		}
	}

	public void Start()
	{
		if (!isBound && IsDataSourceValid() && DataTarget.IsValid)
		{
			Bind();
		}
	}

	public void OnDisable()
	{
		Unbind();
	}

	public void Update()
	{
		if (sourceProperty != null && targetProperty != null)
		{
			if (sourceProperty.HasChanged)
			{
				targetProperty.Value = sourceProperty.Value;
				sourceProperty.ClearChangedFlag();
			}
			else if (TwoWay && targetProperty.HasChanged)
			{
				sourceProperty.Value = targetProperty.Value;
				targetProperty.ClearChangedFlag();
			}
		}
	}

	public void Bind()
	{
		if (isBound)
		{
			return;
		}
		if (!IsDataSourceValid())
		{
			Debug.LogError($"Invalid data binding configuration - Source:{DataSource}, Target:{DataTarget}");
			return;
		}
		if (!DataTarget.IsValid)
		{
			Debug.LogError($"Invalid data binding configuration - Source:{DataSource}, Target:{DataTarget}");
			return;
		}
		dfDataObjectProxy dfDataObjectProxy2 = DataSource.Component as dfDataObjectProxy;
		sourceProperty = dfDataObjectProxy2.GetProperty(DataSource.MemberName);
		targetProperty = DataTarget.GetProperty();
		isBound = sourceProperty != null && targetProperty != null;
		if (isBound)
		{
			targetProperty.Value = sourceProperty.Value;
		}
		attachEvent();
	}

	public void Unbind()
	{
		if (isBound)
		{
			detachEvent();
			sourceProperty = null;
			targetProperty = null;
			isBound = false;
		}
	}

	private bool IsDataSourceValid()
	{
		if (DataSource == null && !(DataSource.Component != null) && string.IsNullOrEmpty(DataSource.MemberName))
		{
			return (DataSource.Component as dfDataObjectProxy).Data != null;
		}
		return true;
	}

	private void attachEvent()
	{
		if (!eventsAttached)
		{
			eventsAttached = true;
			dfDataObjectProxy dfDataObjectProxy2 = DataSource.Component as dfDataObjectProxy;
			if (dfDataObjectProxy2 != null)
			{
				dfDataObjectProxy2.DataChanged += handle_DataChanged;
			}
		}
	}

	private void detachEvent()
	{
		if (eventsAttached)
		{
			eventsAttached = false;
			dfDataObjectProxy dfDataObjectProxy2 = DataSource.Component as dfDataObjectProxy;
			if (dfDataObjectProxy2 != null)
			{
				dfDataObjectProxy2.DataChanged -= handle_DataChanged;
			}
		}
	}

	private void handle_DataChanged(object data)
	{
		Unbind();
		if (IsDataSourceValid())
		{
			Bind();
		}
	}

	public override string ToString()
	{
		string text = ((DataSource != null && DataSource.Component != null) ? DataSource.Component.GetType().Name : "[null]");
		string text2 = ((DataSource != null && !string.IsNullOrEmpty(DataSource.MemberName)) ? DataSource.MemberName : "[null]");
		string text3 = ((DataTarget != null && DataTarget.Component != null) ? DataTarget.Component.GetType().Name : "[null]");
		string text4 = ((DataTarget != null && !string.IsNullOrEmpty(DataTarget.MemberName)) ? DataTarget.MemberName : "[null]");
		return $"Bind {text}.{text2} -> {text3}.{text4}";
	}
}
