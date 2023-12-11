using System.Drawing;

part1();
part2();


static void part1() {
	var coords = new List<Point>();
	var usedRows = new List<bool>();
	var usedCols = new List<bool>();
	var yy = 0;
	foreach (var line in File.ReadLines("input.txt")) {
		if (string.IsNullOrEmpty(line)) continue;

		for (int x = 0; x < line.Length; x++) {
			if (line[x] != '#') continue;

			coords.Add(new Point(x, yy));
			while (usedCols.Count <= x+1) usedCols.Add(false);
			usedCols[x] = true;
			while (usedRows.Count <= yy+1) usedRows.Add(false);
			usedRows[yy] = true;
		}
		yy++;
	}

	// Expand
	for (int i = 0; i < coords.Count; i++) {
		var c = coords[i];
		for (int x = c.X; x >= 0; x--) {
			if (!usedCols[x]) c.X++;
		}
		for (int y = c.Y; y >= 0; y--) {
			if (!usedRows[y]) c.Y++;
		}
		coords[i] = c;
	}

	var sum = 0;
	for (int i = 1; i < coords.Count; i++)
	for (int j = 0; j < i; j++) {
		var dist = Math.Abs(coords[i].X-coords[j].X) + Math.Abs(coords[i].Y-coords[j].Y);
		sum += dist;
	}
	Console.WriteLine(sum);
}

static void part2() {
	var coords = new List<LongPoint>();
	var usedRows = new List<bool>();
	var usedCols = new List<bool>();
	var yy = 0;
	foreach (var line in File.ReadLines("input.txt")) {
		if (string.IsNullOrEmpty(line)) continue;

		for (int x = 0; x < line.Length; x++) {
			if (line[x] != '#') continue;

			coords.Add(new LongPoint(x, yy));
			while (usedCols.Count <= x+1) usedCols.Add(false);
			usedCols[x] = true;
			while (usedRows.Count <= yy+1) usedRows.Add(false);
			usedRows[yy] = true;
		}
		yy++;
	}

	// Expand
	const long scale = 999_999; // Since we're adding, this is the multiply factor minus one
	for (int i = 0; i < coords.Count; i++) {
		var c = coords[i];
		for (int x = (int)c.X; x >= 0; x--) {
			if (!usedCols[x]) c.X+= scale;
		}
		for (int y = (int)c.Y; y >= 0; y--) {
			if (!usedRows[y]) c.Y+= scale;
		}
		coords[i] = c;
	}

	var sum = 0L;
	for (int i = 1; i < coords.Count; i++)
	for (int j = 0; j < i; j++) {
		var dist = Math.Abs(coords[i].X-coords[j].X) + Math.Abs(coords[i].Y-coords[j].Y);
		sum += dist;
	}
	Console.WriteLine(sum);
}

struct LongPoint (long X, long Y) {
	public long X = X;
	public long Y = Y;
}