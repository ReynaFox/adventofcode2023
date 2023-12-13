part1();
part2();


static void part1() {
	var sum = 0;
	var map = new List<string>();
	foreach (var line in File.ReadLines("input.txt")) {
		if (!string.IsNullOrEmpty(line)) {
			map.Add(line);
			continue;
		}

		var horizontal = findHorizontalLine(map);
		if (horizontal == -1) {
			var vertical = findHorizontalLine(transpose(map));
			sum += vertical+1;
		} else {
			sum += (horizontal+1)*100;
		}

		map.Clear();
	}
	Console.WriteLine(sum);

	int findHorizontalLine(List<string> map) {
		for (int i = 0; i < map.Count-1; i++) {
			if (map[i] != map[i+1]) continue;

			int j = 1;
			bool mirrored = true;
			while (i-j >= 0 && i+1+j < map.Count) {
				if (map[i-j] != map[i+1+j]) {
					mirrored = false;
					break;
				}
				j++;
			}

			if (mirrored) return i;
		}

		return -1;
	}
}

static void part2() {
	var sum = 0;
	var map = new List<string>();
	foreach (var line in File.ReadLines("input.txt")) {
		if (!string.IsNullOrEmpty(line)) {
			map.Add(line);
			continue;
		}

		var horizontal = findHorizontalLine(map);
		if (horizontal == -1) {
			var vertical = findHorizontalLine(transpose(map));
			sum += vertical;
		} else {
			sum += horizontal*100;
		}

		map.Clear();
	}
	Console.WriteLine(sum);

	int findHorizontalLine(List<string> map) {
		var diffMap = new int[map.Count, map.Count];

		for (int i = 0; i < map.Count; i++)
		for (int j = i+1; j < map.Count; j++) {
			var diffs = 0;
			for (int k = 0; k < map[i].Length; k++) {
				if (map[i][k] != map[j][k]) diffs++;
			}
			diffMap[i,j] = diffs;
		}

		// A regular mirrored option will have a diagonal along +i,-j of all zeroes.
		// A mirrored option with one "fix" will have such a diagonal with one 1 in it.

		for (int i = 1; i < map.Count; i++) {
			var others = 0;
			var ones = 0;
			for (int j = 0; j < Math.Min(i, map.Count-i); j++) {
				var c = diffMap[i-1-j, i+j];
				if (c == 1) ones++;
				else if (c != 0) others++;
			}
			if (ones == 1 && others == 0) return i;
		}
		return -1;
	}
}

static List<string> transpose(List<string> map) {
	var result = new List<string>();

	for (int i = 0; i < map[0].Length; i++) {
		var c = new char[map.Count];
		for (int j = 0; j < map.Count; j++) {
			c[j] = map[j][i];
		}
		result.Add(new string(c));
	}

	return result;
}
