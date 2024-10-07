using UnityEngine;

namespace RG_GameCamera.Utils;

public class Popup
{
	public delegate void ListCallBack();

	private static int popupListHash = "PopupList".GetHashCode();

	public static bool List(Rect position, ref bool showList, ref int listEntry, GUIContent buttonContent, object[] list, GUIStyle listStyle, ListCallBack callBack)
	{
		return List(position, ref showList, ref listEntry, buttonContent, list, "button", "box", listStyle, callBack);
	}

	public static bool List(Rect position, ref bool showList, ref int listEntry, GUIContent buttonContent, object[] list, GUIStyle buttonStyle, GUIStyle boxStyle, GUIStyle listStyle, ListCallBack callBack)
	{
		int controlID = GUIUtility.GetControlID(popupListHash, FocusType.Passive);
		bool flag = false;
		switch (Event.current.GetTypeForControl(controlID))
		{
		case EventType.MouseDown:
			if (position.Contains(Event.current.mousePosition))
			{
				GUIUtility.hotControl = controlID;
				showList = true;
			}
			break;
		case EventType.MouseUp:
			if (showList)
			{
				flag = true;
				callBack();
			}
			break;
		}
		GUI.Label(position, buttonContent, buttonStyle);
		if (showList)
		{
			string[] array = new string[list.Length];
			for (int i = 0; i < list.Length; i++)
			{
				array[i] = list[i].ToString();
			}
			Rect position2 = new Rect(position.x, position.y, position.width, list.Length * 20);
			GUI.Box(position2, "", boxStyle);
			listEntry = GUI.SelectionGrid(position2, listEntry, array, 1, listStyle);
		}
		if (flag)
		{
			showList = false;
		}
		return flag;
	}
}
