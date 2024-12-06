namespace EffectBus;

using System.Collections.Generic;
using Godot;

[GlobalClass]
[Icon("res://addons/effectbus/effect_bus_icon.png")]
public partial class EffectBus : Node
{
    /// <summary>
    /// uint Kind, Transform2D Transform, Color Color, float Speed
    /// </summary>
    public readonly struct EffectData
    {
        public readonly StringName Name;
        public readonly Transform2D Transform;
        public readonly Color Color;
        public readonly float Speed;

        public EffectData(StringName name, Transform2D transform, Color color, float speed = 1F)
        {
            Name = name;
            Transform = transform;
            Color = color;
            Speed = speed;
        }
    }
    private static event System.Action<EffectData>? Effect;
    public static void PlayEffect(EffectData data)
    {
        Effect?.Invoke(data);
    }
    private partial class Instance : AnimatedSprite2D
    {
        private int frameCount;
        private StringName[] animNames;
        private bool hideOnFinish;
        public Instance(SpriteFrames frames, bool offsetCentered, Vector2 offset, bool hideOnFinish, bool zIndexRelative, int zIndex)
        {
            SpriteFrames = frames;
            Offset = offset;
            Centered = offsetCentered;
            this.hideOnFinish = hideOnFinish;
            ZIndex = zIndex;
            ZAsRelative = zIndexRelative;
            var names = frames.GetAnimationNames();
            animNames = new StringName[names.Length];
            int i = 0;
            foreach (var s in names)
            {
                animNames[i++] = new StringName(s);
            }
        }
        public override void _Ready()
        {
            base._Ready();
            if (Engine.IsEditorHint()) return;
            Hide();
            PhysicsInterpolationMode = PhysicsInterpolationModeEnum.Off;
            if (hideOnFinish)
            {
                AnimationFinished += Hide;
            }
        }
        public override void _ExitTree()
        {
            base._ExitTree();
            if (Engine.IsEditorHint()) return;
            if (hideOnFinish)
            {
                AnimationFinished -= Hide;
            }
        }
        public void Start(Transform2D transform, Color color, float speed)
        {
            if (Engine.IsEditorHint()) return;
            Show();
            if (animNames.Length < 1) return;
            if (IsPlaying()) Stop();
            GlobalTransform = transform;
            SelfModulate = color;
            SpeedScale = speed;
            Play(animNames[GD.RandRange(0, animNames.Length - 1)], speed);
            Frame = 0;
        }
    }
    private class Pool
    {
        private readonly Instance[] instances;
        private int index = 0;
        public Pool(Node root, SpriteFrames frames, int count, bool hideOnFinish, bool offsetCentered, Vector2 offset, bool zIndexRelative, int zIndex)
        {
            instances = new Instance[count];
            for (int i = 0; i < count; i++)
            {
                Instance instance = new(frames, offsetCentered, offset, hideOnFinish, zIndexRelative, zIndex);
                instances[i] = instance;
                root.CallDeferred(Node.MethodName.AddChild, instance);
            }
        }
        public void Play(Transform2D transform, Color color, float speed)
        {
            if (Engine.IsEditorHint()) return;
            instances[index].Start(transform, color, speed);
            index = Mathf.PosMod(index + 1, instances.Length);
        }
    }
    public static class EffectId
    {
        public const int STEP = 0;
    }
    [Export]
    private Godot.Collections.Array<SpriteFramesData> SpriteFrameData = [];

    private readonly Dictionary<StringName, Pool> pools = [];
    public override void _Ready()
    {
        base._Ready();
        if (Engine.IsEditorHint()) return;
        var parent = GetParent<Node>();
        foreach (var data in SpriteFrameData)
        {
            var frames = data.Frames;
            if (frames != null)
                pools.Add(data.Name, new(
                    parent, frames, data.Count, data.OffsetCentered, data.HideOnFinish, data.Offset, data.ZIndexRelative, data.ZIndex
                    ));
        }
        Effect += OnEffect;
    }
    private void OnEffect(EffectData data)
    {
        if (pools.TryGetValue(data.Name, out var pool))
        {
            pool.Play(data.Transform, data.Color, data.Speed);
        }
        else
        {
            GD.PrintErr($"ERROR: trying to play an EffectBus effect of StringName name '{data.Name}' that did not exist in the effect bus' exported variable!");
        }
    }
}