using System.Collections.Generic;
using UnityEngine;

namespace BubbleGL
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class GLRenderer : MonoBehaviour
    {
        private static GLRenderer instance;

        public static GLRenderer Instance
        {
            private set { }
            get { return instance; }
        }

        private List<IGL> renderObjects;
        public Material lineMaterial;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                SetUp();
            }
            else
                Destroy(this);
        }

        public void OnPostRender()
        {
            Draw();
        }

        public void SetUp()
        {
            instance = this;
            renderObjects = new List<IGL>();
        }

        public void Add(IGL gl)
        {
            renderObjects.Add(gl);
        }

        public void Remove(IGL gl)
        {
            renderObjects.Remove(gl);
        }

        public void Draw()
        {
            GL.PushMatrix();
            try
            {
                if (renderObjects != null)
                    for (int i = 0; i < renderObjects.Count; ++i)
                    {
                        IGL line = renderObjects[i];
                        line.Draw();
                    }
            }
            finally
            {
                GL.PopMatrix();
            }
        }
    }
}