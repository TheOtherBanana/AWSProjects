using AWSProjects.CommonUtils;
using Gma.CodeCloud.Controls;
using Gma.CodeCloud.Controls.TextAnalyses;
using Gma.CodeCloud.Controls.TextAnalyses.Blacklist;
using Gma.CodeCloud.Controls.TextAnalyses.Blacklist.En;
using Gma.CodeCloud.Controls.TextAnalyses.Extractors;
using Gma.CodeCloud.Controls.TextAnalyses.Processing;
using Gma.CodeCloud.Controls.TextAnalyses.Stemmers;
using Gma.CodeCloud.Controls.TextAnalyses.Stemmers.En;
using QuoraWCGeneratorBL.DataContracts;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;


namespace QuoraWCGeneratorBL
{
    internal static class ComponentFactory
    {
        public static IWordStemmer CreateWordStemmer(bool groupSameStemWords)
        {
            return groupSameStemWords
                       ? (IWordStemmer)new PorterStemmer()
                       : new NullStemmer();
        }

        public static IBlacklist CreateBlacklist(bool excludeEnglishCommonWords)
        {
            return excludeEnglishCommonWords
                       ? (IBlacklist)new CommonWords()
                       : new NullBlacklist();
        }

        public static IEnumerable<string> CreateExtractor(string input)
        {
            return new StringExtractor(input);
        }


    }

    public class WordCloudGenerator
    {

        private CloudControl cloudControl = null;
        public string ImgurClientId = string.Empty;
        public string AlchemyApiKey = string.Empty;

        public WordCloudGenerator()
        {
            this.cloudControl = new CloudControl();
        }

        public GenerateWordCloudResponse GenerateWordCloud(GenerateWordCloudArgs args)
        {
            InitializePanel(args);

            if (Uri.IsWellFormedUriString(args.Text, UriKind.Absolute))
            {
                args.Text = this.GetTextFromUrl(args.Text);
            }

            IEnumerable<string> customWordsToExclude = this.GetCustomWordsFromString(args.WordsToExclude); 

            IBlacklist blacklist = ComponentFactory.CreateBlacklist(args.ExcludeCommonWords);
            IBlacklist customBlacklist = new CommonBlacklist(customWordsToExclude);
           
            IEnumerable<string> terms = ComponentFactory.CreateExtractor(args.Text);
            IWordStemmer stemmer = ComponentFactory.CreateWordStemmer(args.ExcludeCommonWords);

            IEnumerable<IWord> words = terms
               .Filter(blacklist)
               .Filter(customBlacklist)
               .CountOccurences();

            cloudControl.WeightedWords =
                words
                    .GroupByStem(stemmer)
                    .SortByOccurences()
                    .Cast<IWord>();

            string imageUrl = string.Empty;
            imageUrl = UploadToImgurAndGetUrl(imageUrl);

            GenerateWordCloudResponse response = new GenerateWordCloudResponse
            {
                ImgurUrl = imageUrl
            };

            return response;
        }

        private string UploadToImgurAndGetUrl(string imageUrl)
        {
            int width = this.cloudControl.Size.Width;
            int height = this.cloudControl.Size.Height;
            Bitmap bitmap = new Bitmap(width, height);
            this.cloudControl.DrawToBitmap(bitmap, new Rectangle(0, 0, width, height));
            string fileName = Guid.NewGuid().ToString() + ".jpg";
            string imagePath = Path.Combine(Path.GetTempPath(), fileName);
            bitmap.Save(imagePath);

            using (var w = new WebClient())
            {
                string clientID = this.ImgurClientId;
                w.Headers.Add("Authorization", "Client-ID " + clientID);
                var values = new NameValueCollection
                {
                    { "image", Convert.ToBase64String(File.ReadAllBytes(imagePath)) }
                };

                byte[] response = w.UploadValues("https://api.imgur.com/3/upload.xml", values);
                var responseString = Encoding.UTF8.GetString(response);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(responseString);
                imageUrl = xmlDoc.GetElementsByTagName("link")[0].InnerText;
            }
            return imageUrl;
        }

        private string GetTextFromUrl(string url)
        {
            string baseUrlFormat = @"https://access.alchemyapi.com/calls/url/URLGetText?url={0}&apikey={1}";
            string requestUri = string.Format(baseUrlFormat, url, this.AlchemyApiKey);
            string response = RestApiUtils.ExecuteRestApi(requestUri);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(response);
            string text = xmlDoc.GetElementsByTagName("text")[0].InnerText;

            return text;
        }
        private void InitializePanel(GenerateWordCloudArgs args)
        {
            this.cloudControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cloudControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cloudControl.Font = new Font(args.Font, 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.cloudControl.LayoutType = Gma.CodeCloud.Controls.LayoutType.Spiral;
            this.cloudControl.Location = new System.Drawing.Point(12, 151);
            this.cloudControl.MaxFontSize = (int)args.MaxFontSize;
            this.cloudControl.MinFontSize = (int)args.MinFontSize;
            this.cloudControl.Name = "cloudControl";
            this.cloudControl.Palette = GetColorPalette(args.Palette).ToArray();
            this.cloudControl.TabIndex = 3;
            this.cloudControl.WeightedWords = null;
            this.cloudControl.BackColor = Color.FromName(args.BackgroundColor);
            this.cloudControl.Width = (int)args.Width;
            this.cloudControl.Height = (int)args.Height;
        }

        private IEnumerable<Color> GetColorPalette(string[] colors)
        {
            foreach(var color in colors)
            {
                yield return Color.FromName(color);
            }
        }

        private IEnumerable<string> GetCustomWordsFromString(string customWords)
        {
            if(!string.IsNullOrEmpty(customWords))
            {
                var words = customWords.Split(',');
                foreach(var word in words)
                {
                    yield return word;
                }
            }
            else
            {
                yield return null;
            }
        }
    }
}
