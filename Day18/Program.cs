using System.Globalization;

part1();
part2();

static void part1() {
	var actions = new List<(Direction direction, int steps)>();
	var p = new Vec2(0, 0);
	var min = new Vec2(0, 0);
	var max = new Vec2(0, 0);

	var directions = new Dictionary<Direction, Vec2> {
		{ Direction.up, new Vec2( 0, -1) },
		{ Direction.down, new Vec2( 0,  1) },
		{ Direction.right,  new Vec2( 1,  0) },
		{ Direction.left,  new Vec2(-1,  0) },
	};

	foreach (var line in File.ReadLines("example.txt")) {
		var dir = line[0] switch {
			'U' => Direction.up,
			'R' => Direction.right,
			'D' => Direction.down,
			'L' => Direction.left,
		};
		var steps = int.Parse(line[2..line.IndexOf('(')]);
		actions.Add((dir, steps));

		// Track the path already to compute the bounding box
		p += directions[dir]*steps;
		min.x = Math.Min(min.x, p.x);
		min.y = Math.Min(min.y, p.y);
		max.x = Math.Max(max.x, p.x);
		max.y = Math.Max(max.y, p.y);
	}

	var map = new int[max.x-min.x+1, max.y-min.y+2]; // We make the map one row taller for simplification later on
	p = new Vec2(-min.x, -min.y);
	map[p.x, p.y] = 1;

	// Trace path again, paint in the path
	foreach (var (dir, steps)in actions) {
		var d = directions[dir];
		for (int i = 0; i < steps; i++) {
			p += d;
			map[p.x, p.y] = 1;
		}
	}
	/*
	for (int y = 0; y < map.GetLength(1); y++) {
		var s = "";
		for (int x = 0; x < map.GetLength(0); x++) {
			s += map[x, y] == 0 ? '.' : '#';
		}
		Console.WriteLine(s);
	}
	*/

	// In-out algorithm seems to be safe for the input, there are no crossing sections in the
	// boundary.
	var count = 0;
	for (int y = 0; y < map.GetLength(1); y++) {
		var inside = false;
		for (int x = 0; x < map.GetLength(0); x++) {
			var tile = map[x, y];
			if (tile != 0) {
				count++;
				// Flip inside/outside on corners │, ┌, ┐
				if (map[x, y+1] != 0) inside = !inside;
			} else if (inside) {
				count++;
			}
		}
	}

	Console.WriteLine(count);
}

static void part2() {

	var directions = new Dictionary<Direction, Vec2> {
		{ Direction.up, new Vec2( 0, -1) },
		{ Direction.down, new Vec2( 0,  1) },
		{ Direction.right,  new Vec2( 1,  0) },
		{ Direction.left,  new Vec2(-1,  0) },
	};

	var points = new List<Vec2>();
	var p = new Vec2(0, 0);
	var boundaryLength = 0L;
	foreach (var line in File.ReadLines("input.txt")) {
		var color = line[(line.IndexOf('(')+2)..line.IndexOf(')')];
		var steps = int.Parse(color[..5], NumberStyles.HexNumber);
		var dir = color[5] switch {
			'0' => Direction.right,
			'1' => Direction.down,
			'2' => Direction.left,
			'3' => Direction.up,
		};

		boundaryLength += steps;
		p += directions[dir]*steps;
		points.Add(p);
	}

	// Compute polygon area
	var sum = 0L;
	var cornersCw = 0;
	var cornersCcw = 0;
	var windingSum = 0L;
	for (int i = 0; i < points.Count; i++) {
		var p0 = points[(i+points.Count-1)%points.Count];
		var p1 = points[i];
		var p2 = points[(i+1)%points.Count];

		var turnsCCW = isLeft(p0, p1, p2);
		if (turnsCCW) cornersCcw++;
		else cornersCw++;

		windingSum += (p2.x-p1.x)*(p2.y+p1.y);

		var val = p1.x*p2.y - p2.x*p1.y;
		sum += val;
	}

	var windsClockwise = windingSum < 0;
	if (!windsClockwise) (cornersCcw, cornersCw) = (cornersCw, cornersCcw);
	var correction = (boundaryLength+(cornersCw-cornersCcw)/2);

	Console.WriteLine($"Winding: {windingSum} Boundary: {boundaryLength} CW {cornersCw} CCW {cornersCcw}");
	Console.WriteLine($"Correction {boundaryLength+cornersCw-cornersCcw}");
	Console.WriteLine((sum+correction)/2);

	bool isLeft(Vec2 l1, Vec2 l2, Vec2 p) {
		return (l2.x-l1.x)*(p.y-l1.y) - (l2.y-l1.y)*(p.x-l1.x) < 0;
	}
}

enum Direction {
	up,
	right,
	down,
	left
}

class Vec2(long x, long y) {
	public long x = x;
	public long y = y;

	public static Vec2 operator +(Vec2 a, Vec2 b) {
		return new Vec2(a.x+b.x, a.y+b.y);
	}

	public static Vec2 operator *(Vec2 a, long b) {
		return new Vec2(a.x*b, a.y*b);
	}
}