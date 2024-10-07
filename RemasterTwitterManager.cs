using RG.Core.SaveSystem;
using RG.Parsecs.Scavenge;
using RG.SecondsRemaster.EventEditor;
using Twitter;
using UnityEngine;

public class RemasterTwitterManager : MonoBehaviour
{
	private enum TwitterErrorCodes
	{
		CouldNotAuthenticate = 32,
		UserNotFound = 50,
		UserSuspended = 63,
		UserSuspendedAndNotPermitted = 64,
		ClientNotPermittedThisAction = 87,
		InvalidOrExpiredToken = 89,
		UnableToVerifyCredentials = 99,
		BadAuthenticationData = 215,
		RateLimit = 88,
		TwitterIsOverCapacity = 130,
		PostingLimitationReached = 185,
		StatusIsDuplicate = 187
	}

	[SerializeField]
	private GlobalStringVariable _twitterUserId;

	[SerializeField]
	private GlobalStringVariable _twitterUserScreenName;

	[SerializeField]
	private GlobalStringVariable _twitterUserToken;

	[SerializeField]
	private GlobalStringVariable _twitterUserTokenSecret;

	public GenericEvent OnShowPINFieldRequested = new GenericEvent();

	public GenericEvent<bool> OnAccessTokenResponse = new GenericEvent<bool>();

	public string CONSUMER_KEY = "ZTIGErlLad0vDNU3FywuuAUh0";

	public string CONSUMER_SECRET = "jhrOOy3p3X9PaL1Ocuqo1I1D3xOousXwa5hyiudIYa2vleL51r ";

	private const string PLAYER_PREFS_TWITTER_USER_ID = "TwitterUserID";

	private const string PLAYER_PREFS_TWITTER_USER_SCREEN_NAME = "TwitterUserScreenName";

	private const string PLAYER_PREFS_TWITTER_USER_TOKEN = "TwitterUserToken";

	private const string PLAYER_PREFS_TWITTER_USER_TOKEN_SECRET = "TwitterUserTokenSecret";

	private RequestTokenResponse m_RequestTokenResponse;

	private AccessTokenResponse m_AccessTokenResponse;

	public bool IsLoggedIn { get; protected set; }

	private void Start()
	{
		LoadTwitterUserInfo();
	}

	public void LogIn()
	{
		StartCoroutine(API.GetRequestToken(CONSUMER_KEY, CONSUMER_SECRET, OnRequestTokenCallback));
	}

	public void EnterPin(string pin)
	{
		StartCoroutine(API.GetAccessToken(CONSUMER_KEY, CONSUMER_SECRET, m_RequestTokenResponse.Token, pin, OnAccessTokenCallback));
	}

	public void PostTweet(string text)
	{
		StartCoroutine(API.PostTweet(text, CONSUMER_KEY, CONSUMER_SECRET, m_AccessTokenResponse, OnPostTweet));
	}

	public void PostTweetGIF(string text, string path)
	{
		StartCoroutine(API.PostGIFTweet(path, text, CONSUMER_KEY, CONSUMER_SECRET, m_AccessTokenResponse, OnPostTweet));
	}

	public API.GIFTweet PrepareTweetGIF(string path)
	{
		return API.PrepareGIFTweet(path, CONSUMER_KEY, CONSUMER_SECRET, m_AccessTokenResponse, OnPostTweet);
	}

	private void LoadTwitterUserInfo()
	{
		m_AccessTokenResponse = new AccessTokenResponse();
		m_AccessTokenResponse.UserId = _twitterUserId.Value;
		m_AccessTokenResponse.ScreenName = _twitterUserScreenName.Value;
		m_AccessTokenResponse.Token = _twitterUserToken.Value;
		m_AccessTokenResponse.TokenSecret = _twitterUserTokenSecret.Value;
		if (!string.IsNullOrEmpty(m_AccessTokenResponse.Token) && !string.IsNullOrEmpty(m_AccessTokenResponse.ScreenName) && !string.IsNullOrEmpty(m_AccessTokenResponse.Token) && !string.IsNullOrEmpty(m_AccessTokenResponse.TokenSecret))
		{
			MonoBehaviour.print("LoadTwitterUserInfo - succeeded");
			IsLoggedIn = true;
		}
	}

	private void OnRequestTokenCallback(bool success, RequestTokenResponse response)
	{
		if (success)
		{
			MonoBehaviour.print("OnRequestTokenCallback - succeeded");
			m_RequestTokenResponse = response;
			OnShowPINFieldRequested.Invoke();
			API.OpenAuthorizationPage(response.Token);
		}
		else
		{
			MonoBehaviour.print("OnRequestTokenCallback - failed.");
		}
	}

	private void OnAccessTokenCallback(bool success, AccessTokenResponse response)
	{
		if (success)
		{
			MonoBehaviour.print("OnAccessTokenCallback - succeeded");
			m_AccessTokenResponse = response;
			_twitterUserId.Value = response.UserId;
			_twitterUserScreenName.Value = response.ScreenName;
			_twitterUserToken.Value = response.Token;
			_twitterUserTokenSecret.Value = response.TokenSecret;
			if (StorageDataManager.TheInstance != null)
			{
				StorageDataManager.TheInstance.Save("settings", delegate
				{
					Debug.Log("done");
				}, null);
			}
			IsLoggedIn = true;
		}
		else
		{
			MonoBehaviour.print("OnAccessTokenCallback - failed.");
		}
		OnAccessTokenResponse.Invoke(success);
	}

	private void OnPostTweet(bool success, int code)
	{
		if (!success)
		{
			switch ((TwitterErrorCodes)code)
			{
			case TwitterErrorCodes.CouldNotAuthenticate:
			case TwitterErrorCodes.UserNotFound:
			case TwitterErrorCodes.UserSuspended:
			case TwitterErrorCodes.UserSuspendedAndNotPermitted:
			case TwitterErrorCodes.ClientNotPermittedThisAction:
			case TwitterErrorCodes.InvalidOrExpiredToken:
			case TwitterErrorCodes.UnableToVerifyCredentials:
			case TwitterErrorCodes.BadAuthenticationData:
				UnlinkTwitter();
				break;
			}
		}
		MonoBehaviour.print("OnPostTweet - " + (success ? "succedded." : "failed."));
	}

	private void UnlinkTwitter()
	{
		IsLoggedIn = false;
		m_AccessTokenResponse = new AccessTokenResponse();
		_twitterUserId.Value = string.Empty;
		_twitterUserScreenName.Value = string.Empty;
		_twitterUserToken.Value = string.Empty;
		_twitterUserTokenSecret.Value = string.Empty;
		if (StorageDataManager.TheInstance != null)
		{
			StorageDataManager.TheInstance.Save("settings", delegate
			{
				Debug.Log("done");
			}, null);
		}
	}
}
