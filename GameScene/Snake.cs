using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Scene;

namespace GameScene;

public enum Direction
{
  Up = 0b0001,
  Down = 0b0010,
  Left = 0b0100,
  Right = 0b1000,

  Y = Up | Down,
  X = Left | Right,
}


public class SnakeScene : Scene.Scene
{
  private Song _bgm;
  private SoundEffect _eat;
  private SoundEffect _levelUp;
  private SoundEffect _gameOver;
  private Texture2D _lattice;
  private Texture2D _circle;
  private Point _food;
  private Direction _direction = Direction.Right;
  public static readonly int _height = 720;
  public static readonly int _width = 1280;
  public static readonly int _latticeSize = 20;
  private readonly int _rows;
  private readonly int _cols;
  private readonly int _padding = 2;
  private readonly Random _random = new();
  private readonly List<Point> _snake = [];
  private double _moveTimer = 0;
  private double _moveInterval = .1;
  private int _score = 0;

  public SnakeScene(SceneContext context) : base(context)
  {
    _rows = _height / _latticeSize;
    _cols = _width / _latticeSize;
    Content.RootDirectory = "Content";
  }

  public override void Initialize()
  {
    _lattice = GetTexture(GraphicsDevice, Color.White);
    _circle = GetCircle(GraphicsDevice, _latticeSize / 2, Color.Orange);
    _food = GenerateFood();
    _snake.Add(new(_padding, _padding));
    base.Initialize();
  }

  public override void LoadContent()
  {
    _bgm = Content.Load<Song>("music/bgm");
    _eat = Content.Load<SoundEffect>("music/eat");
    _levelUp = Content.Load<SoundEffect>("music/level up");
    _gameOver = Content.Load<SoundEffect>("music/game over");

    MediaPlayer.IsRepeating = true;
    MediaPlayer.Volume = .5f;
    if (MediaPlayer.State == MediaState.Playing)
    {
      MediaPlayer.Stop();
    }
    MediaPlayer.Play(_bgm);
    base.LoadContent();
  }

  public override void Update(GameTime gametime)
  {
    _moveTimer += gametime.ElapsedGameTime.TotalSeconds;

    if (_moveTimer >= _moveInterval)
    {
      _moveTimer -= _moveInterval;

      if (_snake[0] == _food)
      {
        _food = GenerateFood();
        _score++;
        _eat.Play();

        var tail = _snake[^1];

        switch (_direction)
        {
          case Direction.Up:
            tail.Y++;
            break;
          case Direction.Down:
            tail.Y--;
            break;
          case Direction.Left:
            tail.X++;
            break;
          case Direction.Right:
            tail.X--;
            break;
        }
        _snake.Add(new(tail.X, tail.Y));
        _levelUp.Play();
      }

      for (int i = _snake.Count - 1; i >= 0; i--)
      {
        if (i != 0)
        {
          _snake[i] = _snake[i - 1];
        }
        else
        {
          var head = _snake[i];
          switch (_direction)
          {
            case Direction.Up:
              head.Y--;
              break;
            case Direction.Down:
              head.Y++;
              break;
            case Direction.Left:
              head.X--;
              break;
            case Direction.Right:
              head.X++;
              break;
          }
          if (head.X < _padding) head.X = _cols - _padding - 1;
          if (head.X >= _cols - _padding) head.X = _padding;
          if (head.Y < _padding) head.Y = _rows - _padding - 1;
          if (head.Y >= _rows - _padding) head.Y = _padding;
          _snake[i] = head;
        }
      }
    }

    base.Update(gametime);
  }

  protected override void ListenKeyboard(KeyboardState currentState, KeyboardState prevState)
  {
    if (currentState.IsKeyDown(Keys.Up) || currentState.IsKeyDown(Keys.W))
    {
      ChangeDirection(Direction.Up);
    }
    if (currentState.IsKeyDown(Keys.Down) || currentState.IsKeyDown(Keys.S))
    {
      ChangeDirection(Direction.Down);
    }
    if (currentState.IsKeyDown(Keys.Left) || currentState.IsKeyDown(Keys.A))
    {
      ChangeDirection(Direction.Left);
    }
    if (currentState.IsKeyDown(Keys.Right) || currentState.IsKeyDown(Keys.D))
    {
      ChangeDirection(Direction.Right);
    }
    base.ListenKeyboard(currentState, prevState);
  }

  public override void Draw(GameTime gameTime)
  {
    GraphicsDevice.Clear(Color.CornflowerBlue);

    Context.SpriteBatch.Begin();
    Context.SpriteBatch.Draw(_circle, new Rectangle(_food.X * _latticeSize, _food.Y * _latticeSize, _latticeSize, _latticeSize), Color.Orange);
    Context.SpriteBatch.End();

    Context.SpriteBatch.Begin();
    foreach (var item in _snake)
    {
      Context.SpriteBatch.Draw(_lattice, new Rectangle(item.X * _latticeSize, item.Y * _latticeSize, _latticeSize, _latticeSize), Color.Black);
    }
    Context.SpriteBatch.End();

    Context.SpriteBatch.Begin();
    var scoreText = $"Score: {_score}";
    var scoreSize = Context.Font.MeasureString(scoreText);
    Context.SpriteBatch.DrawString(Context.Font, scoreText, scoreSize, Color.Black);
    Context.SpriteBatch.End();

    base.Draw(gameTime);
  }

  private Point GenerateFood()
  {
    Point food = new(_padding, _padding);
    while (true)
    {
      int x = _random.Next(_padding, _cols - _padding - 1);
      int y = _random.Next(_padding, _rows - _padding - 1);
      food.X = x;
      food.Y = y;
      if (!_snake.Contains(food))
        break;
    }
    return food;
  }

  private void ChangeDirection(Direction direction)
  {
    if (
      _direction == direction
      || (_direction | direction) == Direction.Y
      || (_direction | direction) == Direction.X
    )
    {
      return;
    }
    _direction = direction;
  }

  private static Texture2D GetTexture(GraphicsDevice graphics, Color color)
  {
    Texture2D texture = new(graphics, 1, 1);
    texture.SetData([color]);
    return texture;
  }

  private static Texture2D GetCircle(GraphicsDevice graphics, int radius, Color color)
  {
    int diameter = radius * 2;
    Texture2D texture = new(graphics, diameter, diameter);
    Color[] colorData = new Color[diameter * diameter];

    for (int y = 0; y < diameter; y++)
    {
      for (int x = 0; x < diameter; x++)
      {
        int dx = x - radius;
        int dy = y - radius;
        if (dx * dx + dy * dy <= radius * radius)
        {
          if (dx * dx + dy * dy <= radius * radius)
          {
            colorData[y * diameter + x] = color;
          }
          else
          {
            colorData[y * diameter + x] = Color.Transparent;
          }
        }
      }
    }

    texture.SetData(colorData);

    return texture;
  }
}
