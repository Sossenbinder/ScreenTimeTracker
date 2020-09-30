using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace ScreenTimeTracker.Services
{
	public static class ConfigBuilder
	{
		private const string ConfigFilePath = "config.json";

		public static IConfiguration ValidateAndBuildConfiguration()
		{
			if (!File.Exists(ConfigFilePath))
			{
				throw new ArgumentException("Couldn't find the config.json");
			}

			var configuration = new ConfigurationBuilder()
				.AddJsonFile(ConfigFilePath)
				.Build();

			if (configuration["Token"] == null)
			{
				throw new ArgumentException("config.json does not contain a 'Token' key");
			}

			return configuration;
		}
	}
}