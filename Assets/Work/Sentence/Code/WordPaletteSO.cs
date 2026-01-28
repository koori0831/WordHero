using UnityEngine;
using System.Collections.Generic;

namespace Work.Sentence.Code
{
	[CreateAssetMenu(menuName = "SO/Words/Word Palette")]
    public class WordPaletteSO : ScriptableObject
	{
		public List<WordDefinitionSO> Words;
    }
}