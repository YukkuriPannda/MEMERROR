using UnityEngine;

public class NormalBullet : MonoBehaviour
{
	[SerializeField] float speed = 10f;
	[SerializeField] float lifeTime = 2f;

	void Start()
	{
		Destroy(gameObject, lifeTime);
	}

	void Update()
	{
		transform.Translate(Vector3.up * speed * Time.deltaTime);
	}
}