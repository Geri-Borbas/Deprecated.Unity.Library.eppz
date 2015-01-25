* Doin'

	+ `EPPZGeometry_Polygon` / `EPPZGeometry_IntersectionVertex`
		+ Create Offset polygon from raw offset polygon
			+ Intersections are determined
				+ Construct offset polygon
					+ First without loop polygons
					+ Then safely with a single loop polygon
					+ Then full loop polygon validating

	+ Sub polygons within polygons
		+ Test
			+ Point containment
			+ Permiter point containment
			+ Segment intersection
			+ Polygon inside test

* 0.1.7 (02:20)

	+ Polygon offset algorithm
		+ Fixed `Edge.nextEdge` calculations
		+ Loop polygon tests implemented
		+ Fixed `IntersectionVertex.CalculateIntersection` calculations
		+ Test scene
			+ Created intersection vertex renderers (really custom only for this)
			+ Intersection vertices can be inspected

* 0.1.6 (01:00)

	+ Intersection point and intersection test has merged
		+ Uses `Physics.Raycast` API style (bool return, output parameter as result)
		+ Can use accuracy, containment method
		+ Some documentation

* 0.1.54

	+ Replaced intersection algorithm

* 0.1.53

	+ Implemented `08 Segment-segment intersection point` test scene
	+ Hooked in segment intersection algorithm to `Geometry`

* 0.1.52

	+ Created `08 Segment-segment intersection point` test scene

* 0.1.51

	+ GitHub deploy
	+ Detached version control from tangram! project included as a submodule

* 0.1.5

	+ Fixed `08 Polygon offset` test scene
	+ Scoping

* 0.1.4

	+ Polygon offset extracted
		+ `Edge`
			+ `perpendicular` / `normal` calculations
			+ Automatic calculation can be managed by `alwaysCalculate`
		+ `Vertex`
			+ `bisector` / `normal` calculations
			+ Automatic calculation can be managed by `alwaysCalculate`
			+ `nextEdge` fix
		+ `Polygon.OffsetPolygon()` added
		+ `08 Polygon offset` test scene
	+ `EPPZGeometry_PolygonDebugRenderer`
		+ `Polygon` model can be sourced from outside

* 0.1.3

	+ Area / Winding Direction calculations
		+ Calculate signed area first, assign winding direction, then area
		+ Calculation results are read-only properties
			+ `bounds`
			+ `signedArea`										
			+ `area`		
			+ `windingDirection`
		+ Corresponding test scene added

* 0.1.2

	+ Test scene / documentation improvements
	+ Debug renderes now works in Scene View as well
	+ Polygon winding direction can be `Unknown` by default

* 0.1.1

	+ `Segment.IsPointLeft()` added with testbed

* 0.1.0

	+ `Segment` / `Edge` relationship
		+ Segment.a / Segment.b accessors made virtual
		+ Subclasses can override so `Edge` now sources `Polygon` points directly 
	+ Test scenes comply

* 0.0.9

	+ `Edge` / `Vertex` classes
		+ Seemingly `Edge` instances can't reach `Vector2` sources this way
		+ Bounding / inheritance should clean up

* 0.0.8

	+ Pass `EPPZPolygon_2` test scene

* 0.0.7 - 0.0.8

	+ Integrate permiter containment test into `Polygon`

* 0.0.5 - 0.0.6

	+ Segment point containment test
	+ Bounds Containment (with equality) is fixed
	+ Bounds Rect is converted to world coordinate system (Y-up)
	+ `Segment.IsSegmentIntersecting` uses `Rect.Overlaps` test for optimization (again)

* 0.0.4

	+ More testbed scenes with selectable test algorithms

* 0.0.3

	+ `EPPZPolygon` Test scene
		+ Test `Segment.IsIntersectingWithSegment`
		+ Test `PolygonIsIntersectingWithSegment`
	+ Fixed bounds overlap test
	+ Debug renderers
		+ Draw bounds as well
		+ Works with identy (zero) source transforms (!)

* 0.0.2

	+ `EPPZ_PolygonSource` / `EPPZ_SegmentSource`
		+ sources now carry a model instance
		+ can be updated runtime
	+ Test scenes
		+ `EPPZMesh`
		+ `EPPZPolygon`

* 0.0.1

	+ Naming
	+ Extracted `EPPZ_DebugRenderer`
		+ Coupled with `EPPZ_DebugCamera
	+ `EPPZGeometry`		
		+ Moved `Vector2` / `Vector3` extensions into 
		+ Still contains `Polygon` / `Segment` / `Point`
		+ Moved polygon source into
	+ `EPPZMesh` with ring and circle 2D