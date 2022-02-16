using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo2D : MonoBehaviour {
    private Graphics m_Graphics;

    private void Awake()
    {
        m_Graphics = gameObject.AddComponent<Graphics>();
    }

    void Start ()
    {
        m_Graphics.LineWidth = 2;
        m_Graphics.Begin();
        m_Graphics.Rect(10, 15, 100, 100);
        m_Graphics.SetFillColor(1, 0, 0);
        m_Graphics.SetLineColor(0, 0, 0);
        m_Graphics.Fill();
        //m_Graphics.Stroke();
        
   
        m_Graphics.SetFillColor(0, 0, 1);
        m_Graphics.LineWidth = 5;
        m_Graphics.Circle(200, 200, 50);
        m_Graphics.Fill();
        m_Graphics.Stroke();
      

        m_Graphics.SetLineColor(0, 1, 0);
        m_Graphics.MoveTo(300, 150);
        m_Graphics.LineTo(350, 250);
         m_Graphics.LineTo(400, 150);
         m_Graphics.LineTo(300, 150);
        m_Graphics.Stroke();
        

        m_Graphics.SetLineColor(0, 0, 0);
        m_Graphics.MoveTo(500, 150);
        m_Graphics.LineTo(550, 250);
        m_Graphics.LineTo(600, 150);
        m_Graphics.LineTo(500, 150);
        m_Graphics.Fill();
        m_Graphics.Stroke();
        
        m_Graphics.MoveTo(700, 150);
        m_Graphics.BezierCurveTo(710, 250, 740, 250, 750, 250);
        m_Graphics.LineTo(800, 150);
        m_Graphics.LineTo(700, 150);
        m_Graphics.Fill();
        m_Graphics.Stroke();
      
        m_Graphics.MoveTo(900, 150);
        m_Graphics.BezierCurveTo(910, 250, 940, 250, 950, 250);
        m_Graphics.LineTo(1000, 150);
        m_Graphics.Stroke();
        
      m_Graphics.Ellipse(100, -200, 50, 30);
        m_Graphics.Fill();
        m_Graphics.Stroke();
        
      m_Graphics.SetLineColor(0, 0, 0);
      m_Graphics.MoveTo(130, -150);
      m_Graphics.LineTo(180, -50);
      m_Graphics.LineTo(230, -150);
      m_Graphics.LineTo(130, -150);
       m_Graphics.BeginContour();
      m_Graphics.MoveTo(200, -120, false);//don't clear the shape 
      m_Graphics.LineTo(180, -80);
      m_Graphics.LineTo(160, -120);
      m_Graphics.LineTo(200, -120);
      m_Graphics.Fill();
      m_Graphics.Stroke();
       
        m_Graphics.Arc(900, 0, 150, 0.0f, 2.0f * Mathf.PI / 3.0f);
      m_Graphics.Fill();
      m_Graphics.Stroke();
      
        m_Graphics.End();
	}
	
}
