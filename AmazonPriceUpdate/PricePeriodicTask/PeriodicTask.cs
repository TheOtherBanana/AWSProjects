using AmazonProductAPIWrapper;
using EmailUtils;
using PriceUpdateWebAPIBL;
using ProductUpdateCatalogProvider.CatalogEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace PricePeriodicTask
{
    public class PeriodicTask
    {
        public void CheckPriceAndSendMail(string admindEmailId)
        {
            try
            {
                //EmailManagerContext.Instance.SendHtmlEmail(admindEmailId, "Periodic task started successfully", "Start time " + DateTime.UtcNow);
                this.SendPeriodicStatusMail("Periodic Task Started", "Start time " + DateTime.UtcNow, admindEmailId);
                var catalogProvider = PriceUpdateContext.Instance.CatalogFactory.GetProductCatalogProvider();
                
                var allProducts = catalogProvider.GetProducts();
                Dictionary<string, List<ProductEmailDetails>> emailsToSend = new Dictionary<string, List<ProductEmailDetails>>();
                DateTime todayDate = DateTime.UtcNow.Date;
                DayOfWeek day = todayDate.DayOfWeek;
                List<ProductCatalogEntity> entitiesToUpdate = new List<ProductCatalogEntity>();
                foreach (ProductCatalogEntity product in allProducts)
                {
                    try
                    {
                        if (product.IsConfirmed == false)
                        {
                            continue;
                        }
                        if (product.ToEmailOnPrice && todayDate <= product.EmailOnPriceDuration)
                        {
                            //Check if last emailed on is not null
                            //GetPrice
                            //If price is lower than specifie add to email. Mark Email sent on
                            //Check if duration is passed and delete

                            var offers = AmazonProductHelper.GetOffers(product.ASIN, product.Country);
                            double itemPrice = double.Parse(offers.Items.FirstOrDefault().Item.FirstOrDefault().OfferSummary.LowestNewPrice.Amount);
                            itemPrice = itemPrice / 100;
                            double priceToEmail = double.Parse(product.PriceToEmail);

                            if (itemPrice <= priceToEmail)
                            {
                                product.CurrentPrice = Convert.ToString(itemPrice);
                                AddItemToEmail(emailsToSend, product);
                                product.EmailSentOn = todayDate;
                                product.ToEmailOnPrice = false;
                                entitiesToUpdate.Add(product);
                            }

                        }

                        if (product.ToEmailEveryWeek && todayDate <= product.EmailEveryWeekDuration)
                        {
                            DayOfWeek dayToEmail = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), product.DayToEmail);
                            if (dayToEmail == day)
                            {
                                AddItemToEmail(emailsToSend, product);
                            }
                        }

                        if (product.ToEmailOnDate && todayDate == product.DateToEmail)
                        {
                            AddItemToEmail(emailsToSend, product);
                        }

                        //If the duration has passed, delete it
                        if (todayDate > product.EmailEveryWeekDuration && product.EmailEveryWeekDuration != DateTime.MinValue)
                        {
                            product.ToEmailEveryWeek = false;
                            entitiesToUpdate.Add(product);
                        }
                        if (todayDate > product.EmailOnPriceDuration && product.EmailOnPriceDuration != DateTime.MinValue)
                        {
                            product.ToEmailOnPrice = false;
                            entitiesToUpdate.Add(product);
                        }
                    }
                    catch(Exception ex)
                    {
                        this.SendPeriodicStatusMail("Failed processing product object " + product.ASIN, ex.Message, admindEmailId);
                        continue;
                    }
                }

                //Send all emails
                foreach (var emailId in emailsToSend.Keys)
                {
                    //Figure out a way to fix this
                    MailMessage mailToSend = ProductEmailHelper.ProductHtmlMailContent(emailsToSend[emailId], AmazonProductAPIContext.Regions.IN);
                    string subject = "Price updates for your products!";
                    mailToSend.To.Add(emailId);
                    mailToSend.Subject = subject;
                    mailToSend.SubjectEncoding = Encoding.UTF8;
                    EmailManagerContext.Instance.SendHtmlEmail(mailToSend);
                }
                
                //After all mails are sent
                entitiesToUpdate.ForEach(entity => catalogProvider.UpdateProduct(entity));
            }
            catch(Exception ex)
            {
                this.SendPeriodicStatusMail("Periodic Task Failed", ex.Message, admindEmailId);
            }

            this.SendPeriodicStatusMail("Periodic Task Finished", "Finish time " + DateTime.UtcNow, admindEmailId);
        }

        private void SendPeriodicStatusMail(string subject, object message, string emailId)
        {
            var mailToSend = new MailMessage
            {
                IsBodyHtml = true,
                BodyEncoding = System.Text.Encoding.UTF8,
                Body = (string)message
            };

            mailToSend.To.Add(emailId);
            mailToSend.Subject = subject;
            mailToSend.SubjectEncoding = Encoding.UTF8;
            EmailManagerContext.Instance.SendHtmlEmail(mailToSend);

        }
        private static void AddItemToEmail(Dictionary<string, List<ProductEmailDetails>> emailsToSend, ProductCatalogEntity product)
        {
            var itemDetail = AmazonProductHelper.GetItemDetails(product.ASIN, product.Country);
            string itemName = itemDetail.Items.FirstOrDefault().Item.FirstOrDefault().ItemAttributes.Title;
            var cartDetail = AmazonProductHelper.GetCartDetails(product.ASIN, product.Country);
            string cartUrl = cartDetail.Cart.FirstOrDefault().PurchaseURL;
            const string htmlATag = @"<a href={0}>Click here to buy!</a>";
            ProductEmailDetails emailDetails = new ProductEmailDetails
            {
                ProductName = itemName,
                CurrentPrice = product.CurrentPrice,
                InitialPrice = product.InitialPrice,
                ProductPurchaseLink = string.Format(htmlATag,cartUrl)
            };

            if (!emailsToSend.ContainsKey(product.EmailId))
            {
                emailsToSend.Add(product.EmailId, new List<ProductEmailDetails> {emailDetails});
            }
            else
            {
                emailsToSend[product.EmailId].Add(emailDetails);
            }
        }
    }
}
