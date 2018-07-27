A simple wrapper around Unity's new experimental Vector API, letting you draw as if you were inside a HTML5 canvas. Demonstrates basic fills, strokes and fill rules.

For example:

```
m_Graphics.MoveTo(900, 150);
m_Graphics.BezierCurveTo(910, 250, 940, 250, 950, 250);
m_Graphics.LineTo(1000, 150);
m_Graphics.Stroke();

// and so on...

```

![Demo Screen](/Assets/DemoScreen.png)
