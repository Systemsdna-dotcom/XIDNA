using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.Models
{
	public class BCOneCommand
	{
		public string AccessKey { get; set; }
		public int AuditId { get; set; }
		public string Owner { get; set; }
		public string RepositoryName { get; set; }
		public string BranchName { get; set; }
		public Command Command { get; set;}
	}

	public enum Command
	{
		CloneRepo=1,
		SyncLatestCommits=2
	}
}
