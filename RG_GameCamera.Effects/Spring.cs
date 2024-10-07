namespace RG_GameCamera.Effects;

public class Spring
{
	private float mass;

	private float distance;

	private float springConstant;

	private float damping;

	private float acceleration;

	private float velocity;

	private float springForce;

	public void Setup(float mass, float distance, float springStrength, float damping)
	{
		this.mass = mass;
		this.distance = distance;
		springConstant = springStrength;
		this.damping = damping;
		velocity = 0f;
	}

	public void AddForce(float force)
	{
		velocity += force;
	}

	public float Calculate(float timeStep)
	{
		springForce = (0f - springConstant) * distance - velocity * damping;
		acceleration = springForce / mass;
		velocity += acceleration * timeStep;
		distance += velocity * timeStep;
		return distance;
	}
}
