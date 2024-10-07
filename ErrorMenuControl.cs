using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class ErrorMenuControl : MonoBehaviour
{
	private bool _bugIssues;

	private bool _visualIssues;

	private bool _techIssues;

	private bool _otherIssues;

	private string _body = string.Empty;

	public bool BugIssues
	{
		get
		{
			return _bugIssues;
		}
		set
		{
			_bugIssues = value;
		}
	}

	public bool VisualIssues
	{
		get
		{
			return _visualIssues;
		}
		set
		{
			_visualIssues = value;
		}
	}

	public bool TechIssues
	{
		get
		{
			return _techIssues;
		}
		set
		{
			_techIssues = value;
		}
	}

	public bool OtherIssues
	{
		get
		{
			return _otherIssues;
		}
		set
		{
			_otherIssues = value;
		}
	}

	public string Body
	{
		get
		{
			return _body;
		}
		set
		{
			_body = value;
		}
	}

	private void Start()
	{
		dfControl component = GetComponent<dfControl>();
		component.BringToFront();
		component.Focus();
	}

	private void Update()
	{
	}

	public void Exit()
	{
		base.gameObject.transform.parent.gameObject.SetActive(value: false);
	}

	public void SendMessage()
	{
		MailMessage mailMessage = new MailMessage();
		mailMessage.From = new MailAddress("bugs@robotgentleman.com");
		mailMessage.To.Add("domx@robotgentleman.com");
		mailMessage.Subject = "60 Seconds! Report ";
		if (_bugIssues)
		{
			mailMessage.Subject += "[BUG]";
		}
		if (_visualIssues)
		{
			mailMessage.Subject += "[VISUALS]";
		}
		if (_techIssues)
		{
			mailMessage.Subject += "[TECH]";
		}
		if (_otherIssues)
		{
			mailMessage.Subject += "[OTHER]";
		}
		mailMessage.Body = _body;
		SmtpClient obj = new SmtpClient("smtp.gmail.com")
		{
			Port = 587,
			Credentials = new NetworkCredential("bot@robotgentleman.com", "termopile237"),
			EnableSsl = true
		};
		ServicePointManager.ServerCertificateValidationCallback = (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true;
		obj.Send(mailMessage);
		Exit();
	}
}
