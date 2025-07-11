using GameScene;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Scene;

namespace simple_game_studty;

public class Game1 : Game
{
  private GraphicsDeviceManager _graphics;
  private SpriteBatch _spriteBatch;
  private SpriteFont _font;
  private SceneContext _sceneContext;
  private SceneManager _sceneManager;

  public Game1()
  {
    _graphics = new GraphicsDeviceManager(this);
    _graphics.PreferredBackBufferHeight = SnakeScene._height;
    _graphics.PreferredBackBufferWidth = SnakeScene._width;
    _graphics.ApplyChanges();
    Content.RootDirectory = "Content";
    IsMouseVisible = true;
  }

  protected override void Initialize()
  {
    base.Initialize();
  }

  protected override void LoadContent()
  {
    _spriteBatch = new SpriteBatch(GraphicsDevice);
    _font = Content.Load<SpriteFont>("default");
    _sceneContext = new(Services, Content.RootDirectory, GraphicsDevice, _spriteBatch, _font);
    _sceneManager = new();
    _sceneManager.LoadScene(new Title(_sceneContext));
  }

  protected override void Update(GameTime gameTime)
  {
    base.Update(gameTime);
    _sceneManager.Update(gameTime);
  }

  protected override void Draw(GameTime gameTime)
  {
    GraphicsDevice.Clear(Color.CornflowerBlue);
    _sceneManager.Draw(gameTime);
    base.Draw(gameTime);
  }
}
