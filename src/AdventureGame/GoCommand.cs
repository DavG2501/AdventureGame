namespace AdventureGame;

public class GoCommand : ICommand
{
	public string Verb => ICommand.GO;

	public bool HasValidArguments(string[] words, out string message)
	{
		if (words.Length == 2)
		{
			message = string.Empty;
			return true;
		}

		message = $"Use {Verb} followed by one direction.";
		return false;
	}

	public void Execute(Player player, string target)
	{
		bool moved = player.TryMove(target, out string message);
		Console.WriteLine(message);

		if (moved)
		{
			Program.ShowRoom(player.CurrentRoom);
		}
		else
		{
			Console.WriteLine($"You remain in {player.CurrentRoom.Name}.");
		}
	}
}