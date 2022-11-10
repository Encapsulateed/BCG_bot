

using sample;






while (true)
{
	try
	{
		 await Bot.Start();
    }
	catch (Exception ex)
	{
		Console.WriteLine(ex);
		throw;
	}
}
