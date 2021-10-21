using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace InputDemoRecorder
{
    public class ColliderVisualizer : MonoBehaviour
    {
        static Material lineMaterial;
        static Color TriggerVolumeColor = Color.green;
        static Color ColliderColor = Color.blue;
        static Color BoundsColor = Color.red;
        private const int MAX_COLLIDERS_TO_DRAW = 10;
        private Collider[] collidersToDraw = new Collider[MAX_COLLIDERS_TO_DRAW];
        private int amountToDraw = 0;

        private void Start()
        {
            StartCoroutine("UpdateCollidersListWithDelay");
        }
        private void UpdateCollidersList(float radius)
        {
            amountToDraw = Physics.OverlapSphereNonAlloc(Locator.GetActiveCamera().transform.position, radius, collidersToDraw);
        }
        //From unity GL docs
        static void CreateLineMaterial()
        {
            if (!lineMaterial)
            {
                Shader shader = Shader.Find("Hidden/Internal-Colored");
                lineMaterial = new Material(shader)
                {
                    hideFlags = HideFlags.HideAndDontSave
                };
                // Turn on alpha blending
                lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                // Turn backface culling off
                lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                // Turn off depth writes
                lineMaterial.SetInt("_ZWrite", 0);
            }
        }
        private IEnumerator UpdateCollidersListWithDelay()
        {
            while (true)
            {
                UpdateCollidersList(100f);
                yield return new WaitForSeconds(0.5f);
            }
        }

        public void OnRenderObject()
        {
            CreateLineMaterial();
            lineMaterial.SetPass(0);

            //Collider Bounds
            GL.PushMatrix();
            GL.MultMatrix(Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one));

            for (int i = 0; i < amountToDraw; i++)
            {
                if (collidersToDraw[i] != null)
                    GLHelpers.DrawWireframeCube(collidersToDraw[i].bounds.size, collidersToDraw[i].bounds.center - collidersToDraw[i].bounds.extents, BoundsColor);
            }
            GL.PopMatrix();

            //Collider 
            for (int i = 0; i < amountToDraw; i++)
            {
                if (collidersToDraw[i] != null)
                {
                    Color colorToUse = collidersToDraw[i].isTrigger ? TriggerVolumeColor : ColliderColor;
                    Type colliderType = collidersToDraw[i].GetType();
                    if (colliderType == typeof(BoxCollider))
                    {
                        BoxCollider box = (BoxCollider)collidersToDraw[i];
                        GL.PushMatrix();
                        GL.MultMatrix(collidersToDraw[i].transform.localToWorldMatrix);
                        GLHelpers.DrawWireframeCube(box.size, box.center - box.size / 2f, colorToUse);
                        GL.PopMatrix();
                    }
                    else if (colliderType == typeof(SphereCollider))
                    {
                        SphereCollider sphere = (SphereCollider)collidersToDraw[i];
                        GL.PushMatrix();
                        GL.MultMatrix(collidersToDraw[i].transform.localToWorldMatrix);
                        GLHelpers.DrawSimpleWireframeSphere(sphere.radius, sphere.center, colorToUse, 12);
                        GL.PopMatrix();
                    }
                    else if (colliderType == typeof(CapsuleCollider))
                    {
                        CapsuleCollider capsule = (CapsuleCollider)collidersToDraw[i];
                        GL.PushMatrix();
                        GL.MultMatrix(collidersToDraw[i].transform.localToWorldMatrix);
                        GLHelpers.DrawSimpleWireframeCapsule(capsule.radius, capsule.height, capsule.center, colorToUse, 12);
                        GL.PopMatrix();
                    }
                }
            }
        }
    }
}
