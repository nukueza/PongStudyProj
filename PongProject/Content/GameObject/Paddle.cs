using Microsoft.Xna.Framework;
using System;

public class Paddle
{
  public Vector2 Position;
  public Vector2 Direction;
  public float Speed { get; private set; }
  public int Width { get; private set; }
  public int Height { get; private set; }

  public Rectangle Bounds => new Rectangle((int)Math.Round(Position.X), (int)Math.Round(Position.Y), Width, Height);

  public Paddle(Vector2 pos, float speed, int width, int height)
  {
    Position = pos;
    Speed = speed;
    Width = width;
    Height = height;
    Direction = Vector2.Zero;
  }

  public void Update(float dt, int vh)
  {
    Position += Direction * dt * Speed;

    Position.Y = Math.Clamp(Position.Y, 0, vh - Height);
  }
}
