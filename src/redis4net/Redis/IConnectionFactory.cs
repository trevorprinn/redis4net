using System;

namespace redis4net.Redis
{
	using System.Threading;

	public class ConnectionFactory : IConnectionFactory
	{
		private static readonly object Lock = new object();

		private readonly string _hostname;
		private readonly int _portNumber;
		private readonly int _failedConnectionRetryTimeoutInSeconds;
		private readonly string _listName;
		private readonly IConnection _connection;
        private readonly int _database;

		public ConnectionFactory(IConnection connection, string hostName, int portNumber, int failedConnectionRetryTimeoutInSeconds, string listName, int database)
		{
			_connection = connection;

			_hostname = hostName;
			_portNumber = portNumber;
			_failedConnectionRetryTimeoutInSeconds = failedConnectionRetryTimeoutInSeconds;
			_listName = listName;
            _database = database;
		}

		public IConnection GetConnection()
		{
			InitializeConnection();
			return _connection;
		}

		private void InitializeConnection()
		{
			if (_connection.IsOpen())
			{
				return;
			}

			lock (Lock)
			{
				try
				{
					OpenConnection();

					if (!_connection.IsOpen())
					{
						Thread.Sleep(TimeSpan.FromSeconds(_failedConnectionRetryTimeoutInSeconds));
						OpenConnection();
					}
				}
				catch
				{
					// Nothing to do if this fails
				}
			}
		}

		private void OpenConnection()
		{
			if (!_connection.IsOpen())
			{
				_connection.Open(_hostname, _portNumber, _listName, _database);
			}
		}
	}
}
