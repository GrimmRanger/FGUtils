using System;
using MonoTouch.Foundation;

namespace FGUtils
{
	[Preserve (AllMembers = true)]
	public class Message
	{
		public Message () {}

		public int Id { get; set; }
		public string Title { get; set; }
		public string Detail { get; set; }
	}
}