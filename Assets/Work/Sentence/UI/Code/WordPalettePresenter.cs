using UnityEngine;
using Work.Sentence.Code;

namespace Work.Sentence.UI.Code
{
	public class WordPalettePresenter
	{
		private readonly WordPaletteView _view;

		public WordPalettePresenter(WordPaletteView view) { _view = view; }

		public void ChangeWord(int index, WordDefinitionSO word)
		{
			_view.SetWord(index, word.DisplayName);
        }

		public void ChangePalette(int num, WordPaletteSO palette)
		{
            // 인덱스 0~3 까지 단어 설정. 없으면 "없음"으로 설정
			for (int i = 0; i < 4; i++)
            {
				if (palette.Words[i] != null)
				{
					_view.SetWord(i, palette.Words[i].DisplayName);
				}
				else
				{
					_view.SetWord(i, "없음");
                }
            }

            _view.SetPaletteID(num == 0 ? "A" : "B");
        }
    }
}