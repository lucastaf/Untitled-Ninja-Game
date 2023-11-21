using Godot;
using System;

public partial class health_component : Node2D
{
	// Called when the node enters the scene tree for the first time.
	[Export] public int totalHealth = 10;
	[Export] private ProgressBar healthbar = null;
	[Signal] public delegate void DeathEventHandler();
	[Signal] public delegate void OverHealthEventHandler();
	public int currentHealth;

	public override void _Ready()
	{
		currentHealth = totalHealth;
		if(healthbar != null)
		{
			healthbar.MaxValue = totalHealth;
			healthbar.Value = currentHealth;
		}
	}

	public void takeDamage(int ammount)
	{
		currentHealth-= ammount;
		if (currentHealth <= 0)
		{
			currentHealth = 0;
			EmitSignal(SignalName.Death);
		}
		refreshHealthBar();
	}

	public void takeHealth(int ammount)
	{
		if (currentHealth + ammount <= totalHealth)
		{
			currentHealth+= ammount;
		}
		else
		{
			currentHealth = totalHealth;
			EmitSignal(SignalName.OverHealth);
		}
		refreshHealthBar();
	}

	private void refreshHealthBar()
	{
		if(healthbar != null)
		{
			healthbar.Value = currentHealth;
		}
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.

}
