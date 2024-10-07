using Rewired.Dev;

namespace RG.SecondsRemaster.Inputs;

public static class RewiredConsts
{
	public static class Action
	{
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "InitPlayer")]
		public const int INITPLAYER = 0;

		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "PauseMenu")]
		public const int PAUSEMENU = 12;

		[ActionIdFieldInfo(categoryName = "Scavenge", friendlyName = "MoveHorizontal")]
		public const int MOVEHORIZONTAL = 2;

		[ActionIdFieldInfo(categoryName = "Scavenge", friendlyName = "MoveVertical")]
		public const int MOVEVERTICAL = 3;

		[ActionIdFieldInfo(categoryName = "Scavenge", friendlyName = "Interact")]
		public const int INTERACT = 4;

		[ActionIdFieldInfo(categoryName = "Scavenge", friendlyName = "Rotate")]
		public const int ROTATE = 14;

		[ActionIdFieldInfo(categoryName = "UI", friendlyName = "SelectVertical")]
		public const int SELECTVERTICAL = 25;

		[ActionIdFieldInfo(categoryName = "UI", friendlyName = "SelectHorizontal")]
		public const int SELECTHORIZONTAL = 28;

		[ActionIdFieldInfo(categoryName = "UI", friendlyName = "Confirm")]
		public const int CONFIRM = 29;

		[ActionIdFieldInfo(categoryName = "UI", friendlyName = "Cancel")]
		public const int CANCEL = 30;

		[ActionIdFieldInfo(categoryName = "UI", friendlyName = "MoveCursorVertical")]
		public const int MOVECURSORVERTICAL = 32;

		[ActionIdFieldInfo(categoryName = "UI", friendlyName = "MoveCursorHorizontal")]
		public const int MOVECURSORHORIZONTAL = 31;

		[ActionIdFieldInfo(categoryName = "UI", friendlyName = "ConfirmMouse")]
		public const int CONFIRMMOUSE = 36;

		[ActionIdFieldInfo(categoryName = "Survival", friendlyName = "ChangeHatOrSkinToNext")]
		public const int CHANGEHATORSKINTONEXT = 33;

		[ActionIdFieldInfo(categoryName = "Survival", friendlyName = "ChangeHatOrSkinToPrevious")]
		public const int CHANGEHATORSKINTOPREVIOUS = 37;

		[ActionIdFieldInfo(categoryName = "Survival", friendlyName = "NextItem")]
		public const int NEXTITEM = 34;

		[ActionIdFieldInfo(categoryName = "Survival", friendlyName = "PreviousItem")]
		public const int PREVIOUSITEM = 35;

		[ActionIdFieldInfo(categoryName = "Survival", friendlyName = "OpenOrHideJournal")]
		public const int OPENORHIDEJOURNAL = 38;

		[ActionIdFieldInfo(categoryName = "Survival", friendlyName = "NextPage")]
		public const int NEXTPAGE = 39;

		[ActionIdFieldInfo(categoryName = "Survival", friendlyName = "PreviousPage")]
		public const int PREVIOUSPAGE = 40;

		[ActionIdFieldInfo(categoryName = "Survival", friendlyName = "ShowPrompts")]
		public const int SHOWPROMPTS = 41;
	}

	public static class Category
	{
		public const int DEFAULT = 0;

		public const int MOUSE = 6;

		public const int KEYBOARD = 7;

		public const int GAMEPAD = 8;

		public const int MOUSEKEYBOARD = 9;
	}

	public static class Layout
	{
		public static class Joystick
		{
			public const int DEFAULT = 0;
		}

		public static class Keyboard
		{
			public const int DEFAULT = 0;

			public const int AZERTY = 1;
		}

		public static class Mouse
		{
			public const int DEFAULT = 0;
		}

		public static class CustomController
		{
			public const int DEFAULT = 0;
		}
	}

	public static class Player
	{
		[PlayerIdFieldInfo(friendlyName = "System")]
		public const int SYSTEM = 9999999;

		[PlayerIdFieldInfo(friendlyName = "Main")]
		public const int MAIN = 0;
	}
}
