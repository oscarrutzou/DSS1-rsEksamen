using DoctorsDungeon.ComponentPattern;
using DoctorsDungeon.ComponentPattern.WorldObjects;
using DoctorsDungeon.GameManagement;

namespace DoctorsDungeon.Factory;

public static class TransferDoorFactory
{
    // Oscar
    public static GameObject Create()
    {
        GameObject go = new();
        go.Transform.Scale = new(4, 4);

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.SetLayerDepth(LayerDepth.BackgroundDecoration);
        sr.SetSprite(TextureNames.DoorClosed);
        sr.IsCentered = false;

        go.AddComponent<Collider>();
        go.AddComponent<TransferDoor>();

        return go;
    }
}