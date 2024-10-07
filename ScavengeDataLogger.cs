using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using DunGen;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScavengeDataLogger : MonoBehaviour
{
	public static ScavengeDataLogger Instance = null;

	[SerializeField]
	private float _playerMovementFrequency = 0.1f;

	private bool _terminate;

	private string _login = string.Empty;

	private float _playtime;

	private EGameType _gameType;

	private int _runTime;

	private int _prepTime;

	private float _startTime;

	private float _endTime;

	private Vector3 _shelterPosition = Vector3.zero;

	private string _levelName;

	private bool _forcedHouse;

	private float _scavengeFinishedTime;

	private List<SObjectDataLog> _itemCollectedDataLog = new List<SObjectDataLog>(40);

	private List<SObjectDataLog> _collisionDataLog = new List<SObjectDataLog>();

	private List<float> _dropDataLog = new List<float>();

	private List<SBaseDataLog> _playerDataLog = new List<SBaseDataLog>(600);

	private SObjectDataLog[] _itemDataLog;

	private SObjectDataLog[] _roomDataLog;

	private static string _version = "0004";

	public bool Terminate
	{
		get
		{
			return _terminate;
		}
		set
		{
			_terminate = value;
		}
	}

	private void Awake()
	{
		Instance = this;
		if (!Settings.Data.DataLogging)
		{
			UnityEngine.Object.Destroy(this);
		}
	}

	private void Start()
	{
		StartLog();
	}

	public void Save(bool gameFinished, bool cloud, bool async)
	{
		try
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(Settings.Data.DataFilepath);
			if (!directoryInfo.Exists)
			{
				directoryInfo.Create();
			}
			string text = SaveFile(Settings.Data.DataFilepath, gameFinished);
			if (!string.IsNullOrEmpty(text))
			{
				if (cloud)
				{
					StartCoroutine(SendToFirebase(text));
				}
				else if (async)
				{
					StartCoroutine(SendToWWWAsync(text));
				}
				else
				{
					SendToWWW(text);
				}
			}
		}
		catch (Exception)
		{
		}
		UnityEngine.Object.Destroy(this, 10f);
	}

	public string SaveFile(string filepath, bool gameFinished)
	{
		string text = filepath + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + _login + ".txt";
		using StreamWriter streamWriter = new StreamWriter(text);
		streamWriter.Write(GenerateLog(gameFinished));
		return text;
	}

	private IEnumerator SendToFirebase(string filepath)
	{
		filepath.Substring(filepath.LastIndexOf('/'));
		WWW www = new WWW("hosting6112512.az.pl/get_time.php");
		yield return www;
		_ = www.text;
	}

	private void SendToWWW(string filepath)
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("action", "log upload");
		wWWForm.AddField("file", "file");
		byte[] contents = File.ReadAllBytes(filepath);
		wWWForm.AddBinaryData("file", contents, Path.GetFileName(filepath), "text/plain");
		if (new WWW("hosting6112512.az.pl/dataupload.php", wWWForm).error == null)
		{
			File.Delete(filepath);
		}
	}

	private IEnumerator SendToWWWAsync(string filepath)
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("action", "log upload");
		wWWForm.AddField("file", "file");
		byte[] contents = File.ReadAllBytes(filepath);
		wWWForm.AddBinaryData("file", contents, Path.GetFileName(filepath), "text/plain");
		WWW w = new WWW("hosting6112512.az.pl/dataupload.php", wWWForm);
		yield return w;
		if (w.error == null)
		{
			File.Delete(filepath);
		}
	}

	public void SendToFtp(string folderName, string fileName, string user, string pass)
	{
		try
		{
			string fileName2 = Path.GetFileName(fileName);
			FtpWebRequest ftpWebRequest = WebRequest.Create(new Uri(string.Format("ftp://{0}/{1}/{2}", "ftp.hosting6112512.az.pl", folderName, fileName2))) as FtpWebRequest;
			ftpWebRequest.Method = "STOR";
			ftpWebRequest.UseBinary = true;
			ftpWebRequest.UsePassive = true;
			ftpWebRequest.KeepAlive = true;
			ftpWebRequest.Credentials = new NetworkCredential(user, pass);
			ftpWebRequest.ConnectionGroupName = "group";
			using FileStream fileStream = File.OpenRead(fileName);
			byte[] array = new byte[fileStream.Length];
			fileStream.Read(array, 0, array.Length);
			fileStream.Close();
			Stream requestStream = ftpWebRequest.GetRequestStream();
			requestStream.Write(array, 0, array.Length);
			requestStream.Flush();
			requestStream.Close();
		}
		catch (Exception)
		{
		}
	}

	public void StartLog()
	{
		_levelName = SceneManager.GetActiveScene().name;
		_forcedHouse = GlobalTools.GetController<GameFlow>().ForcedHouse;
		LogIdData();
		LogSetupData();
		StartCoroutine(LogPlayerData(_playerMovementFrequency));
	}

	public void EndLog(bool gameFinished, bool cloud, bool async)
	{
		_scavengeFinishedTime = GameSessionData.Instance.ScavengeFinishedTime;
		_terminate = true;
		Save(gameFinished, cloud, async);
	}

	private string GenerateLog(bool gameFinished)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(_version);
		stringBuilder.Append(';');
		stringBuilder.Append(DateTime.Now.Year.ToString() + DateTime.Now.Month + DateTime.Now.Day);
		stringBuilder.Append(';');
		stringBuilder.Append(_login);
		stringBuilder.Append(';');
		stringBuilder.Append(_levelName);
		stringBuilder.Append(';');
		stringBuilder.Append(gameFinished);
		stringBuilder.Append(';');
		stringBuilder.Append(_playtime.ToString());
		stringBuilder.Append(';');
		stringBuilder.Append(_forcedHouse ? 1 : 0);
		stringBuilder.Append(';');
		stringBuilder.Append(_gameType);
		stringBuilder.Append(';');
		stringBuilder.Append(_prepTime);
		stringBuilder.Append(';');
		stringBuilder.Append(_runTime);
		stringBuilder.Append(';');
		stringBuilder.Append(_scavengeFinishedTime);
		stringBuilder.Append(';');
		stringBuilder.Append(_shelterPosition.ToString());
		stringBuilder.Append(';');
		stringBuilder.Append(_endTime);
		stringBuilder.Append(';');
		stringBuilder.Append("ROOMS");
		stringBuilder.Append(';');
		stringBuilder.Append((_roomDataLog != null) ? _roomDataLog.Length : 0);
		stringBuilder.Append(';');
		if (_roomDataLog != null)
		{
			for (int i = 0; i < _roomDataLog.Length; i++)
			{
				stringBuilder.Append(_roomDataLog[i].GetString(';'));
				stringBuilder.Append(';');
			}
		}
		stringBuilder.Append("ITEMS");
		stringBuilder.Append(';');
		stringBuilder.Append((_itemDataLog != null) ? _itemDataLog.Length : 0);
		stringBuilder.Append(';');
		if (_itemDataLog != null)
		{
			for (int j = 0; j < _itemDataLog.Length; j++)
			{
				stringBuilder.Append(_itemDataLog[j].GetString(';'));
				stringBuilder.Append(';');
			}
		}
		stringBuilder.Append("MOVEMENT");
		stringBuilder.Append(';');
		stringBuilder.Append(_startTime);
		stringBuilder.Append(';');
		stringBuilder.Append(_playerDataLog.Count);
		stringBuilder.Append(';');
		for (int k = 0; k < _playerDataLog.Count; k++)
		{
			stringBuilder.Append(_playerDataLog[k].GetString(';'));
			stringBuilder.Append(';');
		}
		stringBuilder.Append("COLLECTED");
		stringBuilder.Append(';');
		stringBuilder.Append(_itemCollectedDataLog.Count);
		stringBuilder.Append(';');
		for (int l = 0; l < _itemCollectedDataLog.Count; l++)
		{
			stringBuilder.Append(_itemCollectedDataLog[l].GetString(';'));
			stringBuilder.Append(';');
		}
		stringBuilder.Append("DROPS");
		stringBuilder.Append(';');
		stringBuilder.Append(_dropDataLog.Count);
		stringBuilder.Append(';');
		for (int m = 0; m < _dropDataLog.Count; m++)
		{
			stringBuilder.Append(_dropDataLog[m].ToString());
			stringBuilder.Append(';');
		}
		stringBuilder.Append("COLLISIONS");
		stringBuilder.Append(';');
		stringBuilder.Append(_collisionDataLog.Count);
		stringBuilder.Append(';');
		for (int n = 0; n < _collisionDataLog.Count; n++)
		{
			stringBuilder.Append(_collisionDataLog[n].GetString(';'));
			stringBuilder.Append(';');
		}
		return stringBuilder.ToString();
	}

	public void LogItemPickupData(string id, float time, Vector3 pos, Quaternion rot)
	{
		_itemCollectedDataLog.Add(new SObjectDataLog(time, pos, rot, id));
	}

	public void LogDropData(float time)
	{
		_dropDataLog.Add(time);
	}

	public void LogCollisionData(string id, float time, Vector3 pos, Quaternion rot)
	{
		_collisionDataLog.Add(new SObjectDataLog(time, pos, rot, id));
	}

	public void LogIdData()
	{
		_login = string.Empty;
		_login = SteamUser.GetSteamID().m_SteamID.ToString();
		_playtime = Settings.Data.PlayerProfile.GetRecord<float>("Playtime") + Time.time;
	}

	public void LogSetupData()
	{
		_gameType = GameSessionData.Instance.Setup.GameType;
		_prepTime = GameSessionData.Instance.GetCurrentDifficulty().PrepareTime;
		_runTime = GameSessionData.Instance.GetCurrentDifficulty().ScavengeTime;
		_startTime = Time.time;
		Tile[] array = UnityEngine.Object.FindObjectsOfType<Tile>();
		if (array != null)
		{
			_roomDataLog = new SObjectDataLog[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				Transform transform = array[i].gameObject.transform;
				_roomDataLog[i] = new SObjectDataLog(Time.time, transform.position, transform.rotation, array[i].gameObject.name);
			}
		}
		ScavengeItemController[] array2 = UnityEngine.Object.FindObjectsOfType<ScavengeItemController>();
		if (array2 != null)
		{
			_itemDataLog = new SObjectDataLog[array2.Length];
			for (int j = 0; j < array2.Length; j++)
			{
				Transform transform2 = array2[j].gameObject.transform;
				_itemDataLog[j] = new SObjectDataLog(Time.time, transform2.position, transform2.rotation, array2[j].SurvivalName);
			}
		}
		Shelter shelter = GlobalTools.GetShelter();
		if (shelter != null)
		{
			_shelterPosition = shelter.gameObject.transform.position;
		}
	}

	public IEnumerator LogPlayerData(float frequency)
	{
		GameObject player = GlobalTools.GetPlayer();
		Transform playerTransform = null;
		if (player == null)
		{
			_terminate = true;
		}
		else
		{
			playerTransform = player.transform;
		}
		while (!_terminate)
		{
			_playerDataLog.Add(new SBaseDataLog(Time.time, playerTransform.position, playerTransform.rotation));
			yield return new WaitForSeconds(frequency);
		}
		_endTime = Time.time;
	}
}
