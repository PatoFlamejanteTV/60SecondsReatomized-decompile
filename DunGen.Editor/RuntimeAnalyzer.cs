using System.Diagnostics;
using System.Text;
using DunGen.Analysis;
using DunGen.Graph;
using UnityEngine;

namespace DunGen.Editor;

[AddComponentMenu("DunGen/Analysis/Runtime Analyzer")]
public sealed class RuntimeAnalyzer : MonoBehaviour
{
	public DungeonFlow DungeonFlow;

	public int Iterations = 100;

	public int MaxFailedAttempts = 20;

	public bool RunOnStart = true;

	public float MaximumAnalysisTime;

	public float PerFrameAnalysisTime = 0.1f;

	private DungeonGenerator generator = new DungeonGenerator();

	private GenerationAnalysis analysis;

	private StringBuilder infoText = new StringBuilder();

	private int targetIterations;

	private int currentIterations;

	private double analysisTime;

	private bool finishedEarly;

	private bool prevShouldRandomizeSeed;

	private void Start()
	{
		if (RunOnStart)
		{
			Analyze();
		}
	}

	public void Analyze()
	{
		bool flag = false;
		if (DungeonFlow == null)
		{
			UnityEngine.Debug.LogError("No DungeonFlow assigned to analyzer");
		}
		else if (Iterations <= 0)
		{
			UnityEngine.Debug.LogError("Iteration count must be greater than 0");
		}
		else if (MaxFailedAttempts <= 0)
		{
			UnityEngine.Debug.LogError("Max failed attempt count must be greater than 0");
		}
		else
		{
			flag = true;
		}
		if (flag)
		{
			prevShouldRandomizeSeed = generator.ShouldRandomizeSeed;
			generator.DungeonFlow = DungeonFlow;
			generator.MaxAttemptCount = MaxFailedAttempts;
			generator.ShouldRandomizeSeed = true;
			analysis = new GenerationAnalysis(Iterations);
			analysisTime = 0.0;
			currentIterations = 0;
			targetIterations = Iterations;
		}
	}

	private void Update()
	{
		if (targetIterations <= 0)
		{
			return;
		}
		Stopwatch stopwatch = Stopwatch.StartNew();
		int num = 0;
		int num2 = targetIterations - currentIterations;
		for (int i = 0; i < num2; i++)
		{
			if (stopwatch.Elapsed.TotalSeconds >= (double)PerFrameAnalysisTime)
			{
				break;
			}
			if (generator.Generate())
			{
				analysis.IncrementSuccessCount();
				analysis.Add(generator.GenerationStats);
			}
			currentIterations++;
			num++;
		}
		analysisTime += stopwatch.Elapsed.TotalSeconds;
		if (MaximumAnalysisTime > 0f && analysisTime >= (double)MaximumAnalysisTime)
		{
			targetIterations = currentIterations;
			finishedEarly = true;
		}
		if (currentIterations >= targetIterations)
		{
			targetIterations = 0;
			analysis.Analyze();
			Object.Destroy(generator.Root);
			OnAnalysisComplete();
		}
	}

	private void OnAnalysisComplete()
	{
		generator.ShouldRandomizeSeed = prevShouldRandomizeSeed;
		infoText.Length = 0;
		UnityEngine.Debug.Log(analysis.MaxBranchDepth);
		if (finishedEarly)
		{
			infoText.AppendLine("[ Reached maximum analysis time before the target number of iterations was reached ]");
		}
		infoText.AppendFormat("Iterations: {0}, Max Failed Attempts: {1}", finishedEarly ? analysis.IterationCount : analysis.TargetIterationCount, MaxFailedAttempts);
		infoText.AppendFormat("\nTotal Analysis Time: {0:0.00} seconds", analysisTime);
		infoText.AppendFormat("\nDungeons successfully generated: {0}% ({1} failed)", Mathf.RoundToInt(analysis.SuccessPercentage), analysis.TargetIterationCount - analysis.SuccessCount);
		infoText.AppendLine();
		infoText.AppendLine();
		infoText.Append("## TIME TAKEN (in milliseconds) ##");
		infoText.AppendFormat("\n\tPre-Processing:\t\t\t\t\t{0}", analysis.PreProcessTime);
		infoText.AppendFormat("\n\tMain Path Generation:\t\t{0}", analysis.MainPathGenerationTime);
		infoText.AppendFormat("\n\tBranch Path Generation:\t\t{0}", analysis.BranchPathGenerationTime);
		infoText.AppendFormat("\n\tPost-Processing:\t\t\t\t{0}", analysis.PostProcessTime);
		infoText.Append("\n\t-------------------------------------------------------");
		infoText.AppendFormat("\n\tTotal:\t\t\t\t\t\t\t\t{0}", analysis.TotalTime);
		infoText.AppendLine();
		infoText.AppendLine();
		infoText.AppendLine("## ROOM DATA ##");
		infoText.AppendFormat("\n\tMain Path Rooms: {0}", analysis.MainPathRoomCount);
		infoText.AppendFormat("\n\tBranch Path Rooms: {0}", analysis.BranchPathRoomCount);
		infoText.Append("\n\t-------------------");
		infoText.AppendFormat("\n\tTotal: {0}", analysis.TotalRoomCount);
		infoText.AppendLine();
		infoText.AppendLine();
		infoText.AppendFormat("Retry Count: {0}", analysis.TotalRetries);
	}

	private void OnGUI()
	{
		if (analysis == null || infoText == null || infoText.Length == 0)
		{
			string text = ((analysis.SuccessCount < analysis.IterationCount) ? ("\nFailed Dungeons: " + (analysis.IterationCount - analysis.SuccessCount)) : "");
			GUILayout.Label($"Analyzing... {currentIterations} / {targetIterations} ({(float)currentIterations / (float)targetIterations * 100f:0.0}%){text}");
		}
		else
		{
			GUILayout.Label(infoText.ToString());
		}
	}
}
