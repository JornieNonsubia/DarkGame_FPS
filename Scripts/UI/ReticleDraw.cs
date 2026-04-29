using Godot;
using System.Collections.Generic;

[Tool]
public partial class ReticleDraw : Control
{
    private float _radius = 30.0f;
    private float _thickness = 1.0f;
    private Color _color = Colors.White;
    private float _gapAngle = 45.0f;
    private int _segments = 32;

    [Export] public float Radius
    {
        get => _radius;
        set
        { _radius = value;
          QueueRedraw(); }
    }

    [Export] public float Thickness
    {
        get => _thickness;
        set
        { _thickness = value;
          QueueRedraw(); }
    }

    [Export] public Color CrosshairColor
    {
        get => _color;
        set
        { _color = value;
          QueueRedraw(); }
    }

    [Export] public float GapAngle
    {
        get => _gapAngle;
        set
        { _gapAngle = value;
          QueueRedraw(); }
    }

    [Export] public int Segments
    {
        get => _segments;
        set
        { _segments = value;
          QueueRedraw(); }
    }

    public override void _Draw()
    {
        DrawCircleCrosshair();
    }

    private void DrawCircleCrosshair()
    {
        float gapRad = Mathf.DegToRad(_gapAngle);
        Vector2 center = Size / 2; // Центрування в межах Control

        // Визначення квадрантів (початок та кінець дуги)
        float[][] arcSegments = new float[][]
        {
            new float[] { gapRad / 2, Mathf.Pi / 2 - gapRad / 2 },
            new float[] { Mathf.Pi / 2 + gapRad / 2, Mathf.Pi - gapRad / 2 },
            new float[] { Mathf.Pi + gapRad / 2, 3 * Mathf.Pi / 2 - gapRad / 2 },
            new float[] { 3 * Mathf.Pi / 2 + gapRad / 2, 2 * Mathf.Pi - gapRad / 2 }
        };

        foreach (var arc in arcSegments)
        {
            float startAngle = arc[0];
            float endAngle = arc[1];

            var points = new List<Vector2>();
            float angleStep = (endAngle - startAngle) / _segments;

            for (int i = 0; i <= _segments; i++)
            {
                float angle = startAngle + i * angleStep;
                Vector2 point = center + new Vector2(
                    _radius * Mathf.Cos(angle),
                    _radius * Mathf.Sin(angle)
                );
                points.Add(point);
            }

            if (points.Count > 1)
            {
                DrawPolyline(points.ToArray(), _color, _thickness, true);
            }
        }
    }
}