using System.Threading.Tasks;
using System.Net.Mail;

namespace EmailUtils.Net
{
	public interface ITemplateEmailSender
	{
		string LayoutFilePath { get; set; }

        MailMessage ConstructMailMessage(string templatePath, object variables);
	}
}