using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScreenTimeTracker.Management.Models
{
	public class RegisterModel
	{
		public string Name { get; set; }

		public string Password { get; set; }

		public string ConfirmPassword { get; set; }
	}
}