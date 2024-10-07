using System;
using System.Collections;
using Rewired;
using RG.Parsecs.Common;
using Twitter;
using UnityEngine;
using UnityToolbag;

namespace RG.Parsecs.Socials;

public class RemasterShare2SocialsManager : MonoBehaviour
{
	public GrabRecorderDataSystem GrabRecorderDataSystem;

	public RemasterTwitterManager TwitterManager;

	public GIFPreviewWindow GifPreviewWindow;

	public Achievement SpaceSelfieAchievement;

	[SerializeField]
	private bool _useDefaultFbAppId = true;

	[SerializeField]
	private string _customFbAppId;

	[SerializeField]
	private bool _useDefaultImgurTitle = true;

	[SerializeField]
	private string _customImgurTitle;

	public string DefaultTweet = "60 Parsecs!";

	private bool _saved;

	private Player _player;

	private string _url;

	private string _link;

	private SharePromise _facebookPromise;

	private API.GIFTweet _gifTweet;

	private string _text;

	private Action<bool> _textIsReady;

	private SharePromise _shareToTwitter;

	private SharePromise logInPromise;

	private void Start()
	{
		_player = ReInput.players.GetPlayer(0);
		if (GrabRecorderDataSystem.Ready)
		{
			GifPreviewWindow.FacebookButtonInteractable = true;
			GifPreviewWindow.TwitterButtonInteractable = true;
			GifPreviewWindow.SaveButtonInteractable = true;
			GifPreviewWindow.SetFrames(GrabRecorderDataSystem.Frames);
		}
		else
		{
			GifPreviewWindow.FacebookButtonInteractable = false;
			GifPreviewWindow.TwitterButtonInteractable = false;
			GifPreviewWindow.SaveButtonInteractable = false;
			GrabRecorderDataSystem.OnReady.Add(OnReady);
			GrabRecorderDataSystem.OnProgressChanged.Add(OnProgressChanged);
			GifPreviewWindow.SetProgress(GrabRecorderDataSystem.Progress);
		}
	}

	public void Update()
	{
		if (_player != null && _player.GetButtonDown(30))
		{
			Hide();
		}
	}

	private void OnProgressChanged(float p)
	{
		Dispatcher.InvokeAsync(delegate
		{
			GifPreviewWindow.SetProgress(p);
		});
	}

	private void OnReady()
	{
		Dispatcher.InvokeAsync(delegate
		{
			GifPreviewWindow.FacebookButtonInteractable = true;
			GifPreviewWindow.TwitterButtonInteractable = true;
			GifPreviewWindow.SaveButtonInteractable = true;
			GifPreviewWindow.SetProgress(1f);
			GifPreviewWindow.SetFrames(GrabRecorderDataSystem.Frames);
		});
	}

	private void SetUrl(string url)
	{
		_url = url;
	}

	public void ShareAdventure()
	{
		GifPreviewWindow.Show();
	}

	public void Hide()
	{
		GifPreviewWindow.Hide();
	}

	private void Imgur_OnImageUploadProgress(object sender, ImgurClient.OnImageUploadProgressEventArgs e)
	{
	}

	private void Imgur_OnImageUploaded(object sender, ImgurClient.OnImageUploadedEventArgs e)
	{
	}

	public void ShareToFacebook()
	{
		if (_facebookPromise != null)
		{
			return;
		}
		_facebookPromise = new SharePromise();
		_facebookPromise.OnFailure.AddOneTime(delegate
		{
			_facebookPromise = null;
			GifPreviewWindow.FacebookSlider.value = 0f;
		});
		_facebookPromise.OnSuccess.AddOneTime(delegate
		{
			if (SpaceSelfieAchievement != null)
			{
				AchievementsSystem.UnlockAchievement(SpaceSelfieAchievement);
			}
		});
		ImgurClient imgurClient = ((!_useDefaultImgurTitle) ? new ImgurClient("5907300c80d376b", "tHeuZVItCgmsh7IdhrliwZfOlA4Dp1PyR6FjsnnIk71hzAsHUT", _customImgurTitle) : new ImgurClient("5907300c80d376b", "tHeuZVItCgmsh7IdhrliwZfOlA4Dp1PyR6FjsnnIk71hzAsHUT"));
		imgurClient.OnImageUploaded += delegate(object sender, ImgurClient.OnImageUploadedEventArgs e)
		{
			if (e.success)
			{
				if (_useDefaultFbAppId)
				{
					Dispatcher.InvokeAsync(delegate
					{
						Facebook.Share(e.response.data.link, "Name", "Caption", "Description", e.response.data.link, "http://www.facebook.com/");
						GifPreviewWindow.FacebookSlider.value = (_facebookPromise.Progress = 1f);
						_facebookPromise.Success();
					});
				}
				else
				{
					Dispatcher.InvokeAsync(delegate
					{
						Facebook.Share(_customFbAppId, e.response.data.link, "Name", "Caption", "Description", e.response.data.link, "http://www.facebook.com/");
						GifPreviewWindow.FacebookSlider.value = (_facebookPromise.Progress = 1f);
						_facebookPromise.Success();
					});
				}
			}
			else
			{
				Dispatcher.InvokeAsync(delegate
				{
					_facebookPromise.Failure();
				});
			}
		};
		imgurClient.OnImageUploadProgress += delegate(object sender, ImgurClient.OnImageUploadProgressEventArgs e)
		{
			Dispatcher.InvokeAsync(delegate
			{
				if (e.progress > _facebookPromise.Progress)
				{
					_facebookPromise.Progress = e.progress;
					GifPreviewWindow.FacebookSlider.value = e.progress;
				}
			});
		};
		GifPreviewWindow.FacebookSlider.value = (_facebookPromise.Progress = 0.05f);
		if (!imgurClient.UploadImageFromFilePath(GrabRecorderDataSystem.Path))
		{
			_facebookPromise.Failure();
		}
		StartCoroutine(_facebookFakeProgress(_facebookPromise));
	}

	private IEnumerator _facebookFakeProgress(SharePromise promise)
	{
		while (promise != null && !promise.IsFinished && promise.Progress < 0.5f)
		{
			promise.Progress += Time.deltaTime * (1f / 15f);
			promise.Progress = Mathf.Min(promise.Progress, 1f);
			GifPreviewWindow.FacebookSlider.value = promise.Progress;
			yield return null;
		}
		while (promise != null && !promise.IsFinished && promise.Progress < 0.7f)
		{
			promise.Progress += Time.deltaTime * (1f / 30f);
			promise.Progress = Mathf.Min(promise.Progress, 1f);
			GifPreviewWindow.FacebookSlider.value = promise.Progress;
			yield return null;
		}
		while (promise != null && !promise.IsFinished && promise.Progress < 0.8f)
		{
			promise.Progress += Time.deltaTime * 0.02f;
			promise.Progress = Mathf.Min(promise.Progress, 1f);
			GifPreviewWindow.FacebookSlider.value = promise.Progress;
			yield return null;
		}
		while (promise != null && !promise.IsFinished && promise.Progress < 0.9f)
		{
			promise.Progress += Time.deltaTime * 0.0125f;
			promise.Progress = Mathf.Min(promise.Progress, 1f);
			GifPreviewWindow.FacebookSlider.value = promise.Progress;
			yield return null;
		}
	}

	private void WaitForLogIn(SharePromise promise)
	{
		TwitterManager.OnShowPINFieldRequested.AddOneTime(ShowTwitterPinMenu);
		TwitterManager.OnAccessTokenResponse.AddOneTime(promise.Resolve);
		TwitterManager.LogIn();
	}

	private void ShowTwitterPinMenu()
	{
		GifPreviewWindow.ShowTwitterPinMenu();
	}

	public void PinCancelled()
	{
		if (logInPromise != null)
		{
			logInPromise.Failure();
		}
		GifPreviewWindow.HideTwitterPinMenu();
	}

	public void PinConfirmed()
	{
		TwitterManager.EnterPin(GifPreviewWindow.TwitterPin.text);
		GifPreviewWindow.HideTwitterPinMenu();
	}

	private void ShowTwitterTextMenu()
	{
		GifPreviewWindow.ShowTwitterTextMenu();
	}

	public void NotifyTextIsReady()
	{
		_text = GifPreviewWindow.TweetText.text;
		if (string.IsNullOrEmpty(_text))
		{
			_text = DefaultTweet;
		}
		if (_textIsReady != null)
		{
			_textIsReady(obj: true);
		}
		GifPreviewWindow.HideTwitterTextMenu();
	}

	public void TextCancelled()
	{
		if (_textIsReady != null)
		{
			_textIsReady(obj: false);
		}
		GifPreviewWindow.HideTwitterTextMenu();
	}

	private IEnumerator WaitForUpload(SharePromise promise)
	{
		_gifTweet = TwitterManager.PrepareTweetGIF(GrabRecorderDataSystem.Path);
		_gifTweet.OnUploadProgressChanged.Add(delegate(float p)
		{
			if (p > promise.Progress)
			{
				promise.Progress = p;
				GifPreviewWindow.TwitterSlider.value = p;
			}
		});
		StartCoroutine(_twitterFakeProgress(promise));
		_gifTweet.OnUploadFinished.AddOneTime(promise.Resolve);
		yield return StartCoroutine(_gifTweet.Upload());
	}

	private IEnumerator _twitterFakeProgress(SharePromise promise)
	{
		while (promise != null && !promise.IsFinished && promise.Progress < 0.5f)
		{
			promise.Progress += Time.deltaTime * 0.1f;
			promise.Progress = Mathf.Min(promise.Progress, 1f);
			GifPreviewWindow.TwitterSlider.value = promise.Progress;
			yield return null;
		}
		while (promise != null && !promise.IsFinished && promise.Progress < 0.7f)
		{
			promise.Progress += Time.deltaTime * 0.05f;
			promise.Progress = Mathf.Min(promise.Progress, 1f);
			GifPreviewWindow.TwitterSlider.value = promise.Progress;
			yield return null;
		}
		while (promise != null && !promise.IsFinished && promise.Progress < 0.8f)
		{
			promise.Progress += Time.deltaTime * 0.025f;
			promise.Progress = Mathf.Min(promise.Progress, 1f);
			GifPreviewWindow.TwitterSlider.value = promise.Progress;
			yield return null;
		}
		while (promise != null && !promise.IsFinished && promise.Progress < 0.9f)
		{
			promise.Progress += Time.deltaTime * 0.0125f;
			promise.Progress = Mathf.Min(promise.Progress, 1f);
			GifPreviewWindow.TwitterSlider.value = promise.Progress;
			yield return null;
		}
	}

	private void WaitForText(SharePromise promise)
	{
		_textIsReady = promise.Resolve;
		ShowTwitterTextMenu();
	}

	private void Tweet(SharePromise promise)
	{
		_gifTweet.OnTweetPosted.AddOneTime(promise.Resolve);
		StartCoroutine(_gifTweet.Tweet(_text));
	}

	public void ShareToTwitter()
	{
		if (_shareToTwitter != null)
		{
			return;
		}
		_shareToTwitter = new SharePromise(Tweet);
		_shareToTwitter.OnSuccess.AddOneTime(delegate
		{
			GifPreviewWindow.TwitterSlider.value = 1f;
			if (SpaceSelfieAchievement != null)
			{
				AchievementsSystem.UnlockAchievement(SpaceSelfieAchievement);
			}
		});
		_shareToTwitter.OnFailure.AddOneTime(delegate
		{
			_shareToTwitter = null;
			GifPreviewWindow.TwitterSlider.value = 0f;
			GifPreviewWindow.HideTwitterTextMenu();
			GifPreviewWindow.HideTwitterPinMenu();
		});
		SharePromise uploadPromise = new SharePromise(WaitForUpload, this);
		_shareToTwitter.Require(uploadPromise);
		SharePromise sharePromise = new SharePromise(WaitForText);
		_shareToTwitter.Require(sharePromise);
		sharePromise.OnFailure.AddOneTime(delegate
		{
			uploadPromise.Failure();
		});
		uploadPromise.OnFailure.AddOneTime(delegate
		{
			GifPreviewWindow.HideTwitterTextMenu();
		});
		if (!TwitterManager.IsLoggedIn)
		{
			logInPromise = new SharePromise(WaitForLogIn);
			uploadPromise.Require(logInPromise);
			sharePromise.Require(logInPromise);
			logInPromise.Process();
		}
		else
		{
			uploadPromise.Process();
			sharePromise.Process();
		}
	}

	public void Save()
	{
		if (!_saved)
		{
			StartCoroutine(_SaveFakeProgress());
		}
		_saved = true;
		OpenFileLocation.Open(GrabRecorderDataSystem.Path);
	}

	private IEnumerator _SaveFakeProgress()
	{
		float progress = 0f;
		while (progress < 1f)
		{
			progress += Time.deltaTime * 3f;
			progress = Mathf.Min(progress, 1f);
			GifPreviewWindow.SaveSlider.value = progress;
			yield return null;
		}
	}

	private void OnDestroy()
	{
		if (!_saved)
		{
			GrabRecorderDataSystem.DeleteGIF();
		}
		else
		{
			GrabRecorderDataSystem.Clear();
		}
	}
}
