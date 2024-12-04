namespace EffectBus;
using Godot;

public static class Events
{
    /// <summary>
    /// uint Kind, Transform2D Transform, Color Color, float Speed
    /// </summary>
    public readonly struct EffectData
    {
        public readonly int Kind;
        public readonly Transform2D Transform;
        public readonly Color Color;
        public readonly float Speed;

        public EffectData(int kind, Transform2D transform, Color color, float speed = 1F)
        {
            Kind = kind;
            Transform = transform;
            Color = color;
            Speed = speed;
        }
    }
    public static event System.Action<EffectData>? Effect;
    public static void DoEffect(EffectData data)
    {
        Effect?.Invoke(data);
    }
}