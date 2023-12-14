using Godot;
using System;
using System.Linq.Expressions;

public partial class archer : CharacterBody2D
{
    // Called when the node enters the scene tree for the first time.

    const int arrowDelay = 100;


    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    //States
    [Export] private PackedScene flechaScene;
    private bool isalive = true;
    private bool chasePlayer = false;
    private bool shootPlayer = false;
    public const float speed = 50.0f;
    private Node2D chasedPlayerObj;
    private AnimationTree animacao;
    private int arrowTimeCounter = 0;
    public override void _Ready()
    {
        animacao = GetNode<AnimationTree>("AnimationTree");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
    {
        if (isalive)
        {
            Vector2 velocity = Velocity;
            if (!IsOnFloor())
            {
                velocity.Y += gravity * (float)delta;
            }
            if (chasePlayer)
            {
                Vector2 direction = new Vector2(chasedPlayerObj.GlobalPosition.X - GlobalPosition.X, 0);
                direction = direction.Normalized();
                velocity.X = direction.X * speed;
            }
            if (shootPlayer)
            {
                if (arrowTimeCounter == 0)
                {
                    Vector2 Arrowdirection = new Vector2(chasedPlayerObj.GlobalPosition.X - GlobalPosition.X, chasedPlayerObj.GlobalPosition.Y - GlobalPosition.Y);
                    Arrowdirection = Arrowdirection.Normalized() * 250;
                    arrow flechaNode = (arrow)flechaScene.Instantiate();
                    flechaNode.Transform = this.Transform;
                    flechaNode.Rotation = Arrowdirection.Angle();
                    flechaNode.ApplyCentralImpulse(Arrowdirection);
                    
                    this.GetParent().AddChild(flechaNode);    
                }
                arrowTimeCounter++;
            }
            if(arrowTimeCounter == arrowDelay) arrowTimeCounter = 0;
            Velocity = velocity;
            MoveAndSlide();
        }
    }

    public void _on_range_chase_body_entered(Node2D body)
    {
        if (body is ninjaCharacter)
        {
            chasePlayer = true;
            chasedPlayerObj = body;
        }
    }

    public void _on_range_chase_body_exited(Node2D body)
    {
        if (body is ninjaCharacter)
        {
            chasePlayer = false;
            chasedPlayerObj = null;
        }
    }

    public void _on_range_shoot_body_entered(Node2D body)
    {
        if (body is ninjaCharacter)
        {
            chasePlayer = false;
            shootPlayer = true;
        }
    }

    public void _on_range_shoot_body_exited(Node2D body)
    {
        if (body is ninjaCharacter)
        {
            shootPlayer = false;
            chasePlayer = true;
        }
    }

    public void _on_health_component_death()
    {
        isalive = false;
        animacao.Set("parameters/conditions/isDead",true);
    }
    public void _on_animation_tree_animation_finished(string NomeAnimacao)
    {
        if (NomeAnimacao == "Death") this.QueueFree();
    }


}
