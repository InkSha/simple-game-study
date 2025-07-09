using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GameDemo;

public enum Direction
{
  Up = 0b0001,
  Down = 0b0010,
  Left = 0b0100,
  Right = 0b1000,

  Y = Up | Down,
  X = Left | Right,
}

public class Snake : Game
{
  private GraphicsDeviceManager _graphics;
  private SpriteBatch _spriteBatch;
  private SpriteFont _font;
  private Song _bgm;
  private SoundEffect _eat;
  private SoundEffect _levelUp;
  private SoundEffect _gameOver;
  private Texture2D _lattice;
  private Texture2D _circle;
  private Point _food;
  private Direction _direction = Direction.Right;
  private readonly int _height;
  private readonly int _width;
  private readonly int _latticeSize;
  private readonly int _rows;
  private readonly int _cols;
  private readonly int _padding = 2;
  private readonly Random _random = new();
  private readonly List<Point> _snake = [];
  private double _moveTimer = 0;
  private double _moveInterval = .1;
  private int _score = 0;

  public Snake(int width = 1280, int height = 720, int latticeSize = 20)
  {
    // _windowSize = (width, height);
    _width = width;
    _height = height;
    _rows = height / latticeSize;
    _cols = width / latticeSize;
    _latticeSize = latticeSize;
    _graphics = new GraphicsDeviceManager(this);
    Content.RootDirectory = "Content";
    IsMouseVisible = true;
  }

  protected override void Initialize()
  {
    _graphics.PreferredBackBufferWidth = _width;
    _graphics.PreferredBackBufferHeight = _height;
    _graphics.ApplyChanges();
    _lattice = GetTexture(GraphicsDevice, Color.White);
    _circle = GetCircle(GraphicsDevice, _latticeSize / 2, Color.Orange);
    _food = GenerateFood();
    _snake.Add(new(_padding, _padding));
    base.Initialize();
  }

  protected override void LoadContent()
  {
    _spriteBatch = new SpriteBatch(GraphicsDevice);
    _font = Content.Load<SpriteFont>("default");
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
  }

  protected override void Update(GameTime gameTime)
  {
    _moveTimer += gameTime.ElapsedGameTime.TotalSeconds;

    var keyboardState = Keyboard.GetState();
    if (keyboardState.IsKeyDown(Keys.Up))
    {
      ChangeDirection(Direction.Up);
    }
    if (keyboardState.IsKeyDown(Keys.Down))
    {
      ChangeDirection(Direction.Down);
    }
    if (keyboardState.IsKeyDown(Keys.Left))
    {
      ChangeDirection(Direction.Left);
    }
    if (keyboardState.IsKeyDown(Keys.Right))
    {
      ChangeDirection(Direction.Right);
    }


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
    base.Update(gameTime);
  }

  protected override void Draw(GameTime gameTime)
  {
    GraphicsDevice.Clear(Color.CornflowerBlue);

    _spriteBatch.Begin();
    _spriteBatch.Draw(_circle, new Rectangle(_food.X * _latticeSize, _food.Y * _latticeSize, _latticeSize, _latticeSize), Color.Orange);
    _spriteBatch.End();

    _spriteBatch.Begin();
    foreach (var item in _snake)
    {
      _spriteBatch.Draw(_lattice, new Rectangle(item.X * _latticeSize, item.Y * _latticeSize, _latticeSize, _latticeSize), Color.Black);
    }
    _spriteBatch.End();

    _spriteBatch.Begin();
    var scoreText = $"Score: {_score}";
    var scoreSize = _font.MeasureString(scoreText);
    _spriteBatch.DrawString(_font, scoreText, scoreSize, Color.Black);
    _spriteBatch.End();

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
