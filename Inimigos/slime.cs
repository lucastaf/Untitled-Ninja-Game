using Godot;
using System;
using System.Linq.Expressions;

public partial class slime : CharacterBody2D
{
	[Export] public health_component health;
	[Export] public hitBox_Component hitBox;
	[Export] private CollisionShape2D attackColission;


	private bool isAlive = true;
	private bool chasePlayer = false;
	private Node2D chasedPlayer;
    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
    public const float speed = 50.0f;
	private AnimationTree animacao;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animacao = GetNode<AnimationTree>("AnimationTree");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
        Vector2 velocity = Velocity;
        if (!IsOnFloor())
		{
			velocity.Y += gravity * (float)delta;
        }
		
		if (chasePlayer)
		{
			Vector2 direction = new Vector2(chasedPlayer.GlobalPosition.X - GlobalPosition.X, 0);
			direction = direction.Normalized();
            velocity.X = direction.X * speed;
		}
		else
		{
            velocity.X = 0;
		}


		Velocity = velocity;
        if(isAlive)MoveAndSlide();

		animacao.Set("parameters/IWR/blend_position",Math.Abs(Velocity.X));

    }

	private void _on_area_2d_body_entered(Node2D body)
	{
		if(body is ninjaCharacter)
		{
			chasedPlayer = body;
            chasePlayer = true;
        }
			
	}

    private void _on_area_2d_body_exited(Node2D body)
	{
		if (body is ninjaCharacter)
		{
            chasePlayer = false;
            chasedPlayer = null;
        }
    }

	private void _on_health_component_death()
	{
		animacao.Set("parameters/conditions/isDead", true);
		isAlive = false;
		attackColission.QueueFree();
    }

	private void _on_animation_tree_animation_finished(string name)
	{
		if(name == "death")
		{
			this.QueueFree();
		}
	}


}
