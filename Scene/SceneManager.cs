using Microsoft.Xna.Framework;

namespace Scene;

public class SceneManager
{
  private Scene currentScene;

  public void LoadScene(Scene scene)
  {
    currentScene?.Dispose();
    currentScene = scene;
    currentScene.Initialize();
  }

  public void Update(GameTime gameTime)
  {
    currentScene?.Update(gameTime);
  }

  public void Draw(GameTime gameTime)
  {
    currentScene?.Draw(gameTime);
  }
}
