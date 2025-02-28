using UnityEngine;

public struct HitData
{
	public Vector3 HitPosition;
	public Vector3 HitDirection;
	public float Damage;
	public float Knockback;
	public float Stun;
	public GameObject Attacker;

	public HitData(Vector3 hitPos, Vector3 hitDir, float damage, float knockback, float stun, GameObject attacker)
	{
		HitPosition = hitPos;
		HitDirection = hitDir;
		Damage = damage;
		Knockback = knockback;
		Stun = stun;
		Attacker = attacker;
	}
}
