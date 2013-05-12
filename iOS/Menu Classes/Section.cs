using System;
using System.Collections.Generic;

namespace Firefly
{
	public class Section
	{
		public string Title { get; set; }
		public List<Row> Rows { get; set; }
		public MenuViewableKind ViewableBy { get; set; }

		public Section ()
		{
		}
	}
}

