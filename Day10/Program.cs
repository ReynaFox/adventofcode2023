using System.Diagnostics;
using System.Drawing;
using System.Linq;

//part1();
part2();


static void part1() {
	var map = new List<Map.Tile[]>();

	var startPos = new Point(0, 0);
	var y = 0;
	foreach (var line in File.ReadLines("input.txt")) {
		if (string.IsNullOrEmpty(line)) continue;

		var mapLine = new Map.Tile[line.Length];
		for (int i = 0; i < line.Length; i++) {
			mapLine[i] = line[i] switch {
				'.' => Map.Tile.empty,
				'S' => Map.Tile.start,
				'|' => Map.Tile.ns,
				'-' => Map.Tile.ew,
				'L' => Map.Tile.ne,
				'J' => Map.Tile.nw,
				'7' => Map.Tile.sw,
				'F' => Map.Tile.se,
			};
			if (mapLine[i] == Map.Tile.start)
				startPos = new Point(i, y);
		}
		map.Add(mapLine);
		y++;
	}

	foreach (var dir in Enum.GetValues(typeof(Map.Direction)).Cast<Map.Direction>()) {
		var move = Map.directions[dir];
		var path = new List<Point> { startPos };
		tracePath(map, new Point(startPos.X+move.X, startPos.Y+move.Y), dir, path);
		var end = path[^1];
		if (map[end.Y][end.X] == Map.Tile.start) {
			printMap(map);
			printMap(map, path);

			Console.WriteLine((path.Count-1)/2);
			break;
		}
	}
}

static void part2() {
	var map = new List<Map.Tile[]>();

	var startPos = new Point(0, 0);
	var y = 0;
	// Make the map 1 bigger on all sides when reading.
	foreach (var line in File.ReadLines("input.txt")) {
		if (string.IsNullOrEmpty(line)) continue;

		var mapLine = new Map.Tile[line.Length+2];
		for (int i = 0; i < line.Length; i++) {
			mapLine[i+1] = line[i] switch {
				'.' => Map.Tile.empty,
				'S' => Map.Tile.start,
				'|' => Map.Tile.ns,
				'-' => Map.Tile.ew,
				'L' => Map.Tile.ne,
				'J' => Map.Tile.nw,
				'7' => Map.Tile.sw,
				'F' => Map.Tile.se,
			};
			if (mapLine[i+1] == Map.Tile.start)
				startPos = new Point(i+1, y+1);
		}
		map.Add(mapLine);
		y++;
	}
	map.Add(new Map.Tile[map[0].Length]);
	map.Insert(0, new Map.Tile[map[0].Length]);

	var path = new List<Point>();
	foreach (var dir in Enum.GetValues(typeof(Map.Direction)).Cast<Map.Direction>()) {
		var move = Map.directions[dir];
		path.Clear();
		path.Add(startPos);
		var endDir = tracePath(map, new Point(startPos.X+move.X, startPos.Y+move.Y), dir, path);
		var end = path[^1];
		if (map[end.Y][end.X] == Map.Tile.start) {
			// Overwrite the start tile with the fitting piece
			map[end.Y][end.X] = (dir, endDir) switch {
				(Map.Direction.south, Map.Direction.south) or
					(Map.Direction.north, Map.Direction.north) => Map.Tile.ns,

				(Map.Direction.east, Map.Direction.east) or
					(Map.Direction.west, Map.Direction.west) => Map.Tile.ew,

				(Map.Direction.east, Map.Direction.north) or
					(Map.Direction.south, Map.Direction.west) => Map.Tile.se,
				(Map.Direction.south, Map.Direction.east) or
					(Map.Direction.west, Map.Direction.north) => Map.Tile.sw,

				(Map.Direction.north, Map.Direction.east) or
					(Map.Direction.west, Map.Direction.south) => Map.Tile.nw,
				(Map.Direction.north, Map.Direction.west) or
					(Map.Direction.east, Map.Direction.south) => Map.Tile.ne,
			};
			printMap(map);
			printMap(map, path);
			break;
		}
	}

	const int empty = 0;
	const int pipe = 1;
	const int outside = 2;
	const int inside = 3;

	var inout = new int[map[0].Length, map.Count];
	// Write pipe locations
	foreach (var p in path) {
		inout[p.X, p.Y] = pipe;
	}
	floodfill(0, 0, outside);

	// Map that indicates where tiles to the side of the pipe are. Every side also has an indication
	// of how it's related to the pipe itself (roughly analogous to "left" and "right" of the pipe, but
	// directionless: side 0 and side 1). All curved pipes have side 0 as outside the curve.
	var pipeSideMap = new Dictionary<Map.Tile, (Map.Direction d, int side)[]> {
		{ Map.Tile.ns, new[] { (Map.Direction.east, 1), (Map.Direction.west, 0) } },
		{ Map.Tile.ew, new[] { (Map.Direction.north, 0), (Map.Direction.south, 1) } },
		{ Map.Tile.se, new[] { (Map.Direction.north, 0), (Map.Direction.west, 0) } },
		{ Map.Tile.sw, new[] { (Map.Direction.north, 0), (Map.Direction.east, 0) } },
		{ Map.Tile.nw, new[] { (Map.Direction.south, 0), (Map.Direction.east, 0) } },
		{ Map.Tile.ne, new[] { (Map.Direction.south, 0), (Map.Direction.west, 0) } }
	};

	// When going from one pipe tile to another, this indicates if sides should swap. If not in here,
	// sides don't swap.
	var sidesSwap = new HashSet<(Map.Tile, Map.Tile)> {
		(Map.Tile.ns, Map.Tile.sw),
		(Map.Tile.ns, Map.Tile.nw),

		(Map.Tile.ew, Map.Tile.nw),
		(Map.Tile.ew, Map.Tile.ne),

		(Map.Tile.se, Map.Tile.nw),
		(Map.Tile.sw, Map.Tile.ne),
	};

	// Find a pipe segment that touches the outside
	int startIdx = 0;
	var sides = new[] { 0, 0 };
	for (int i = 0; i < path.Count; i++) {
		var p = path[i];
		foreach (var side in pipeSideMap[map[p.Y][p.X]]) {
			var d = Map.directions[side.d];
			if (inout[p.X+d.X, p.Y+d.Y] == outside) {
				startIdx = i;
				sides[side.side] = outside;
				sides[1-side.side] = inside;
				break;
			}
		}
	}

	// Trace the path, floodfill inside/outside as necessary
	var j = startIdx;
	do {
		var p = path[j];
		var tile = map[p.Y][p.X];
		foreach (var side in pipeSideMap[tile]) {
			var d = Map.directions[side.d];
			if (inout[p.X+d.X, p.Y+d.Y] == empty) {
				floodfill(p.X+d.X, p.Y+d.Y, sides[side.side]);
			}
		}
		j = (j+1)%path.Count;
		var nextP = path[j];
		var nextTile = map[nextP.Y][nextP.X];
		if (sidesSwap.Contains((tile, nextTile)) || sidesSwap.Contains((nextTile, tile))) {
			(sides[0], sides[1]) = (sides[1], sides[0]);
		}
	} while (j != startIdx);

	printMap(map, null, inout);

	var count = 0;
	for (int yy = 0; yy<inout.GetLength(1); yy++)
	for (int xx = 0; xx<inout.GetLength(0); xx++) {
		if (inout[xx, yy] == inside) count++;
	}

	Console.WriteLine(count);

	void floodfill(int startX, int startY, int value) {
		var queue = new Queue<Point>();
		queue.Enqueue(new Point(startX, startY));

		while (queue.Count > 0) {
			var p = queue.Dequeue();
			if (inout[p.X, p.Y] != empty) continue;
			inout[p.X, p.Y] = value;

			if (p.X > 0 && inout[p.X-1, p.Y] == empty) queue.Enqueue(new Point(p.X-1, p.Y));
			if (p.Y > 0 && inout[p.X, p.Y-1] == empty) queue.Enqueue(new Point(p.X, p.Y-1));
			if (p.X < inout.GetLength(0)-1 && inout[p.X+1, p.Y] == empty) queue.Enqueue(new Point(p.X+1, p.Y));
			if (p.Y < inout.GetLength(1)-1 && inout[p.X, p.Y+1] == empty) queue.Enqueue(new Point(p.X, p.Y+1));
		}
	}
}

static Map.Direction tracePath(List<Map.Tile[]> map, Point start, Map.Direction oldTo, List<Point> path) {
	do {
		if (start.Y < 0 || start.X < 0 || start.Y >= map.Count || start.X >= map[0].Length) break;
		path.Add(start);
		var tile = map[start.Y][start.X];
		if (tile == Map.Tile.start) break;

		if (!Map.movements.TryGetValue((tile, oldTo), out var to)) break;

		var dir = Map.directions[to];
		start = new Point(start.X+dir.X, start.Y+dir.Y);
		oldTo = to;
	} while (true);
	return oldTo;
}

static void printMap(List<Map.Tile[]> map, List<Point> path = null, int[,] inout = null) {
	var tileMap = new Dictionary<Map.Tile, char> {
		{ Map.Tile.empty, '·' },
		{ Map.Tile.start, 'S' },
		{ Map.Tile.ns, '│' },
		{ Map.Tile.ew, '─' },
		{ Map.Tile.se, '┌' },
		{ Map.Tile.sw, '┐' },
		{ Map.Tile.nw, '┘' },
		{ Map.Tile.ne, '└' },
	};
	if (inout != null) {
		for (int y = 0; y < inout.GetLength(1); y++) {
			var s = "";
			for (int x = 0; x < inout.GetLength(0); x++) {
				s += inout[x, y] switch {
					0 => ' ',
					1 => tileMap[map[y][x]],
					2 => 'O',
					3 => 'I',
				};
			}
			Console.WriteLine(s);
		}
		return;
	}
	if (path!=null) {
		var map2 = new List<Map.Tile[]>();
		foreach (var line in map) {
			var line2 = new Map.Tile[line.Length];
			map2.Add(line2);
		}

		foreach (var point in path) {
			map2[point.Y][point.X] = map[point.Y][point.X];
		}
		map = map2;
	}
	foreach (var line in map) {
		var s = "";
		foreach (var tile in line) {
			s += tileMap[tile];
		}
		Console.WriteLine(s);
	}
}

class Map {
	public enum Tile {
		empty,
		start,
		ns,
		ew,
		se,
		sw,
		nw,
		ne
	}

	public enum Direction {
		north,
		east,
		south,
		west
	}

	public static Dictionary<Direction, Point> directions = new() {
		{ Direction.north, new Point( 0, -1) },
		{ Direction.south, new Point( 0,  1) },
		{ Direction.east,  new Point( 1,  0) },
		{ Direction.west,  new Point(-1,  0) },
	};

	public static Dictionary<(Tile, Direction), Direction> movements = new() {
		{ (Tile.ns, Direction.south), Direction.south },
		{ (Tile.ns, Direction.north), Direction.north },

		{ (Tile.ew, Direction.west),  Direction.west },
		{ (Tile.ew, Direction.east),  Direction.east },

		{ (Tile.se, Direction.north), Direction.east },
		{ (Tile.se, Direction.west),  Direction.south },

		{ (Tile.sw, Direction.north), Direction.west },
		{ (Tile.sw, Direction.east),  Direction.south },

		{ (Tile.ne, Direction.south), Direction.east },
		{ (Tile.ne, Direction.west),  Direction.north },

		{ (Tile.nw, Direction.south), Direction.west },
		{ (Tile.nw, Direction.east),  Direction.north },
	};
}