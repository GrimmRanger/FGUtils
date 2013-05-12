using System;

namespace Firefly
{
	public class Row
	{
		public string Title { get; set; }
		public Action OnSelect { get; set; }

		public Row ()
		{
		}
	}
}

