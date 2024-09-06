using Microsoft.Xna.Framework;

namespace ShamansDungeon.GameManagement;

// Oscar
public class Camera
{
    public Vector2 Position { get; set; }           // The camera's position in the game world.
    public Vector2 Origin;
    public float Zoom;                 // The zoom level of the camera.
    private Matrix _transformMatrix;    // A transformation matrix used for rendering.

    private float _maxZoom;

    public Camera()
    {
        Position = Vector2.Zero;   // Initialize the camera's position at the origin.
        Zoom = 1f;                 // Initialize the camera's zoom level to 1f
        _maxZoom = 2f;              // Any higher will remove sprites since it would be inside the camera
        SetOriginCenter();
    }

    /// <summary>
    /// Call this method again if the screen size gets changed in the middle of the game.
    /// </summary>
    public void SetOriginCenter()
    {
        Origin = new Vector2(GameWorld.Instance.GfxManager.PreferredBackBufferWidth / 2, GameWorld.Instance.GfxManager.PreferredBackBufferHeight / 2);
    }

    /// <summary>
    /// Update the camera's position by adding a vector.
    /// </summary>
    /// <param name="delta"></param>
    public void Move(Vector2 delta)
    {
        if (!float.IsNaN(delta.X))
        {
            Position += delta;
        }
    }

    #region Parameters

    public Vector2 TopLeft
    {
        get { return Position - new Vector2(GameWorld.Instance.GfxManager.PreferredBackBufferWidth / 2, GameWorld.Instance.GfxManager.PreferredBackBufferHeight / 2); }
    }

    public Vector2 TopCenter
    {
        get { return Position - new Vector2(0, GameWorld.Instance.GfxManager.PreferredBackBufferHeight / 2); }
    }

    public Vector2 TopRight
    {
        get { return Position + new Vector2(GameWorld.Instance.GfxManager.PreferredBackBufferWidth / 2, -GameWorld.Instance.GfxManager.PreferredBackBufferHeight / 2); }
    }

    public Vector2 LeftCenter
    {
        get { return Position - new Vector2(GameWorld.Instance.GfxManager.PreferredBackBufferWidth / 2, 0); }
    }

    public Vector2 Center
    {
        get { return Position; }
    }

    public Vector2 RightCenter
    {
        get { return Position + new Vector2(GameWorld.Instance.GfxManager.PreferredBackBufferWidth / 2, 0); }
    }

    public Vector2 BottomLeft
    {
        get { return Position + new Vector2(-GameWorld.Instance.GfxManager.PreferredBackBufferWidth / 2, GameWorld.Instance.GfxManager.PreferredBackBufferHeight / 2); }
    }

    public Vector2 BottomCenter
    {
        get { return Position + new Vector2(0, GameWorld.Instance.GfxManager.PreferredBackBufferHeight / 2); }
    }

    public Vector2 BottomRight
    {
        get { return Position + new Vector2(GameWorld.Instance.GfxManager.PreferredBackBufferWidth / 2, GameWorld.Instance.GfxManager.PreferredBackBufferHeight / 2); }
    }

    #endregion Parameters

    /// <summary>
    /// Dosent work
    /// <para>Change zoom amount, but is limited by some paramaters like maxzoom.</para>
    /// </summary>
    /// <param name="amount"></param>
    public void ChangeZoom(float amount)
    {
        Zoom += amount;

        if (Zoom < 1f) Zoom = 1f; //Cant get under 1 zoom
        else if (Zoom > _maxZoom) Zoom = _maxZoom;
    }

    public Matrix GetMatrix()
    {
        // Create a transformation matrix that represents the camera's view.
        // This matrix is used to adjust rendering based on the camera's position and zoom level.

        // 1. Translate to the negative of the camera's position.
        Matrix translationMatrix = Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0));

        // 2. Scale the view based on the camera's zoom level.
        Matrix scaleMatrix = Matrix.CreateScale(Zoom);

        // 3. Translate the view to center it on the screen.
        // This assumes the camera view is centered within the game window.
        // The following lines center the view using the screen's dimensions.
        Matrix centerMatrix = Matrix.CreateTranslation(new Vector3(Origin.X, Origin.Y, 0));

        // Combine the matrices in the correct order to create the final transformation matrix.
        _transformMatrix = translationMatrix * scaleMatrix * centerMatrix;

        return _transformMatrix; // Return the transformation matrix for rendering.
    }
}