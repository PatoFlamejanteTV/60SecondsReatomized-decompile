using System.Diagnostics;

namespace DunGen;

public sealed class GenerationStats
{
	private Stopwatch stopwatch = new Stopwatch();

	private GenerationStatus generationStatus;

	public int MainPathRoomCount { get; private set; }

	public int BranchPathRoomCount { get; private set; }

	public int TotalRoomCount { get; private set; }

	public int MaxBranchDepth { get; private set; }

	public int TotalRetries { get; private set; }

	public float PreProcessTime { get; private set; }

	public float MainPathGenerationTime { get; private set; }

	public float BranchPathGenerationTime { get; private set; }

	public float PostProcessTime { get; private set; }

	public float TotalTime { get; private set; }

	internal void Clear()
	{
		int num2 = (TotalRetries = 0);
		int num4 = (MaxBranchDepth = num2);
		int num6 = (TotalRoomCount = num4);
		int mainPathRoomCount = (BranchPathRoomCount = num6);
		MainPathRoomCount = mainPathRoomCount;
		float num9 = (TotalTime = 0f);
		float num11 = (PostProcessTime = num9);
		float num13 = (BranchPathGenerationTime = num11);
		float preProcessTime = (MainPathGenerationTime = num13);
		PreProcessTime = preProcessTime;
	}

	internal void IncrementRetryCount()
	{
		TotalRetries++;
	}

	internal void SetRoomStatistics(int mainPathRoomCount, int branchPathRoomCount, int maxBranchDepth)
	{
		MainPathRoomCount = mainPathRoomCount;
		BranchPathRoomCount = branchPathRoomCount;
		MaxBranchDepth = maxBranchDepth;
		TotalRoomCount = MainPathRoomCount + BranchPathRoomCount;
	}

	internal void BeginTime(GenerationStatus status)
	{
		if (stopwatch.IsRunning)
		{
			EndTime();
		}
		generationStatus = status;
		stopwatch.Reset();
		stopwatch.Start();
	}

	internal void EndTime()
	{
		stopwatch.Stop();
		float num = (float)stopwatch.Elapsed.TotalMilliseconds;
		switch (generationStatus)
		{
		case GenerationStatus.PreProcessing:
			PreProcessTime += num;
			break;
		case GenerationStatus.MainPath:
			MainPathGenerationTime += num;
			break;
		case GenerationStatus.Branching:
			BranchPathGenerationTime += num;
			break;
		case GenerationStatus.PostProcessing:
			PostProcessTime += num;
			break;
		}
		TotalTime = PreProcessTime + MainPathGenerationTime + BranchPathGenerationTime + PostProcessTime;
	}

	public GenerationStats Clone()
	{
		return new GenerationStats
		{
			MainPathRoomCount = MainPathRoomCount,
			BranchPathRoomCount = BranchPathRoomCount,
			TotalRoomCount = TotalRoomCount,
			MaxBranchDepth = MaxBranchDepth,
			TotalRetries = TotalRetries,
			PreProcessTime = PreProcessTime,
			MainPathGenerationTime = MainPathGenerationTime,
			BranchPathGenerationTime = BranchPathGenerationTime,
			PostProcessTime = PostProcessTime,
			TotalTime = TotalTime
		};
	}
}
