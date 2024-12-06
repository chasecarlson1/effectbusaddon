namespace Chase;
using Godot;

public partial class Test : Node2D
{
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (Input.IsMouseButtonPressed(MouseButton.Left))
        {
            Transform2D xform = new(0f, new(1f, 1f), 0f, GetGlobalMousePosition());
            EffectBus.EffectBus.PlayEffect(new("test", xform, Colors.RosyBrown));
        }
    }
}