using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;


namespace EPPZ
{


	public class EPPZ_RingMesh2D : MonoBehaviour
	{


		public const int MINIMUM_SEGMENTS = 3;
		public const int MAXIMUM_SEGMENTS = 2048;
		
		public const float DEFAULT_RADIUS = 1.0f;
		public const float DEFAULT_WIDTH = 0.25f;
		public const int DEFAULT_SEGMENTS = 36;

		public enum Mapping { Planar, Circular }
		public const Mapping DEFAULT_MAPPING = Mapping.Planar;


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
		{ return Mathf.Clamp(radius_, _width, Mathf.Infinity); } // Clamp

		private float _width = DEFAULT_WIDTH;
		public float width
		{
			get { return _width; }
			set
			{
				if (value == _width) return; // Only if changed
				_width = ValidWidth(value); // Validate / Set

				// Layout only.
				layoutMesh(); 
			}
		}

		public float ValidWidth(float width_)
		{ return Mathf.Clamp(width_, 0.0f, _radius); } // Clamp
		
		private int _segments = DEFAULT_SEGMENTS;
		public int segments		
		{
			get { return _segments; }
			set
			{
				if (value == _segments) return; // Only if changed
				_segments = ValidSegments(value); // Validate (against values set from code instead of inspector)

				// Recreate mesh data.
				calculateUnitCircleVertices();
				buildTopology();
				layoutMesh();
			}
		}
		
		public int ValidSegments(int segments_)
		{ return Mathf.Clamp(segments_, MINIMUM_SEGMENTS, MAXIMUM_SEGMENTS); } // Clamp

		private Mapping _mapping = DEFAULT_MAPPING;
		public Mapping mapping
		{
			get { return _mapping; }
			set
			{
				if (value == _mapping) return; // Only if changed
				_mapping = value; // Validate / Set
				
				// Layout only.
				layoutMesh(); 
			}
		}


		// Circle geometry.
		private float _theta; 
		private float _tangetialFactor;
		private float _radialFactor;

		// Unit circle.
		private Vector3[] unitCircleVertices;

		// Mesh properties.
		private int _vertexCount;
		private int _triangleCount;
		private float _innerRadius;

		// Mesh data (should keep synced whenever vertices change).
		private Vector3[] _vertices;
		private Vector2[] _uv;
		private Mesh _mesh;


		void Start()
		{
			calculateUnitCircleVertices();
			buildTopology();
			layoutMesh();
		}

		void calculateUnitCircleVertices()
		{
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
			_vertexCount = segments * 2;
			_triangleCount = segments * 2;

			_vertices = new Vector3[_vertexCount];
			_uv = new Vector2[_vertexCount];
			Vector3[] normals = new Vector3[_vertexCount];
			int[] triangles = new int[_triangleCount * 3];
			
			for(int i = 0; i < _vertexCount; i+= 2) 
			{
				// Vertices.
				int i_unit = i / 2; // unitCircleVertices index
				_vertices[i] = unitCircleVertices[i_unit]; // Outer (still with 1 unit radius)
				_vertices[i+1] = unitCircleVertices[i_unit]; // Inner (still with 1 unit radius)

				// UV layed out in `layoutMesh()`

				// Normals.
				normals[i] = Vector3.forward;
				normals[i+1] = Vector3.forward;
			}     

			// Triangles (strip).
			for (int i = 0; i < triangles.Length; i+= 6) // Build triangle pairs per segment
			{
				// Vertex indices.
				int i_0 = i / 3;
				int i_1 = i_0 + 1;
				int i_2 = i_0 + 2;
				int i_3 = i_0 + 3;

				// The last of the triangles.
				if (i_0 >= _vertexCount - 2)
				{
					i_2 = 0;
					i_3 = 1;
				}

				// Outer triangle (CCW).
				triangles[i] = i_0;
				triangles[i+1] = i_1;
				triangles[i+2] = i_2;

				// Inner triangle (CCW).
				triangles[i+3] = i_2;
				triangles[i+4] = i_1;
				triangles[i+5] = i_3;
			}

			// (Re)create / setup mesh.
			_mesh = new Mesh();
			_mesh.vertices = _vertices;
			_mesh.uv = _uv;
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
			_mesh.name = "EPPZ_RingMesh2D ("+radius+", "+width+", "+segments+")";

			// Calculate inner radius.
			_innerRadius = radius - width;
			float _innerRadiusFactor = _innerRadius / _radius;

			// Layout vertices.
			for(int i = 0; i < _vertexCount; i+= 2) 
			{
				// Vertices.
				int i_unit = i / 2; // unitCircleVertices index
				_vertices[i] = unitCircleVertices[i_unit] * _radius; // Outer
				_vertices[i+1] = unitCircleVertices[i_unit] * _innerRadius; // Inner

				// UV (planar mapping).
				if (mapping == Mapping.Planar)
				{
					Vector2 polar = new Vector2(unitCircleVertices[i_unit].x * 0.5f, unitCircleVertices[i_unit].y * 0.5f);
					Vector2 offset = new Vector2(0.5f, 0.5f);
					_uv[i] = polar + offset; // Outer
					_uv[i+1] = polar * _innerRadiusFactor + offset; // Inner
				}

				// UV (circular mapping).
				else
				{
					float innerOffset = (1.0f - _innerRadiusFactor) / 2.0f;
					bool evenVertexPair = (i % 4 != 0 || i == _vertexCount - 1);
					if (evenVertexPair)
					{
						_uv[i] = new Vector2(0.0f, 1.0f); // Outer
						_uv[i+1] = new Vector2(innerOffset, 0.0f);// Inner
					}
					else
					{
						_uv[i] = new Vector2(1.0f, 1.0f); // Outer
						_uv[i+1] = new Vector2(1.0f - innerOffset, 0.0f); // Inner
					}
				}
			}    

			// Set.
			_mesh.vertices = _vertices;
			_mesh.uv = _uv;
		}
	}

	#if UNITY_EDITOR
	[CustomEditor(typeof(EPPZ_RingMesh2D))]
	public class EPPZ_RingMesh2D_Editor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			
			EPPZ_RingMesh2D ringMesh = target as EPPZ_RingMesh2D;
			ringMesh.radius = EditorGUILayout.FloatField("Radius", ringMesh.radius);
			ringMesh.width = EditorGUILayout.FloatField("Width", ringMesh.width);
			ringMesh.segments = EditorGUILayout.IntField("Segments", ringMesh.segments);
			ringMesh.mapping = (EPPZ_RingMesh2D.Mapping)EditorGUILayout.EnumPopup("Mapping", ringMesh.mapping);
		}
	}
	#endif
}
