using System.Diagnostics;

part1();
part2();


// Note about the input: The X-to-Y-maps happen to all be in sequence.
static void part1() {
	var hands = new List<(Hand hand, int bid)>();
	foreach (var line in File.ReadLines("input.txt")) {
		if (string.IsNullOrEmpty(line)) continue;

		var parts = line.Split(' ');
		hands.Add((new Hand(parts[0]), int.Parse(parts[1])));
	}

	hands.Sort((a, b) => {
		for (var i = 0; i < 6; i++) {
			var c = a.hand.data[i].CompareTo(b.hand.data[i]);
			if (c != 0) return c;
		}
		return 0;
	});

	var sum = 0;
	for (int i = 0; i < hands.Count; i++) {
		sum += hands[i].bid * (i+1);
	}

	Console.WriteLine(sum);
}

static void part2() {
	var hands = new List<(Hand hand, int bid)>();
	foreach (var line in File.ReadLines("input.txt")) {
		if (string.IsNullOrEmpty(line)) continue;

		var parts = line.Split(' ');
		hands.Add((new Hand(parts[0], true), int.Parse(parts[1])));
	}

	hands.Sort((a, b) => {
		for (var i = 0; i < 6; i++) {
			var c = a.hand.data[i].CompareTo(b.hand.data[i]);
			if (c != 0) return c;
		}
		return 0;
	});

	var sum = 0;
	for (int i = 0; i < hands.Count; i++) {
		sum += hands[i].bid * (i+1);
	}

	Console.WriteLine(sum);
}

internal class Hand {
	private const int fiveKind = 7;
	private const int fourKind = 6;
	private const int fullHouse = 5;
	private const int threeKind = 4;
	private const int twoPair = 3;
	private const int onePair = 2;
	private const int highCard = 1;

	public int[] data = new int[6];

	public Hand(string s, bool joker = false) {
		for (var i = 0; i < 5; i++) {
			data[i+1] = s[i] switch {
				'A' => 14,
				'K' => 13,
				'Q' => 12,
				'J' => joker ? 1 : 11,
				'T' => 10,
				_ => s[i]-'0'
			};
		}

		var cards = data.Skip(1);
		var jokerCount = cards.Count(c => c==1);
		var counts = (from x in data.Skip(1)
			  where x!=1
				group x by x
				into value
				let count = value.Count()
				orderby count descending
				select (value, count)).ToArray();

		if (joker && counts.Length == 0) {
			data[0] = fiveKind;
			return;
		}
		if (counts[0].count == 5)
			data[0] = fiveKind;
		else if (counts[0].count == 4)
			data[0] = fourKind;
		else if (counts[0].count == 3)
			data[0] = counts.Length > 1 && counts[1].count == 2 ? fullHouse : threeKind;
		else if (counts[0].count == 2)
			data[0] = counts.Length > 1 && counts[1].count == 2 ? twoPair : onePair;
		else
			data[0] = highCard;

		if (joker && jokerCount > 0) {
			if (data[0] == twoPair) data[0] = fullHouse;
			else {
				var orig = data[0];
				data[0] += jokerCount;
				if (orig < twoPair && data[0]>= twoPair) data[0]++;
				if (orig < fullHouse&& data[0]>= fullHouse) data[0]++;
			}
		}
	}
}