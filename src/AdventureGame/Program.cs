using AdventureGame;
using System.Linq;

public class Program
{
	public const string NORTH = "NORTH";
	public const string SOUTH = "SOUTH";
	public const string EAST = "EAST";
	public const string WEST = "WEST";
	public const string QUIT = "QUIT";
	public const string GO = "GO";
	public const string LOOK = "LOOK";
	public const string TAKE = "TAKE";
	public const string DROP = "DROP";
	public const string INVENTORY = "INVENTORY";

	public static void Main()
	{
		Room entrance = new Room("Entrance", "A dry stone chamber with daylight spilling through the doorway.");
		Room gallery = new Room("Gallery", "A long gallery lined with faded banners.");
		Room archive = new Room("Archive", "An archive whose shelves hold dust-covered chronicles.");

		entrance.Connect(EAST, gallery);
		gallery.Connect(NORTH, archive);

		Item oldCoin = new Item("Old Coin", "A tarnished coin stamped with a forgotten crest.");
		Item brassToken = new Item("Brass Token", "A square brass token etched with a small tower.");
		Item copperRing = new Item("Copper Ring", "A small ring with a dull copper finish.");

		gallery.PlaceItem(oldCoin);
		gallery.PlaceItem(copperRing);
		archive.PlaceItem(brassToken);

		Player player = new Player(entrance);

		IReadOnlyList<ICommand> commands = new ICommand[]
		{
			new GoCommand(),
			new LookCommand(),
			new TakeCommand(),
			new DropCommand(),
			new InventoryCommand()
		};

		Console.WriteLine("Adventure Game — Inventory Lab");
		Console.WriteLine($"Commands: {GO} <direction>, {LOOK}, {TAKE} <item name>, {DROP} <item name>, {INVENTORY}, {QUIT}.");
		ShowRoom(player.CurrentRoom);

		while (true)
		{
			Console.Write("> ");
			string normalized = NormalizeInput(Console.ReadLine());

			if (normalized.Length == 0)
			{
				Console.WriteLine($"Use {GO} <direction>, {LOOK}, {TAKE} <item name>, {DROP} <item name>, {INVENTORY}, or {QUIT}.");
				continue;
			}

			string[] words = normalized.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
			string verb = words[0];

			if (verb == QUIT)
			{
				if (words.Length != 1)
				{
					Console.WriteLine($"{QUIT} takes no arguments.");
					continue;
				}
				Console.WriteLine("You leave the dungeon.");
				break;
			}

			ICommand? selected = FindCommand(verb, commands);

			if (selected is null)
			{
				Console.WriteLine($"Unknown command. Use {GO} <direction>, {LOOK}, {TAKE} <item name>, {DROP} <item name>, {INVENTORY}, or {QUIT}.");
				continue;
			}

			if (!selected.HasValidArguments(words, out string errorMessage))
			{
				Console.WriteLine(errorMessage);
				continue;
			}

			string target = words.Length > 1 ? string.Join(" ", words, 1, words.Length - 1) : string.Empty;
			selected.Execute(player, target);
		}
	}

	private static ICommand? FindCommand(string verb, IReadOnlyList<ICommand> commands)
	{
		foreach (ICommand candidate in commands)
		{
			if (candidate.Verb == verb)
			{
				return candidate;
			}
		}
		return null;
	}

	public static string NormalizeInput(string? input)
	{
		return input?.Trim().ToUpperInvariant() ?? QUIT;
	}

	public static void ShowRoom(Room room)
	{
		Console.WriteLine($"{room.Name}: {room.Description}");
		Console.WriteLine($"  {NORTH}: {room.North?.Name ?? "(none)"}");
		Console.WriteLine($"  {SOUTH}: {room.South?.Name ?? "(none)"}");
		Console.WriteLine($"   {EAST}: {room.East?.Name ?? "(none)"}");
		Console.WriteLine($"   {WEST}: {room.West?.Name ?? "(none)"}");

		if (room.Items.Count == 0)
		{
			Console.WriteLine("  Items: (none)");
			return;
		}

		string itemList = string.Join(", ", room.Items.Select(item => item.Name));
		Console.WriteLine($"  Items: {itemList}");
	}

	public static void ShowInventory(Player player)
	{
		if (player.Inventory.Count == 0)
		{
			Console.WriteLine("Inventory: (empty)");
			return;
		}

		Console.WriteLine("Inventory:");
		foreach (Item item in player.Inventory)
		{
			Console.WriteLine($"  - {item.Name}: {item.Description}");
		}
	}

	public static void PreviewExit(Room fromRoom, string direction)
	{
		if (direction != NORTH && direction != SOUTH && direction != EAST && direction != WEST)
		{
			Console.WriteLine($"Unknown direction. Choose {NORTH}, {SOUTH}, {EAST}, or {WEST}.");
			return;
		}

		Room? neighbor = direction switch
		{
			NORTH => fromRoom.North,
			SOUTH => fromRoom.South,
			EAST => fromRoom.East,
			WEST => fromRoom.West,
			_ => null
		};

		if (neighbor is null)
		{
			Console.WriteLine($"There is no connected room {direction} of {fromRoom.Name}.");
			return;
		}

		Console.WriteLine($"Connected room {direction} of {fromRoom.Name}: {neighbor.Name}");
		Console.WriteLine($"Description: {neighbor.Description}");
	}
}