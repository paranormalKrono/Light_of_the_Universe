using System.Xml.Serialization;

[XmlRoot("names")]
public class TextNames
{
	[XmlArray("names")]
	[XmlArrayItem("name")]
	public Name[] names;

	[System.Serializable]
	public class Name
	{
		[XmlAttribute("name")]
		public string name;
		[XmlAttribute("namecolor")]
		public string namecolor;
		[XmlAttribute("textcolor")]
		public string textcolor;
	}
}

[XmlRoot("dialogue")]
public class TextDialog
{
	[XmlElement("node")]
	public Node[] nodes;

	[System.Serializable]
	public class Node
	{
		[XmlAttribute("isDescription")]
		public bool isDescription = false;
		[XmlAttribute("isAnswer")]
		public bool isAnswer = false;
		[XmlAttribute("nameid")]
		public int nameid;
		[XmlElement("text")]
		public string text;

		[XmlArray("answers")]
		[XmlArrayItem("answer")]
		public Answer[] answers;

	}
	[System.Serializable]
	public class Answer
	{
		[XmlAttribute("tonode")]
		public int nextNode;
		[XmlElement("text")]
		public string text;
		[XmlElement("end")]
		public bool isend;
	}
}

[XmlRoot("gametext")]
public class TextInGame
{
	[XmlArray("startdialogs")]
	[XmlArrayItem("dialog")]
	public Dialog[] dialogs;

	[XmlArray("ingamedialogs")]
	[XmlArrayItem("ingamedialog")]
	public InGameDialog[] ingamedialogs;

	[XmlArray("goals")]
	[XmlArrayItem("goal")]
	public string[] goals;

	[System.Serializable]
	public class Dialog
	{
		[XmlAttribute("nameid")]
		public int nameid;
		[XmlAttribute("textsize")]
		public int textsize = 12;
		[XmlElement("prevtext")]
		public string prevtext;
		[XmlElement("text")]
		public string text;
	}
	[System.Serializable]
	public class InGameDialog
	{
		[XmlAttribute("time")]
		public float time;

		[XmlArray("dialogs")]
		[XmlArrayItem("dialog")]
		public Dialog[] dialogs;
	}

}

[XmlRoot("textbasegoals")]
public class TextBaseGoals
{
	[XmlArray("goals")]
	[XmlArrayItem("goal")]
	public Goal[] goals;

	[XmlElement("fieldname")]
	public string fieldname;

	[XmlElement("collectionname")]
	public string collectionname;

	[XmlElement("goalname")]
	public string goalname;

	[XmlArray("fielddescriptions")]
	[XmlArrayItem("field")]
	public string[] fielddescriptions;

	[XmlArray("missionnames")]
	[XmlArrayItem("missionname")]
	public string[] missionnames;

	[XmlElement("reward")]
	public string reward;

	[XmlElement("rewardtext")]
	public string rewardtext;

	[System.Serializable]
	public class Goal
	{
		[XmlAttribute("goalid")]
		public int goalid;
		[XmlAttribute("missionnameid")]
		public int missionnameid;
		[XmlAttribute("fielddescriptionid")]
		public int fielddescriptionid;

		[XmlElement("header")]
		public string header;
		[XmlElement("text")]
		public string[] texts;


	}

}

[XmlRoot("textbasenews")]
public class TextBaseNews
{
	[XmlElement("news")]
	public News[] news;


	public class News
	{
		[XmlElement("item")]
		public Item[] items;

		public class Item
		{
			[XmlElement("header")]
			public string header;
			[XmlElement("text")]
			public string[] texts;
		}
		[XmlElement("text")]
		public string[] texts;
	}
}