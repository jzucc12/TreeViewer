using UnityEngine;
using UnityEngine.UIElements;

namespace JZ.Common
{
    /// <summary>
    /// Some helpful mesh extensions
    /// </summary>
    public static class MeshExtensions
    {
        /// <summary>
        /// Creates a mesh from a list of points intended for a visual elements "generate content" functionality.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="resolution"></param>
        /// <param name="thickness"></param>
        /// <param name="lineColor"></param>
        /// <param name="vertices"></param>
        /// <param name="indices"></param>
         public static void MakeVEMesh(this MeshGenerationContext mgc, Vector3[] points, int resolution, float thickness, Color lineColor)
         {
            //Get vertices and indices
            ushort[] indices = new ushort[(resolution-1) * 6];
            Vertex[] vertices = new Vertex[2*resolution];
            for(int ii = 0; ii < resolution; ii++)
            {
                //Determine normal vector
                Vector3 currentDir;
                if(ii == 0)
                {
                    currentDir = points[ii+1] - points[ii];
                }
                else if(ii == resolution - 1)
                {
                    currentDir = points[ii] - points[ii-1];
                }
                else
                {
                    currentDir = points[ii+1] - points[ii-1];
                }
                Vector3 currentNormal = Quaternion.Euler(0, 0, 90) * currentDir.normalized;

                //Set vertices
                int verticesIndex = 2 * ii;
                Vector3 thicknessVector = currentNormal * thickness * 0.5f;
                vertices[verticesIndex].position = points[ii] - thicknessVector;
                vertices[verticesIndex+1].position = points[ii] + thicknessVector;
                vertices[verticesIndex].tint = lineColor;
                vertices[verticesIndex+1].tint = lineColor;

                //Set indices
                if(ii < resolution - 1)
                {
                    int indicesIndex = 6 * ii;
                    indices[indicesIndex]   = (ushort)(verticesIndex);
                    indices[indicesIndex+1] = (ushort)(verticesIndex+3);
                    indices[indicesIndex+2] = (ushort)(verticesIndex+1);
                    indices[indicesIndex+3] = (ushort)(verticesIndex+3);
                    indices[indicesIndex+4] = (ushort)(verticesIndex);
                    indices[indicesIndex+5] = (ushort)(verticesIndex+2);
                }
            }

            //Create mesh
            MeshWriteData mw = mgc.Allocate(vertices.Length, indices.Length);
            mw.SetAllVertices(vertices);
            mw.SetAllIndices(indices);
         }
    }
}