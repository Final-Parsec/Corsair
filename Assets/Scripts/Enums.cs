using System;
using UnityEngine;

public enum Border
{
	Center=-1,
	Down=3,
	DownLeft=0,
	Left=4,
	UpLeft=6,
	Up=1,
	UpRight=2,
	Right=5,
	DownRight=7
}

public enum EnemyType
{
	Start=-1,
	FastStorm,
	FastVoodoo,
	StrongEarth,
	StrongFire,
	SpawnerFire,
	SpawnerStorm,
	Max
}

public enum WaveId
{
    Start = -1,
    Zombie,
    Horde,
    PregerWench,
    SeaDog,
    Dog,
    Seagull,
    Boss1,
    Boss2,
    Boss3,
    Max
}

public enum BossType
{
	Start=-1,
	FastStormBoss,
	FastVoodooBoss,
	StrongEarthBoss,
	StrongFireBoss,
	SpawnerFireBoss,
	SpawnerStormBoss,
	Max
}

public enum TurretType
{
	EarthTurret=0,
	FireTurret,
	StormTurret,
	VoodooTurret
}

public enum ObstacleType
{
	Barracks45,
	Church45,
	Firestation45,
	GardenEmpty45,
	GardenFull45,
	GardenHalf45,
	SimpleHouse,
	Weaponsmith45,
	WeaponsmithForge45
}

public enum StatusEffects
{
	Burn,
	Poison,
	Slow,
	MindControl,
	ReducedArmor,
}

public enum GameSpeed
{
	Paused=0,
	X1=1,
	X2=2,
	X3=3
}

public enum MapType
{
	Open,
	Obstacles
};

public enum State
{
	Walking=0
};