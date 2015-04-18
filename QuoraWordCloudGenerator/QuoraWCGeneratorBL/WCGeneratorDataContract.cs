using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace QuoraWCGeneratorBL.DataContracts
{
    [DataContract]
    public class GenerateWordCloudArgs
    {
        [DataMember(IsRequired=true)]
        public string Text { get; set; }
        [DataMember(IsRequired = true)]
        public string Font { get; set; }
        [DataMember(IsRequired = true)]
        public string[] Palette { get; set; }
        [DataMember(IsRequired = true)]
        public string BackgroundColor { get; set; }
        [DataMember(IsRequired = true)]
        public bool ExcludeCommonWords { get; set; }
        [DataMember(IsRequired = true)]
        public double Height { get; set; }
        [DataMember(IsRequired = true)]
        public double Width { get; set; }
        [DataMember(IsRequired = true)]
        public double MaxFontSize { get; set; }
        [DataMember(IsRequired = true)]
        public double MinFontSize { get; set; }
        [DataMember(IsRequired = true)]
        public string WordsToExclude { get; set; } 
    }

    public class GenerateWordCloudResponse
    {
        [DataMember(IsRequired = true)]
        public string ImgurUrl { get; set; }
    }
}
