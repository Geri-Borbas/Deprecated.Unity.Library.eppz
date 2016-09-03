using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TriangleNet.Algorithm;
using TriangleNet.Geometry;
using TriangleNet.Data;
using TriangleNet.Tools;
using MeshExplorer.Generators;



namespace EPPZ.Geometry
{


	public static class Geometry_UnityEngine
	{


	#region Polygon

		public static UnityEngine.Mesh Mesh(this Polygon this_, string name = "")
		{
			// Create geometry.
			InputGeometry geometry = this_.InputGeometry();

			// Triangulate.
			TriangleNet.Mesh triangulatedMesh = new TriangleNet.Mesh();
			triangulatedMesh.Triangulate(geometry);

			// Counts.
			int vertexCount = triangulatedMesh.vertices.Count;
			int triangleCount = triangulatedMesh.triangles.Count;

			// Debug.Log("Mesh.vertexCount ("+vertexCount+")"); // NumberOfInputPoints
			// Debug.Log("Mesh.triangleCount ("+triangleCount+")"); // NumberOfInputPoints

			// Mesh store.
			Vector3[] _vertices = new Vector3[vertexCount];
			Vector2[] _uv = new Vector2[vertexCount];
			Vector3[] _normals = new Vector3[vertexCount];
			int[] _triangles = new int[triangleCount * 3];

			foreach (KeyValuePair<int, TriangleNet.Data.Vertex> eachEntry in triangulatedMesh.vertices)
			{
				int index = eachEntry.Key;
				TriangleNet.Data.Vertex eachVertex = eachEntry.Value;

				_vertices[index] = new Vector3(
					(float)eachVertex.x,
					(float)eachVertex.y,
					0.0f // As of 2D
				);

				_uv[index] = _vertices[index];
				_normals[index] = Vector3.forward;
			}

			int cursor = 0;
			foreach (KeyValuePair<int, TriangleNet.Data.Triangle> eachPair in triangulatedMesh.triangles)
			{
				TriangleNet.Data.Triangle eachTriangle = eachPair.Value;
				_triangles[cursor] = eachTriangle.P2;
				_triangles[cursor + 1] = eachTriangle.P1;
				_triangles[cursor + 2] = eachTriangle.P0;
				cursor += 3;
			}

			// Create / setup mesh.
			Mesh mesh = new Mesh();
			mesh.vertices = _vertices;
			mesh.uv = _uv;
			mesh.normals = _normals;
			mesh.subMeshCount = 1;
			mesh.SetTriangles(_triangles, 0);
			mesh.name = name;

			return mesh;
		}

	#endregion

	}
}

