using Godot;
using System;

public partial class ninjaCharacter : CharacterBody2D
{
    [Export] private PackedScene kunaiScene;
    public const float Speed = 100.0f;
    public const float JumpVelocity = -300.0f;
    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
    private AnimationTree Animacoes;
    private AnimationNodeStateMachinePlayback AnimacoesStationMachine;
    private CollisionShape2D colisaoAtaque;
    private health_component staminaComponent;
    private int dashTimer = 0;
    private int numPulos = 2;
    private bool onGround;


    public override void _Ready()
    {
        Animacoes = GetNode<AnimationTree>("Animations/AnimationTree");
        AnimacoesStationMachine = (AnimationNodeStateMachinePlayback)Animacoes.Get("parameters/playback");
        colisaoAtaque = GetNode<CollisionShape2D>("attackBox/AttackColision");
        staminaComponent = GetNode<health_component>("StaminaComponent");
    }
    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;

        //FISICA CHAO OU AR
        if (!IsOnFloor())
        {
            velocity.Y += gravity * (float)delta;
            onGround = false;
        }
        else
        {
            numPulos = 2;
            if (!onGround)
            {
                //PUFF
            }
            onGround = true;
        }


        //INPUTS ------///
        if (Input.IsActionJustPressed("pulo") && numPulos > 0)
        {
            velocity.Y = JumpVelocity;
            numPulos--;
        }

        if (Input.IsActionJustPressed("ataque"))
        {
            colisaoAtaque.Disabled = false;
            AnimacoesStationMachine.Travel("ataque");
        }
        if (Input.IsActionJustPressed("kunai"))
        {
            kunai kunaiNode = (kunai)kunaiScene.Instantiate();
            kunaiNode.Transform = this.Transform;
            kunaiNode.Rotation = this.Rotation;
            kunaiNode.ApplyCentralImpulse(new Vector2(this.Scale.Y * 500, 0));

            this.GetParent().AddChild(kunaiNode);
        }
        if (Input.IsActionJustPressed("dash"))
        {
            if (staminaComponent.currentHealth > 250)
            {
                dashTimer = 15;
                staminaComponent.takeDamage(250);
            }
        }
        //FIM DOS INPUTS///

        //DEFINIR DIRECAO
        Vector2 direction = Input.GetVector("esquerda", "direita", "ui_up", "ui_down");
        if (direction != Vector2.Zero)
        {
            velocity.X = direction.X * Speed;

        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
        }
        //------FIM DA DIRECAO-----//

        //Direcao
        if (velocity.X > 0)
        {
            this.RotationDegrees = 0;
            this.Scale = new Vector2(1, 1);
        }
        else if (velocity.X < 0)
        {
            this.RotationDegrees = 180;
            this.Scale = new Vector2(1, -1);
        }
        //Fim da direcaoo

        //Set de variaveis e animacoes
        if ((this.Position.Y) > 500)
        {
            GetTree().ReloadCurrentScene();
        }
        Animacoes.Set("parameters/IWR/blend_position", velocity.Abs());

        if (dashTimer > 0)
        {
            dashTimer--;
            velocity.X = direction.X * Speed * 3;
        }
        staminaComponent.takeHealth(1);
        Velocity = velocity;
        MoveAndSlide();
        //-----
    }

    private void _animation_finished(string nomeAnimacao)
    {
        if (nomeAnimacao == "ataque")
        {
            colisaoAtaque.Disabled = true;
        }
    }

    private void _on_health_component_death()
    {
        GetTree().ReloadCurrentScene();
    }

}
