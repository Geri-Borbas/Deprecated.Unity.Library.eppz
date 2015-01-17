using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;


namespace EPPZ
{


	public class EPPZ_CircleMesh2D : MonoBehaviour
	{

		
		public const int MINIMUM_SEGMENTS = 3;
		public const int MAXIMUM_SEGMENTS = 2048;

		public const float DEFAULT_RADIUS = 1.0f;
		public const int DEFAULT_SEGMENTS = 36;


		private float _radius = DEFAULT_RADIUS;
		public float radius
		{
			get
			{ return _radius; }
			set
			{
				if (value == _radius) return; // Only if changed
				_radius = ValidRadius(value); // Validate / Set

				// Layout only.
				layoutMesh(); 
			}
		}

		public float ValidRadius(float radius_)
		{ return Mathf.Clamp(radius_, 0.0f, Mathf.Infinity); } // Clamp
		
		private int _segments = DEFAULT_SEGMENTS;
		public int segments		
		{
			get { return _segments; }
			set
			{
				if (value == _segments) return; // Only if changed
				_segments = ValidSegments(value); // Validate / Set

				// Recreate mesh data.
				calculateUnitCircleVertices();
				buildTopology();
				layoutMesh();
			}
		}
		
		public int ValidSegments(int segments_)
		{ return Mathf.Clamp(segments_, MINIMUM_SEGMENTS, MAXIMUM_SEGMENTS); } // Clamp


		// Circle geometry.
		private float _theta; 
		private float _tangetialFactor;
		private
			float _radialFactor;

		// Unit circle.
		private Vector3[] unitCircleVertices;

		// Mesh properties.
		private int _vertexCount;
		private int _triangleCount;

		// Mesh data (should keep synced whenever vertices change).
		private Vector3[] _vertices;
		private Mesh _mesh;


		void Start()
		{
			calculateUnitCircleVertices();
			buildTopology();
			layoutMesh();
		}

		void calculateUnitCircleVertices()
		{
			Debug.Log(name+".calculateUnitCircleVertices()");

			// Allocate vertices.
			unitCircleVertices = new Vector3[segments];

			// Smart circle algorithm from http://slabode.exofire.net/circle_draw.shtml.
			_theta = 2 * Mathf.PI / (float)segments; 
			_tangetialFactor = Mathf.Tan(_theta);
			_radialFactor = Mathf.Cos(_theta);

			// Start at angle 0.0f 
			float x = 1.0f;
			float y = 0; 

			for(int i = 0; i < segments; i++) 
			{
				// Vertices.
				unitCircleVertices[i] = new Vector3(x, y, 0.0f);
				
				// Calculate the tangential vector.
				float tx = -y; 
				float ty = x; 
				
				// Add the tangential vector. 
				x += tx * _tangetialFactor; 
				y += ty * _tangetialFactor; 
				
				// Correct using the radial factor. 
				x *= _radialFactor; 
				y *= _radialFactor;
			}
		}

		void buildTopology()
		{   
			Debug.Log(name+".buildTopology()");

			_vertexCount = segments + 1; // +1 for center
			_triangleCount = segments;

			_vertices = new Vector3[_vertexCount];
			Vector2[] uv = new Vector2[_vertexCount];
			Vector3[] normals = new Vector3[_vertexCount];
			int[] triangles = new int[_triangleCount * 3];

			// Circle circumfence vertices.
			for(int i = 0; i < _vertexCount - 1; i++) 
			{
				_vertices[i] = unitCircleVertices[i]; // Vertices (still with 1 unit radius)
				uv[i] = new Vector2(unitCircleVertices[i].x * 0.5f + 0.5f, unitCircleVertices[i].y * 0.5f + 0.5f); // UV (planar)
				normals[i] = Vector3.forward; // Normals
			}   

			// Center vertex.
			int i_c = _vertexCount - 1; // Is the last
			_vertices[i_c] = Vector3.zero; // Center
			uv[i_c] = new Vector2(0.5f, 0.5f); // UV (texture center)
			normals[i_c] = Vector3.forward; // Normals

			// Triangles (strip).
			for (int i = 0; i < _triangleCount; i++) // Build triangle pairs per segment
			{
				// Vertex indices.
				int i_0 = i;
				int i_1 = i_c;
				int i_2 = i_0 + 1;
				
				// The last of the triangles.
				if (i_0 >= _triangleCount - 1)
				{
					i_2 = 0; 
				}

				// Cake slice face (CCW).
				int i_triangle = i * 3;
				triangles[i_triangle] = i_0;
				triangles[i_triangle+1] = i_1;
				triangles[i_triangle+2] = i_2;
			}

			// (Re)create / setup mesh.
			_mesh = new Mesh();
			_mesh.vertices = _vertices;
			_mesh.uv = uv;
			_mesh.normals = normals;
			_mesh.subMeshCount = 1;
			_mesh.SetTriangles(triangles, 0);

			// Assign component.
			MeshFilter meshFilter = GetComponent<MeshFilter>();
			meshFilter.sharedMesh = _mesh;
		}

		void layoutMesh()
		{
			if (_mesh == null) return; // Only having any mesh built

			// Name indicating dimensions.
			_mesh.name = "EPPZ_CircleMesh2D ("+radius+", "+segments+")";

			// Layout circumfence vertices.
			for(int i = 0; i < _vertexCount - 1; i++) 
			{
				// Vertices.
				_vertices[i] = unitCircleVertices[i] * _radius; // Outer
			}    

			// Sets.
			_mesh.vertices = _vertices;
		}
	}


	#if UNITY_EDITOR
	[CustomEditor(typeof(EPPZ_CircleMesh2D))]
	public class EPPZ_CircleMesh2D_Editor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			
			EPPZ_CircleMesh2D circleMesh = target as EPPZ_CircleMesh2D;
			
			circleMesh.radius = EditorGUILayout.FloatField("Radius", circleMesh.radius);
			circleMesh.segments = EditorGUILayout.IntField("Segments", circleMesh.segments);
		}
	}
	#endif
}
