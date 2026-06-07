using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace PongProject;

public class Game1 : Game
{
  private GraphicsDeviceManager _graphics;
  private SpriteBatch _spriteBatch;

  // texture
  private Texture2D _pixel;

  // score font
  private SpriteFont _myFont;
  private int _scoreLeft, _scoreRight;

  // paddle left
  private Rectangle _paddleLeft;
  private Vector2 _paddleLeftPos;
  private Vector2 _paddleLeftDir;

  // paddle right
  private Rectangle _paddleRight;
  private Vector2 _paddleRightPos;
  private Vector2 _paddleRightDir;

  // ball
  private Rectangle _ball;
  private Vector2 _ballPos;
  private Vector2 _ballVel;

  // size ball and paddle
  private const int PADDLE_WIDTH = 5;
  private const int PADDLE_HEIGHT = 20;
  private const int BALL_SIZE = 4;

  // rendertarget
  private RenderTarget2D _renderTarget;
  private const int VIRTUAL_WIDTH = 320;
  private const int VIRTUAL_HEIGHT = 180;
  private const int CANVAS_WIDTH = 1280;
  private const int CANVAS_HEIGHT = 720;

  public Game1()
  {
    _graphics = new GraphicsDeviceManager(this);
    Content.RootDirectory = "Content";
    IsMouseVisible = true;

    _graphics.PreferredBackBufferWidth = CANVAS_WIDTH;
    _graphics.PreferredBackBufferHeight = CANVAS_HEIGHT;
    _graphics.ApplyChanges();
  }

  protected override void Initialize()
  {
    base.Initialize();
  }

  protected override void LoadContent()
  {
    _spriteBatch = new SpriteBatch(GraphicsDevice);

    _pixel = new Texture2D(GraphicsDevice, 1, 1);
    _pixel.SetData(new Color[] { Color.White });

    _renderTarget = new RenderTarget2D(GraphicsDevice, VIRTUAL_WIDTH, VIRTUAL_HEIGHT);

    _myFont = Content.Load<SpriteFont>("Font/myFont");

    _paddleLeftPos = new Vector2(10, VIRTUAL_HEIGHT / 2 - PADDLE_HEIGHT / 2);
    _paddleRightPos = new Vector2(VIRTUAL_WIDTH - 15, VIRTUAL_HEIGHT / 2 - PADDLE_HEIGHT / 2);
    _ballPos = new Vector2(VIRTUAL_WIDTH / 2 - BALL_SIZE / 2, VIRTUAL_HEIGHT / 2 - BALL_SIZE / 2);

  }

  protected override void Update(GameTime gameTime)
  {
    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
      Exit();

    base.Update(gameTime);
  }

  protected override void Draw(GameTime gameTime)
  {
    GraphicsDevice.SetRenderTarget(_renderTarget);
    GraphicsDevice.Clear(new Color(45, 55, 75));

    _spriteBatch.Begin();
    
    // paddle left
    _paddleLeft = new Rectangle((int)Math.Round(_paddleLeftPos.X), (int)Math.Round(_paddleLeftPos.Y), PADDLE_WIDTH, PADDLE_HEIGHT);
    _spriteBatch.Draw(_pixel, _paddleLeft, Color.White);

    // paddle right
    _paddleRight = new Rectangle((int)Math.Round(_paddleRightPos.X), (int)Math.Round(_paddleRightPos.Y), PADDLE_WIDTH, PADDLE_HEIGHT);
    _spriteBatch.Draw(_pixel, _paddleRight, Color.White);

    // ball
    _ball = new Rectangle((int)Math.Round(_ballPos.X), (int)Math.Round(_ballPos.Y), BALL_SIZE, BALL_SIZE);
    _spriteBatch.Draw(_pixel, _ball, Color.White);

    // font
    string leftScore = $"{_scoreLeft}";
    string rightScore = $"{_scoreRight}";

    Vector2 leftScoreSize = _myFont.MeasureString(leftScore);
    Vector2 rightScoreSize = _myFont.MeasureString(rightScore);

    _spriteBatch.DrawString(_myFont, leftScore, new Vector2(VIRTUAL_WIDTH / 2 - 30 - leftScoreSize.X / 2, 10), Color.White);
    _spriteBatch.DrawString(_myFont, rightScore, new Vector2(VIRTUAL_WIDTH / 2 + 30 - rightScoreSize.X / 2, 10), Color.White);

    _spriteBatch.End();

    GraphicsDevice.SetRenderTarget(null);
    GraphicsDevice.Clear(Color.Black);

    _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
    _spriteBatch.Draw(_renderTarget, new Rectangle(0, 0, CANVAS_WIDTH, CANVAS_HEIGHT), Color.White);
    _spriteBatch.End();

    base.Draw(gameTime);
  }
}
