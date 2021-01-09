//Based on the intercept Script by Daniel Brauer : http://wiki.unity3d.com/index.php/Calculating_Lead_For_Projectiles
using UnityEngine;

public  class HelperFunctions
{
    /*#region singleton
	public static HelperFunctions instance;

	private void Awake()
	{
		if (HelperFunctions.instance != null)
		{
			Debug.LogWarning("More than one instance of ");
			return;
		}
		instance = this;
	}
	#endregion
	*/

    #region intercept
    public static Vector3 Intercept
	(
		Vector3 shooterPosition,
		Vector3 shooterVelocity, //used for rockets and bombs but not for bullets, since they don't retain their parents momentum
		float shotSpeed,
		Vector3 targetPosition,
		Vector3 targetVelocity
	)
	{
		Vector3 targetRelativePosition = targetPosition - shooterPosition;
		Vector3 targetRelativeVelocity = targetVelocity - shooterVelocity;
		float t = FirstOrderInterceptTime
		(
			shotSpeed,
			targetRelativePosition,
			targetRelativeVelocity
		);
		return targetPosition + t * (targetRelativeVelocity);
	}

	//first-order intercept using relative target position
	public static float FirstOrderInterceptTime
	(
		float shotSpeed,
		Vector3 targetRelativePosition,
		Vector3 targetRelativeVelocity
	)
	{
		float velocitySquared = targetRelativeVelocity.sqrMagnitude;
		if (velocitySquared < 0.001f)
			return 0f;

		float a = velocitySquared - shotSpeed * shotSpeed;

		//handle similar velocities
		if (Mathf.Abs(a) < 0.001f)
		{
			float t = -targetRelativePosition.sqrMagnitude /
			(
				2f * Vector3.Dot
				(
					targetRelativeVelocity,
					targetRelativePosition
				)
			);
			return Mathf.Max(t, 0f); //don't shoot back in time
		}

		float b = 2f * Vector3.Dot(targetRelativeVelocity, targetRelativePosition);
		float c = targetRelativePosition.sqrMagnitude;
		float determinant = b * b - 4f * a * c;

		if (determinant > 0f)
		{ //determinant > 0; two intercept paths (most common)
			float t1 = (-b + Mathf.Sqrt(determinant)) / (2f * a),
					t2 = (-b - Mathf.Sqrt(determinant)) / (2f * a);
			if (t1 > 0f)
			{
				if (t2 > 0f)
					return Mathf.Min(t1, t2); //both are positive
				else
					return t1; //only t1 is positive
			}
			else
				return Mathf.Max(t2, 0f); //don't shoot back in time
		}
		else if (determinant < 0f) //determinant < 0; no intercept path
			return 0f;
		else //determinant = 0; one intercept path, pretty much never happens
			return Mathf.Max(-b / (2f * a), 0f); //don't shoot back in time
	}

    #endregion

    #region LookAt
    public static void LookAt(Transform self, Vector3 target, float rotationSpeed, Vector3 up)
	{
		Vector3 lookPos = target - self.position;
		Quaternion rotation = Quaternion.LookRotation(lookPos, up);
		self.rotation = Quaternion.Slerp(self.rotation, rotation, Time.deltaTime * rotationSpeed);
	}
	public static void LookAt(Transform self, Vector3 target, float rotationSpeed)
	{
		Vector3 lookPos = target - self.position;
		Quaternion rotation = Quaternion.LookRotation(lookPos);
		self.rotation = Quaternion.Slerp(self.rotation, rotation, Time.deltaTime * rotationSpeed);
	}
	#endregion

	#region LineOfSight

	public static Collider GetObjectInSights(Vector3 start, Vector3 direction)
    {
		return GetObjectInSights(start, direction, 5000f, new LayerMask());
    }
	
	public static Collider GetObjectInSights(Vector3 start, Vector3 direction, float range)
    {
		return GetObjectInSights(start, direction, range, new LayerMask());
    }

	public static Collider GetObjectInSights(Vector3 start, Vector3 direction, LayerMask ignoreLayer)
	{
		return GetObjectInSights(start, direction, 5000f, ignoreLayer);
	}
	
	public static Collider GetObjectInSights(Vector3 start, Vector3 direction, float range, LayerMask ignoreLayer)
    {
		RaycastHit raycastHit;
		Ray ray = new Ray(start, direction);
		Physics.Raycast(ray, out raycastHit, range, ~ignoreLayer);

		return raycastHit.collider;
    }

	public static bool FreeLOS(Vector3 start, Vector3 end)
	{
		return FreeLOS(start, end, 5000f, new LayerMask());
	}
	public static bool FreeLOS(Vector3 start, Vector3 end, float range)
	{
		return FreeLOS(start, end, range, new LayerMask());
	}

	public static bool FreeLOS(Vector3 start, Vector3 end, float range, LayerMask ignoreLayer)
    {
		Collider obj = GetObjectInSights(start, (end - start), range, ignoreLayer);

		if (obj == null) return true;
		return false;
    }
	#endregion

	#region explosions
	public static Collider[] SpawnExplosion(GameObject explosionObject, float radius, Vector3 position)
    {
		//Debug.Log("Boom");
		//Spawn explosion Visuals

		GameObject ex = GameObject.Instantiate(explosionObject);
		Explosion expl = ex.GetComponent<Explosion>();

		if (expl == null) expl = ex.AddComponent<Explosion>();

		expl.Instantiate(radius * 2f);
		ex.transform.position = position;

		Collider[] colliders = Physics.OverlapSphere(position, radius);
		return colliders;
	}
    #endregion
}