namespace Scene;

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public struct SceneContext(
  IServiceProvider ServiceProvider,
  string RootDirectory,
  GraphicsDevice GraphicsDevice,
  SpriteBatch SpriteBatch,
  SpriteFont Font
)
{
  public IServiceProvider ServiceProvider = ServiceProvider;
  public string RootDirectory = RootDirectory;
  public GraphicsDevice GraphicsDevice = GraphicsDevice;
  public SpriteBatch SpriteBatch = SpriteBatch;
  public SpriteFont Font = Font;
}

public class Scene : IDisposable
{
  public Action ToggleScene;

  private MouseState prevMouseState;
  private KeyboardState prevKeyboardState;

  protected readonly ContentManager Content;
  protected readonly GraphicsDevice GraphicsDevice;
  protected readonly SpriteBatch SpriteBatch;
  protected readonly SceneContext Context;

  public bool IsDisposed { get; private set; }

  public Scene(SceneContext context)
  {
    GraphicsDevice = context.GraphicsDevice;
    SpriteBatch = context.SpriteBatch;
    Context = context;
    Content = new(context.ServiceProvider)
    {
      RootDirectory = context.RootDirectory
    };
  }

  ~Scene() => Dispose(false);

  public virtual void Initialize()
  {
    LoadContent();
  }

  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }

  public virtual void Dispose(bool disposing)
  {
    if (IsDisposed)
    {
      return;
    }
    if (disposing)
    {
      UnloadContent();
      Content.Dispose();
    }
  }

  public virtual void LoadContent() { }

  public virtual void UnloadContent() { }

  public virtual void Update(GameTime gametime)
  {
    if (IsDisposed)
    {
      return;
    }

    var keyboardState = Keyboard.GetState();
    var mouseState = Mouse.GetState();

    ListenKeyboard(keyboardState, prevKeyboardState);

    ListenMouse(mouseState, prevMouseState);

    prevKeyboardState = keyboardState;
    prevMouseState = mouseState;
  }

  public virtual void Draw(GameTime gameTime) { }

  protected virtual void ListenKeyboard(KeyboardState currentState, KeyboardState prevState) { }

  protected virtual void ListenMouse(MouseState currentState, MouseState prevState) { }
}
