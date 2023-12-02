part1();
part2();

static void part1() {
	var sum = 0;
	foreach (var line in File.ReadLines("input.txt")) {
		if (string.IsNullOrEmpty(line)) continue;
		var parts = line.Split(':');
		var id = int.Parse(parts[0].Substring(parts[0].LastIndexOf(' ')+1));

		const int validRed = 12;
		const int validGreen = 13;
		const int validBlue = 14;

		var draws = parts[1].Split(';', StringSplitOptions.RemoveEmptyEntries);
		var valid = true;
		foreach (var draw in draws) {
			var cubes = draw.Split(',', StringSplitOptions.RemoveEmptyEntries);
			foreach (var cube in cubes) {
				parts = cube.Split(' ', StringSplitOptions.RemoveEmptyEntries);
				var num = int.Parse(parts[0]);
				var color = parts[1];
				if (color == "red" && num > validRed) valid = false;
				if (color == "green" && num > validGreen) valid = false;
				if (color == "blue" && num > validBlue) valid = false;
			}
		}

		if (valid) sum += id;
	}

	Console.WriteLine(sum);
}

static void part2() {
	var sum = 0;
	foreach (var line in File.ReadLines("input.txt")) {
		if (string.IsNullOrEmpty(line)) continue;
		var parts = line.Split(':');
		var id = int.Parse(parts[0].Substring(parts[0].LastIndexOf(' ')+1));

		var reqdRed = 0;
		var reqdGreen = 0;
		var reqdBlue = 0;

		var draws = parts[1].Split(';', StringSplitOptions.RemoveEmptyEntries);
		var valid = true;
		foreach (var draw in draws) {
			var cubes = draw.Split(',', StringSplitOptions.RemoveEmptyEntries);
			foreach (var cube in cubes) {
				parts = cube.Split(' ', StringSplitOptions.RemoveEmptyEntries);
				var num = int.Parse(parts[0]);
				var color = parts[1];
				if (color == "red") reqdRed = Math.Max(reqdRed, num);
				if (color == "green") reqdGreen = Math.Max(reqdGreen, num);
				if (color == "blue") reqdBlue = Math.Max(reqdBlue, num);
			}
		}

		sum += reqdRed*reqdBlue*reqdGreen;
	}

	Console.WriteLine(sum);
}