using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VectorGraphics;


public class Graphics : MonoBehaviour
{
    //private Path m_Path;
    private Scene m_Scene;
    private VectorUtils.TessellationOptions m_Options;

    private Mesh m_mesh;
    public float PixelsPerUnit = 100;
    private float m_LineWidth = 1f;
    private Color m_LineColor = Color.black;
    private Color m_FillColor = Color.black;
    private IFill m_CurrentFill = new SolidFill() { Color = Color.white };

    private List<BezierPathSegment> m_Segments = new List<BezierPathSegment>();
    private List<BezierContour> m_Contours = new List<BezierContour>();

    private Shape m_CurrentShape;



    public void SetLineColor(float r, float g, float b, float a = 1)
    {
        m_LineColor = new Color(r, g, b, a);
    }

    public float LineWidth
    {
        get
        {
            return m_LineWidth;
        }
        set
        {
            m_LineWidth = value / PixelsPerUnit;
        }
    }

    public IFill FillStyle
    {
        get
        {
            return m_CurrentFill;
        }
        set
        {
            m_CurrentFill = value;
        }
    }

    public void SetFillColor(float r, float g, float b, float a = 1)
    {
        m_CurrentFill = new SolidFill() { Color = new Color(r, g, b, a) };
        m_FillColor = new Color(r, g, b, a);
    }

    private PathProperties NewPathProperties()
    {
        if (m_LineWidth > 0.0f)
        {
            var v = LocalPoint(m_LineWidth, 0) - LocalPoint(0, 0);

            return new PathProperties()
            {
                Stroke = new Stroke()
                {
                    Color = m_LineColor,
                    HalfThickness = m_LineWidth / 2//v.magnitude / 2
                }
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

    void addFillAndStroke(Shape shape)
    {
        addFill(shape);
        addStroke(shape);
      
       
    }

    void addFill(Shape shape)
    {
        shape.Fill = new SolidFill() { Color = m_FillColor };
    }

    void addStroke(Shape shape)
    {
        shape.PathProps = new PathProperties()
        {
            Stroke = new Stroke() { Color = m_LineColor }
        };
        if (shape.PathProps.Stroke.HalfThickness == 0)
            shape.PathProps.Stroke.HalfThickness = m_LineWidth / 2;
    }

    void addToScene(Shape shape)
    {
       
        m_Scene.Root.Shapes.Add(shape);//.Children.Add(node);
        m_Contours.Clear();
    }

    public void Rect(float x, float y, float w, float h)
    {
        m_CurrentShape = new Shape();
        VectorUtils.MakeRectangleShape(m_CurrentShape, new Rect(x/PixelsPerUnit, y/PixelsPerUnit, w/PixelsPerUnit, h/PixelsPerUnit));
        //addToScene(rectShape);
    }

    public void Circle(float x, float y, float r)
    {
        m_CurrentShape = new Shape();
        VectorUtils.MakeCircleShape(m_CurrentShape, new Vector2(x/PixelsPerUnit, y/PixelsPerUnit), r/PixelsPerUnit);
        addToScene(m_CurrentShape);
    }


    public void Ellipse(float x, float y, float rx, float ry)
    {
        m_CurrentShape = new Shape();
        VectorUtils.MakeEllipseShape(m_CurrentShape, new Vector2(x/PixelsPerUnit, y/PixelsPerUnit), rx/PixelsPerUnit, ry/PixelsPerUnit);
        
    }


    public void MoveTo(float x, float y, bool clearShape = true)
    {
        if (clearShape)
            m_CurrentShape = new Shape();
        m_Segments.Clear();
        m_Segments.Add(new BezierPathSegment()
        {
            P0 = LocalPoint(x/PixelsPerUnit, y/PixelsPerUnit)//(x / PixelsPerUnit, y / PixelsPerUnit)
        }) ;
    }

    public void LineTo(float x, float y)
    {
       
        if (m_Segments.Count == 0)
        {
            MoveTo(x, y);
        }
        var n = m_Segments.Count;

        var a = m_Segments[n - 1].P0;
      
        var b = LocalPoint(x/PixelsPerUnit, y/PixelsPerUnit);
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
        var b = LocalPoint(cx1/PixelsPerUnit, cy1/PixelsPerUnit);
        var c = LocalPoint(cx2/PixelsPerUnit, cy2/PixelsPerUnit);
        var d = LocalPoint(x/PixelsPerUnit, y/PixelsPerUnit);

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
        m_CurrentShape = new Shape();
       // m_Segments.Clear();
        var rWorld = (LocalPoint((x/PixelsPerUnit) + (r/PixelsPerUnit), (y/PixelsPerUnit)) - LocalPoint(x/PixelsPerUnit, y/PixelsPerUnit)).magnitude;
        var segments = VectorUtils.MakeArc(LocalPoint(x / PixelsPerUnit, y / PixelsPerUnit), startAngle, endAngle - startAngle, rWorld);// rWorld/PixelsPerUnit);
        foreach (var s in segments)
        {
            m_Segments.Add(s);
        }
    }

    public void Fill()
    {
        //var drawables = m_Scene.Root.Drawables;

         CloseContour();
         if (m_Contours.Count > 0)
         {
            m_CurrentShape.Contours = m_Contours.ToArray();
             m_Contours.Clear();
            
         }
         else
         {
             /*var d = drawables[drawables.Count - 1];
             if (d != null)
             {
                 if (d is Filled)
                 {
                     ((Filled)d).Fill = m_CurrentFill;
                 }
             }*/
         }
        addFill(m_CurrentShape);
        addToScene(m_CurrentShape);
    }

    public void Stroke()
    {
        CloseContour();
        if (m_Contours.Count > 0)
        {
            m_CurrentShape.Contours = m_Contours.ToArray();
            m_CurrentShape.PathProps = NewPathProperties(); 
        }
        else
        {
            /* var d = drawables[drawables.Count - 1];
             if (d != null)
             {
                 if (d is Filled)
                 {
                     ((Filled)d).PathProps = NewPathProperties();
                 }
             }*/
        }


        addStroke(m_CurrentShape);
        addToScene(m_CurrentShape);
    }

    private void Awake()
    {
        LineWidth = m_LineWidth; //to kick in the PPU
        m_Scene = new Scene()
        {
            Root = new SceneNode() { }

        };
        m_Scene.Root.Children = new List<SceneNode>();
        m_Scene.Root.Shapes = new List<Shape>();

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
        // m_Scene.Root.Drawables.Clear();
        m_mesh.Clear();
    }

    public void End()
    {
        //m_mesh.Clear();
        var geoms = VectorUtils.TessellateScene(m_Scene, m_Options);
        VectorUtils.FillMesh(m_mesh, geoms, 1.0f);
    }

    private Vector2 LocalPoint(float x, float y)
    {
        // var currentCamera = Camera.main;
        //var p = currentCamera.ScreenToWorldPoint(new Vector3(x, y));
        return new Vector2(x, y);//(p.x, p.y);
    }


}
