using Godot;
using System;

public partial class hitBox_Component : Area2D
{
	[Export] public health_component health;
	// Called when the node enters the scene tree for the first time.

	public void _on_area_entered(Area2D area)
	{
		if(area is attackBox_Component)
		{
			int damage = ((attackBox_Component)area).damage;
			if (health != null)
			{
				health.takeDamage(damage);
			}
		}
	}
}
