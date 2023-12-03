part1();
part2();

static void part1() {
	var sum = 0;
	int[] prevLine = null;

	foreach (var line in File.ReadLines("input.txt")) {
		var numStart = -1;
		var currLine = new int[line.Length];
		for (int i = 0; i < line.Length; i++) {
			if (line[i]>='0' && line[i]<='9') {
				if (numStart==-1) numStart = i;
			} else {
				currLine[i] = line[i] == '.' ? 0 : 0x1000000;
				if (numStart!=-1) {
					var num = int.Parse(line.Substring(numStart, i-numStart));
					currLine[numStart] = num;
					for (int j = numStart+1; j < i; j++) currLine[j] = numStart-j;
					numStart = -1;
				}
			}
		}

		if (numStart != -1) {
			var num = int.Parse(line.Substring(numStart, line.Length-numStart));
			currLine[numStart] = num;
			for (int j = numStart+1; j < line.Length; j++) currLine[j] = numStart-j;
		}


		checkLine(currLine, currLine);
		if (prevLine != null) {
			checkLine(currLine, prevLine);
			checkLine(prevLine, currLine);
		}

		prevLine = currLine;
	}

	Console.WriteLine(sum);


	void checkLine(int[] markLine, int[] numLine) {
		for (int i = 0; i < markLine.Length; i++) {
			if (markLine[i] != 0x1000000) continue;

			for (int j = -1; j<=1; j++) {
				if (i+j < 0 || i+j >= numLine.Length) continue;

				if (numLine[i+j] < 0) {
					// number is offset
					sum += numLine[i+j+numLine[i+j]];
					blank(numLine, i+j);
				}

				if (numLine[i+j]>0 && numLine[i+j] != 0x1000000) {
					sum += numLine[i+j];
					blank(numLine, i+j);
				}
			}
		}
	}

	void blank(int[] line, int i) {
		if (line[i] < 0) i += line[i];
		do {
			line[i] = 0;
			i++;
		} while (i<line.Length && line[i] < 0);
	}
}

static void part2() {
	var sum = 0;
	int[] prevLine = null;
	Dictionary<int, List<int>> prevGears = null;

	foreach (var line in File.ReadLines("input.txt")) {
		var numStart = -1;
		var currLine = new int[line.Length];
		var currGears = new Dictionary<int, List<int>>();
		for (int i = 0; i < line.Length; i++) {
			if (line[i]>='0' && line[i]<='9') {
				if (numStart==-1) numStart = i;
			} else {
				if (line[i] == '*') {
					currGears[i] = new List<int>();
				}

				//currLine[i] = line[i] == '*' ? 0x1000000 : 0;
				currLine[i] = 0;
				if (numStart!=-1) {
					var num = int.Parse(line.Substring(numStart, i-numStart));
					currLine[numStart] = num;
					for (int j = numStart+1; j < i; j++) currLine[j] = numStart-j;
					numStart = -1;
				}
			}
		}

		if (numStart != -1) {
			var num = int.Parse(line.Substring(numStart, line.Length-numStart));
			currLine[numStart] = num;
			for (int j = numStart+1; j < line.Length; j++) currLine[j] = numStart-j;
		}

		checkLine(currGears, currLine);
		if (prevLine != null) {
			checkLine(currGears, prevLine);
			checkLine(prevGears, currLine);

			foreach (var (pos, list) in prevGears) {
				if (list.Count != 2) continue;
				var ratio = list[0] * list[1];
				sum += ratio;
			}
		}

		prevGears = currGears;
		prevLine = currLine;
	}

	Console.WriteLine(sum);

	void checkLine(Dictionary<int, List<int>> gearLine, int[] numLine) {
		foreach (var (pos, list) in gearLine) {
			for (int j = -1; j <= 1; j++) {
				if (numLine[pos+j] < 0) {
					j += numLine[pos+j];
				}
				if (numLine[pos+j] > 0) {
					list.Add(numLine[pos+j]);
					while (pos+j < numLine.Length && numLine[pos+j]!=0) j++;
				}
			}
		}
	}
}