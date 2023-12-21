//part1();

part2();

static void part1() {
	var modules = read();

	var highSum = 0L;
	var lowSum = 0L;
	for (int i = 0; i < 1000; i++) {
		var (h, l) = execute(modules, (_,_,_)=>{ });
		highSum += h;
		lowSum += l;
	}
	Console.WriteLine($"Low: {lowSum} High: {highSum} Result: {highSum*lowSum}");
}

static void part2() {
	var modules = read();
	var presses = 0;
	var done = false;
	var conjToRx = "";
	foreach (var (name, mod) in modules) {
		if (mod.targets.Contains("rx")) {
			conjToRx = name;
			break;
		}
	}
	var cycles = new Dictionary<string, List<int>>();
	foreach (var (name, mod) in modules) {
		if (mod.targets.Contains(conjToRx)) {
			cycles.Add(name, new List<int>());
		}
	}
	// Analysis of the module graph shows there are effectively four subgraphs with an individual
	// cycle. The following code crudely figures out the cycle length for that.
	do {
		presses++;
		execute(modules, (src, tgt, sig)=>{
			if (tgt=="rx" && !sig) {
				done = true;
				Console.WriteLine($"Complete after {presses}");
			}
			if (tgt==conjToRx && sig) {
				cycles[src].Add(presses);
			}
		});
		var allCycles = true;
		foreach (var (_, cycle) in cycles) {
			// Cycle count is certain when the deltas between the last three button presses are equal.
			if (cycle.Count <= 2 || cycle[^1]-cycle[^2] != cycle[^2]-cycle[^3]) {
				allCycles = false;
				break;
			}
		}
		done = allCycles;
	} while(!done);
	var cycleSizes = new List<int>();
	foreach (var (_, cycle) in cycles) {
		// Cycle count is certain when the deltas between the last three button presses are equal.
		cycleSizes.Add(cycle[^1]-cycle[^2]);
	}
	Console.WriteLine(lcmArr(cycleSizes.ToArray()));
}

static Dictionary<string, Module> read() {
	var modules = new Dictionary<string, Module>();
	foreach (var line in File.ReadLines("input.txt")) {
		var parts = line.Split("->", StringSplitOptions.TrimEntries);
		string name;
		Module mod;
		if (parts[0] == "broadcaster") {
			name = parts[0];
			mod = new Broadcast();
		} else {
			mod = parts[0][0] switch {
				'%' => new Flipflop(),
				'&' => new Conjunction()
			};
			name = parts[0][1..];
		}
		mod.targets = parts[1].Split(',', StringSplitOptions.TrimEntries);
		modules[name] = mod;
	}

	foreach (var (name, mod) in modules) {
		foreach (var tgt in mod.targets) {
			if (modules.TryGetValue(tgt, out var tgtMod)) tgtMod.addSource(name);
		}
	}
	return modules;
}

static (int h, int l) execute(Dictionary<string, Module> modules, Action<string, string, bool> sigCallback) {
	var h = 0;
	var l = 0;
	var sent = new List<(string[] targets, bool sig, string src)> { (["broadcaster"], false, "<button>") };

	var names = new []{"lz", "lf", "vq", "qz", "bh", "hq", "mh", "xp", "mq", "jd", "vx", "ds", "kz", "pn"};
	do {
		var next = sent[0];
		sent.RemoveAt(0);

		foreach (var tgt in next.targets) {
			if (next.sig) h++;
			else l++;

			sigCallback(next.src, tgt, next.sig);

			if (Array.IndexOf(names, tgt)!=-1) {
				//Console.WriteLine($"{next.src} -{(next.sig ? "high" : "low")} -> {tgt}");
			}

			if (modules.TryGetValue(tgt, out var mod)) {
				var result = mod.signal(next.sig, next.src);
				if (result!=null) {
					sent.Add((mod.targets, result.Value, tgt));
				}
			}
		}
	} while (sent.Count > 0);

	return (h, l);
}
static long gcd(long a, long b) {
	while (b != 0) {
		var t = b;
		b = a%b;
		a = t;
	}
	return a;
}

static long lcm(long a, long b) {
	return a*(b/gcd(a,b));
}

static long lcmArr(int[] ints) {
	long result = ints[0];
	for (int i = 1; i < ints.Length; i++) {
		result = lcm(result, ints[i]);
	}
	return result;
}

abstract class Module {
	public string[] targets;

	public virtual void addSource(string name) { }
	public abstract bool? signal(bool sig, string src);
}

class Broadcast: Module {
	public override bool? signal(bool sig, string src) {
		return sig;
	}
}

class Flipflop: Module {
	private bool state = false;

	public override bool? signal(bool sig, string src) {
		if (sig) return null;
		state = !state;
		return state;
	}
}

class Conjunction: Module {
	private readonly Dictionary<string, bool> sourceState = new();

	public override void addSource(string name) {
		sourceState.Add(name, false);
	}

	public override bool? signal(bool sig, string src) {
		sourceState[src] = sig;
		return !sourceState.Values.All(x => x);
	}
}

