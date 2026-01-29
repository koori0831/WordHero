using UnityEngine;
using TMPro;

namespace Work.Sentence.UI.Code
{
	public class WordPaletteView : MonoBehaviour
	{
		[SerializeField] private TMP_Text _paletteID; // A or B
		[SerializeField] private TMP_Text _wordUP, _wordDown, _wordLeft, _wordRight;

		public void SetPaletteID(string id)
		{
			_paletteID.text = id;
        }

		public void SetWord(int index, string word) // 0 = left, 1 = up, 2 = right, 3 = down
		{
			switch (index)
			{
				case 0:
					_wordLeft.text = word;
					break;
				case 1:
					_wordUP.text = word;
					break;
				case 2:
					_wordRight.text = word;
					break;
				case 3:
					_wordDown.text = word;
					break;
			}
        }
    }
}