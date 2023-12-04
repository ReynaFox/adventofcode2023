//part1();
part2();

static void part1() {
	var sum = 0;

	foreach (var line in File.ReadLines("input.txt")) {
		var winning = matchingNumbers(line);
		var score = winning== 0 ? 0 : 1<<(winning-1);
		sum += score;
	}

	Console.WriteLine(sum);
}

static void part2() {
	var sum = 0;

	var copies = new List<int>();

	foreach (var line in File.ReadLines("input.txt")) {
		var winning = matchingNumbers(line);
		var copyCount = copies.Count == 0 ? 1 : copies[0]+1;
		if (copies.Count > 0) copies.RemoveAt(0);

		while (copies.Count < winning) copies.Add(0);

		for (int i = 0; i < winning; i++) {
			copies[i]+= copyCount;
		}

		sum += copyCount;
	}

	Console.WriteLine(sum);
}

static int matchingNumbers(string game) {
	var parts = game.Split(':');
	var sets = parts[1].Split('|');

	var winning = new HashSet<int>(
		from num in sets[0].Split(' ', StringSplitOptions.RemoveEmptyEntries)
		select int.Parse(num)
	);

	var numbers = new HashSet<int>(
		from num in sets[1].Split(' ', StringSplitOptions.RemoveEmptyEntries)
		select int.Parse(num)
	);

	winning.IntersectWith(numbers);
	return winning.Count;
}