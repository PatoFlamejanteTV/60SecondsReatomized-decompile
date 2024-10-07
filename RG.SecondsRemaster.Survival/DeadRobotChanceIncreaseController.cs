using System;
using RG.Parsecs.EventEditor;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

public class DeadRobotChanceIncreaseController : MonoBehaviour
{
	[SerializeField]
	private GlobalBoolVariable _deadRobotChanceIncrease;

	private void Start()
	{
		_deadRobotChanceIncrease.Value = DateTime.Today.DayOfWeek == DayOfWeek.Thursday;
	}
}
