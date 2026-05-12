namespace DevWinUI;

public partial class LiveGraph
{
    public void AddDynamicPoint(string key, GraphPoint point)
    {
        AddDynamicPoint(key, point, false);
    }
    public void AddDynamicPoint(string key, GraphPoint point, bool isRounded)
    {
        if (canvas == null) return;

        float height = (float)canvas.Size.Height;
        float canvasWidth = (float)canvas.Size.Width;

        if (!livePolygons.TryGetValue(key, out var polygon))
        {
            // Create a new live polygon if it doesn't exist
            polygon = new UserPolygon
            {
                OffsetX = 0,
                IsRounded = isRounded,
                Key = key
            };

            livePolygons[key] = polygon;
            polygons.Add(polygon);
        }

        float startX = polygon.Points.Count > 0
            ? polygon.Points.Last().X + point.Space
            : canvasWidth + 10;

        float y = NormalizeY(point.Value, height);

        polygon.CurrentY = y;

        polygon.Points.Add(new Vector2(startX, y));
    }

    public void AddStaticPoints(string key, IEnumerable<GraphPoint> points)
    {
        AddStaticPoints(key, points, false);
    }
    public void AddStaticPoints(string key, IEnumerable<GraphPoint> points, bool isRounded)
    {
        if (canvas == null) return;

        var height = (float)canvas.Size.Height;
        float startX = (float)canvas.Size.Width + 10; // start just outside view

        var newPolygon = new UserPolygon();
        float currentX = startX;

        foreach (var p in points)
        {
            float y = NormalizeY(p.Value, height);
            newPolygon.Points.Add(new Vector2(currentX, y));
            currentX += p.Space; // move to next X
        }

        newPolygon.OffsetX = 0;
        newPolygon.IsRounded = isRounded;
        newPolygon.Key = key;

        polygons.Add(newPolygon);
    }
}
