part1();
part2();


// Note about the input: The X-to-Y-maps happen to all be in sequence.
static void part1() {
	var numbers = new List<long>();
	int endMarker = 0;

	foreach (var line in File.ReadLines("input.txt")) {
		if (string.IsNullOrEmpty(line)) continue;

		if (numbers.Count == 0) {
			// Should be the seeds: line
			numbers = strToInts(line[7..]);
			continue;
		}

		if (line[^4..] == "map:") {
			// New group
			endMarker = numbers.Count-1;
			continue;
		}

		// Range
		var range = strToInts(line);
		var tgtRangeStart = range[0];
		var srcRangeStart = range[1];
		var size = range[2];
		for (int i = 0; i <= endMarker; ) {
			if (numbers[i] >= srcRangeStart && numbers[i] < srcRangeStart+size) {
				// In range: update
				var newNum = numbers[i]-srcRangeStart+tgtRangeStart;
				numbers[i] = numbers[endMarker];
				numbers[endMarker] = newNum;
				endMarker--;
			} else {
				// Out of range: ignore
				i++;
			}
		}
	}

	Console.WriteLine(numbers.Min());
}

static void part2() {
	var ranges = new List<Range>();
	var newRanges = new List<Range>();
	foreach (var line in File.ReadLines("input.txt")) {
		if (string.IsNullOrEmpty(line)) continue;

		if (line.StartsWith("seeds:")) {
			var numbers = strToInts(line[7..]);
			for (int i = 0; i < numbers.Count; i += 2) {
				newRanges.Add(new Range(numbers[i], numbers[i]+numbers[i+1]-1));
			}
			continue;
		}

		if (line[^4..] == "map:") {
			// New group
			newRanges.AddRange(ranges);
			ranges = newRanges;
			newRanges = new List<Range>();
			continue;
		}

		// Range
		var range = strToInts(line);
		var tgtRangeStart = range[0];
		var srcRangeStart = range[1];
		var size = range[2];
		for (int i = 0; i < ranges.Count; ) {
			var srcRangeEnd = srcRangeStart+size-1;
			if (ranges[i].overlaps(srcRangeStart, srcRangeEnd)) {
				ranges.AddRange(ranges[i].split(srcRangeStart, srcRangeEnd));
				ranges[i].shift(tgtRangeStart-srcRangeStart);
				newRanges.Add(ranges[i]);
				ranges.RemoveAt(i);
			} else {
				i++;
			}
		}
	}

	newRanges.AddRange(ranges);
	Console.WriteLine(newRanges.Min(i => i.start));
}

static List<long> strToInts(string s) {
	return (
		from num in s.Split(' ', StringSplitOptions.RemoveEmptyEntries)
		select long.Parse(num)
	).ToList();
}

class Range (long start, long end) {
	public long start = start;

	public bool overlaps(long otherStart, long otherEnd) {
		return start <= otherEnd && otherStart <= end;
	}

	public IEnumerable<Range> split(long otherStart, long otherEnd) {
		if (start < otherStart) {
			yield return new Range(start, otherStart-1);
			start = otherStart;
		}
		if (end > otherEnd) {
			yield return new Range(otherEnd+1, end);
			end = otherEnd;
		}
	}

	public void shift(long delta) {
		start += delta;
		end += delta;
	}

	public override string ToString() {
		return $"[{start}, {end}] (size {end-start+1})";
	}
}