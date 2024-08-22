using DoctorsDungeon.CommandPattern;
using DoctorsDungeon.ComponentPattern.GUI;
using DoctorsDungeon.GameManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DoctorsDungeon.ComponentPattern.Path;

public enum CellWalkableType
{
    NotValid,
    FullValid,
}

// Oscar
public class Cell : Component
{
    public static int Dimension = 16;

    //public static readonly Vector2 ScaleSize = new(4, 4);
    public static int Scale = 4;

    private SpriteRenderer _spriteRenderer;

    /// <summary>
    /// Used when selecting which room is active on each grid. Base is -1, so they dont count as a room
    /// </summary>
    public int CollisionNr { get; set; } = -1;

    public int RoomNr { get; set; } = -1;

    public bool ShouldDraw { get; set; }

    // For the Astar algortihm
    public CellWalkableType CellWalkableType;

    public int cost = 1;

    public int G;
    public int H;
    public int F => G + H;

    public Color NotDiscoveredColor => GameWorld.BackGroundColor;

    /// <summary>
    /// Parent is for the Astar, not the GameObject that is attached as "GameObject".
    /// </summary>
    public GameObject Parent { get; set; }

    public Cell(GameObject gameObject, Grid grid, Point point) : base(gameObject)
    {
        GameObject.Transform.GridPosition = point;
        GameObject.Transform.Scale = new(Scale, Scale);

        CellWalkableType = CellWalkableType.NotValid;

        // Centers the position of the cell.
        GameObject.Transform.Position = grid.StartPostion
            + new Vector2(point.X * Dimension * Scale + Dimension * Scale / 2,
                          point.Y * Dimension * Scale + Dimension * Scale / 2);
    }

    public Cell(GameObject gameObject, Grid grid, Point point, CellWalkableType type) : base(gameObject)
    {
        GameObject.Transform.GridPosition = point;
        GameObject.Transform.Scale = new(Scale, Scale);

        CellWalkableType = type;

        // Centers the position of the cell.
        GameObject.Transform.Position = grid.StartPostion
            + new Vector2(point.X * Dimension * Scale + Dimension * Scale / 2,
                          point.Y * Dimension * Scale + Dimension * Scale / 2);
    }

    public Cell(GameObject gameObject, Grid grid, Point point, CellWalkableType type, int collisionNr, int roomNr) : base(gameObject)
    {
        GameObject.Transform.GridPosition = point;
        GameObject.Transform.Scale = new(Scale, Scale);

        CellWalkableType = type;
        CollisionNr = collisionNr;
        RoomNr = roomNr;

        // Centers the position of the cell.
        GameObject.Transform.Position = grid.StartPostion
            + new Vector2(point.X * Dimension * Scale + Dimension * Scale / 2,
                          point.Y * Dimension * Scale + Dimension * Scale / 2);
    }

    public override void Start()
    {
        _spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null) throw new System.Exception("Cell need a spriteRenderer");

        ChangeCellWalkalbeType(CellWalkableType); //Just the same here, so it turns the correct color.
    }

    /// <summary>
    /// Resets the cell, to make it ready for another path.
    /// </summary>
    public void Reset()
    {
        Parent = null;
        G = 0;
        H = 0;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (_spriteRenderer == null) return;

        if (!InputHandler.Instance.DebugMode) return;
        Vector2 offset = new Vector2(10, 0);
        GuiMethods.DrawTextCentered(spriteBatch, GlobalTextures.DefaultFont, GameObject.Transform.Position - offset, RoomNr.ToString(), Color.Yellow);
        GuiMethods.DrawTextCentered(spriteBatch, GlobalTextures.DefaultFont, GameObject.Transform.Position + offset, CollisionNr.ToString(), Color.HotPink);
    }

    public void ChangeCellWalkalbeType(CellWalkableType cellWalkableType)
    {
        if (_spriteRenderer == null) return;

        if (GridManager.Instance.CurrentDrawSelected == DrawMapSelecter.DrawBlackedOutRooms) return;

        if (InputHandler.Instance.DebugMode)
        {
            if (CollisionNr == -1) GameObject.IsEnabled = false;
            else GameObject.IsEnabled = true;
        }

        CellWalkableType = cellWalkableType;

        switch (CellWalkableType)
        {
            case CellWalkableType.FullValid:
                _spriteRenderer.Color = Color.DarkOliveGreen;
                break;
        }
    }
}