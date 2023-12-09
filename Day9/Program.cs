part1();
part2();

static void part1() {
	var sum = 0L;

	foreach (var line in File.ReadLines("input.txt")) {
		var numbers = strToInts(line);
		var lastNumbers = new List<long> { numbers[^1] };
		var previous = numbers.ToArray();

		bool allZero;
		do {
			allZero = true;
			var deltas = new long[previous.Length-1];
			for (int i = 0; i < deltas.Length; i++) {
				deltas[i] = previous[i+1] - previous[i];
				allZero &=  deltas[i] == 0;
			}
			lastNumbers.Add(deltas[^1]);
			previous = deltas;
		} while (!allZero);
		for (int i = lastNumbers.Count-2; i>=0; i--) {
			lastNumbers[i] += lastNumbers[i+1];
		}
		sum += lastNumbers[0];
	}

	Console.WriteLine(sum);
}

static void part2() {
	var sum = 0L;

	foreach (var line in File.ReadLines("input.txt")) {
		var numbers = strToInts(line);
		var firstNumbers = new List<long> { numbers[0] };
		var previous = numbers.ToArray();

		bool allZero;
		do {
			allZero = true;
			var deltas = new long[previous.Length-1];
			for (int i = 0; i < deltas.Length; i++) {
				deltas[i] = previous[i+1] - previous[i];
				allZero &=  deltas[i] == 0;
			}
			firstNumbers.Add(deltas[0]);
			previous = deltas;
		} while (!allZero);
		for (int i = firstNumbers.Count-2; i>=0; i--) {
			firstNumbers[i] -= firstNumbers[i+1];
		}
		sum += firstNumbers[0];
	}

	Console.WriteLine(sum);
}

static List<long> strToInts(string s) {
	return (
		from num in s.Split(' ', StringSplitOptions.RemoveEmptyEntries)
		select long.Parse(num)
	).ToList();
}