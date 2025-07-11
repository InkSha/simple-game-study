using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Scene;

public struct Click(
  Rectangle rectangle,
  Action action
)
{
  public Rectangle Rectangle = rectangle;
  public Action Action = action;
}

public class Title : Scene
{
  private (float xPadding, float yPadding) padding;
  private Texture2D texture;
  protected List<Click> clicks = [];

  public Title(SceneContext context) : base(context)
  {
    float rate = 0.1f;
    padding = (
      Math.Min(context.GraphicsDevice.Viewport.Width * rate, 100),
      Math.Min(context.GraphicsDevice.Viewport.Height * rate, 100)
    );
  }

  public Vector2 GetPosition<T>(T x, T y) where T : System.Numerics.INumber<T>
  {
    return new(
      float.CreateChecked(x) + padding.xPadding,
      float.CreateChecked(y) + padding.yPadding
    );
  }

  public override void LoadContent()
  {
    base.LoadContent();
    texture = new(Context.GraphicsDevice, 1, 1);
    texture.SetData([Color.White]);
  }

  public override void Update(GameTime gametime)
  {
    base.Update(gametime);
  }

  public override void Draw(GameTime gameTime)
  {
    base.Draw(gameTime);

    Context.SpriteBatch.Begin();
    var title = "Title 标题";
    var titleSize = Context.Font.MeasureString(title);
    var center = (Context.GraphicsDevice.Viewport.Width + titleSize.X) / 2;
    Context.SpriteBatch.DrawString(Context.Font, title, new Vector2(center, padding.yPadding), Color.White);
    Context.SpriteBatch.End();

    Context.SpriteBatch.Begin();
    for (int i = 1; i < 6; i++)
    {
      var option = $"options {i}";
      var optionSize = Context.Font.MeasureString(option);
      var size = 50;
      var position = new Vector2(padding.xPadding, padding.yPadding + i * size * 2);
      var region = new Click(new((int)position.X, (int)position.Y, 100, size), () =>
      {
        Console.WriteLine($"click options {option}");
      });
      clicks.Add(region);
      Context.SpriteBatch.Draw(texture, region.Rectangle, Color.Aqua);
      Context.SpriteBatch.DrawString(Context.Font, option, position, Color.Black);
    }

    Context.SpriteBatch.End();
  }

  protected override void ListenMouse(MouseState currentState, MouseState prevState)
  {
    if (!(currentState.LeftButton == ButtonState.Pressed && prevState.LeftButton == ButtonState.Released))
    {
      return;
    }

    foreach (var reg in clicks)
    {
      if (reg.Rectangle.Contains(currentState.Position))
      {
        Console.WriteLine("click", reg.Rectangle);
        reg.Action();
      }
    }
  }
}
