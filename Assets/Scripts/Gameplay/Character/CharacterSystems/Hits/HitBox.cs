using UnityEngine;

public class Hitbox : MonoBehaviour
{
	public HitData hitData;
	public Character owner;

	private void OnTriggerEnter2D(Collider2D other)
	{
		IHittable target = other.GetComponent<IHittable>();
		if (target != null && other.gameObject != owner.gameObject)
		{
			//target.ReceiveHit(hitData, other.transform.position, owner);
		}
	}

	public void EnableHitbox() => gameObject.SetActive(true);
	public void DisableHitbox() => gameObject.SetActive(false);
}
