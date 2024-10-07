using System;
using System.Collections.Generic;
using Rewired;
using UnityEngine;

public class SurvivalPromptsController : MonoBehaviour
{
	[NonSerialized]
	public bool IsShowingPrompts;

	private List<SurvivalPrompt> _prompts = new List<SurvivalPrompt>();

	private Player _player;

	public static SurvivalPromptsController Instance { get; private set; }

	public SurvivalPromptsController()
	{
		Instance = this;
	}

	private void Awake()
	{
		_player = ReInput.players.GetPlayer(0);
	}

	private void Update()
	{
		if (_player.GetButtonDown(41))
		{
			StartShowingPrompts();
		}
		if (_player.GetButtonUp(41))
		{
			StopShowingPrompts();
		}
	}

	private void StartShowingPrompts()
	{
		IsShowingPrompts = true;
		for (int i = 0; i < _prompts.Count; i++)
		{
			_prompts[i].Show();
		}
	}

	private void StopShowingPrompts()
	{
		IsShowingPrompts = false;
		for (int i = 0; i < _prompts.Count; i++)
		{
			_prompts[i].Hide();
		}
	}

	public void RegisterPrompt(SurvivalPrompt prompt)
	{
		if (!_prompts.Contains(prompt))
		{
			_prompts.Add(prompt);
			if (IsShowingPrompts)
			{
				prompt.Show();
			}
		}
	}

	public void UnregisterPrompt(SurvivalPrompt prompt)
	{
		if (_prompts.Contains(prompt))
		{
			_prompts.Remove(prompt);
			prompt.Hide();
		}
	}
}
