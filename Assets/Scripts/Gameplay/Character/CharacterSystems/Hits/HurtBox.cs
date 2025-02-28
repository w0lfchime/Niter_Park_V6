


using UnityEngine;

public class Hurtbox : MonoBehaviour
{
	public Character owner;

	private void Start()
	{
		if (owner == null)
		{
			owner = GetComponentInParent<Character>();
		}
	}
}
