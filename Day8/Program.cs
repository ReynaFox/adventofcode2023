using System.Text.RegularExpressions;

part1();
part2();

static void part1() {
	var map = new Dictionary<string, (string l, string r)>();
	var regex = new Regex(@"(\w+) = \((\w+), (\w+)\)");

	var file = File.ReadLines("input.txt");
	var directions = file.First();
	foreach (var line in file.Skip(1)) {
		if (string.IsNullOrEmpty(line)) continue;

		var match = regex.Match(line);
		map.Add(match.Groups[1].Value, (match.Groups[2].Value, match.Groups[3].Value));
	}

	var steps = 0;
	var node = "AAA";
	do {
		node = directions[steps % directions.Length] == 'L' ? map[node].l : map[node].r;
		steps++;
	} while (node != "ZZZ");

	Console.WriteLine(steps);
}

static void part2() {
	var map = new Dictionary<string, (string l, string r)>();
	var regex = new Regex(@"(\w+) = \((\w+), (\w+)\)");
	var nodes = new List<string>();

	var file = File.ReadLines("input.txt");
	var directions = file.First();
	foreach (var line in file.Skip(1)) {
		if (string.IsNullOrEmpty(line)) continue;

		var match = regex.Match(line);
		var name = match.Groups[1].Value;
		map.Add(name, (match.Groups[2].Value, match.Groups[3].Value));
		if (name[2] == 'A') {
			nodes.Add(name);
		}
	}

	var cycleSizes = new int[nodes.Count];
	var dl = directions.Length;
	for (int i = 0; i < nodes.Count; i++) {
		var node = nodes[i];
		var steps = 0;
		var marks = new Dictionary<string, Dictionary<int, int>>();
		int cyclesToEnd = -1;
		do {
			if (node[2]=='Z') {
				cyclesToEnd = steps/dl;
			}
			if (!marks.TryGetValue(node, out var s)) {
				s = new Dictionary<int, int>();
				marks.Add(node, s);
			}
			if (s.ContainsKey(steps%dl)) {
				break;
			}
			s.Add(steps%dl, steps);
			node = directions[steps % dl] == 'L' ? map[node].l : map[node].r;
			steps++;
		} while (true);

		cycleSizes[i] = cyclesToEnd;
	}

	// Analysis of the input shows that a valid end node is only ever encountered once in a cycle,
	// and only ever when we have just finished a list of directions.
	// As such, we can simply find out the minimum of direction list repeats we need to do to have
	// everything in sync - the least common multiple of the cycleSizes array - and then multiply by
	// the direction list length for total steps.

	Console.WriteLine(lcmArr(cycleSizes)*dl);
}

static long gcd(long a, long b) {
	while (b != 0) {
		var t = b;
		b = a%b;
		a = t;
	}
	return a;
}

static long lcm(long a, long b) {
	return a*(b/gcd(a,b));
}

static long lcmArr(int[] ints) {
	long result = ints[0];
	for (int i = 1; i < ints.Length; i++) {
		result = lcm(result, ints[i]);
	}
	return result;
}