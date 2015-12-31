
using System;

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

public enum WaveId
{
    Zombie,
    Horde,
    PregerWench,
    SeaDog,
    Dog,
    Seagull,
    Boss1,
    Boss2,
    Boss3
}

public enum TurretType
{
    Pistolman = 0,
    Rifleman,
    Cannon,
    Netter
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

[Flags]
public enum AttackOptionsFlags
{
    None = 0,
    Ground = 1,
    Air = 2
}