part1();
part2();


// Note about the input: The X-to-Y-maps happen to all be in sequence.
static void part1() {
	var lines = File.ReadLines("input.txt").ToArray();
	var times = strToInts(lines[0].Split(':')[1]);
	var dists = strToInts(lines[1].Split(':')[1]);

	var prod = 1;
	// Formula for distance d traveled in a race of time t by holding the button for time x:
	// d(x) = x(t-x) = -x^2+tx
	for (int i = 0; i < times.Count; i++) {
		var b = times[i];
		var c = dists[i];
		var D = b*b-4*c;
		var x1 = (b-Math.Sqrt(D))/2;
		var x2 = (b+Math.Sqrt(D))/2;
		var optMin = (int)Math.Ceiling(x1);
		var optMax = (int)Math.Floor(x2);
		if (optMin*(b-optMin) == c) optMin++;
		if (optMax*(b-optMax) == c) optMax--;
		var numOpts = optMax-optMin+1;
		prod *= numOpts;
	}

	Console.WriteLine(prod);
}

static void part2() {
	var lines = File.ReadLines("input.txt").ToArray();
	var time = long.Parse(lines[0].Split(':')[1].Replace(" ", ""));
	var dist = long.Parse(lines[1].Split(':')[1].Replace(" ", ""));

	var b = time;
	var c = dist;
	var D = b*b-4*c;
	var x1 = (b-Math.Sqrt(D))/2;
	var x2 = (b+Math.Sqrt(D))/2;
	var optMin = (int)Math.Ceiling(x1);
	var optMax = (int)Math.Floor(x2);
	if (optMin*(b-optMin) == c) optMin++;
	if (optMax*(b-optMax) == c) optMax--;
	var numOpts = optMax-optMin+1;

	Console.WriteLine(numOpts);
}

static List<long> strToInts(string s) {
	return (
		from num in s.Split(' ', StringSplitOptions.RemoveEmptyEntries)
		select long.Parse(num)
	).ToList();
}
