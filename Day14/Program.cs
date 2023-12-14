part1();
part2();


static void part1() {
	var lines = new List<string>();
	foreach (var line in File.ReadLines("input.txt")) {
		if (string.IsNullOrEmpty(line)) continue;
		lines.Add(line);
	}

	const int empty = 0;
	const int unmoving = 1;
	const int rolling = 2;

	var grid = new int[lines[0].Length, lines.Count];
	for (int y = 0; y < lines.Count; y++) {
		var line = lines[y];
		for (int x = 0; x < line.Length; x++) {
			grid[x, y] = line[x] switch {
				'.' => empty,
				'#' => unmoving,
				'O' => rolling
			};
		}
	}

	var topmostAvailable = new int[grid.GetLength(0)];
	for (int y = 0; y < grid.GetLength(1); y++) {
		for (int x = 0; x < grid.GetLength(0); x++) {
			switch (grid[x, y]) {
			case unmoving:
				topmostAvailable[x] = y+1;
				break;
			case rolling:
				grid[x, y] = empty;
				grid[x, topmostAvailable[x]] = rolling;
				topmostAvailable[x]++;
				break;
			}
		}
	}

	var sum = 0;
	for (int y = 0; y < grid.GetLength(1); y++) {
		var count = 0;
		var s = "";
		for (int x = 0; x < grid.GetLength(0); x++) {
			s += grid[x, y] switch {
				empty => '.',
				unmoving => '#',
				rolling => 'O',
			};
			if (grid[x, y]==rolling) count++;
		}
		sum += count*(grid.GetLength(1)-y);
		//Console.WriteLine(s);
	}
	Console.WriteLine(sum);
}

static void part2() {
	var lines = new List<string>();
	foreach (var line in File.ReadLines("input.txt")) {
		if (string.IsNullOrEmpty(line)) continue;
		lines.Add(line);
	}

	const int empty = 0;
	const int unmoving = 1;
	const int rolling = 2;

	var grid = new int[lines[0].Length, lines.Count];
	for (int y = 0; y < lines.Count; y++) {
		var line = lines[y];
		for (int x = 0; x < line.Length; x++) {
			grid[x, y] = line[x] switch {
				'.' => empty,
				'#' => unmoving,
				'O' => rolling
			};
		}
	}

	int runs = 0;
	int diffs = 0;
	int lastDiffs = int.MaxValue;
	int lastCycleDiffs = int.MaxValue;
	int[,] firstCycleGrid = null;
	int cycleStart = 0;
	bool cycleFound = false;
	var loads = new List<int>(){0};
	do {
		var gridCopy = new int[grid.GetLength(0), grid.GetLength(1)];
		Array.Copy(grid, gridCopy, grid.GetLength(0)*grid.GetLength(1));
		shift(1, 1);
		shift(0, 1);
		shift(1, -1);
		shift(0, -1);

		loads.Add(computeLoad(grid));
		runs++;
		diffs = diffGrid(grid, gridCopy);
		Console.Write($"After cycle {runs} diffs {diffs} load {loads[^1]}");
		if (firstCycleGrid == null) {
			if (diffs == lastDiffs) {
				Console.Write($": Possible cycle start at {runs-1}");
				firstCycleGrid = gridCopy;
				cycleStart = runs-1;
				lastCycleDiffs = diffs;
			} else {
				lastDiffs = diffs;
			}
		} else {
			if (diffs < lastCycleDiffs) {
				Console.Write("Cycle reset");
				firstCycleGrid = null;
			} else {
				var cycleDiffs = diffGrid(grid, firstCycleGrid);
				Console.Write($": Cycle diffs {cycleDiffs}");
				//*
				if (cycleDiffs > diffs) {
					Console.Write(": Reset");
					firstCycleGrid = null;
				} else if (cycleDiffs == 0) cycleFound = true;
			}
		}
		Console.WriteLine();
		//printGrid();
	} while (!cycleFound);

	Console.WriteLine($"Cycle starts at {cycleStart}");
	Console.WriteLine($"Cycle ends at {runs}");
	const int targetRuns = 1_000_000_000;
	var cycleSize = runs-cycleStart;
	var skippedCycles = (targetRuns - runs)/cycleSize;
	var resumeAt = runs+skippedCycles*cycleSize;
	var inCycleIndex = targetRuns-resumeAt;
	Console.WriteLine($"Resume at {runs+skippedCycles*cycleSize}, offset {inCycleIndex}");


	Console.WriteLine(loads[cycleStart+inCycleIndex]);

	void printGrid() {
		for (int y = 0; y < grid.GetLength(1); y++) {
			var s = "";
			for (int x = 0; x < grid.GetLength(0); x++) {
				s += grid[x, y] switch {
					empty => '.',
					unmoving => '#',
					rolling => 'O',
				};
			}
			Console.WriteLine(s);
		}
		Console.WriteLine();
	}

	int computeLoad(int[,] grid) {
		var sum = 0;
		for (int y = 0; y < grid.GetLength(1); y++) {
			var count = 0;
			for (int x = 0; x < grid.GetLength(0); x++) {
				if (grid[x, y]==rolling) count++;
			}
			sum += count*(grid.GetLength(1)-y);
		}
		return sum;
	}

	int diffGrid(int[,] gridA, int[,] gridB) {
		var diffs = 0;
		for (int y = 0; y < gridA.GetLength(1); y++) {
			for (int x = 0; x < gridA.GetLength(0); x++) {
				if (gridA[x, y] != gridB[x, y]) {
					diffs++;
				}
			}
		}
		return diffs;
	}

	void shift(int dimension, int direction) {
		var topmostAvailable = new int[grid.GetLength(1-dimension)];
		if (direction < 0) {
			var n = grid.GetLength(dimension)-1;
			for (int x = 0; x < grid.GetLength(1-dimension); x++) {
				topmostAvailable[x] = n;
			}
		}
		for (int yy = 0; yy < grid.GetLength(dimension); yy++) {
			/*
			if (dimension==0) {
				Console.WriteLine("Step");
				printGrid();
			}
			*/
			var y = direction > 0 ? yy : grid.GetLength(dimension)-1-yy;
			for (int x = 0; x < grid.GetLength(1-dimension); x++) {
				var gx = dimension == 1 ? x : y;
				var gy = dimension == 1 ? y : x;
				switch (grid[gx,gy]) {
				case unmoving:
					topmostAvailable[x] = y+direction;
					break;
				case rolling:
					grid[gx, gy] = empty;
					if (dimension == 1)
						grid[gx, topmostAvailable[x]] = rolling;
					else
						grid[topmostAvailable[x], gy] = rolling;
					topmostAvailable[x]+= direction;
					break;
				}
			}
		}
	}
}