using TMPro;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

public class GoalJournalContentDisplayer : JournalContentDisplayer<GoalJournalContent>
{
	[SerializeField]
	private TextMeshProUGUI _textField;

	[SerializeField]
	private GameObject[] _done;

	[SerializeField]
	private GameObject[] _fail;

	[SerializeField]
	private GameObject[] _brackets;

	public TextMeshProUGUI TextField => _textField;

	public override int LinesAmount => _textField.textInfo.lineCount;

	public override void SetContentData(JournalContent content)
	{
		if (content.Type == EJournalContentType.GOAL)
		{
			GoalJournalContent goalJournalContent = (GoalJournalContent)content;
			_textField.text = goalJournalContent.Term;
			goalJournalContent.CheckmarkIndex = Random.Range(0, _done.Length);
			_brackets[goalJournalContent.CheckmarkIndex].SetActive(value: true);
			if (goalJournalContent.IsAchieved)
			{
				_done[goalJournalContent.CheckmarkIndex].SetActive(value: true);
			}
			else if (goalJournalContent.IsFailed)
			{
				_fail[goalJournalContent.CheckmarkIndex].SetActive(value: true);
			}
			else if (!goalJournalContent.IsFailed && !goalJournalContent.IsAchieved)
			{
				_fail[goalJournalContent.CheckmarkIndex].SetActive(value: false);
				_done[goalJournalContent.CheckmarkIndex].SetActive(value: false);
			}
			ForceTextMeshProUpdate();
		}
	}

	private void ForceTextMeshProUpdate()
	{
		Canvas.ForceUpdateCanvases();
		_textField.ForceMeshUpdate();
	}

	public void AttachPreviousTextMeshPro(TextMeshProUGUI previousTmp)
	{
		previousTmp.overflowMode = TextOverflowModes.Linked;
		previousTmp.linkedTextComponent = _textField;
		Canvas.ForceUpdateCanvases();
		_textField.ForceMeshUpdate();
		previousTmp.ForceMeshUpdate();
	}

	public void AppendText(TextJournalContent content, string separator = "\n")
	{
		if (string.IsNullOrEmpty(content.Term.mTerm) || (string)content.Term == null)
		{
			TextMeshProUGUI textField = _textField;
			textField.text = textField.text + separator + content.PureText;
		}
		else
		{
			content.RegisterManagers();
			TextMeshProUGUI textField2 = _textField;
			textField2.text = textField2.text + separator + content.Term;
			content.UnregisterManagers();
		}
		ForceTextMeshProUpdate();
	}
}
