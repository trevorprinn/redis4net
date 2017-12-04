using System;
using System.Threading.Tasks;

namespace redis4net.Redis
{
	using StackExchange.Redis;

	public class Connection : IConnection, IDisposable
	{
		private string _listName;
		private ConnectionMultiplexer redis;
        private int _database;

		public void Open(string hostname, int port, string listName, int database)
		{
			try
			{
				_listName = listName;
                _database = database;
				redis = ConnectionMultiplexer.Connect(string.Format("{0}:{1}", hostname, port));
			}
			catch
			{
				redis = null;
			}
		}

		public bool IsOpen()
		{
			return redis != null;
		}

		public Task<long> AddToList(string content)
		{
			var database = redis.GetDatabase(_database);
			return database.ListRightPushAsync(_listName, new RedisValue[] { content });
		}

		public void Dispose()
		{
			if (redis == null)
			{
				return;
			}

			redis.Close(false);
			redis.Dispose();
		}
	}
}
