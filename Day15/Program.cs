part1();
part2();


static void part1() {
	var line = File.ReadLines("input.txt").First();
	var parts = line.Split(',');

	var sum = 0;
	foreach (var part in parts) {
		sum += hash(part);
	}

	Console.WriteLine(sum);
}

static void part2() {
	var line = File.ReadLines("input.txt").First();
	var parts = line.Split(',');

	var boxes = new List<(string label, int focal)>[256];
	for (int i = 0; i < boxes.Length; i++) {
		boxes[i] = [];
	}
	foreach (var part in parts) {
		var labelLen = part.IndexOfAny(['-', '=']);
		var label = part.Substring(0, labelLen);
		var box = boxes[hash(label)];

		var index = box.FindIndex(x => x.label==label);
		if (part[labelLen] == '-') {
				if (index != -1) box.RemoveAt(index);
		} else {
			var num = int.Parse(part[(labelLen+1)..]);
			if (index != -1) box[index] = (label, num);
			else box.Add((label, num));
		}
	}

	var sum = 0;
	for (int i = 0; i < boxes.Length; i++) {
		var box = boxes[i];
		for (int j = 0; j < box.Count; j++) {
			sum+= (i+1)*(j+1)*box[j].focal;
		}
	}

	Console.WriteLine(sum);
}

static int hash(string s) {
	var result = 0;
	foreach (var c in s) {
		result = ((result + c)*17) & 0xFF;
	}
	return result;
}