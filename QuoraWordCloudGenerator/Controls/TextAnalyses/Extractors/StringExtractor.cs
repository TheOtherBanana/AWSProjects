using System.Collections.Generic;

namespace Gma.CodeCloud.Controls.TextAnalyses.Extractors
{
    public class StringExtractor : BaseExtractor
    {
        private readonly string m_Text;

        public StringExtractor(string text)
            : base()
        {
            m_Text = text;
        }

        public override IEnumerable<string> GetWords()
        {
            return GetWords(m_Text);
        }

    }
}
