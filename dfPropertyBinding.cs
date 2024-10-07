using System;
using UnityEngine;

[Serializable]
[AddComponentMenu("Daikon Forge/Data Binding/Property Binding")]
public class dfPropertyBinding : MonoBehaviour, IDataBindingComponent
{
	public dfComponentMemberInfo DataSource;

	public dfComponentMemberInfo DataTarget;

	public string FormatString;

	public bool TwoWay;

	public bool AutoBind = true;

	public bool AutoUnbind = true;

	protected dfObservableProperty sourceProperty;

	protected dfObservableProperty targetProperty;

	protected bool isBound;

	protected bool useFormatString;

	public bool IsBound => isBound;

	public virtual void OnEnable()
	{
		if (AutoBind && DataSource != null && DataTarget != null && !isBound && DataSource.IsValid && DataTarget.IsValid)
		{
			Bind();
		}
	}

	public virtual void Start()
	{
		if (AutoBind && DataSource != null && DataTarget != null && !isBound && DataSource.IsValid && DataTarget.IsValid)
		{
			Bind();
		}
	}

	public virtual void OnDisable()
	{
		if (AutoUnbind)
		{
			Unbind();
		}
	}

	public virtual void OnDestroy()
	{
		Unbind();
	}

	public virtual void Update()
	{
		if (sourceProperty != null && targetProperty != null)
		{
			if (sourceProperty.HasChanged)
			{
				targetProperty.Value = formatValue(sourceProperty.Value);
				sourceProperty.ClearChangedFlag();
			}
			else if (TwoWay && targetProperty.HasChanged)
			{
				sourceProperty.Value = targetProperty.Value;
				targetProperty.ClearChangedFlag();
			}
		}
	}

	public static dfPropertyBinding Bind(Component sourceComponent, string sourceProperty, Component targetComponent, string targetProperty)
	{
		return Bind(sourceComponent.gameObject, sourceComponent, sourceProperty, targetComponent, targetProperty);
	}

	public static dfPropertyBinding Bind(GameObject hostObject, Component sourceComponent, string sourceProperty, Component targetComponent, string targetProperty)
	{
		if (hostObject == null)
		{
			throw new ArgumentNullException("hostObject");
		}
		if (sourceComponent == null)
		{
			throw new ArgumentNullException("sourceComponent");
		}
		if (targetComponent == null)
		{
			throw new ArgumentNullException("targetComponent");
		}
		if (string.IsNullOrEmpty(sourceProperty))
		{
			throw new ArgumentNullException("sourceProperty");
		}
		if (string.IsNullOrEmpty(targetProperty))
		{
			throw new ArgumentNullException("targetProperty");
		}
		dfPropertyBinding obj = hostObject.AddComponent<dfPropertyBinding>();
		obj.DataSource = new dfComponentMemberInfo
		{
			Component = sourceComponent,
			MemberName = sourceProperty
		};
		obj.DataTarget = new dfComponentMemberInfo
		{
			Component = targetComponent,
			MemberName = targetProperty
		};
		obj.Bind();
		return obj;
	}

	public virtual bool CanSynchronize()
	{
		if (DataSource == null || DataTarget == null)
		{
			return false;
		}
		if (!DataSource.IsValid && !DataTarget.IsValid)
		{
			return false;
		}
		if (DataTarget.GetMemberType() != DataSource.GetMemberType())
		{
			return false;
		}
		return true;
	}

	public virtual void Bind()
	{
		if (isBound)
		{
			return;
		}
		if (!DataSource.IsValid || !DataTarget.IsValid)
		{
			Debug.LogError($"Invalid data binding configuration - Source:{DataSource}, Target:{DataTarget}");
			return;
		}
		sourceProperty = DataSource.GetProperty();
		targetProperty = DataTarget.GetProperty();
		isBound = sourceProperty != null && targetProperty != null;
		if (isBound)
		{
			if (targetProperty.PropertyType == typeof(string) && sourceProperty.PropertyType != typeof(string))
			{
				useFormatString = !string.IsNullOrEmpty(FormatString);
			}
			targetProperty.Value = formatValue(sourceProperty.Value);
		}
	}

	public virtual void Unbind()
	{
		if (isBound)
		{
			sourceProperty = null;
			targetProperty = null;
			isBound = false;
		}
	}

	private object formatValue(object value)
	{
		try
		{
			if (useFormatString && !string.IsNullOrEmpty(FormatString))
			{
				return string.Format(FormatString, value);
			}
		}
		catch (FormatException message)
		{
			Debug.LogError(message, this);
			if (Application.isPlaying)
			{
				base.enabled = false;
			}
		}
		return value;
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
