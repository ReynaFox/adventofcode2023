part1();
part2();

static void part1() {
	var digits = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

	var sum = 0;
	foreach (var line in File.ReadLines("input.txt")) {
		var first = line[line.IndexOfAny(digits)].ToString();
		var last = line[line.LastIndexOfAny(digits)].ToString();
		var num = int.Parse(first+last);
		sum += num;
	}

	Console.WriteLine(sum);
}

static void part2() {
	var digits = new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
	var words = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };

	var sum = 0;
	foreach (var line in File.ReadLines("input.txt")) {
		var firstPos = int.MaxValue;
		var firstDigit = 0;
		var lastPos = -1;
		var lastDigit = 0;
		for (int i = 0; i < 10; i++) {
			newFirstPos(line.IndexOf(digits[i]), i, ref firstPos, ref firstDigit);
			newFirstPos(line.IndexOf(words[i]), i, ref firstPos, ref firstDigit);

			newLastPos(line.LastIndexOf(digits[i]), i, ref lastPos, ref lastDigit);
			newLastPos(line.LastIndexOf(words[i]), i, ref lastPos, ref lastDigit);
		}

		var num = firstDigit*10+lastDigit;
		sum += num;
	}

	Console.WriteLine(sum);
}

static void newFirstPos(int foundPos, int digit, ref int firstPos, ref int firstDigit) {
	if (foundPos != -1 && foundPos < firstPos) {
		firstPos = foundPos;
		firstDigit = digit;
	}
}
static void newLastPos(int foundPos, int digit, ref int lastPos, ref int lastDigit) {
	if (foundPos != -1 && foundPos > lastPos) {
		lastPos = foundPos;
		lastDigit = digit;
	}
}