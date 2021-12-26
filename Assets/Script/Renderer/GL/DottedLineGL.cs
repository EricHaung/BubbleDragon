using UnityEngine;

namespace BubbleGL
{
    public class DottedLineGL : IGL
    {
        public Color color;
        public Vector3 starPosition;
        public Vector3 endPosition;

        public DottedLineGL(Color _color, Vector3 _starPosition, Vector3 _endPosition)
        {
            color = _color;
            starPosition = _starPosition;
            endPosition = _endPosition;
            GLRenderer.Instance.Add(this);
        }

        public void UpdatePosition(Vector3 _starPosition, Vector3 _endPosition)
        {
            starPosition = _starPosition;
            endPosition = _endPosition;
        }

        public void Draw()
        {
            //待補虛線方法
            GLRenderer.Instance.lineMaterial.SetPass(0);
            GL.LoadOrtho();
            GL.Begin(GL.LINES);
            GL.Color(color);
            GL.Vertex(starPosition);
            GL.Vertex(endPosition);
            GL.End();
        }

        public void Dispose()
        {
            GLRenderer.Instance.Remove(this);
        }
    }
}