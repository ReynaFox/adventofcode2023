using System.Text.RegularExpressions;

part1();
part2();

static void part1() {
	var (bricks, minArea, maxArea) = read("input.txt");
	computeSupport(bricks, maxArea);

	var count = 0;
	foreach (var brick in bricks) {
		var safeToClear = brick.supports.All(x => x.supportedBy.Length > 1);
		if (safeToClear) count++;
	}
	Console.WriteLine(count);
}

static void part2() {
	var (bricks, minArea, maxArea) = read("input.txt");
	computeSupport(bricks, maxArea);

	var sum = 0;
	for (int i = bricks.Count-1; i>=0; i--) {
		var brick = bricks[i];

		var safeToClear = brick.supports.All(x => x.supportedBy.Length > 1);
		if (!safeToClear) {
			var falling = new HashSet<Brick> { brick };

			var processList = new List<Brick>();
			processList.AddRange(brick.supports);
			while (processList.Count > 0) {
				var b = processList[0];
				processList.RemoveAt(0);
				if (b.supportedBy.All(falling.Contains)) {
					falling.Add(b);
					processList.AddRange(b.supports);
				}
			}

			sum += falling.Count-1;
		}
	}
	Console.WriteLine(sum);
}

static (List<Brick> bricks, Vec3 minArea, Vec3 maxArea) read(string file) {
	var minArea = new Vec3(int.MaxValue, int.MaxValue, int.MaxValue);
	var maxArea = new Vec3(0, 0, 0);
	var bricks = new List<Brick>();

	var regex = new Regex(@"(\d+),(\d+),(\d+)~(\d+),(\d+),(\d+)");
	foreach (var line in File.ReadLines(file)) {
		if (string.IsNullOrEmpty(line)) continue;
		var match = regex.Match(line);

		var brick = new Brick(
			new Vec3(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value)),
			new Vec3(int.Parse(match.Groups[4].Value), int.Parse(match.Groups[5].Value), int.Parse(match.Groups[6].Value))
		);
		bricks.Add(brick);

		minArea = Vec3.min(Vec3.min(minArea, brick.a), brick.b);
		maxArea = Vec3.max(Vec3.max(maxArea, brick.a), brick.b);
	}
	return (bricks, minArea, maxArea);
}

static void computeSupport(List<Brick> bricks, Vec3 maxArea) {
	bricks.Sort((a, b) => Math.Min(a.a.z, a.b.z).CompareTo(Math.Min(b.a.z, b.b.z)));
	var lowest = new (int z, Brick brick)[maxArea.x+1, maxArea.y+1];
	foreach (var brick in bricks) {
		// Drop a brick
		var min = Vec3.min(brick.a, brick.b);
		var max = Vec3.max(brick.a, brick.b);
		var highest = 0;
		for (int y = min.y; y <= max.y; y++)
		for (int x = min.x; x <= max.x; x++) {
			highest = Math.Max(highest, lowest[x, y].z);
		}

		// Place the brick one higher.
		var lowZ = highest+1;
		var highZ = highest+1+max.z-min.z;

		var supports = new HashSet<Brick>();
		for (int y = min.y; y <= max.y; y++)
		for (int x = min.x; x <= max.x; x++) {
			if (lowest[x, y].z == lowZ-1) {
				supports.Add(lowest[x, y].brick);
				lowest[x, y].brick?.supports.Add(brick);
			}
			lowest[x, y] = (highZ, brick);
			brick.supportedBy = supports.ToArray();
		}
	}
}

class Vec3(int x, int y, int z) {
	public int x = x;
	public int y = y;
	public int z = z;

	public static Vec3 operator +(Vec3 a, Vec3 b) {
		return new Vec3(a.x+b.x, a.y+b.y, a.z+b.z);
	}

	public static Vec3 min(Vec3 a, Vec3 b) {
		return new Vec3(
			Math.Min(a.x, b.x),
			Math.Min(a.y, b.y),
			Math.Min(a.z, b.z)
		);
	}

	public static Vec3 max(Vec3 a, Vec3 b) {
		return new Vec3(
			Math.Max(a.x, b.x),
			Math.Max(a.y, b.y),
			Math.Max(a.z, b.z)
		);
	}

	public override string ToString() {
		return $"Vec3({x}, {y}, {z})";
	}
}

class Brick(Vec3 a, Vec3 b) {
	public Vec3 a = a;
	public Vec3 b = b;

	public Brick[] supportedBy;
	public readonly HashSet<Brick> supports = [];
}