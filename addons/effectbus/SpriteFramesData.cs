using Godot;

namespace EffectBus;

[GlobalClass]
public partial class SpriteFramesData : Resource
{
    [Export]
    public StringName Name = "effect";
    [Export]
    public SpriteFrames? Frames;
    [Export(PropertyHint.Range, "1, 50, 1, or_greater")]
    public int Count = 20;
    [Export]
    public bool OffsetCentered = true;
    [Export]
    public Vector2 Offset = Vector2.Zero;
    [Export]
    public bool HideOnFinish = true;
    [Export]
    public bool ZIndexRelative = true;
    [Export]
    public int ZIndex = 0;
}