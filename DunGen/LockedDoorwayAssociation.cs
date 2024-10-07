using System;

namespace DunGen;

[Serializable]
public sealed class LockedDoorwayAssociation
{
	public DoorwaySocketType SocketGroup;

	public GameObjectChanceTable LockPrefabs = new GameObjectChanceTable();
}
