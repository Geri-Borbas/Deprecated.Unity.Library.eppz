Unity.EPPZKit
-------------

Unity.EPPZGeometry
==================

2D geometry for everyday polygon hassle. Made for tangram! so production proof (yet not complete).

Core features

+ .. Segment-segment intersection
+ .. Segment-point intersection
+ 00 Polygon-point containment
+ 01 Polygon-segment intersection test
+ 02 Polygon permiter-point containment (precise)
+ 03 Polygon permiter-point containment (default)
+ 04 Polygon-segment containment
+ 05 Polygon-polygon containment
+ 06 Vertex normal facing	
+ 07 Polygon area, winding direction
+ 08 Segment-segment intersection point
+ 09 Polygon offset

+ Polygon outline / Polygon offset / polygon buffer (however you name it)
+ Polygon triangulation (actually using a subset of Triangle.NET for now)
+ Polygon area / bounds calculations

Prefixed files, namespaced classes, no collisions with other classes. Some core geometry features extracted to a static class so can easily fit into other point / vertex / segment/ edge / polygon implementations if needed.

All works with closed convex / concave even self-intersecting polygons. There is no such thing as trigonometric function call at all in this library. Only `+` and `*` (and some `Mathf.Sqrt` for distances sometimes). 


Test scenes
-----------

Test scenes are designed to experience / proof the features. Hit play, then manipulate the geometry in Scene window while in game mode (strict to XY plane, and move the points directly, not their parent container). Every relevant code is in `<Test_scene>_Controller.cs`, so you can see **how to use the API**.

You can define a polygon with simple `Vector2[]` array, but for sake of simplicity, test scenes uses some **polygon sourcing helper classes** those take simple GameObject transforms as input. They also keep the polygon models updated on `GameObject` changes.

Another helper objects are **polygon debug renderers**. They draw simple lines in Game View (using `GL_LINES`) after main camera finished rendering, also draws debug lines in scene view (using `Debug.DrawLine`).

Beside these helper classes, you can easily construct `Polygon` / `Segment` instances using simple `Vector2` inputs as well.

Having these test scenes, you can easily provision the mechanism of each EPPZGeometry feature even with your own polygons.


00 Polygon-point containment
----------------------------

The star polygon drawn yellow when it contains all three points.

+ When points appear to be on a polygon edge, test will return false
+ When point is at a polygon vertex, test will return false

Usage:
```C#
bool test = polygon.ContainsPoint(point);
```
See `TestScene_00_Controller.cs` for the full context.


01 Polygon-segment intersection
-------------------------------

The star polygon drawn yellow when any of the two segments intersects any polygon edge.

+ Returns false when a segment endpoint appears to be on a polygon edge
+ Returns false when a segment endpoint is at a polygon vertex

Usage:
```C#
bool test = polygon.IsIntersectingWithSegment(segment);
```
See `TestScene_01_Controller.cs` for the full context.



02 Polygon permiter-point containment 1
---------------------------------------

The star polygon drawn yellow when the point is contained by the polygon permiter. Accuracy means the line width of the polygon permiter (is `1.0f` by default).

+ Returns true even if the point appears to be on a polygon edge
+ Returns true even if the point is at a polygon vertex

Usage:
```C#
bool test = polygon.PermiterContainsPoint(point, accuracy, Segment.ContainmentMethod.Precise);
```
See `TestScene_02_Controller.cs` for the full context.

TODO: Explaination of Segment.ContainmentMethod.


03 Polygon permiter-point containment 2
---------------------------------------

Actually the same as before, but a smaller accuracy is given (`0.1f`). The star polygon drawn yellow when the point appears to be on any polygon edge of at a polygon vertex.

+ Returns true even if the point appears to be on a polygon edge
+ Returns true even if the point is at a polygon vertex

Usage:
```C#
float accuracy = 0.1f;	
bool test = polygon.PermiterContainsPoint(point, accuracy);
```
See `TestScene_03_Controller.cs` for the full context.


04 Polygon-segment containment (TODO: unfinished test scene)
------------------------------

The star drawn yellow when it contains both edge. This is a compund test of polygon-point containment, polygon permiter-point containment, polygon-segment intersection.

+ Returns true even if the point appears to be on a polygon edge (thanks to permiter test)
+ Returns true even if the point is at a polygon vertex (thanks to permiter test)

See `TestScene_04_Controller.cs` for the full context.


05 Polygon-polygon containment
------------------------------

The star drawn yellow when it contain the rectangular polygon.  This is also a compund test of polygon-point containment, polygon-segment intersection, polygon permiter-point containment.

A polygon contains a foreign polygon, when
+ foreign polygon vertices are contained by polygon
+ foreign polygon segments are not intersecting with polygon
+ foreign polygon vertices are not contained by polygon permiter

See `TestScene_05_Controller.cs` for the full context.

Among some other orientation normalizer stuff, this is the core of tangram! puzzle solving engine, so it is proven by millions of gameplay hours.


06 Vertex normal facing	(TODO: Rename to something like "On which side of a segment a point resides")
-----------------------

I made `Vertex` / `Edge : Segment` classes to add some contextual features to points and segments when participating in a polygon. Further polygon calculations make a good use of these contextual stuff, like `normal`, `bisector`, `previousPoint` / `previousEdge` and such.

The segments here are imaginary polygon edges with CW winding direction. The normal segment drawn white when it faces inward the polygon, drawn light green when facing outward.

It uses a nice little `Segment` method `public bool IsPointLeft(Vector2 point)` to create a compund test with both segments. It goes like below.

The two segments encompasses an acute angle if
+ the endpoint of the second segment lies on the left of the first segment

The normal facing outward if
+ the neighbouring segment encompasses an acute angle
	+ and the normal point lies on the left of both segments
+ or the neighbouring segments encompass an obtuse angle
	+ and the normal point lies on the left of one of the segments

Implemented like:
```C#
bool NormalFacingTest()
{
	Vector2 point = normal.b;
	bool acute = segmentA.IsPointLeft(segmentB.b);
	bool leftA = segmentA.IsPointLeft(point);
	bool leftB = segmentB.IsPointLeft(point);
	bool outward = (acute) ? leftA && leftB : leftA || leftB;
	bool inward = !outward;
	return inward;
}	
```

See `TestScene_06_Controller.cs` for the full context.


07 Polygon area, winding direction
----------------------------------

The winding direction of a polygon comes to a good use when you want to validate the result of further polygon operations. The winding direction of each `Polygon` instance gets calculated at construction time, and each time it's `CalculateArea()` method gets called (actually every frame in these example scenes).

Here basically you can see how area and winding direction of a polygon gets calculated. Just hit play and nudge some points around.

Usage:

```C#
// After a polygon constructed, you can simply access values.
float area = polygon.area;
bool isCW = polygon.isCW;

// If polygon topology changed, invoke calculations directly.
polygon.CalculateArea();

// Same goes for bounds.
polygon.CalculateBounds();	

// Or you can use a shortcut for both.
polygon.UpdateCalculations();
```

See `EPPZGeometry_Polygon.cs` source for the details.


Extras
------

	Extensions, like `Vector2.xy()`
	Some `Mesh2D` component (circle and ring)
	Debug renderes as a lovely debug tool