using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
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
  private Paddle _paddleLeft;

  // paddle right
  private Paddle _paddleRight;

  // ball
  private Ball _ball;

  // size ball and paddle
  private const int PADDLE_WIDTH = 5;
  private const int PADDLE_HEIGHT = 20;
  private const int BALL_SIZE = 4;

  // speed paddle and ball
  private const float PADDLE_LEFT_SPEED = 120f;
  private const float PADDLE_RIGHT_SPEED = 100f;
  private const float BALL_SPEED = 180f;

  // rendertarget
  private RenderTarget2D _renderTarget;
  private const int VIRTUAL_WIDTH = 320;
  private const int VIRTUAL_HEIGHT = 180;
  private const int CANVAS_WIDTH = 1280;
  private const int CANVAS_HEIGHT = 720;

  // random
  private Random _rand = new();

  // state
  private GameState _state = GameState.Serving;
  private int _lastScore = 0;
  private float _timeState = 0;
  private float _serveTime = 1.5f;
  private int _winning = 2;

  // sound
  private SoundEffect _hitPaddle;
  private SoundEffect _score;
  private SoundEffect _hitScreen;
  private SoundEffectInstance _soundInstance;

  public Game1()
  {
    _graphics = new GraphicsDeviceManager(this);
    Content.RootDirectory = "Content";
    IsMouseVisible = true;

    IsFixedTimeStep = true;

    _graphics.PreferredBackBufferWidth = CANVAS_WIDTH;
    _graphics.PreferredBackBufferHeight = CANVAS_HEIGHT;
    _graphics.ApplyChanges();
  }

  protected override void Initialize()
  {
    TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 120.0);
    base.Initialize();
  }

  protected override void LoadContent()
  {
    _spriteBatch = new SpriteBatch(GraphicsDevice);

    _pixel = new Texture2D(GraphicsDevice, 1, 1);
    _pixel.SetData(new Color[] { Color.White });

    _renderTarget = new RenderTarget2D(GraphicsDevice, VIRTUAL_WIDTH, VIRTUAL_HEIGHT);

    _myFont = Content.Load<SpriteFont>("Font/myFont");

    _hitPaddle = Content.Load<SoundEffect>("Audio/paddle_hit");
    _score = Content.Load<SoundEffect>("Audio/score");
    _hitScreen = Content.Load<SoundEffect>("Audio/wall_hit");

    _paddleLeft = new Paddle(
        new Vector2(10, VIRTUAL_HEIGHT / 2 - PADDLE_HEIGHT / 2), 
        PADDLE_LEFT_SPEED,
        PADDLE_WIDTH,
        PADDLE_HEIGHT
        );

    _paddleRight = new Paddle(
        new Vector2(VIRTUAL_WIDTH - 15, VIRTUAL_HEIGHT / 2 - PADDLE_HEIGHT / 2), 
        PADDLE_RIGHT_SPEED,
        PADDLE_WIDTH,
        PADDLE_HEIGHT
        );

    _ball = new Ball(
        new Vector2(VIRTUAL_WIDTH / 2 - BALL_SIZE / 2, VIRTUAL_HEIGHT / 2 - BALL_SIZE / 2),
        BALL_SIZE,
        _hitScreen
        );
  }

  private void HandleInput()
  {
    _paddleLeft.Direction = Vector2.Zero;
    KeyboardState currentState = Keyboard.GetState();
    if (currentState.IsKeyDown(Keys.Up)) _paddleLeft.Direction.Y = -1;
    if (currentState.IsKeyDown(Keys.Down)) _paddleLeft.Direction.Y = 1;
  }

  private void PaddleRightAI()
  {
    _paddleRight.Direction = Vector2.Zero;

    if (_ball.Velocity.X < 0) return;

    float targetY;
    float centerPaddleY = _paddleRight.Position.Y + PADDLE_HEIGHT / 2;

    if (_ball.Position.X > VIRTUAL_WIDTH / 2) targetY = _ball.Position.Y + BALL_SIZE / 2;
    else targetY = VIRTUAL_HEIGHT / 2;

    if (targetY > centerPaddleY + 2) _paddleRight.Direction.Y = 1;
    else if (targetY < centerPaddleY - 2) _paddleRight.Direction.Y = -1;
  }

  protected override void Update(GameTime gameTime)
  {
    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
      Exit();
    
    HandleInput();
    PaddleRightAI();

    float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

    _paddleLeft.Update(dt, VIRTUAL_HEIGHT);
    _paddleRight.Update(dt, VIRTUAL_HEIGHT);

    switch (_state)
    {
      case GameState.Serving:
        if (Keyboard.GetState().IsKeyDown(Keys.Space))
        {
          _state = GameState.Playing;
          Serving();
        }
        break;
      case GameState.Playing:
        _ball.Update(dt, VIRTUAL_HEIGHT);

        if (_ball.Position.X < 0)
        {
          _scoreRight++;
          _lastScore = 1;
          OnScored();
          _score.Play();
        }
        else if (_ball.Position.X > VIRTUAL_WIDTH)
        {
          _scoreLeft++;
          _lastScore = -1;
          OnScored();
          _score.Play();
        }
        break;
      case GameState.Scored:
        _timeState -= dt;
        if (_timeState <= 0)
        {
          _state = GameState.Serving;
          ResetBall();
        }
        break;
      case GameState.Winning:
        if (Keyboard.GetState().IsKeyDown(Keys.Enter))
        {
          _lastScore = 0;
          _scoreLeft = 0;
          _scoreRight = 0;
          _state = GameState.Serving;
        }
        break;
    }
      

   CheckCollision(); 

    base.Update(gameTime);
  }

  private void CheckCollision()
  {
    if (_ball.Bounds.Intersects(_paddleLeft.Bounds) && _ball.Velocity.X < 0)
    {
      _ball.Position.X = _paddleLeft.Bounds.Right;
      _ball.Velocity.X = Math.Abs(_ball.Velocity.X) * 1.03f;
      _ball.Velocity.Y = (float)(_rand.NextDouble() * 200 - 100);
      _hitPaddle.Play();
    }
    if (_ball.Bounds.Intersects(_paddleRight.Bounds) && _ball.Velocity.X > 0)
    {
      _ball.Position.X = _paddleRight.Bounds.Left;
      _ball.Velocity.X = -Math.Abs(_ball.Velocity.X) * 1.03f;
      _ball.Velocity.Y = (float)(_rand.NextDouble() * 200 - 100);
      _hitPaddle.Play();
    }
  }

  private void Serving()
  {
    float angle = (float)(_rand.NextDouble() * Math.PI / 3 - Math.PI / 6);
    float dirX = _lastScore != 0 ? -_lastScore : 1;
    _ball.Velocity = new Vector2(dirX * (float)Math.Cos(angle), (float)Math.Sin(angle)) * BALL_SPEED;
  }

  private void ResetBall()
  {
    _ball.Position = new Vector2(VIRTUAL_WIDTH / 2 - BALL_SIZE / 2, VIRTUAL_HEIGHT / 2 - BALL_SIZE / 2);
    _ball.Velocity = Vector2.Zero;
  }

  private void OnScored()
  {
    _state = GameState.Scored;
    _timeState = _serveTime;
    Winning();
  }

  private void Winning()
  {
    if (_scoreLeft >= _winning && _scoreRight < _winning) _state = GameState.Winning;
    else if (_scoreRight >= _winning && _scoreLeft < _winning) _state = GameState.Winning;
  }

  protected override void Draw(GameTime gameTime)
  {
    GraphicsDevice.SetRenderTarget(_renderTarget);
    GraphicsDevice.Clear(new Color(45, 55, 75));

    _spriteBatch.Begin();
    
    // paddle left
    _spriteBatch.Draw(_pixel, _paddleLeft.Bounds, Color.White);

    // paddle right
    _spriteBatch.Draw(_pixel, _paddleRight.Bounds, Color.White);

    // ball
    _spriteBatch.Draw(_pixel, _ball.Bounds, Color.White);

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

    _spriteBatch.Begin();
    for (int i = 0; i < CANVAS_HEIGHT; i+=20)
      _spriteBatch.Draw(_pixel, new Rectangle(CANVAS_WIDTH / 2 - BALL_SIZE / 2, i, 4, 10), Color.White * 0.3f);

    if (_state == GameState.Serving)
    {
      string serveMsg = $"Press Space for Serv";
      Vector2 serveMsgSize = _myFont.MeasureString(serveMsg);
      _spriteBatch.DrawString(_myFont, serveMsg, new Vector2(CANVAS_WIDTH / 2 - serveMsgSize.X / 2, 150), Color.White);
    }

    if (_state == GameState.Winning)
    {
      string winMsg = _scoreLeft != _winning ? $"Player 2 is Winning" : $"Player 1 is Winning";
      Vector2 winMsgSize = _myFont.MeasureString(winMsg);
      _spriteBatch.DrawString(_myFont, winMsg, new Vector2(CANVAS_WIDTH / 2 - winMsgSize.X / 2, 200), Color.Gold);
    }

    _spriteBatch.End();

    base.Draw(gameTime);
  }

  public enum GameState
  {
    Serving,
    Playing,
    Scored,
    Winning
  }
}
