using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;

public class Ball
{
  public Vector2 Position;
  public Vector2 Velocity;
  private SoundEffect _sound;

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

  public Ball(Vector2 pos, int size, SoundEffect sound)
  {
    Position = pos;
    Size = size;
    _sound = sound;
    Velocity = Vector2.Zero;
  }

  public void Update(float dt, int vh)
  {
    Position += Velocity * dt;

    if (Position.Y <= 0)
    {
      Position.Y = 0;
      Velocity.Y = Math.Abs(Velocity.Y);
      _sound.Play();
    }
    else if (Position.Y >= vh - Size)
    {
      Position.Y = vh - Size;
      Velocity.Y = -Math.Abs(Velocity.Y);
      _sound.Play();
    }
  }

}
