part1();
part2();

static void part1() {
	var lines = new List<string>();
	foreach (var line in File.ReadLines("input.txt")) {
		if (string.IsNullOrEmpty(line)) continue;
		lines.Add(line);
	}


	var grid = new EnergyGrid(lines);
	var result = grid.traceEnergized(new Vec2(0, 0), EnergyGrid.right);

	Console.WriteLine(result);
}

static void part2() {
	var lines = new List<string>();
	foreach (var line in File.ReadLines("input.txt")) {
		if (string.IsNullOrEmpty(line)) continue;
		lines.Add(line);
	}

	var grid = new EnergyGrid(lines);
	var best = 0;
	for (var x = 0; x < lines[0].Length; x++) {
		best = Math.Max(best, grid.traceEnergized(new Vec2(x, 0), EnergyGrid.down));
		best = Math.Max(best, grid.traceEnergized(new Vec2(x, lines.Count-1), EnergyGrid.up));
	}
	for (var y = 0; y < lines.Count; y++) {
		best = Math.Max(best, grid.traceEnergized(new Vec2(0, y), EnergyGrid.right));
		best = Math.Max(best, grid.traceEnergized(new Vec2(lines[0].Length-1, y), EnergyGrid.left));
	}

	Console.WriteLine(best);
}

class Vec2(int x, int y) {
	public int x = x;
	public int y = y;

	public static Vec2 operator +(Vec2 a, Vec2 b) {
		return new Vec2(a.x+b.x, a.y+b.y);
	}
}

class EnergyGrid {
	private const int empty = 0;
	private const int mirrorBack = 1;
	private const int mirrorFwd = 2;
	private const int splitVert = 3;
	private const int splitHor = 4;

	public const int up = 0;
	public const int left = 1;
	public const int down = 2;
	public const int right = 3;

	private readonly Vec2[] directions = [new Vec2(0, -1), new Vec2(-1, 0), new Vec2(0, 1), new Vec2(1, 0)];

	private int[,] grid;
	private bool[,] energized;
	private bool[,][] reachedFrom;
	private Stack<(Vec2 p, int dir)> beamStack = new();
	
	public EnergyGrid(List<string> lines) {
		grid = new int[lines[0].Length, lines.Count];
		for (int y = 0; y < lines.Count; y++) {
			var line = lines[y];
			for (int x = 0; x < line.Length; x++) {
				grid[x, y] = line[x] switch {
					'.' => empty,
					'\\' => mirrorBack,
					'/' => mirrorFwd,
					'|' => splitVert,
					'-' => splitHor,
				};
			}
		}
		energized = new bool[grid.GetLength(0), grid.GetLength(1)];
		reachedFrom = new bool[grid.GetLength(0), grid.GetLength(1)][];
	}

	public int traceEnergized(Vec2 p, int dir) {
		energized = new bool[grid.GetLength(0), grid.GetLength(1)];
		reachedFrom = new bool[grid.GetLength(0), grid.GetLength(1)][];
		for (int y = 0; y < grid.GetLength(1); y++)
		for (int x = 0; x < grid.GetLength(0); x++) {
			reachedFrom[x, y] = new bool[4];
		}

		beamStack.Clear();
		beamStack.Push((p, dir));

		while (beamStack.Count > 0) {
			var item = beamStack.Pop();
			traceBeam(item.p, item.dir);
		}

		var count = 0;
		for (int y = 0; y < grid.GetLength(1); y++)
		for (int x = 0; x < grid.GetLength(0); x++) {
			if (energized[x, y]) count++;
		}
		return count;
	}

	void traceBeam(Vec2 p, int dir) {
		if (p.x<0 || p.y<0 || p.x>=grid.GetLength(0) || p.y>=grid.GetLength(1)) return;
		do {
			if (reachedFrom[p.x, p.y][dir]) return;
			reachedFrom[p.x, p.y][dir] = true;
			energized[p.x, p.y] = true;
			switch (grid[p.x, p.y]) {
			//case empty: break;
			case mirrorBack:
				dir = dir switch {
					up => left,
					left => up,
					down => right,
					right => down
				};
				break;
			case mirrorFwd:
				dir = dir switch {
					up => right,
					right => up,
					left => down,
					down => left
				};
				break;
			case splitHor:
				if (dir == up || dir == down) {
					dir = left;
					beamStack.Push((p+directions[right], right));
				}
				break;
			case splitVert:
				if (dir == left || dir == right) {
					dir = up;
					beamStack.Push((p+directions[down], down));
				}
				break;
			}
			p += directions[dir];
		} while (p.x>=0 && p.y>=0 && p.x<grid.GetLength(0) && p.y<grid.GetLength(1));
	}
}