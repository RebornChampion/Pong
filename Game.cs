using Godot;
using System;
using System.Collections.Generic;

public partial class Game : Node2D
{
    // Constant Dictionary for Input Actions
    private static readonly Dictionary<string, StringName> InputActions = new()
    {
        { "LeftPaddleMoveUp", "left_paddle_move_up" },
        { "LeftPaddleMoveDown", "left_paddle_move_down" },
        { "RightPaddleMoveUp", "right_paddle_move_up" },
        { "RightPaddleMoveDown", "right_paddle_move_down" }
    };

    // Constants
    const int INITIAL_BALL_SPEED = 80; // Pixels per second
    const int PAD_SPEED = 150; // Pixels per second

    // Member Variables
    private Vector2 _screenSize;
    private Vector2 _padSize;
    private Vector2 _direction = new(1.0f, 0.0f);
    private float _ballSpeed = INITIAL_BALL_SPEED;
    private Random _random = new();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Initialize Member Variables
        _screenSize = GetViewportRect().Size;
        _padSize = GetNode<Sprite2D>("left").Texture.GetSize();

        // Enable Processing
        SetProcess(true);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // Get Sprite Positions
        Vector2 ballPosition = GetNode<Sprite2D>("ball").Position;
        Vector2 leftPalletPosition = GetNode<Sprite2D>("left").Position;
        Vector2 rightPalletePosition = GetNode<Sprite2D>("right").Position;

        // Get Sprite Rectangles (for collision detection)
        // Note: offset by half the pad size because the position is the center of the sprite
        var leftRect = new Rect2(leftPalletPosition - (_padSize * 0.5f), _padSize);
        var rightRect = new Rect2(rightPalletePosition - (_padSize * 0.5f), _padSize);

        BounceOnHorizontalCollision(ballPosition);

        // Flip, Change Direction, and Speed Up Ball on Left Paddle Collision
        if (leftRect.HasPoint(ballPosition) && _direction.X < 0)
        {
            BounceOnPaddleCollision();
        }

        // Flip, Change Direction, and Speed Up Ball on Right Paddle Collision
        if (rightRect.HasPoint(ballPosition) && _direction.X > 0)
        {
            BounceOnPaddleCollision();
        }

        // Check if gameover
        if (ballPosition.X < 0 || ballPosition.X > _screenSize.X)
        {
            // Reset Ball Position and Speed
            ballPosition = _screenSize / 2;
            _ballSpeed = INITIAL_BALL_SPEED;
            _direction = new Vector2(-1.0f, 0.0f);
        }

        // Move Left Paddle
        if (Input.IsActionPressed(InputActions["LeftPaddleMoveUp"]) && leftPalletPosition.Y > 0)
        {
            leftPalletPosition.Y -= PAD_SPEED * (float)delta;
        }
        if (Input.IsActionPressed(InputActions["LeftPaddleMoveDown"]) && leftPalletPosition.Y < _screenSize.Y)
        {
            leftPalletPosition.Y += PAD_SPEED * (float)delta;
        }

        // Update the Left Paddle Position
        GetNode<Sprite2D>("left").Position = leftPalletPosition;

        // Move Right Paddle
        if (Input.IsActionPressed(InputActions["RightPaddleMoveUp"]) && rightPalletePosition.Y > 0)
        {
            rightPalletePosition.Y -= PAD_SPEED * (float)delta;
        }
        if (Input.IsActionPressed(InputActions["RightPaddleMoveDown"]) && rightPalletePosition.Y < _screenSize.Y)
        {
            rightPalletePosition.Y += PAD_SPEED * (float)delta;
        }

        // Update the Right Paddle Position
        GetNode<Sprite2D>("right").Position = rightPalletePosition;

        // Move Ball
        ballPosition += _direction * _ballSpeed * (float)delta;

        // Update the ball position
        GetNode<Sprite2D>("ball").Position = ballPosition;
    }

    private void BounceOnPaddleCollision()
    {
        _direction.X = -_direction.X;
        // Random Y Direction between -1 and 1
        // The Math: NextDouble gives range [0-1]; times 2 gives you [0-2]; -1 gives you [-1-1]
        _direction.Y = (float)(_random.NextDouble() * 2.0f - 1.0f);
        _direction = _direction.Normalized();
        _ballSpeed *= 1.1f;
    }

    private void BounceOnHorizontalCollision(Vector2 ballPosition)
    {

        // Invert Ball Direction on Ceiling Collision
        if (ballPosition.Y < 0 && _direction.Y < 0)
        {
            _direction.Y = -_direction.Y;
        }

        // Invert Ball Direction on Floor Collision
        if (ballPosition.Y > _screenSize.Y && _direction.Y > 0)
        {
            _direction.Y = -_direction.Y;
        }
    }
}
