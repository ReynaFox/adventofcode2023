part1();
part2();

static void part1() {
	var actions = new Dictionary<string, List<Condition>>();
	var inItems = false;
	var sum = 0;
	foreach (var line in File.ReadLines("input.txt")) {
		if (string.IsNullOrEmpty(line)) {
			inItems = true;
			continue;
		}
		if (!inItems) {
			parseAction(line, actions);
		} else {
			var item = new Dictionary<char, int>();
			var open = line.IndexOf('{');
			var close = line.IndexOf('}');
			foreach (var part in line[(open+1)..close].Split(',')) {
				item[part[0]] = int.Parse(part[2..]);
			}

			var next = "in";
			do {
				var act = actions[next];
				foreach (var c in act) {
					if (c.matches(item)) {
						next = c.next;
						break;
					}
				}
			} while (next != "R" && next != "A");

			if (next == "A") {
				var isum = item.Sum(x => x.Value);
				Console.WriteLine($"Item {line} accepted with value {isum}");
				sum += item.Sum(x => x.Value);
			}
		}
	}

	Console.WriteLine(sum);
}

static void part2() {
	var actions = new Dictionary<string, List<Condition>>();
	foreach (var line in File.ReadLines("input.txt")) {
		if (string.IsNullOrEmpty(line)) {
			break;
		}
		parseAction(line, actions);
	}

	var items = new List<(string next, Item)> {
		("in", new Item {
			{ 'x', new Range(1, 4000) },
			{ 'm', new Range(1, 4000) },
			{ 'a', new Range(1, 4000) },
			{ 's', new Range(1, 4000) }
		})
	};


	var accepted = new List<Item>();

	while (items.Count > 0) {
		/*
		foreach (var i in items) {
			Console.WriteLine(i);
		}
		Console.WriteLine("-------------------");
		*/
		var (next, item) = items[^1];
		items.RemoveAt(items.Count-1);
		var act = actions[next];
		foreach (var c in act) {
			var rejected = item.rejectedPart(c);
			if (!item.isEmpty()) {
				if (c.next == "A")
					accepted.Add(item);
				else if (c.next != "R")
					items.Add((c.next, item));
			}
			if (rejected == null) break;
			item = rejected;
		}
	}

	Console.WriteLine("Accepted");
	var sum = 0L;
	foreach (var i in accepted) {
		Console.WriteLine(i);
		var count = i['x'].size*i['m'].size*i['a'].size*i['s'].size;
		sum += count;
		Console.WriteLine(count);
	}

	Console.WriteLine(sum);
}

static void parseAction(string line, Dictionary<string, List<Condition>> actions) {
	var open = line.IndexOf('{');
	var close = line.IndexOf('}');
	var name = line[..open];
	var conditions = new List<Condition>();
	foreach (var part in line[(open+1)..close].Split(',')) {
		conditions.Add(new Condition(part));
	}
	actions[name] = conditions;
}

class Condition {
	public readonly char var;
	public readonly char op;
	public readonly int val;
	public readonly string next;

	public Condition(string s) {
		if (s.Length <= 1 || (s[1] != '<' && s[1] != '>')) {
			var = '*';
			op = '*';
			val = 0;
			next = s;
		} else {
			var = s[0];
			op = s[1];
			var cPos = s.IndexOf(':');
			val = int.Parse(s[2..cPos]);
			next = s[(cPos+1)..];
		}
	}

	public bool matches(Dictionary<char, int> item) {
		switch (op) {
		case '<': return item[var] < val;
		case '>': return item[var] > val;
		case '*': return true;
		}
		return true;
	}
}

class Range {
	public int min;
	public int max;

	public Range(int min, int max) {
		this.min = min;
		this.max = max;
	}

	public Range(Range other) {
		this.min = other.min;
		this.max = other.max;
	}

	public bool isEmpty() => min>=max;
	public long size => max-min+1;

	public override string ToString() {
		return $"Range({min}, {max})";
	}
}

class Item: Dictionary<char, Range?> {
	public Item(): base() { }
	public Item(Item other): base(other) { }

	public Item? rejectedPart(Condition c) {
		if (c.op == '*') return null;

		Range rejectedRange;
		if (c.op == '<') {
			rejectedRange = new Range(c.val, this[c.var].max);
			this[c.var].max = c.val-1;
		} else {
			rejectedRange = new Range(this[c.var].min, c.val);
			this[c.var].min = c.val+1;
		}

		if (this[c.var].isEmpty()) this[c.var] = null;

		var result = new Item() {
			{ 'x', new Range(this['x']) },
			{ 'm', new Range(this['m']) },
			{ 'a', new Range(this['a']) },
			{ 's', new Range(this['s']) }
		};
		result[c.var] = rejectedRange.isEmpty() ? null : rejectedRange;

		return result;
	}

	public bool isEmpty() => this.Any(x => x.Value == null);

	public override string ToString() {
		return $"Item({string.Join(", ", this.Select(x => $"[{x.Key}, {x.Value}]"))})";
	}
}