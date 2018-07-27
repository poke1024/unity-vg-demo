using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VectorGraphics;

public class Graphics : MonoBehaviour
{
    private Path m_Path;
    private Scene m_Scene;
    private VectorUtils.TessellationOptions m_Options;

    private Mesh m_mesh;

    private float m_LineWidth = 1.0f;
    private Color m_LineColor = Color.black;
    private IFill m_CurrentFill = new SolidFill() { Color = Color.white };

    private List<BezierPathSegment> m_Segments = new List<BezierPathSegment>();
    private List<BezierContour> m_Contours = new List<BezierContour>();

    public void SetLineColor(float r, float g, float b, float a = 1)
    {
        m_LineColor = new Color(r, g, b, a);
    }

    public float LineWidth
    {
        get {
            return m_LineWidth;
        }
        set {
            m_LineWidth = value;
        }
    }

    public IFill FillStyle
    {
        get {
            return m_CurrentFill;
        }
        set {
            m_CurrentFill = value;
        }
    }

    public void SetFillColor(float r, float g, float b, float a = 1)
    {
        m_CurrentFill = new SolidFill() { Color = new Color(r, g, b, a) };
    }

    private PathProperties NewPathProperties()
    {
        if (m_LineWidth > 0.0f)
        {
            var v = LocalPoint(m_LineWidth, 0) - LocalPoint(0, 0);

            return new PathProperties()
            {
                Stroke = new Stroke() {
                    Color = m_LineColor,
                    HalfThickness = v.magnitude / 2 }
            };
        }
        else
        {
            return new PathProperties();
        }
    }

    public void BeginContour()
    {
        CloseContour();
    }

    public void CloseContour()
    {
        if (m_Segments.Count > 0)
        {
            var contour = new BezierContour()
            {
                Segments = m_Segments.ToArray()
            };

            m_Contours.Add(contour);
            m_Segments.Clear();

            if (VectorUtils.PathEndsPerfectlyMatch(contour.Segments))
            {
                contour.Closed = true;
            }
        }
    }

    public void Rect(float x, float y, float w, float h)
    {
        var rectangle = new Rectangle()
        {
            Position = LocalPoint(x, y),
            Size = LocalPoint(x + w, y + h) - LocalPoint(x, y),
            FillTransform = Matrix2D.identity
        };
        m_Scene.Root.Drawables.Add(rectangle);
    }

    public void Circle(float x, float y, float r)
    {
        m_Scene.Root.Drawables.Add(VectorUtils.MakeCircle(
            LocalPoint(x, y),
            (LocalPoint(x + r, y) - LocalPoint(x, y)).magnitude));
    }

    public void Ellipse(float x, float y, float rx, float ry)
    {
        m_Scene.Root.Drawables.Add(VectorUtils.MakeEllipse(
            LocalPoint(x, y),
            (LocalPoint(x + rx, y) - LocalPoint(x, y)).magnitude,
            (LocalPoint(x + ry, y) - LocalPoint(x, y)).magnitude));
    }


    public void MoveTo(float x, float y)
    {
        m_Segments.Clear();
        m_Segments.Add(new BezierPathSegment()
        {
            P0 = LocalPoint(x, y)
        });
    }

    public void LineTo(float x, float y)
    {
        if (m_Segments.Count == 0) {
            MoveTo(x, y);
        }
        var n = m_Segments.Count;

        var a = m_Segments[n - 1].P0;
        var b = LocalPoint(x, y);
        var line = VectorUtils.MakeLine(a, b);

        m_Segments[n - 1] = new BezierPathSegment()
        {
            P0 = line.P0,
            P1 = line.P1,
            P2 = line.P2
        };

        m_Segments.Add(new BezierPathSegment()
        {
            P0 = line.P3
        });
    }

    public void BezierCurveTo(float cx1, float cy1, float cx2, float cy2, float x, float y)
    {
        if (m_Segments.Count == 0)
        {
            MoveTo(x, y);
        }
        var n = m_Segments.Count;

        var a = m_Segments[n - 1].P0;
        var b = LocalPoint(cx1, cy1);
        var c = LocalPoint(cx2, cy2);
        var d = LocalPoint(x, y);

        m_Segments[n - 1] = new BezierPathSegment()
        {
            P0 = a,
            P1 = b,
            P2 = c
        };

        m_Segments.Add(new BezierPathSegment()
        {
            P0 = d
        });
    }

    public void Arc(float x, float y, float r, float startAngle, float endAngle)
    {
        var rWorld = (LocalPoint(x + r, y) - LocalPoint(x, y)).magnitude;
        var segments = VectorUtils.MakeArc(LocalPoint(x, y), startAngle, endAngle - startAngle, rWorld);
        foreach (var s in segments) {            
            m_Segments.Add(s);
        }
    }

    public void Fill()
    {
        var drawables = m_Scene.Root.Drawables;

        CloseContour();
        if (m_Contours.Count > 0)
        {
            drawables.Add(new Shape()
            {
                Contours = m_Contours.ToArray(),
                Fill = m_CurrentFill
            });
            m_Contours.Clear();
        }
        else
        {
            var d = drawables[drawables.Count - 1];
            if (d != null)
            {
                if (d is Filled)
                {
                    ((Filled)d).Fill = m_CurrentFill;
                }
            }
        }
    }

    public void Stroke()
    {
        var drawables = m_Scene.Root.Drawables;

        CloseContour();
        if (m_Contours.Count > 0)
        {
            drawables.Add(new Shape()
            {
                Contours = m_Contours.ToArray(),
                PathProps = NewPathProperties()
            });
            m_Contours.Clear();
        }
        else
        {
            var d = drawables[drawables.Count - 1];
            if (d != null)
            {
                if (d is Filled)
                {
                    ((Filled)d).PathProps = NewPathProperties();
                }
            }
        }
    }

    private void Awake()
    {
        m_Scene = new Scene()
        {
           Root = new SceneNode() { Drawables = new List<IDrawable> { } }
        };

        m_Options = new VectorUtils.TessellationOptions()
        {
            StepDistance = 1000.0f,
            MaxCordDeviation = 0.05f,
            MaxTanAngleDeviation = 0.05f,
            SamplingStepSize = 0.01f
        };

        m_mesh = new Mesh();

        var meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = m_mesh;

        var meshRenderer = gameObject.AddComponent<MeshRenderer>();

        Material material = Resources.Load(
            "Default2D", typeof(Material)) as Material;
        meshRenderer.materials = new Material[] { material };
    }

    public void Begin()
    {
        m_Scene.Root.Drawables.Clear();
    }

    public void End()
    {
        m_mesh.Clear();
        var geoms = VectorUtils.TessellateScene(m_Scene, m_Options);
        VectorUtils.FillMesh(m_mesh, geoms, 1.0f);
    }

    private Vector2 LocalPoint(float x, float y)
    {
        var currentCamera = Camera.main;
        var p = currentCamera.ScreenToWorldPoint(new Vector3(x, y));
        return new Vector2(p.x, p.y);
    }
}
