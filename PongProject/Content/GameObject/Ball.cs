using Microsoft.Xna.Framework;
using System;

public class Ball
{
  public Vector2 Position;
  public Vector2 Velocity;

  public int Size { get; private set; }

  public Rectangle Bounds
  {
    get
    {
      return new Rectangle(
          (int)Math.Round(Position.X),
          (int)Math.Round(Position.Y),
          Size,
          Size
          );
    }
  }

  public Ball(Vector2 pos, int size)
  {
    Position = pos;
    Size = size;
    Velocity = Vector2.Zero;
  }

  public void Update(float dt, int vh)
  {
    Position += Velocity * dt;

    if (Position.Y <= 0)
    {
      Position.Y = 0;
      Velocity.Y = Math.Abs(Velocity.Y);
    }
    else if (Position.Y >= vh - Size)
    {
      Position.Y = vh - Size;
      Velocity.Y = -Math.Abs(Velocity.Y);
    }
  }

}
