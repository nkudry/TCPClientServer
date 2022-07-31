using System;
using System.Text.RegularExpressions;
using System.Threading;

namespace TCPClient
{
	class Program
	{
		private static readonly ManualResetEvent terminateApplication = new ManualResetEvent(false);
		private static Client client;

		private static void ErrorHandler(LogLevel status)
		{
			Logger.Log($"{status} occured, terminating application!");
			client.Dispose();
			terminateApplication.Set();
		}

		private static void OnConnect(bool status)
		{
			if (status)
			{
				string testMessage = "Test message";
				Logger.Log($"Sending initial message to server: \"{testMessage}\"");
				client.Send(testMessage);
			}
		}

		private static void OnRecieved(string data)
		{
			Logger.Log($"Response from server: \"{data}\"");
			PromptInput();
		}

		private static void PromptInput()
		{
			Logger.Log("Type your message to send to the server:");
			var input = Console.ReadKey().KeyChar.ToString();
			var inputStr = Regex.Replace(input, @"\s+", "");
			Console.WriteLine();
			if (!string.IsNullOrEmpty(inputStr))
				client.Send(inputStr);
			else
			{
				Logger.Log(LogLevel.Warning, "Invalid input!");
				PromptInput();
			}
		}

		public static void Main(string[] args)
		{
			Logger.OnErrorEvent += new Logger.ErrorEventHandler(ErrorHandler);
			string host = "192.168.100.100";
#if DEBUG
			host = "127.0.0.1";
#endif
			int port = 54321;
			Logger.Log($"Connecting to server {host}:{port}");

			client = new Client(host, port);
			client.OnConnectEvent += new Client.OnConnectEventHandler(OnConnect);
			client.OnDataRecievedEvent += new Client.DataReceivedEventHandler(OnRecieved);
			client.Connect();

			terminateApplication.WaitOne();
		}
	}
}
