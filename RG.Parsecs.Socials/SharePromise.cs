using System;
using System.Collections;
using System.Collections.Generic;
using RG.Parsecs.Scavenge;
using UnityEngine;

namespace RG.Parsecs.Socials;

public class SharePromise
{
	public float Progress;

	public GenericEvent<SharePromise> OnSuccess = new GenericEvent<SharePromise>();

	public GenericEvent<SharePromise> OnFailure = new GenericEvent<SharePromise>();

	private List<SharePromise> _requirements = new List<SharePromise>();

	private Func<SharePromise, IEnumerator> WorkCoroutine;

	private Action<SharePromise> Work;

	private MonoBehaviour CoroutineHost;

	private bool _isProcessing;

	public bool IsFinished { get; protected set; }

	public SharePromise()
	{
	}

	public SharePromise(Action<SharePromise> work)
	{
		Work = work;
	}

	public SharePromise(Func<SharePromise, IEnumerator> work, MonoBehaviour coroutineHost)
	{
		WorkCoroutine = work;
		CoroutineHost = coroutineHost;
	}

	private void requiredSuccess(SharePromise req)
	{
		_requirements.Remove(req);
		if (_requirements.Count == 0)
		{
			Process();
		}
	}

	private void requiredFailure(SharePromise req)
	{
		Failure();
	}

	public void Require(SharePromise promise)
	{
		promise.OnSuccess.AddOneTime(requiredSuccess);
		promise.OnFailure.AddOneTime(requiredFailure);
		_requirements.Add(promise);
	}

	public void Process()
	{
		if (!_isProcessing)
		{
			_isProcessing = true;
			if (Work != null)
			{
				Work(this);
			}
			else if (WorkCoroutine != null)
			{
				CoroutineHost.StartCoroutine(WorkCoroutine(this));
			}
			else
			{
				Success();
			}
		}
	}

	public void Success()
	{
		if (!IsFinished)
		{
			IsFinished = true;
			OnSuccess.Invoke(this);
		}
	}

	public void Failure()
	{
		if (IsFinished)
		{
			return;
		}
		IsFinished = true;
		foreach (SharePromise requirement in _requirements)
		{
			requirement.OnSuccess.Remove(requiredSuccess);
			requirement.OnFailure.Remove(requiredFailure);
		}
		_requirements.Clear();
		OnFailure.Invoke(this);
	}

	public void Resolve(bool s)
	{
		if (s)
		{
			Success();
		}
		else
		{
			Failure();
		}
	}
}
